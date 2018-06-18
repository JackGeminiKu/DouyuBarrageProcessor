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
    public class ChatMessage : ServerMessage
    {
        public int Id { get; set; }
        public DateTime Time { get; set; }
        public string Text { get; set; }
        public int RoomId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int UserLevel { get; set; }
        public string BadgeName { get; set; }
        public int BadgeLevel { get; set; }
        public int BadgeRoom { get; set; }

        static IDbConnection _conn;

        public static void SetProcessResult(ChatMessage message, ProcessResult result)
        {
            if (_conn == null)
                _conn = new SqlConnection(Settings.Default.ConnectionString);
            _conn.Execute("update ChatMessage set ProcessResult = @ProcessResult where Id = @Id",
                new { ProcessResult = result, Id = message.Id }
            );
        }
    }
}
