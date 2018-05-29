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
        public static void Initialize(int roomId)
        {
            if (!Directory.Exists(ObsDir))
                Directory.CreateDirectory(Obs.ObsDir);

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
            get { return JApplication.RootPath + @"\OBS\"; }
        }
    }



    public class TopMovies
    {
        readonly string TopMoviesFile = Obs.ObsDir + "TopMovies.txt";

        public TopMovies(int roomId)
        {
            RoomId = roomId;
        }

        public int RoomId { get; private set; }

        public void Update()
        {
            var names = new List<string>();
            var scores = new List<int>();
            DbService.GetTopMovies(RoomId, names, scores);
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

        private static string _topMovies = "";
    }



    public class TopUsers
    {
        readonly string TopUsersFile = Obs.ObsDir + "TopUsers.txt";

        public TopUsers(int roomId)
        {
            RoomId = roomId;
        }

        public int RoomId { get; private set; }

        public void Update()
        {
            var names = new List<string>();
            var scores = new List<int>();
            var topUsers = "";
            DbService.GetTopUsers(RoomId, out names, out scores);
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
        readonly string UserName = Obs.ObsDir + "PlayMovie_UserName.txt";
        readonly string MovieName = Obs.ObsDir + "PlayMovie_MovieName.txt";
        readonly string MovieFail = Obs.ObsDir + "PlayMovie_PlayFail.txt";

        Timerfile _userName;
        Timerfile _movieName;
        Timerfile _playFail;

        public MovieMessage()
        {
            _userName = new Timerfile(MovieName, 10);
            _movieName = new Timerfile(UserName, 10);
            _playFail = new Timerfile(MovieFail, 10);
        }

        public void PlayMovie(string userName, string movieName, int rank)
        {
            _userName.WriteText(userName);
            _movieName.WriteText("成功投票 " + movieName);
        }

        public void PlayFail(string message)
        {
            _playFail.WriteText(message);
        }

        public void PlayFail(string format, params object[] args)
        {
            PlayFail(string.Format(format, args));
        }
    }



    public class ThanksMessage
    {
        readonly string MessageFile = Obs.ObsDir + "ThanksMessage.txt";

        SelfDeletingFile _snapfile;

        public ThanksMessage()
        {
            _snapfile = new SelfDeletingFile(MessageFile, 3, 10);
        }

        public void AddMessage(string message)
        {
            _snapfile.AppendMessage(message);
        }

        public void AddMessage(string format, params object[] args)
        {
            AddMessage(string.Format(format, args));
        }
    }



    public class OtherMessage
    {
        readonly string MessageFile = Obs.ObsDir + "OtherMessage.txt";

        SelfDeletingFile _messageFile;

        public OtherMessage()
        {
            _messageFile = new SelfDeletingFile(MessageFile, 3, 10);
        }

        public void AddMessage(string message)
        {
            _messageFile.AppendMessage(message);
        }

        public void AddMessage(string format, params object[] args)
        {
            AddMessage(string.Format(format, args));
        }
    }
}
