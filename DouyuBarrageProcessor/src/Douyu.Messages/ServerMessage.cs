using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Dapper;
using System.Data.SqlClient;
using DouyuLiveAssistant.Properties;
using My.Log;

namespace Douyu.Messages
{
    public abstract class ServerMessage
    {
        public DateTime Time { get; set; }

        static IDbConnection _connection;

        public static ServerMessage[] GetMessages(int roomId)
        {
            if (_connection == null)
                _connection = new SqlConnection(Settings.Default.ConnectionString);

            const int TOP_COUNT = 10;
            var messages = new List<ServerMessage>();

            // 获取弹幕
            var chatMessages = _connection.Query<ChatMessage>(
                "select top(@Count) Id, Time, SendingTime, RoomId, UserId, UserName, UserLevel, Text, BadgeRoom, BadgeName, BadgeLevel " +
                "from ChatMessage " +
                "where RoomId = @RoomId and ProcessResult = 0 " +
                "order by Id asc",
                new { Count = TOP_COUNT, RoomId = roomId }
            );
            foreach (var message in chatMessages) {
                messages.Add(message);
            }

            // 获取礼物
            var giftMessages = _connection.Query<GiftMessage>(
                "select top(@Count) Id, Time, RoomId, UserId, UserName, UserLevel, " +
                "GiftId, GiftName, GiftPrice, GiftExperience, GiftDevote, " +
                "Weight, Hits, BadgeRoom, BadgeName, BadgeLevel " +
                "from GiftMessage " +
                "where RoomId = @RoomId and ProcessResult = 0 " +
                "order by Id asc",
                new { Count = TOP_COUNT, RoomId = roomId }
            );
            foreach (var giftMessage in giftMessages) {
                messages.Add(giftMessage);
            }

            // 获取酬勤
            var chouqinMessages = _connection.Query<ChouqinMessage>(
                "select top(@Count) Id, Time, RoomId, UserId, UserLevel, Level, Count, Hits, " +
                "BadgeRoom, BadgeName, BadgeLevel " +
                "from ChouqinMessage " +
                "where RoomId = @RoomId and ProcessResult = 0 " +
                "order by Id asc",
                new { Count = TOP_COUNT, RoomId = roomId }
            );
            foreach (var message in chouqinMessages) {
                messages.Add(message);
            }

            return messages.ToArray();
        }
    }
}
