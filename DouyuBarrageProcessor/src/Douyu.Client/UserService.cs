using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using DouyuLiveAssistant.Properties;
using Jack4net.Log;

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

        public static void AddUserScore(int roomId, int userId, string userName, double userScore)
        {
            try {
                var result = _conn.ExecuteScalar(
                    "select count(*) from UserScore where RoomId = @RoomId and UserId = @UserId",
                    new { RoomId = roomId, UserId = userId }
                );

                if ((int)result == 1) {    // 有积分记录
                    var count = _conn.Execute(
                        "update UserScore set UserScore = UserScore + @UserScore, UserName = @UserName where RoomId = @RoomId and UserId = @UserId",
                        new { UserScore = userScore, UserName = userName, RoomId = roomId, UserId = userId }
                    );
                } else {    // 无积分记录
                    var count = _conn.Execute(
                        "insert into UserScore(RoomId, UserId, UserName, UserScore) values(@RoomId, @UserId, @UserName, @UserScore)",
                        new { RoomId = roomId, UserId = userId, UserName = userName, UserScore = userScore }
                    );
                }
            } catch (Exception ex) {
                LogService.Error("增加用户积分失败!", ex);
            }
        }

        public static int GetUserScore(int roomId, int userId)
        {
            // 找不到这个人, 是新人, 返回0分
            var count = _conn.ExecuteScalar(
                "select count(*) from UserScore where RoomId = @RoomId and UserId = @UserId",
                new { RoomId = roomId, UserId = userId }
            );
            if ((int)count == 0)
                return 0;

            var score = _conn.ExecuteScalar(
                "select UserScore from UserScore where RoomId = @RoomId and UserId = @UserId",
                new { RoomId = roomId, UserId = userId }
            );
            return (int)score;
        }

        public static void GetTopUsers(int roomId, int count, ref List<string> names, ref List<int> scores)
        {
            var users = _conn.Query(
                "select top (@Count) UserName, UserScore from UserScore where RoomId = @RoomId order by UserScore desc",
                new { Count = count, RoomId = roomId }
            );
            foreach (var user in users) {
                names.Add(user.UserName);
                scores.Add(user.UserScore);
            }
        }
    }
}
