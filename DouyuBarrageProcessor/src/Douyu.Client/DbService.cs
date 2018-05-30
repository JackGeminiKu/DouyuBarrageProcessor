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

namespace Douyu.Client
{
    public static class DbService
    {
        private static IDbConnection _conn;

        static DbService()
        {
            string connectString = @"Data Source=10.0.0.2;Initial Catalog=Douyu2;User ID=sa;Password=Jack52664638;"
                + "MultipleActiveResultSets=true";
            _conn = new SqlConnection(connectString);
            _conn.Open();
        }

     

        #region "用户积分相关"

        public static void AddUserScore(int roomId, int userId, string userName, double userScore)
        {
            string command = string.Format(
                "select count(*) from user_score where room_id = {0} and user_id = {1}", roomId, userId
            );
            if (ExecuteScalar<int>(command) == 1) {    // 有积分记录
                command = string.Format(
                    "update user_score set user_score = user_score + {0}, user_name = '{1}' where room_id = {2} and user_id = {3}",
                    userScore, userName, roomId, userId
                );
            } else {    // 无积分记录
                command = string.Format("insert into user_score(room_id, user_id, user_name, user_score) values({0}, {1}, '{2}', {3})",
                    roomId, userId, userName, userScore);
            }
            if (ExecuteNonQuery(command) != 1) {
                LogService.Error("保存弹幕积分失败: " + command);
                return;
            }
        }

        public static int QueryUserScore(int roomId, int userId)
        {
            // 如果没查到这个人呢? 积分为0?
            int score = ExecuteScalar<int>(
                "select user_score from user_score where room_id = {0} and user_id = {1}",
                roomId, userId
            );
            return score;
        }

        public static void GetTopUsers(int roomId, int count, out List<string> names, out List<int> scores)
        {
            SqlDataReader reader = null;
            try {
                string sql = string.Format(
                    "select top ({0}) user_name, user_score from user_score where room_id = {1} order by user_score desc",
                    count, roomId
                );
                reader = CreateCommand(sql).ExecuteReader();
                names = new List<string>();
                scores = new List<int>();
                while (reader.Read()) {
                    names.Add((string)reader["user_name"]);
                    scores.Add((int)reader["user_score"]);
                }
                return;
            } finally {
                if (reader != null) reader.Close();
            }
        }

        #endregion

        #region "电影相关"

        public static void AddMovieScore(int roomId, string movieName, int movieScore)
        {
            string command = string.Format(
                "update movie_score set movie_score = movie_score + {0} where movie_name = '{1}' and room_id = {2}",
                movieScore, movieName, roomId
            );
            if (ExecuteNonQuery(command) != 1)
                LogService.ErrorFormat("更新电影积分失败: {0}", command);
        }

        public static void GetTopMovies(int roomId, int count, List<string> movieNames, List<int> movieScores)
        {
            SqlDataReader reader = null;
            try {
                string sql = string.Format(
                    "select top ({0}) movie_name, movie_score from movie_score where room_id = {1} order by movie_score desc",
                    count, roomId
                );
                reader = CreateCommand(sql).ExecuteReader();
                while (reader.Read()) {
                    movieNames.Add((string)reader["movie_name"]);
                    movieScores.Add((int)reader["movie_score"]);
                }
                return;
            } finally {
                if (reader != null) reader.Close();
            }
        }

        public static bool HasMovie(int roomId, string movieName)
        {
            return ExecuteScalar<int>(
                "select count(*) from movie_score where room_id = {0} and movie_name = '{1}'", roomId, movieName
            ) == 1;
        }

        public static int GetMovieRank(string movieName, int roomId)
        {
            IDataReader reader = ExecuteReader(
                "select * from movie_score where room_id = {0} order by movie_score desc", roomId
            );

            int i = 0;
            while (reader.Read()) {
                if ((string)reader["movie_name"] == movieName) {
                    return i;
                }
                ++i;
            }
            return -1;
        }

        public static string GetCurrentMovie(int roomId)
        {
            var config = _conn.Query("select * from config where name = 'current movie' and room_id = " + roomId);
            return config.First().value;
        }

        public static string GetOfficialMovieName(int roomId, string aliasName)
        {
            var movieAlias = _conn.Query("select * from movie_alias where room_id = @RoomId and movie_alias = @AliasName",
                new { RoomId = roomId, AliasName = aliasName });

            if (movieAlias.Count() != 1) {
                return "";
            }

            return movieAlias.First().movie_name;
        }

        #endregion

        public static void SetChatMessageProcessed(ChatMessage message)
        {
            string command = string.Format("update chat_message set processed = 1 where id = {0}", message.Id);
            if (ExecuteNonQuery(command) != 1)
                LogService.ErrorFormat("弹幕标记为已处理失败: {0}", command);
        }

        public static void SetGiftMessageProcessed(GiftMessage message)
        {
            string command = string.Format("update gift_message set processed = 1 where id = {0}", message.Id);
            if (ExecuteNonQuery(command) != 1)
                LogService.ErrorFormat("礼物标记为已处理失败: {0}", command);
        }

        public static void SetChouqinMessageProcessed(ChouqinMessage message)
        {
            string command = string.Format("update chouqin_message set processed = 1 where id = {0}", message.Id);
            if (ExecuteNonQuery(command) != 1)
                LogService.ErrorFormat("酬勤标记为已处理失败: {0}", command);
        }

        #region "SQL functions"

        static SqlCommand CreateCommand(string sql)
        {
            if (_conn.State != ConnectionState.Open)
                _conn.Open();
            SqlCommand command = new SqlCommand(sql, _conn as SqlConnection);
            command.CommandTimeout = 60;
            return command;
        }

        static int ExecuteNonQuery(string sql)
        {
            try {
                LogService.Debug("[SQL] " + sql);
                return CreateCommand(sql).ExecuteNonQuery();
            } catch (Exception) {
                LogService.Debug("[SQL-2] " + sql);
                return CreateCommand(sql).ExecuteNonQuery();
            }
        }

        static int ExecuteNonQuery(string sql, params object[] args)
        {
            return ExecuteNonQuery(string.Format(sql, args));
        }

        static T ExecuteScalar<T>(string sql)
        {
            try {
                LogService.Debug("[SQL] " + sql);
                return (T)CreateCommand(sql).ExecuteScalar();
            } catch (Exception) {
                LogService.Debug("[SQL-2] " + sql);
                return (T)CreateCommand(sql).ExecuteScalar();
            }
        }

        static T ExecuteScalar<T>(string sql, params object[] args)
        {
            return ExecuteScalar<T>(string.Format(sql, args));
        }

        static IDataReader ExecuteReader(string sql)
        {
            try {
                LogService.Debug("[SQL] " + sql);
                return CreateCommand(sql).ExecuteReader();
            } catch (Exception) {
                LogService.Debug("[SQL-2] " + sql);
                return CreateCommand(sql).ExecuteReader();
            }
        }

        static IDataReader ExecuteReader(string sql, params object[] args)
        {
            return ExecuteReader(string.Format(sql, args));
        }

        #endregion

        #region "礼物相关"

        public static Dictionary<string, object> QueryGiftInfo(int giftId)
        {
            SqlDataReader reader = null;
            try {
                string sql = "select id, name, price, experience from gift_category where id = " + giftId;
                reader = CreateCommand(sql).ExecuteReader();
                if (reader.Read() == false)
                    return null;

                Dictionary<string, object> giftInfo = new Dictionary<string, object>();
                giftInfo["id"] = giftId;
                giftInfo["name"] = reader["name"];
                giftInfo["price"] = reader["price"];
                giftInfo["experience"] = reader["experience"];
                return giftInfo;
            } finally {
                if (reader != null) reader.Close();
            }
        }

        #endregion
    }
}
