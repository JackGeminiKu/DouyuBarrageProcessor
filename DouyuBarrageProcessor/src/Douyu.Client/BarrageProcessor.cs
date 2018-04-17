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
        public int RoomId { get; set; }

        public void ChangeRoomId(int roomId)
        {
            RoomId = roomId;
            Obs.Initialize(roomId);
        }

        public void StartProcess()
        {
            _stopListen = false;
            do {
                try {
                    // 获取未处理的消息
                    ServerMessage[] messages = DbService.GetMessages(RoomId);
                    if (messages.Length == 0) {
                        MyThread.Wait(1000);
                        continue;
                    }

                    foreach (ServerMessage message in messages) {
                        if (message is ChatMessage) {
                            ProcessChatMessage(message as ChatMessage);
                        } else if (message is GiftMessage) {
                            ProcessGiftMessage(message as GiftMessage);
                        } else if (message is ChouqinMessage) {
                            ProcessChouqinMessage(message as ChouqinMessage);
                        }
                    }
                } catch (Exception ex) {
                    LogService.GetLogger("Error").Error("[error]" + ex.Message, ex);
                }
            } while (!_stopListen);
        }

        public void StopProcess()
        {
            try {
                _stopListen = true;
            } catch (Exception ex) {
                throw new DouyuException("停止处理失败: " + ex.Message, ex);
            }
        }

        private bool _stopListen = false;

        #region 各种消息处理

        private void ProcessChatMessage(ChatMessage chatMessage)
        {
            // 发弹幕, 赚积分
            var score = ScoreCalculator.CalChatScore(chatMessage);
            DbService.AddUserScore(chatMessage.RoomId, chatMessage.UserId, chatMessage.UserName, score);

            // 处理弹幕命令
            if (chatMessage.Text.Trim().StartsWith("#"))
                ProcessCommand(chatMessage);

            DbService.SetChatMessageProcessed(chatMessage);

            OnChatMessageProcessed(chatMessage);
        }

        static void ProcessCommand(ChatMessage chatMessage)
        {
            Match match;

            // 替换全角字符
            string message = chatMessage.Text;
            message = message.Replace('－', '-');
            message = message.Replace('（', '(');
            message = message.Replace('）', ')');

            // 命令: #打卡
            match = Regex.Match(message, @"^\s*#\s*打卡\s*$");
            if (match.Success) {
                // Obs.CommandMessage.AddMessage("[{0}]: 无需打卡, 发弹幕也可得积分!", chatMessage.UserName);
                return;
            }

            // 命令: 查询用户积分 #查询
            match = Regex.Match(message, @"^\s*#\s*查询\s*$");
            if (match.Success) {
                Obs.CommandMessage.AddMessage("{0}:\n当前积分 {1}",
                    chatMessage.UserName, DbService.QueryUserScore(chatMessage.RoomId, chatMessage.UserId));
                return;
            }

            // 命令: 点播电影 #功夫-100
            if (Regex.Match(message, @"^\s*#\s*(\w+)\s*$").Success) {
                message += "-1000";
            }
            match = Regex.Match(message, @"^\s*#\s*(\w+)\s*-\s*(\d+)\s*$");
            if (match.Success) {
                string movieName = match.Groups[1].Value;

                // 检查积分是否溢出
                int score = 0;
                if (int.TryParse(match.Groups[2].Value, out score) == false) {
                    Obs.CommandMessage.AddErrorMessage("{0}: 点播【{1}】失败, 无效积分", chatMessage.UserName, movieName);
                    return;
                }

                // 检查点播电影是否是当前正在播放的电影
                string currentMovie = DbService.GetCurrentMovie(chatMessage.RoomId);
                if (currentMovie.Equals(movieName, StringComparison.OrdinalIgnoreCase)) {
                    Obs.CommandMessage.AddErrorMessage("{0}: 【{1}】正在播放, 请不要重复点播", chatMessage.UserName, movieName);
                    return;
                }

                // 检查用户积分是否够
                int userScore = DbService.QueryUserScore(chatMessage.RoomId, chatMessage.UserId);
                if (score > userScore) {
                    Obs.CommandMessage.AddErrorMessage("{0}: 点播【{1}】失败, 积分不够, 当前积分{2}",
                        chatMessage.UserName, movieName, userScore);
                    return;
                }

                // 检查是否支持该电影
                if (DbService.HasMovie(chatMessage.RoomId, movieName) == false) {
                    // 别名?
                    string convertedName = DbService.GetMovieName(chatMessage.RoomId, movieName);
                    if (convertedName == "")
                        Obs.CommandMessage.AddErrorMessage("{0}: 点播【{1}】失败, 没有收录该片", chatMessage.UserName, movieName);
                    else if (DbService.HasMovie(chatMessage.RoomId, convertedName) == false)
                        Obs.CommandMessage.AddErrorMessage("{0}: 点播【{1}】失败, 没有收录该片", chatMessage.UserName, convertedName);
                    else
                        movieName = convertedName;
                    return;
                }

                // 更新电影积分
                DbService.AddMovieScore(chatMessage.RoomId, chatMessage.UserId, movieName, score);
                int rank = DbService.GetMovieRank(movieName, chatMessage.RoomId);
                if (rank == 0)
                    Obs.CommandMessage.AddMessage("{0}:\n{1} +{2}, 下一部就是了", chatMessage.UserName, movieName, score);
                if (rank > 0)
                    Obs.CommandMessage.AddMessage("{0}:\n{1} +{2}, 前面还有{3}部",
                        chatMessage.UserName, movieName, score, rank);

                // 更新用户积分
                DbService.AddUserScore(chatMessage.RoomId, chatMessage.UserId, chatMessage.UserName, score * (-1));
                return;
            }

            // 未知命令
            Obs.CommandMessage.AddErrorMessage("{0}: 无效命令, {1}", chatMessage.UserName, message);
            return;
        }

        private void ProcessGiftMessage(GiftMessage giftMessage)
        {
            // 送礼物, 赚积分
            var score = ScoreCalculator.CalGiftScore(giftMessage);
            if (score < 0) {
                LogService.GetLogger("Error").ErrorFormatted("获取礼物积分失败, 礼物ID为{0}", giftMessage.GiftId);
                return;
            }
            DbService.AddUserScore(giftMessage.RoomId, giftMessage.UserId, giftMessage.UserName, score);
            OnScoreAdded(new ScoreAddedEventArgs(giftMessage.UserName, score, giftMessage.GiftName));

            // 感谢            
            Obs.ThanksGiftMessage.AddMessage("感谢 {0} 送的1个{1}, 总积分{2}",
                giftMessage.UserName, giftMessage.GiftName,
                DbService.QueryUserScore(giftMessage.RoomId, giftMessage.UserId));

            DbService.SetGiftMessageProcessed(giftMessage);
            OnGiftMessageProcessed(giftMessage);
        }

        private void ProcessChouqinMessage(ChouqinMessage chouqinMessage)
        {
            // 酬勤赚积分
            var score = ScoreCalculator.CalChouqinScore(chouqinMessage);
            DbService.AddUserScore(chouqinMessage.RoomId, chouqinMessage.UserId, chouqinMessage.UserName, score);
            OnScoreAdded(new ScoreAddedEventArgs(chouqinMessage.UserName, score, "酬勤" + chouqinMessage.Level));

            // 感谢
            Obs.ThanksGiftMessage.AddMessage("感谢 {0} 送的{1}, 总积分{2}",
                chouqinMessage.UserName, chouqinMessage.ChouqinName,
                DbService.QueryUserScore(chouqinMessage.RoomId, chouqinMessage.UserId));

            DbService.SetChouqinMessageProcessed(chouqinMessage);
            OnChouqinMessageProcessed(chouqinMessage);
        }

        #endregion

        #region events

        public event EventHandler<ChatMessageEventArgs> ChatMessageProcessed;
        public event EventHandler<GiftMessageEventArgs> GiftMessageProcessed;
        public event EventHandler<ChouqinMessageEventArgs> ChouqinMessageProcessed;
        public event EventHandler<ScoreAddedEventArgs> ScoreAdded;

        protected void OnChatMessageProcessed(ChatMessage chatMessage)
        {
            if (ChatMessageProcessed != null)
                ChatMessageProcessed(this, new ChatMessageEventArgs(chatMessage));
        }

        protected void OnGiftMessageProcessed(GiftMessage giftMessage)
        {
            if (GiftMessageProcessed != null)
                GiftMessageProcessed(this, new GiftMessageEventArgs(giftMessage));
        }

        protected void OnChouqinMessageProcessed(ChouqinMessage message)
        {
            if (ChouqinMessageProcessed != null)
                ChouqinMessageProcessed(this, new ChouqinMessageEventArgs(message));
        }

        protected void OnScoreAdded(ScoreAddedEventArgs args)
        {
            if (ScoreAdded != null)
                ScoreAdded(null, args);
        }

        #endregion
    }
}
