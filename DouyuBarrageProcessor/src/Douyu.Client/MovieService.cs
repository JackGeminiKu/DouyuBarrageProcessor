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
                "update RoomMovie " +
                "set MovieScore = MovieScore + @MovieScore " +
                "where RoomId = @RoomId and MovieId = @MovieId",
                new { MovieScore = movieScore, RoomId = roomId, MovieId = GetMovieId(roomId, movieName) }
            );
        }

        static int GetMovieId(int roomId, string movieName)
        {
            var movieId = _conn.ExecuteScalar<int>(
                "select Movie.Id from RoomMovie " +
                "inner join Movie on RoomMovie.MovieId = Movie.Id " +
                "where RoomMovie.RoomId = @RoomId and Movie.MovieName = @MovieName",
                new { RoomId = roomId, MovieName = movieName });
            return movieId;
        }

        public static bool HasMovie(int roomId, string movieName)
        {
            var count = _conn.ExecuteScalar<int>(
                "select count(*) " +
                "from RoomMovie inner join Movie on RoomMovie.MovieId = Movie.Id " +
                "where RoomMovie.RoomId = @RoomId and Movie.MovieName = @MovieName",
                new { RoomId = roomId, MovieName = movieName }
             );

            return count == 1;
        }

        public static void GetTopMovies(int roomId, int count, ref List<string> movieNames, ref List<int> movieScores)
        {
            var movies = _conn.Query(
                "select top (@Count) Movie.MovieName, RoomMovie.MovieScore " +
                "from RoomMovie inner join Movie on RoomMovie.MovieId = Movie.Id " +
                "where RoomMovie.RoomId = @RoomId and Movie.Id not in (select MovieId from MovieBlacklist) " +
                "order by RoomMovie.MovieScore desc",
                new { Count = count, RoomId = roomId }
            );
            foreach (var movie in movies) {
                movieNames.Add(movie.MovieName);
                movieScores.Add(movie.MovieScore);
            }
        }

        public static bool IsMovieOnBlacklist(int roomId, string movieName)
        {
            var count = _conn.ExecuteScalar<int>(
                "select count(*) from MovieBlacklist where MovieId = @MovieId",
                new { MovieId = GetMovieId(roomId, movieName) });
            return count >= 1;
        }

        public static int GetMovieRank(int roomId, string movieName)
        {
            var movies = _conn.Query(
                "select Movie.MovieName " +
                "from RoomMovie inner join Movie on RoomMovie.MovieId = Movie.Id " +
                "where RoomMovie.RoomId = @RoomId " +
                "order by RoomMovie.MovieScore desc",
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
                new { RoomId = roomId, Name = "CurrentMovie" }
            );
            return currentMovie;
        }

        public static string GetOfficialMovieName(int roomId, string aliasName)
        {
            var movieAlias = _conn.Query(
                "select Movie.MovieName " +
                "from MovieAlias inner join Movie on MovieAlias.MovieId = Movie.Id " +
                "where MovieAlias.AliasName = @AliasName " +
                "and Movie.Id in (select RoomMovie.MovieId from RoomMovie where RoomMovie.RoomId = @RoomId)",
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
                "select LastPlayTime from RoomMovie " +
                "where RoomId = @RoomId and MovieId = @MovieId",
                new { RoomId = roomId, MovieId = GetMovieId(roomId, movieName) }
            );

            return lastPlayTime;
        }
    }
}
