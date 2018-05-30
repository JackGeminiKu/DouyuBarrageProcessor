using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Dapper;
using System.Data.SqlClient;
using DouyuLiveAssistant.Properties;
using Jack4net.Log;

namespace Douyu.Messages
{
    public class ServerMessage : Message
    {
        public ServerMessage() { }

        public ServerMessage(string messageText)
        {
            MessageText = messageText;
            Items = ParseServerMessage(messageText);
        }

        public Dictionary<string, string> Items { get; private set; }

        Dictionary<string, string> ParseServerMessage(string messageData)
        {
            // 去除字符串末尾的'/0字符'
            messageData = messageData.Trim('\0');

            // 分析协议字段中的key和value值
            var items = new Dictionary<string, string>();
            var values = messageData.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in values) {
                int separatorStart = item.IndexOf("@=", StringComparison.Ordinal);
                string key = item.Substring(0, separatorStart);
                var value = item.Substring(separatorStart + 2);

                // 子序列化暂时不处理!!!!
                ////// 如果value值中包含子序列化值，则进行递归分析
                ////if (((string)value).Contains("@A")) {
                ////    value = ((string)value).Replace("@S", "/").Replace("@A", "@");
                ////    value = this.ParseRespond((string)value);
                ////}

                items.Add(key, value);
            }

            return items;
        }

        protected override int MessageType
        {
            get { return 690; }
        }

        public override string ToString()
        {
            return MessageText;
        }

        static IDbConnection _connection;

        static ServerMessage()
        {
            _connection = new SqlConnection(Settings.Default.ConnectionString);
        }

        public static ServerMessage[] GetMessages(int roomId)
        {
            const int TOP_COUNT = 10;
            List<ServerMessage> messages = new List<ServerMessage>();

            // 获取弹幕
            var chatMessages = _connection.Query<ChatMessage>(
                "select top(@Count) Id, Time, Text, RoomId, UserId, UserName, UserLevel, BadgeName, BadgeLevel, BadgeRoom" +
                " where Processed = 0 and RoomId = @RoomId order by Id asc",
                new { Count = TOP_COUNT, RoomId = roomId }
            );
            foreach (var message in chatMessages) {
                messages.Add(message);
            }

            // 获取礼物
            var giftMessages = _connection.Query<GiftMessage>(
                "select top(@Count) Id, Time, RoomId, UserId, UserName, UserLevel, Weight, GiftId, Hits, BadgeName, BadgeLevel, BadgeRoom" +
                " where Processed = 0 and RoomId = @RoomId order by Id asc",
                new { Count = TOP_COUNT, RoomId = roomId }
            );
            foreach (var giftMessage in giftMessages) {
                var giftCategory = _connection.Query(
                    "select Name, Experience from GiftCategory where Id = @Id",
                     new { Id = giftMessage.Id }
                );
                if (giftCategory.Count() == 0) {
                    _connection.Execute(
                        "update gift_message set processed = 2 where id = @Id",
                        new { Id = giftMessage.Id });
                    LogService.ErrorFormat("系统中没有id为{0}的礼物", giftMessage.Id);
                    continue;
                }
                giftMessage.GiftName = giftCategory.Name;
                giftMessage.GiftExperience = giftCategory.Experience;
                messages.Add(giftMessage);
            }

            // 获取酬勤
            var chouqinMessages = _connection.Query<ChouqinMessage>(
                "select top(@Count) Id, Time, RoomId, UserId, UserLevel, Level, Count, Hits, BadgeName, BadgeLevel, BadgeRoom" +
                " where Processed = 0 and RoomId = @RoomId order by Id asc",
                new { Count = TOP_COUNT, RoomId = roomId }
            );
            foreach (var message in chouqinMessages) {
                messages.Add(message);
            }

            return messages.ToArray();
        }
    }
}
