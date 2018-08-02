using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Douyu.Messages;
using System.Threading;
using Jack4net.Log;

namespace Douyu.Client
{
    public static class ScoreService
    {
        public static double CalScore(GiftMessage giftMessage)
        {
            var experience = giftMessage.GiftExperience;

            if (experience >= 20000) {
                experience *= 333;
            } else if (experience >= 5000) {
                experience *= 300;
            } else if (experience >= 1000) {
                experience *= 280;
            } else if (experience >= 60) {
                experience *= 250;
            } else if (experience >= 2) {
                experience *= 200;
            } else {
                experience *= 100;
            }
            return experience;
        }

        public static double CalScore(ChatMessage chatMessage)
        {
            return 100;
        }

        public static double CalScore(ChouqinMessage chouqinMessage)
        {
            int score;
            switch (chouqinMessage.Level) {
                case 1:
                    score = 36000;
                    break;
                case 2:
                    score = 75000;
                    break;
                case 3:
                    score = 130000;
                    break;
                default:
                    throw new DouyuException("该等级酬勤(Level {0})还未设定积分值!", chouqinMessage.Level);
            }
            return score;
        }
    }
}
