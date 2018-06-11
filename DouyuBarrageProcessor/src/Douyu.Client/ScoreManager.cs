using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Douyu.Messages;
using System.Threading;
using Jack4net.Log;

namespace Douyu.Client
{
    public static class ScoreManager
    {
        public static double CalGiftScore(GiftMessage giftMessage)
        {
            double experience = giftMessage.GiftExperience;

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

        public static double CalChatScore(ChatMessage chatMessage)
        {
            return 50;
        }

        public static double CalChouqinScore(ChouqinMessage chouqinMessage)
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
                    score = 0; // 未知酬勤
                    break;
            }
            return score;
        }
    }
}
