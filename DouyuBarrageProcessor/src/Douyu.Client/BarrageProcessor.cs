using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Douyu.Messages;
using Douyu.Events;
using Jack4net.Log;
using Jack4net;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace Douyu.Client
{
    public class BarrageProcessor
    {
        public int RoomId { get; private set; }

        public void ChangeRoomId(int roomId)
        {
            RoomId = roomId;
            Obs.Initialize(roomId);
        }

        bool _stopListen = false;

        public void StartProcess()
        {
            _stopListen = false;
            while (!_stopListen) {
                try {
                    // 获取未处理的消息
                    var messages = DbService.GetMessages(RoomId);
                    if (messages.Length == 0) {
                        MyThread.Wait(100);
                        continue;
                    }

                    foreach (var message in messages) {
                        if (message is ChatMessage) {
                            ProcessChatMessage(message as ChatMessage);
                        } else if (message is GiftMessage) {
                            ProcessGiftMessage(message as GiftMessage);
                        } else if (message is ChouqinMessage) {
                            ProcessChouqinMessage(message as ChouqinMessage);
                        }
                    }
                } catch (Exception ex) {
                    LogService.Error("Processing barrage throw exception: " + ex.Message, ex);
                }
            }
        }

        public void StopProcess()
        {
            try {
                _stopListen = true;
            } catch (Exception ex) {
                LogService.Error("Stop processing barrage throw exception: " + ex.Message, ex);
                throw new DouyuException("停止处理失败: " + ex.Message, ex);
            }
        }

        #region 各种消息处理

        void ProcessChatMessage(ChatMessage chatMessage)
        {
            // 发弹幕, 赚积分
            var score = ScoreManager.CalChatScore(chatMessage);
            DbService.AddUserScore(chatMessage.RoomId, chatMessage.UserId, chatMessage.UserName, score);

            // 处理弹幕命令
            if (chatMessage.Text.Trim().StartsWith("#"))
                ProcessBarrageCommand(chatMessage);

            DbService.SetChatMessageProcessed(chatMessage);
            OnChatMessageProcessed(chatMessage);
        }

        void ProcessBarrageCommand(ChatMessage chatMessage)
        {
            Match match;

            // 去除#, 空格, 替换全角字符
            var command = chatMessage.Text.Substring(1).Trim();
            command = command.Replace('－', '-');
            command = command.Replace('（', '(');
            command = command.Replace('）', ')');

            // 命令: #打卡
            if (command == "打卡") {
                // Obs.CommandMessage.AddMessage("[{0}]: 无需打卡, 发弹幕得积分!", chatMessage.UserName);
                return;
            }

            // (1) 命令: 查询用户积分 #查询
            if (command == "查询") {
                Obs.CommandMessage.AddMessage("{0}:\n当前积分 {1}",
                    chatMessage.UserName, DbService.QueryUserScore(chatMessage.RoomId, chatMessage.UserId));
                return;
            }

            // (2) 命令: 点播电影 #功夫-100
            if (Regex.Match(command, @"^(\w+)$").Success) {
                command += "-1000";
            }
            match = Regex.Match(command, @"^(\w+)\s*-\s*(\d+)$");
            if (match.Success) {
                var movieName = match.Groups[1].Value;

                // 检查系统里面是否有这部电影
                if (!DbService.HasMovie(chatMessage.RoomId, movieName)) {
                    var officialName = DbService.GetOfficialMovieName(chatMessage.RoomId, movieName);   // 用的是别名?
                    if (officialName == "" || !DbService.HasMovie(chatMessage.RoomId, officialName)) {
                        Obs.CommandMessage.AddErrorMessage("{0}: 没有找到电影[{1}]!", chatMessage.UserName, movieName);
                        return;
                    }
                    movieName = officialName;
                }

                // 检查点播电影是否是当前正在播放的电影
                var currentMovie = DbService.GetCurrentMovie(chatMessage.RoomId);
                if (currentMovie.Equals(movieName, StringComparison.OrdinalIgnoreCase)) {
                    Obs.CommandMessage.AddErrorMessage("{0}: [{1}]正在播放, 请不要重复点播", chatMessage.UserName, movieName);
                    return;
                }

                // 检查电影冷却时间

                // 检查积分是否溢出
                var playScore = 0;
                if (!int.TryParse(match.Groups[2].Value, out playScore)) {
                    Obs.CommandMessage.AddErrorMessage("{0}: 点播[{1}]失败, 积分无效", chatMessage.UserName, movieName);
                    return;
                }

                // 检查用户积分是否够
                var userScore = DbService.QueryUserScore(chatMessage.RoomId, chatMessage.UserId);
                if (userScore < playScore) {
                    Obs.CommandMessage.AddErrorMessage("{0}: 点播[{1}]失败, 积分不够, 当前积分{2}",
                        chatMessage.UserName, movieName, userScore);
                    return;
                }

                // 更新电影积分
                DbService.AddMovieScore(chatMessage.RoomId, movieName, playScore);
                var rank = DbService.GetMovieRank(movieName, chatMessage.RoomId);
                if (rank == 0)
                    Obs.CommandMessage.AddMessage("{0}:\n{1} +{2}, 下一部就是了!",
                        chatMessage.UserName, movieName, playScore);
                else if (rank > 0)
                    Obs.CommandMessage.AddMessage("{0}:\n{1} +{2}, 前面还有{3}部!",
                        chatMessage.UserName, movieName, playScore, rank);
                else
                    LogService.ErrorFormat("电影积分排名 = {0}, 命令 = {1}", rank, chatMessage.MessageText);

                // 更新用户积分
                DbService.AddUserScore(chatMessage.RoomId, chatMessage.UserId, chatMessage.UserName, playScore * (-1));
                return;
            }

            // 未知命令
            Obs.CommandMessage.AddErrorMessage("{0}: 无效命令, {1}", chatMessage.UserName, command);
            return;
        }

        void ProcessGiftMessage(GiftMessage giftMessage)
        {
            // 送礼物, 赚积分
            var giftScore = ScoreManager.CalGiftScore(giftMessage);
            if (giftScore < 0) {
                LogService.ErrorFormat("获取礼物积分失败, 礼物ID为{0}", giftMessage.GiftId);
                return;
            }
            DbService.AddUserScore(giftMessage.RoomId, giftMessage.UserId, giftMessage.UserName, giftScore);
            OnUserScoreAdded(new ScoreAddedEventArgs(giftMessage.UserName, giftMessage.GiftName, giftScore));

            // 感谢            
            Obs.GiftMessage.AddMessage("感谢 {0} 送的1个{1}, 总积分{2}",
                giftMessage.UserName, giftMessage.GiftName,
                DbService.QueryUserScore(giftMessage.RoomId, giftMessage.UserId));

            DbService.SetGiftMessageProcessed(giftMessage);
            OnGiftMessageProcessed(giftMessage);
        }

        void ProcessChouqinMessage(ChouqinMessage chouqinMessage)
        {
            // 酬勤赚积分
            var chouqinScore = ScoreManager.CalChouqinScore(chouqinMessage);
            DbService.AddUserScore(chouqinMessage.RoomId, chouqinMessage.UserId, chouqinMessage.UserName, chouqinScore);
            OnUserScoreAdded(new ScoreAddedEventArgs(chouqinMessage.UserName, "酬勤" + chouqinMessage.Level, chouqinScore));

            // 感谢
            Obs.GiftMessage.AddMessage("感谢 {0} 送的{1}, 总积分{2}",
                chouqinMessage.UserName, chouqinMessage.ChouqinName,
                DbService.QueryUserScore(chouqinMessage.RoomId, chouqinMessage.UserId));

            DbService.SetChouqinMessageProcessed(chouqinMessage);
            OnChouqinMessageProcessed(chouqinMessage);
        }

        #endregion

        #region events

        public event EventHandler<ServerMessageEventArgs<ChatMessage>> ChatMessageProcessed;
        public event EventHandler<ServerMessageEventArgs<GiftMessage>> GiftMessageProcessed;
        public event EventHandler<ServerMessageEventArgs<ChouqinMessage>> ChouqinMessageProcessed;
        public event EventHandler<ScoreAddedEventArgs> UserScoreAdded;

        protected void OnChatMessageProcessed(ChatMessage chatMessage)
        {
            if (ChatMessageProcessed != null)
                ChatMessageProcessed(this, new ServerMessageEventArgs<ChatMessage>(chatMessage));
        }

        protected void OnGiftMessageProcessed(GiftMessage giftMessage)
        {
            if (GiftMessageProcessed != null)
                GiftMessageProcessed(this, new ServerMessageEventArgs<GiftMessage>(giftMessage));
        }

        protected void OnChouqinMessageProcessed(ChouqinMessage message)
        {
            if (ChouqinMessageProcessed != null)
                ChouqinMessageProcessed(this, new ServerMessageEventArgs<ChouqinMessage>(message));
        }

        protected void OnUserScoreAdded(ScoreAddedEventArgs args)
        {
            if (UserScoreAdded != null)
                UserScoreAdded(this, args);
        }

        #endregion
    }
}
