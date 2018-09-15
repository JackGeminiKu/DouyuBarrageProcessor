using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using DouyuLiveAssistant.Properties;
using Dapper;
using My.Log;

namespace Douyu.Messages
{
    public class GiftMessage : ServerMessage
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int UserLevel { get; set; }
        public int GiftId { get; set; }
        public string GiftName { get; set; }
        public int GiftPrice { get; set; }
        public int GiftExperience { get; set; }
        public double GiftDevote { get; set; }
        public int Weight { get; set; }
        public int Hits { get; set; }
        public int BadgeRoom { get; set; }
        public string BadgeName { get; set; }
        public int BadgeLevel { get; set; }
        public int ProcessResult { get; set; }

        static IDbConnection _conn;

        public static void SetProcessResult(GiftMessage message, ProcessResult result)
        {
            if (_conn == null)
                _conn = new SqlConnection(Settings.Default.ConnectionString);
            _conn.Execute("update GiftMessage set ProcessResult = @ProcessResult where Id = @Id",
                new { ProcessResult = result, Id = message.Id }
            );
        }
    }
}
