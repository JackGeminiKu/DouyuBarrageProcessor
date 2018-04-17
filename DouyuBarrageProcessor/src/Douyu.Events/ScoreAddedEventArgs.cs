using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Douyu.Events
{
    public class ScoreAddedEventArgs : EventArgs
    {
        public ScoreAddedEventArgs(string user, double score, string gift)
        {
            User = user;
            Score = score;
            Gift = gift;
        }

        public string User { get; private set; }
        public double Score { get; private set; }
        public string Gift { get; private set; }
    }
}
