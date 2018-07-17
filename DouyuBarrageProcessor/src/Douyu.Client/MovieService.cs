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

        public static void AddScore(string roomId, string movieName, int movieScore)
        {
            _conn.Execute(
                "update MovieScore set MovieScore = MovieScore + @MovieScore where MovieName = @MovieName and RoomId = @RoomId",
                new { MovieScore = movieScore, MovieName = movieName, RoomId = roomId }
            );
        }

        public static bool HasMovie(string roomId, string movieName)
        {
            var count = _conn.ExecuteScalar(
                "select count(*) from MovieScore where RoomId = @RoomId and MovieName = @MovieName",
                new { RoomId = roomId, MovieName = movieName }
             );

            return (int)count == 1;
        }

        public static void GetTopMovies(string roomId, int count, ref List<string> movieNames, ref List<int> movieScores)
        {
            var movies = _conn.Query(
                "select top (@Count) MovieName, MovieScore from MovieScore where RoomId = @RoomId order by MovieScore desc",
                new { Count = count, RoomId = roomId }
            );
            foreach (var movie in movies) {
                movieNames.Add(movie.MovieName);
                movieScores.Add(movie.MovieScore);
            }
        }

        public static int GetMovieRank(string roomId, string movieName)
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

        public static string GetCurrentMovie(string roomId)
        {
            var currentMovie = _conn.ExecuteScalar<string>(
             "select Value from RoomInfo where Name = @Name and RoomId = @RoomId",
                new { Name = "current movie", RoomId = roomId }
            );
            return currentMovie;
        }

        public static string GetOfficialMovieName(string roomId, string aliasName)
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
