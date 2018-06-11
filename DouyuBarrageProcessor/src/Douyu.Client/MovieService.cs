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

        public static void AddMovieScore(int roomId, string movieName, int movieScore)
        {
            _conn.Execute(
                "update MovieScore set MovieScore = MovieScore + @MovieScore where MovieName = @MovieName and RoomId = @RoomId",
                new { MovieScore = movieScore, MovieName = movieName, RoomId = roomId }
            );
        }

        public static bool HasMovie(int roomId, string movieName)
        {
            var count = _conn.ExecuteScalar(
                "select count(*) from MovieScore where RoomId = @RoomId and MovieName = @MovieName",
                new { RoomId = roomId, MovieName = movieName }
             );

            return (int)count == 1;
        }

        public static void GetTopMovies(int roomId, int count, ref List<string> movieNames, ref List<int> movieScores)
        {
            var movies = _conn.Query(
                "select top (@Count) MovieName, MovieScore from MovieScore where RoomId = @RoomId order by MovieScore desc",
                new { Count = count, Roomid = roomId }
            );
            foreach (var movie in movies) {
                movieNames.Add(movie.MovieName);
                movieScores.Add(movie.MovieScore);
            }
        }

        public static int GetMovieRank(int roomId, string movieName)
        {
            var movies = _conn.Query(
                "select MovieName from MovieScore where RoomId = @RoomId order by MovieScore desc",
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
            // TBD: 还没改好 Confi表改为RoomInfo表?
            var config = _conn.Query(
                "select * from Config where name = 'current movie' and RoomId = @Roomid",
                new { RoomId = roomId }
                );
            return config.First().value;
        }

        public static string GetOfficialMovieName(int roomId, string aliasName)
        {
            var movieAlias = _conn.Query(
                "select MovieName from MovieAlias where RoomId = @RoomId and MovieAlias = @AliasName",
                new { RoomId = roomId, AliasName = aliasName }
            );

            if (movieAlias.Count() != 1) {
                return "";
            }

            return movieAlias.First().MovieName;
        }
    }
}
