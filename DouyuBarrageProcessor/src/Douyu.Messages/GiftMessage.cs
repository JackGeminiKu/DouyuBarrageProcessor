using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using DouyuLiveAssistant.Properties;
using Dapper;
using Jack4net.Log;

namespace Douyu.Messages
{
    public class GiftMessage : ServerMessage
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int UserLevel { get; set; }
        public int Weight { get; set; }
        public int GiftId { get; set; }
        public string GiftName { get; set; }
        public double GiftExperience { get; set; }
        public int Hits { get; set; }
        public string BadgeName { get; set; }
        public int BadgeLevel { get; set; }
        public int BadgeRoom { get; set; }
        
        static IDbConnection _connection;

        static GiftMessage()
        {
            _connection = new SqlConnection(Settings.Default.ConnectionString);
        } 

        public static ServerMessage[] GetMessages(int roomId)
        {
            const int TOP_COUNT = 10;
            List<ServerMessage> messages = new List<ServerMessage>();

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
                giftMessage.GiftName = giftCategory.First().Name;
                giftMessage.GiftExperience = giftCategory.First().Experience;
                messages.Add(giftMessage);
            }

            return messages.ToArray();
        }
    }
}
