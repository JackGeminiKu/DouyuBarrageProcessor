using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using DouyuLiveAssistant.Properties;
using Dapper;

namespace Douyu.Messages
{
    public class ChouqinMessage : ServerMessage
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public byte Level { get; set; }
        public short Count { get; set; }
        public short Hits { get; set; }
        public int UserId { get; set; }
        public string UserName { get; private set; }
        public byte UserLevel { get; set; }
        public string BadgeName { get; set; }
        public byte BadgeLevel { get; set; }
        public int BadgeRoom { get; set; }
        public string ChouqinName { get { return Level == 1 ? "初级酬勤" : (Level == 2 ? "中级酬勤" : "高级酬勤"); } }
        
        static IDbConnection _connection;

        static ChouqinMessage()
        {
            _connection = new SqlConnection(Settings.Default.ConnectionString);
        } 

        public static ServerMessage[] GetMessages(int roomId)
        {
            const int TOP_COUNT = 10;
            List<ServerMessage> messages = new List<ServerMessage>();

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
