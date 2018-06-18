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
        public string ChouqinName
        {
            get
            {
                switch (Level) {
                    case 1:
                        return "初级酬勤";
                    case 2:
                        return "中级酬勤";
                    case 3:
                        return "高级酬勤";
                    default:
                        return "未知酬勤";
                }
            }
        }

        static IDbConnection _conn;

        public static void SetProcessResult(ChouqinMessage message, ProcessResult result)
        {
            if (_conn == null)
                _conn = new SqlConnection(Settings.Default.ConnectionString);
            _conn.Execute("update ChouqinMessage set ProcessResult = @ProcessResult where Id = @Id",
                new { ProcessResult = result, Id = message.Id }
            );
        }
    }
}
