using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using Dapper;
using System.IO;

namespace DouyuBarrageProcessorTest
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void btnScoreTest_Click(object sender, EventArgs e)
        {
            Dictionary<int, int> scores = new Dictionary<int, int>();

            IDbConnection connection = new SqlConnection(
                "Data Source=10.0.0.2;Initial Catalog=Douyu2;User ID=sa;Password=Jack52664638"
            );

            // 计算弹幕积分
            var chatMessages = connection.Query("select * from chat_message where room_id = " + txtRoom.Text);
            for (int i = 0; i < chatMessages.Count(); ++i) {
                int userId = chatMessages.ElementAt(i).user_id;
                if (scores.ContainsKey(userId)) {
                    scores[userId] += 50;
                } else {
                    scores.Add(userId, 50);
                }
            }

            // 计算礼物积分
            Dictionary<int, int> gifts = new Dictionary<int, int>();
            var giftCategory = connection.Query("select * from gift_category");
            for (int i = 0; i < giftCategory.Count(); ++i) {
                gifts.Add(giftCategory.ElementAt(i).id, giftCategory.ElementAt(i).experience);
            }

            var giftMessages = connection.Query("select * from gift_message where room_id = " + txtRoom.Text);
            for (int i = 0; i < giftMessages.Count(); ++i) {
                int user_id = giftMessages.ElementAt(i).user_id;
                int gift_id = giftMessages.ElementAt(i).gift_id;
                if (scores.ContainsKey(user_id)) {
                    scores[user_id] += ScoreCalculator.CalGiftScore(gifts[gift_id]);
                } else {
                    scores.Add(user_id, ScoreCalculator.CalGiftScore(gifts[gift_id]));
                }
            }

            // 计算酬勤积分
            var chouqinMesssages = connection.Query("select * from chouqin_message where room_id = " + txtRoom.Text);
            for (int i = 0; i < chouqinMesssages.Count(); ++i) {
                int user_id = chouqinMesssages.ElementAt(i).user_id;
                int level = chouqinMesssages.ElementAt(i).level;
                if (scores.ContainsKey(user_id)) {
                    scores[user_id] += ScoreCalculator.CalChouqinScore(level);
                } else {
                    scores.Add(user_id, ScoreCalculator.CalChouqinScore(level));
                }
            }

            // 输出积分数据
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "text file|*.txt";
            if (dialog.ShowDialog() != DialogResult.OK) {
                return;
            }
            foreach (KeyValuePair<int, int> item in scores) {
                File.AppendAllText(dialog.FileName, string.Format("{0}\t{1}\t{2}\n", txtRoom.Text, item.Key, item.Value));
            }

            MessageBox.Show("done");
        }

        private void btnTestChatData_Click(object sender, EventArgs e)
        {
            txtChatData.Clear();
            txtChatData.AppendText(string.Format("1小时\t{0}条\n", GetChatMessageCount(txtRoom.Text, 1)));
            txtChatData.AppendText(string.Format("8小时\t{0}条\n", GetChatMessageCount(txtRoom.Text, 8)));
            txtChatData.AppendText(string.Format("24小时\t{0}条\n", GetChatMessageCount(txtRoom.Text, 24)));
        }

        int GetChatMessageCount(string roomId, int time)
        {
            IDbConnection connection = new SqlConnection(
                "Data Source=10.0.0.2;Initial Catalog=Douyu2;User ID=sa;Password=Jack52664638"
            );

            var chatMessages = connection.Query("select * from chat_message where room_id = @room_id and time >= @time ",
                new { room_id = roomId, time = DateTime.Now.AddHours(-1 * time) });

            return chatMessages.Count();
        }

        private void btnTestGiftData_Click(object sender, EventArgs e)
        {
            txtGiftData.Clear();
            txtGiftData.AppendText(string.Format("1小时\t{0}元\n", CalGiftMoney(txtRoom.Text, 1)));
            txtGiftData.AppendText(string.Format("8小时\t{0}元\n", CalGiftMoney(txtRoom.Text, 8)));
            txtGiftData.AppendText(string.Format("24小时\t{0}元\n", CalGiftMoney(txtRoom.Text, 24)));
        }

        double CalGiftMoney(string rooomId, int time)
        {
            double money = 0;

            Dictionary<int, int> scores = new Dictionary<int, int>();

            IDbConnection connection = new SqlConnection(
                "Data Source=10.0.0.2;Initial Catalog=Douyu2;User ID=sa;Password=Jack52664638"
            );

            // 计算礼物积分
            Dictionary<int, double> gifts = new Dictionary<int, double>();
            var giftCategory = connection.Query("select * from gift_category");
            for (int i = 0; i < giftCategory.Count(); ++i) {
                gifts.Add(giftCategory.ElementAt(i).id, giftCategory.ElementAt(i).experience);
            }

            var giftMessages = connection.Query(
                "select * from gift_message where room_id = @room_id and time >= @time",
                new { room_id = txtRoom.Text, time = DateTime.Now.AddHours(-1 * time) }
            );
            for (int i = 0; i < giftMessages.Count(); ++i) {
                int gift_id = giftMessages.ElementAt(i).gift_id;
                money += gifts[gift_id];
            }

            // 计算酬勤积分
            var chouqinMesssages = connection.Query(
                "select * from chouqin_message where room_id = @room_id and time >= @time",
                new { room_id = txtRoom.Text, time = DateTime.Now.AddHours(-1 * time) }
            );
            for (int i = 0; i < chouqinMesssages.Count(); ++i) {
                int level = chouqinMesssages.ElementAt(i).level;
                money += level == 1 ? 15 : (level == 2 ? 30 : 50);
            }

            return money / 10;
        }
    }



    public static class ScoreCalculator
    {
        public static int CalGiftScore(int experience)
        {
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

        public static int CalChatScore()
        {
            return 50;
        }

        public static int CalChouqinScore(int level)
        {
            int score;
            switch (level) {
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
                    break;
            }
            return score;
        }
    }
}
