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

        public bool IsProcessing { get; private set; }

        public void ChangeRoomId(int roomId)
        {
            RoomId = roomId;
            Obs.Initialize(roomId);
        }

        bool _stopProcess = false;

        public void StartProcess()
        {
            _stopProcess = false;
            IsProcessing = true;
            while (!_stopProcess) {
                try {
                    // 获取消息
                    var messages = new List<ServerMessage>();
                    messages.AddRange(ChatMessage.GetMessages(RoomId));
                    messages.AddRange(GiftMessage.GetMessages(RoomId));
                    messages.AddRange(ChouqinMessage.GetMessages(RoomId));
                    if (messages.Count == 0) {
                        MyThread.Wait(100);
                        continue;
                    }

                    // 处理消息
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
                    LogService.Error("处理弹幕消息抛出异常: " + ex.Message, ex);
                }
            }
            IsProcessing = false;
        }

        public void StopProcess()
        {
            try {
                _stopProcess = true;
                var stopwatch = Stopwatch.StartNew();
                do {
                    if (!IsProcessing) break;
                } while (stopwatch.ElapsedMilliseconds < 3000);
                if (IsProcessing)
                    throw new DouyuException("停止处理消息超时!");
            } catch (Exception ex) {
                LogService.Error("停止处理弹幕消息抛出异常: " + ex.Message, ex);
                throw new DouyuException("停止处理消息抛出异常: " + ex.Message, ex);
            }
        }

        #region 各种消息处理

        void ProcessChatMessage(ChatMessage chatMessage)
        {
            // 发弹幕, 赚积分
            var score = ScoreManager.CalChatScore(chatMessage);
            UserService.AddUserScore(chatMessage.RoomId, chatMessage.UserId, chatMessage.UserName, score);

            // 处理弹幕命令
            if (chatMessage.Text.Trim().StartsWith("#"))
                ProcessBarrageCommand(chatMessage);

            MessageService.SetChatMessageProcessed(chatMessage);
            OnChatMessageProcessed(chatMessage);
        }

        void ProcessBarrageCommand(ChatMessage chatMessage)
        {
            // 去除#, 空格, 替换全角字符
            var command = chatMessage.Text.Trim().Substring(1);
            command = command.Replace('－', '-');
            command = command.Replace('（', '(');
            command = command.Replace('）', ')');

            // 命令: #打卡
            if (command == "打卡") {
                // Obs.OtherMessage.AddMessage("[{0}]: 无需打卡, 发弹幕得积分!", chatMessage.UserName);
                return;
            }

            // (1) 命令: 查询用户积分 #查询
            if (command == "查询") {
                Obs.OtherMessage.AddMessage("[{0}]:\n当前积分 {1}",
                    chatMessage.UserName, UserService.GetUserScore(chatMessage.RoomId, chatMessage.UserId));
                return;
            }

            // (2) 命令: 点播电影 #功夫-100
            if (Regex.Match(command, @"^(\w+)$").Success) {
                command += "-1000";
            }
            var match = Regex.Match(command, @"^(\w+)\s*-\s*(\d+)$");
            if (match.Success) {
                // 检查系统里面是否有这部电影
                var movieName = match.Groups[1].Value;
                if (!MovieService.HasMovie(chatMessage.RoomId, movieName)) {
                    var officialName = MovieService.GetOfficialMovieName(chatMessage.RoomId, movieName);   // 用的是别名?
                    if (officialName == "" || !MovieService.HasMovie(chatMessage.RoomId, officialName)) {
                        Obs.MovieMessage.PlayFail("[{0}]: 没有找到电影 {1}", chatMessage.UserName, movieName);
                        return;
                    }
                    movieName = officialName;
                }

                // 检查点播电影是否是当前正在播放的电影
                var currentMovie = MovieService.GetCurrentMovie(chatMessage.RoomId);
                if (currentMovie.Equals(movieName, StringComparison.OrdinalIgnoreCase)) {
                    Obs.MovieMessage.PlayFail("[{0}]: {1} 正在播放, 请不要重复点播", chatMessage.UserName, movieName);
                    return;
                }

                // 检查积分是否溢出
                var playScore = 0;
                if (!int.TryParse(match.Groups[2].Value, out playScore)) {
                    Obs.MovieMessage.PlayFail("[{0}]: 点播 {1} 失败, 积分无效", chatMessage.UserName, movieName);
                    return;
                }

                // 检查用户积分是否够
                var userScore = UserService.GetUserScore(chatMessage.RoomId, chatMessage.UserId);
                if (userScore < playScore) {
                    Obs.MovieMessage.PlayFail("[{0}]: 点播 {1} 失败, 积分不够, 当前积分{2}",
                        chatMessage.UserName, movieName, userScore);
                    return;
                }

                // 更新积分
                MovieService.AddMovieScore(chatMessage.RoomId, movieName, playScore);
                UserService.AddUserScore(chatMessage.RoomId, chatMessage.UserId, chatMessage.UserName, playScore * (-1));

                // 显示成功点播信息
                var rank = MovieService.GetMovieRank(chatMessage.RoomId, movieName);
                Obs.MovieMessage.PlayMovie(chatMessage.UserName, movieName, rank);
                return;
            }

            // 未知命令
            Obs.OtherMessage.AddMessage("[{0}]: 无效命令, {1}", chatMessage.UserName, command);
            return;
        }

        void ProcessGiftMessage(GiftMessage giftMessage)
        {
            // 送礼物, 赚积分
            var giftScore = ScoreManager.CalGiftScore(giftMessage);
            if (giftScore < 0) {
                LogService.ErrorFormat("获取礼物积分失败, 礼物ID为{0}", giftMessage.GiftId);
                // 设置礼物记录为处理失败状态
                // TBD
                return;
            }
            UserService.AddUserScore(giftMessage.RoomId, giftMessage.UserId, giftMessage.UserName, giftScore);
            OnUserScoreAdded(new ScoreAddedEventArgs(giftMessage.UserName, giftMessage.GiftName, giftScore));

            // 感谢            
            Obs.ThanksMessage.AddMessage("感谢 {0} 送的1个{1}, 总积分{2}",
                giftMessage.UserName, giftMessage.GiftName,
                UserService.GetUserScore(giftMessage.RoomId, giftMessage.UserId));

            MessageService.SetGiftMessageProcessed(giftMessage);
            OnGiftMessageProcessed(giftMessage);
        }

        void ProcessChouqinMessage(ChouqinMessage chouqinMessage)
        {
            // 酬勤赚积分
            var chouqinScore = ScoreManager.CalChouqinScore(chouqinMessage);
            if (chouqinScore < 0) {
                LogService.ErrorFormat("获取酬勤积分失败, 酬勤等级为{0}", chouqinMessage.Level);
                // 设置酬勤记录为处理失败状态
                // TBD
                return;
            }

            UserService.AddUserScore(chouqinMessage.RoomId, chouqinMessage.UserId, chouqinMessage.UserName, chouqinScore);
            OnUserScoreAdded(new ScoreAddedEventArgs(chouqinMessage.UserName, "酬勤" + chouqinMessage.Level, chouqinScore));

            // 感谢
            Obs.ThanksMessage.AddMessage("感谢 {0} 送的{1}, 总积分{2}",
                chouqinMessage.UserName, chouqinMessage.ChouqinName,
                UserService.GetUserScore(chouqinMessage.RoomId, chouqinMessage.UserId));

            MessageService.SetChouqinMessageProcessed(chouqinMessage);
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
