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
        public BarrageProcessor()
        { }

        public BarrageProcessor(int roomId)
        {
            RoomId = roomId;
        }

        public int RoomId { get; set; }

        public bool IsProcessing { get; private set; }

        bool _stopProcess = true;

        public void StartProcess()
        {
            // 初始化房间
            Obs.Initialize(RoomId);

            // 开始处理
            _stopProcess = false;
            IsProcessing = true;
            while (!_stopProcess) {
                try {
                    // 获取消息
                    var messages = ChatMessage.GetMessages(RoomId);
                    if (messages.Length == 0) {
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
                    LogService.Error("处理弹幕消息发生异常!", ex);
                }
            }
            IsProcessing = false;
        }

        public void StopProcess()
        {
            const int TIMEOUT_STOP_PROCESS = 3000;
            _stopProcess = true;
            var stopwatch = Stopwatch.StartNew();
            do {
                if (!IsProcessing)
                    break;
                MyThread.Wait(100);
            } while (stopwatch.ElapsedMilliseconds < TIMEOUT_STOP_PROCESS);
            if (IsProcessing)
                throw new DouyuException("停止处理消息超时!");
        }

        #region 各种消息处理

        void ProcessChatMessage(ChatMessage chatMessage)
        {
            try {
                // 发弹幕, 赚积分
                var score = ScoreService.CalScore(chatMessage);
                UserService.AddScore(chatMessage.RoomId, chatMessage.UserId, chatMessage.UserName, score);

                // 处理弹幕命令
                if (IsChatCommand(chatMessage.Text))
                    ProcessChatCommand(chatMessage);

                ChatMessage.SetProcessResult(chatMessage, ProcessResult.Ok);
                OnChatMessageProcessed(chatMessage);
            } catch (Exception) {
                ChatMessage.SetProcessResult(chatMessage, ProcessResult.Error);
                throw;
            }
        }

        bool IsChatCommand(string chatText)
        {
            chatText = chatText.Trim();
            return chatText.StartsWith("#") || chatText.StartsWith("＃");
        }

        void ProcessChatCommand(ChatMessage message)
        {

            // 去除#, 空格, 替换全角字符
            var command = message.Text.Trim().Substring(1);
            command = command.Replace('－', '-');
            command = command.Replace('（', '(');
            command = command.Replace('）', ')');

            // (0) 命令: #打卡
            if (command == "打卡") {
                Obs.OtherMessage.AddMessage("[{0}]: 无需打卡, 发弹幕也可以得积分!", message.UserName);
                return;
            }

            // (1) 命令: 查询用户积分 #查询
            if (command == "查询") {
                var userScore = UserService.GetScore(message.RoomId, message.UserId);
                Obs.OtherMessage.AddMessage("[{0}]: 当前积分 {1}", message.UserName, userScore);
                return;
            }

            // (2) 命令: 点播电影 #功夫-100
            if (Regex.Match(command, @"^(\w+)$").Success) {
                command += "-10000";
            }
            var match = Regex.Match(command, @"^(\w+)\s*-\s*(\d+)$");
            if (match.Success) {
                ProcessChatCommand_PlayMovie(match.Groups[1].Value, match.Groups[2].Value, message);
                return;
            }

            // 未知命令
            Obs.OtherMessage.AddMessage("[{0}]: 无效命令, {1}", message.UserName, command);
            return;
        }

        void ProcessChatCommand_PlayMovie(string movieName, string scoreInCommand, ChatMessage message)
        {
            const int COOLDOWN_PLAY_MOVIE = 4 * 60;

            // 检查系统里面是否有这部电影
            if (!MovieService.HasMovie(message.RoomId, movieName)) {
                var officialName = MovieService.GetOfficialMovieName(message.RoomId, movieName);   // 用的是别名?
                if (officialName == "" || !MovieService.HasMovie(message.RoomId, officialName)) {
                    Obs.MovieMessage.ShowFail("[{0}]: 没有找到电影 {1}!", message.UserName, movieName);
                    return;
                }
                movieName = officialName;
            }

            // 检查是否是禁播的电影
            if (MovieService.IsBannedMovie(movieName)) {
                Obs.MovieMessage.ShowFail("[{0}]: {1} 已经禁播!", message.UserName, movieName);
                return;
            }

            // 检查点播电影是否是当前正在播放的电影
            var currentMovie = MovieService.GetCurrentMovie(message.RoomId);
            if (currentMovie != null && currentMovie.Equals(movieName, StringComparison.OrdinalIgnoreCase)) {
                Obs.MovieMessage.ShowFail("[{0}]: 正在播放 {1} , 请不要重复点播!", message.UserName, movieName);
                return;
            }

            // 电影冷却完成了?
            var lastPlayTime = MovieService.GetLastPlayTime(message.RoomId, movieName);
            if ((DateTime.Now - lastPlayTime).TotalMinutes < COOLDOWN_PLAY_MOVIE) {
                Obs.MovieMessage.ShowFail("[{0}]: 点播 {1} 失败, 还有 {2} 分钟才能点播!",
                    message.UserName, movieName,
                    COOLDOWN_PLAY_MOVIE - (DateTime.Now - lastPlayTime).TotalMinutes);
                return;
            }

            // 检查积分是否溢出
            var movieScore = 0;
            if (!int.TryParse(scoreInCommand, out movieScore)) {
                Obs.MovieMessage.ShowFail("[{0}]: 点播 {1} 失败, 积分无效!", message.UserName, movieName);
                return;
            }
            movieScore = Math.Abs(movieScore);

            // 用户积分够吗?
            var userScore = UserService.GetScore(message.RoomId, message.UserId);
            if (userScore < movieScore) {
                Obs.MovieMessage.ShowFail("[{0}]: 点播 {1} 失败, 积分不够, 当前积分 {2}",
                    message.UserName, movieName, userScore);
                return;
            }

            // 更新积分
            MovieService.AddScore(message.RoomId, movieName, movieScore);
            UserService.AddScore(message.RoomId, message.UserId, message.UserName, movieScore * (-1));

            // 显示成功点播信息
            var rank = MovieService.GetMovieRank(message.RoomId, movieName);
            Obs.MovieMessage.PlayMovie(message.UserName, movieName, rank);
            return;
        }

        void ProcessGiftMessage(GiftMessage message)
        {
            try {
                // 送礼物, 赚积分
                var giftScore = ScoreService.CalScore(message);
                UserService.AddScore(message.RoomId, message.UserId, message.UserName, giftScore);

                // 感谢            
                var userScore = UserService.GetScore(message.RoomId, message.UserId);
                Obs.ThanksMessage.AddMessage("感谢 {0} 送的 {1}, 总积分 {2}",
                    message.UserName, message.GiftName, userScore);

                GiftMessage.SetProcessResult(message, ProcessResult.Ok);
                OnGiftMessageProcessed(message);
            } catch (Exception) {
                GiftMessage.SetProcessResult(message, ProcessResult.Error);
                throw;
            }
        }

        void ProcessChouqinMessage(ChouqinMessage message)
        {
            try {
                // 酬勤赚积分
                var chouqinScore = ScoreService.CalScore(message);
                UserService.AddScore(message.RoomId, message.UserId, message.UserName, chouqinScore);

                // 感谢
                var userScore = UserService.GetScore(message.RoomId, message.UserId);
                Obs.ThanksMessage.AddMessage("感谢 {0} 送的 {1}, 总积分 {2}",
                    message.UserName, message.ChouqinName, userScore);

                ChouqinMessage.SetProcessResult(message, ProcessResult.Ok); ;
                OnChouqinMessageProcessed(message);
            } catch (Exception) {
                ChouqinMessage.SetProcessResult(message, ProcessResult.Error);
                throw;
            }
        }

        #endregion

        #region events

        public event EventHandler<ProcessMessageEventArgs<ChatMessage>> ChatMessageProcessed;
        public event EventHandler<ProcessMessageEventArgs<GiftMessage>> GiftMessageProcessed;
        public event EventHandler<ProcessMessageEventArgs<ChouqinMessage>> ChouqinMessageProcessed;

        protected void OnChatMessageProcessed(ChatMessage message)
        {
            if (ChatMessageProcessed != null)
                ChatMessageProcessed(this, new ProcessMessageEventArgs<ChatMessage>(message));
        }

        protected void OnGiftMessageProcessed(GiftMessage message)
        {
            if (GiftMessageProcessed != null)
                GiftMessageProcessed(this, new ProcessMessageEventArgs<GiftMessage>(message));
        }

        protected void OnChouqinMessageProcessed(ChouqinMessage message)
        {
            if (ChouqinMessageProcessed != null)
                ChouqinMessageProcessed(this, new ProcessMessageEventArgs<ChouqinMessage>(message));
        }

        #endregion
    }
}
