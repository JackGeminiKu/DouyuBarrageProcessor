using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using DouyuLiveAssistant.Properties;
using Dapper;

namespace Douyu.Client
{
    public static class MovieService
    {
        static IDbConnection _conn;

        static MovieService()
        {
            _conn = new SqlConnection(Settings.Default.ConnectionString);
            _conn.Open();
        }

        public static void AddScore(int roomId, string movieName, int movieScore)
        {
            _conn.Execute(
                "update MovieScore " +
                "set MovieScore = MovieScore + @MovieScore " +
                "where RoomId = @RoomId and MovieName = @MovieName",
                new { MovieScore = movieScore, RoomId = roomId, MovieName = movieName }
            );
        }

        public static bool HasMovie(int roomId, string movieName)
        {
            var count = _conn.ExecuteScalar<int>(
                "select count(*) " +
                "from MovieScore " +
                "where RoomId = @RoomId and MovieName = @MovieName",
                new { RoomId = roomId, MovieName = movieName }
             );

            return count == 1;
        }

        public static void GetTopMovies(int roomId, int count, ref List<string> movieNames, ref List<int> movieScores)
        {
            var movies = _conn.Query(
                "select top (@Count) MovieName, MovieScore " +
                "from MovieScore " +
                "where RoomId = @RoomId and MovieName not in (select MovieName from MovieBlacklist)" +
                "order by MovieScore desc",
                new { Count = count, RoomId = roomId }
            );
            foreach (var movie in movies) {
                movieNames.Add(movie.MovieName);
                movieScores.Add(movie.MovieScore);
            }
        }

        public static bool IsBannedMovie(string movieName)
        {
            var count = _conn.ExecuteScalar<int>("select count(*) from MovieBlacklist where MovieName = @MovieName",
                new { MovieName = movieName }
            );
            return count >= 1;
        }

        public static int GetMovieRank(int roomId, string movieName)
        {
            var movies = _conn.Query(
                "select MovieName " +
                "from MovieScore " +
                "where RoomId = @RoomId " +
                "order by MovieScore desc",
                new { RoomId = roomId }
            );
            for (var i = 0; i < movies.Count(); i++) {
                if (movies.ElementAt(i).MovieName == movieName) {
                    return i;
                }
            }
            return -1;
        }

        public static string GetCurrentMovie(int roomId)
        {
            var currentMovie = _conn.ExecuteScalar<string>( // 没找到返回null
             "select Value from RoomInfo " +
             "where RoomId = @RoomId and Name = @Name",
                new { RoomId = roomId, Name = "current movie" }
            );
            return currentMovie;
        }

        public static string GetOfficialMovieName(int roomId, string aliasName)
        {
            var movieAlias = _conn.Query(
                "select MovieName from MovieAlias " +
                "where RoomId = @RoomId and MovieAlias = @AliasName",
                new { RoomId = roomId, AliasName = aliasName }
            );

            if (movieAlias.Count() != 1) {
                return "";
            }

            return movieAlias.First().MovieName;
        }

        public static DateTime GetLastPlayTime(int roomId, string movieName)
        {
            var lastPlayTime = _conn.ExecuteScalar<DateTime>(
                "select LastPlayTime from MovieScore " +
                "where RoomId = @RoomId and MovieName = @MovieName",
                new { RoomId = roomId, MovieName = movieName }
            );

            return lastPlayTime;
        }
    }
}
