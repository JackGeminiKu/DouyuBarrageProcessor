using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data;
using Jack4net.Log;
using Douyu.Messages;
using System.Data.SqlClient;
using Dapper;
using DouyuLiveAssistant.Properties;

namespace Douyu.Client
{
    public static class MessageService
    {
        static IDbConnection _conn;

        static MessageService()
        {
            _conn = new SqlConnection(Settings.Default.ConnectionString);
        }

        public static void SetChatMessageProcessed(ChatMessage message)
        {
            _conn.Execute("update ChatMessage set Processed = 1 where Id = @Id", new { Id = message.Id });
        }

        public static void SetGiftMessageProcessed(GiftMessage message)
        {
            _conn.Execute("update GiftMessage set Processed = 1 where Id = @Id", new { Id = message.Id });
        }

        public static void SetChouqinMessageProcessed(ChouqinMessage message)
        {
            _conn.Execute("update ChouqinMessage set Processed = 1 where Id = @Id", new { Id = message.Id });
        }
    }
}
