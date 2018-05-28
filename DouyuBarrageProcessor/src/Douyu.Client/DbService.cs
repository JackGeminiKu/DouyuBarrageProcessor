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

        public static ServerMessage[] GetMessages(int roomId)
        {
            const int TOP_COUNT = 10;
            IDataReader reader = null;
            List<ServerMessage> messages = new List<ServerMessage>();

            // 获取弹幕
            try {
                reader = ExecuteReader(
                    "select top({0}) * from chat_message where processed = 0 and room_id = {1} order by id asc",
                   TOP_COUNT, roomId
                );
                while (reader.Read()) {
                    ChatMessage chatMessage = new ChatMessage();
                    chatMessage.Id = (int)reader["id"];
                    chatMessage.Time = (DateTime)reader["time"];
                    chatMessage.Text = (string)reader["text"];
                    chatMessage.RoomId = (int)reader["room_id"];
                    chatMessage.UserId = (int)reader["user_id"];
                    chatMessage.UserName = (string)reader["user_name"];
                    chatMessage.UserLevel = (byte)reader["user_level"];
                    chatMessage.BadgeName = (string)reader["badge_name"];
                    chatMessage.BadgeLevel = (byte)reader["badge_level"];
                    chatMessage.BadgeRoom = (int)reader["badge_room"];
                    messages.Add(chatMessage);
                }
            } finally {
                if (reader != null) reader.Close();
            }

            // 获取礼物
            try {
                reader = ExecuteReader(
                    "select top({0}) * from gift_message where processed = 0 and room_id = {1} order by id asc",
                    TOP_COUNT, roomId
                );
                while (reader.Read()) {
                    // 没找到该礼物信息?
                    int giftId = (int)reader["gift_id"];
                    Dictionary<string, object> giftInfo = DbService.QueryGiftInfo(giftId);
                    if (giftInfo == null) {
                        // set processed = 2, 表示没有找到该礼物的相关信息
                        if (ExecuteNonQuery("update gift_message set processed = 2 where id = {0}", reader["id"]) != 1)
                            LogService.ErrorFormat(
                                "将礼物消息中的processed设置为2失败, 礼物id为{0}", giftId
                            );
                        LogService.ErrorFormat("系统中没有id为{0}的礼物", giftId);
                        continue;
                    }

                    // 保存礼物信息
                    GiftMessage giftMessage = new GiftMessage();
                    giftMessage.Id = (int)reader["id"];
                    giftMessage.Time = (DateTime)reader["time"];
                    giftMessage.RoomId = (int)reader["room_id"];
                    giftMessage.UserId = (int)reader["user_id"];
                    giftMessage.UserName = (string)reader["user_name"];
                    giftMessage.UserLevel = (byte)reader["user_level"];
                    giftMessage.Weight = (int)reader["weight"];
                    giftMessage.GiftId = (int)reader["gift_id"];
                    giftMessage.GiftName = (string)giftInfo["name"];
                    giftMessage.GiftExperience = (float)giftInfo["experience"];
                    giftMessage.Hits = (short)reader["hits"];
                    giftMessage.BadgeName = (string)reader["badge_name"];
                    giftMessage.BadgeLevel = (byte)reader["badge_level"];
                    giftMessage.BadgeRoom = (int)reader["badge_room"];
                    messages.Add(giftMessage);
                }
            } finally {
                if (reader != null) reader.Close();
            }

            // 获取酬勤
            try {
                reader = ExecuteReader(
                    "select top({0}) * from chouqin_message where processed = 0 and room_id = {1} order by id asc",
                    TOP_COUNT, roomId
                );
                while (reader.Read()) {
                    ChouqinMessage chouqinMessage = new ChouqinMessage();
                    chouqinMessage.Id = (int)reader["id"];
                    chouqinMessage.Time = (DateTime)reader["time"];
                    chouqinMessage.RoomId = (int)reader["room_id"];
                    chouqinMessage.UserId = (int)reader["user_id"];
                    chouqinMessage.UserLevel = (byte)reader["user_level"];
                    chouqinMessage.Level = (byte)reader["level"];
                    chouqinMessage.Count = (short)reader["count"];
                    chouqinMessage.Hits = (short)reader["hits"];
                    chouqinMessage.BadgeName = (string)reader["badge_name"];
                    chouqinMessage.BadgeLevel = (byte)reader["badge_level"];
                    chouqinMessage.BadgeRoom = (int)reader["badge_room"];
                    messages.Add(chouqinMessage);
                }
            } finally {
                if (reader != null) reader.Close();
            }

            return messages.ToArray();
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

        public static void GetTopUsers(int roomId, out List<string> names, out List<int> scores)
        {
            SqlDataReader reader = null;
            try {
                string sql = string.Format(
                    "select top (10) user_name, user_score from user_score where room_id = {0} order by user_score desc",
                    roomId
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

        public static void GetTopMovies(int roomId, List<string> movieNames, List<int> movieScores)
        {
            SqlDataReader reader = null;
            try {
                string sql = string.Format(
                    "select top (10) movie_name, movie_score from movie_score where room_id = {0} order by movie_score desc",
                    roomId
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
