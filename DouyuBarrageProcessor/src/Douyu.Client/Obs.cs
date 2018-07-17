using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using DouyuLiveAssistant.Properties;
using Jack4net.Log;

namespace Douyu.Client
{
    public static class Obs
    {
        public static void Initialize(string roomId)
        {
            if (!Directory.Exists(ObsDir))
                Directory.CreateDirectory(ObsDir);

            TopMovies = new TopMovies(roomId);
            TopMovies.Update();

            TopUsers = new TopUsers(roomId);
            TopUsers.Update();

            MovieMessage = new MovieMessage();
            ThanksMessage = new ThanksMessage();
            OtherMessage = new OtherMessage();
        }

        public static TopMovies TopMovies { get; private set; }
        public static TopUsers TopUsers { get; private set; }

        public static MovieMessage MovieMessage { get; private set; }
        public static ThanksMessage ThanksMessage { get; private set; }
        public static OtherMessage OtherMessage { get; private set; }

        public static string ObsDir
        {
            get { return JApplication.RootPath + @"OBS\"; }
        }
    }



    public class TopMovies
    {
        readonly string TopMoviesFile = Obs.ObsDir + "TopMovies.txt";
        const int MOVIE_COUNT = 10;

        public TopMovies(string roomId)
        {
            RoomId = roomId;
        }

        public string RoomId { get; private set; }

        public void Update()
        {
            var names = new List<string>();
            var scores = new List<int>();
            MovieService.GetTopMovies(RoomId, MOVIE_COUNT, ref names, ref scores);
            var topMovies = "";
            for (var i = 0; i < names.Count; i++) {
                topMovies += (topMovies == "" ? "" : "\n")
                    + string.Format("[{0:D2}] #{1}-{2}", i + 1, names[i], scores[i]);
            }

            if (_topMovies != topMovies) {
                File.WriteAllText(TopMoviesFile, topMovies);
                _topMovies = topMovies;
            }
        }

        private string _topMovies = "";
    }



    public class TopUsers
    {
        readonly string TopUsersFile = Obs.ObsDir + "TopUsers.txt";

        const int USER_COUNT = 10;

        public TopUsers(string roomId)
        {
            RoomId = roomId;
        }

        public string RoomId { get; private set; }

        public void Update()
        {
            var names = new List<string>();
            var scores = new List<int>();
            var topUsers = "";
            UserService.GetTopUsers(RoomId, USER_COUNT, ref names, ref scores);
            for (var i = 0; i < names.Count; i++) {
                topUsers += (topUsers == "" ? "" : "\n")
                    + string.Format("[{0:D2}] {1} {2}", i + 1, names[i], scores[i]);
            }

            if (_topUsers != topUsers) {
                File.WriteAllText(TopUsersFile, topUsers);
                _topUsers = topUsers;
            }
        }

        private string _topUsers = "";
    }



    public class MovieMessage
    {
        const int MAX_TIME = 10;
        readonly string UserName = Obs.ObsDir + "PlayMovie_UserName.txt";
        readonly string MovieName = Obs.ObsDir + "PlayMovie_MovieName.txt";
        readonly string PlayFail = Obs.ObsDir + "PlayMovie_PlayFail.txt";
        TimerFile _userNameFile;
        TimerFile _movieNameFile;
        TimerFile _playFailFile;

        public MovieMessage()
        {
            _userNameFile = new TimerFile(MovieName, MAX_TIME);
            _movieNameFile = new TimerFile(UserName, MAX_TIME);
            _playFailFile = new TimerFile(PlayFail, MAX_TIME);
        }

        public void PlayMovie(string userName, string movieName, int rank)
        {
            _userNameFile.WriteLine(userName);
            _movieNameFile.WriteLine("成功投票 " + movieName);
        }

        public void ShowFail(string message)
        {
            _playFailFile.WriteLine(message);
        }

        public void ShowFail(string format, params object[] args)
        {
            ShowFail(string.Format(format, args));
        }
    }



    public class ThanksMessage
    {
        const int MAX_LINE_COUNT = 3;
        const int MAX_TIME = 10;
        readonly string MessageFile = Obs.ObsDir + "ThanksMessage.txt";
        SelfDeletingFile _thanksMessageFile;

        public ThanksMessage()
        {
            _thanksMessageFile = new SelfDeletingFile(MessageFile, MAX_LINE_COUNT, MAX_TIME);
        }

        public void AddMessage(string message)
        {
            _thanksMessageFile.WriteLine(message);
        }

        public void AddMessage(string format, params object[] args)
        {
            AddMessage(string.Format(format, args));
        }
    }



    public class OtherMessage
    {
        const int MAX_LINE_COUNT = 3;
        const int MAX_TIME = 10;
        readonly string MessageFile = Obs.ObsDir + "OtherMessage.txt";
        SelfDeletingFile _otherMessageFile;

        public OtherMessage()
        {
            _otherMessageFile = new SelfDeletingFile(MessageFile, MAX_LINE_COUNT, MAX_TIME);
        }

        public void AddMessage(string message)
        {
            _otherMessageFile.WriteLine(message);
        }

        public void AddMessage(string format, params object[] args)
        {
            AddMessage(string.Format(format, args));
        }
    }
}
