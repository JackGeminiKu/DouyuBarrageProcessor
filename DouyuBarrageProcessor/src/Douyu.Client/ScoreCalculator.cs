using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Douyu.Messages;
using System.Threading;
using Jack4net.Log;

namespace Douyu.Client
{
    public static class ScoreCalculator
    {
        public static double CalGiftScore(GiftMessage giftMessage)
        {
            double experience = giftMessage.GiftExperience;
            if (experience <= 1) {
                return experience * 100;
            } else if (experience < 60) {
                return experience * 200;
            } else if (experience < 1000) {
                return experience * 250;
            } else if (experience < 5000) {
                return experience * 280;
            } else if (experience < 20000) {
                return experience * 300;
            } else {
                return experience * 333;
            }
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
                    score = 130000;
                    LogService.Error("");
                    break;
            }
            return score;
        }
    }
}
