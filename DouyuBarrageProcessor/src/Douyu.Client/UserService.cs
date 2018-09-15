using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using DouyuLiveAssistant.Properties;
using My.Log;

namespace Douyu.Client
{
    public static class UserService
    {
        static IDbConnection _conn;

        static UserService()
        {
            _conn = new SqlConnection(Settings.Default.ConnectionString);
            _conn.Open();
        }

        public static void AddScore(int roomId, int userId, string userName, double userScore)
        {
            var scoreCount = _conn.ExecuteScalar<int>(
                "select count(*) from UserScore " +
                "where RoomId = @RoomId and UserId = @UserId",
                new { RoomId = roomId, UserId = userId }
            );

            if (scoreCount == 1) {    // 有积分记录
                _conn.Execute(
                 "update UserScore " +
                 "set UserScore = UserScore + @UserScore, UserName = @UserName " +
                 "where RoomId = @RoomId and UserId = @UserId",
                 new { UserScore = userScore, UserName = userName, RoomId = roomId, UserId = userId }
             );
            } else {    // 无积分记录
                _conn.Execute(
                 "insert into " +
                 "UserScore(RoomId, UserId, UserName, UserScore) " +
                 "values(@RoomId, @UserId, @UserName, @UserScore)",
                 new { RoomId = roomId, UserId = userId, UserName = userName, UserScore = userScore }
             );
            }
        }

        public static int GetScore(int roomId, int userId)
        {
            var score = _conn.ExecuteScalar<int>( // 找不到这个人, 是新人, 返回0分
                "select UserScore from UserScore " +
                "where RoomId = @RoomId and UserId = @UserId",
                new { RoomId = roomId, UserId = userId }
            );
            return score;
        }

        public static void GetTopUsers(int roomId, int count, ref List<string> names, ref List<int> scores)
        {
            var users = _conn.Query(
                "select top (@Count) UserName, UserScore from UserScore " +
                "where RoomId = @RoomId " +
                "order by UserScore desc",
                new { Count = count, RoomId = roomId }
            );
            foreach (var user in users) {
                names.Add(user.UserName);
                scores.Add(user.UserScore);
            }
        }
    }
}
