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
            if (Directory.Exists(ObsDir) == false)
                Directory.CreateDirectory(Obs.ObsDir);

            TopMovies = new TopMovies(roomId);
            TopMovies.Update();

            TopUsers = new TopUsers(roomId);
            TopUsers.Update();

            CommandMessage = new CommandMessage();
            GiftMessage = new ThanksGiftMessage();
        }

        public static TopMovies TopMovies { get; private set; }
        public static TopUsers TopUsers { get; private set; }
        public static CommandMessage CommandMessage { get; private set; }
        public static ThanksGiftMessage GiftMessage { get; private set; }

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
            lock (obj) {
                List<string> names = new List<string>();
                List<int> scores = new List<int>();
                DbService.GetTopMovies(RoomId, names, scores);
                string topMovies = "";
                for (int i = 0; i < names.Count; i++) {
                    topMovies += (topMovies == "" ? "" : "\n")
                        + string.Format("[{0:D2}] #{1}-{2}", i + 1, names[i], scores[i]);
                }
                if (_topMovies != topMovies) {
                    File.WriteAllText(TopMoviesFile, topMovies);
                    _topMovies = topMovies;
                }
            }
        }

        private static readonly object obj = new object();
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
            lock (obj) {
                List<string> names;
                List<int> scores;
                string topUsers = "";
                DbService.GetTopUsers(RoomId, out names, out scores);
                for (int i = 0; i < names.Count; i++) {
                    topUsers += (topUsers == "" ? "" : "\n")
                        + string.Format("[{0:D2}] {1} {2}", i + 1, names[i], scores[i]);
                }

                if (_topUsers != topUsers) {
                    File.WriteAllText(TopUsersFile, topUsers);
                    _topUsers = topUsers;
                }
            }
        }

        private static readonly object obj = new object();
        private string _topUsers = "";
    }



    public class ThanksGiftMessage
    {
        readonly string MessageFile = Obs.ObsDir + "thanks_gift_message.txt";

        Snapfile _snapfile;

        public ThanksGiftMessage()
        {
            _snapfile = new Snapfile(MessageFile, 5, 10);
        }

        public void AddMessage(string message)
        {
            _snapfile.AppendText(message);
        }

        public void AddMessage(string format, params object[] args)
        {
            AddMessage(string.Format(format, args));
        }
    }



    public class CommandMessage
    {
        readonly string ErrorFile = Obs.ObsDir + "command_message_error.txt";
        readonly string MessageFile = Obs.ObsDir + "command_message.txt";

        Timerfile _messageFile;
        Snapfile _errorFile;

        public CommandMessage()
        {
            _messageFile = new Timerfile(MessageFile, 10);
            _errorFile = new Snapfile(ErrorFile, 5, 10);
        }

        public void AddMessage(string message)
        {
            _messageFile.WriteText(message);
        }

        public void AddMessage(string format, params object[] args)
        {
            AddMessage(string.Format(format, args));
        }

        public void AddErrorMessage(string message)
        {
            LogService.Error(message);
            _errorFile.AppendText(message);
        }

        public void AddErrorMessage(string format, params object[] args)
        {
            AddErrorMessage(string.Format(format, args));
        }
    }
}
