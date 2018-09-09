using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using DouyuLiveAssistant.Properties;

namespace Douyu.Client
{
    public static class AppSettings
    {
        static IDbConnection _conn = new SqlConnection(Settings.Default.ConnectionString);

        public static int MovieCooldown
        {
            get
            {
                return _conn.ExecuteScalar<int>("select Value from AppSetting where Name = @Name",
                    new { Name = "MovieCooldown" });
            }
        }


        public static int RoomId { get { return 122402; } }
    }
}
