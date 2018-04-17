using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Douyu.Messages
{
    public class ChouqinMessage : ServerMessage
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public byte Level { get; set; }
        public short Count { get; set; }
        public short Hits { get; set; }
        public int UserId { get; set; }
        public string UserName { get; private set; }
        public byte UserLevel { get; set; }
        public string BadgeName { get; set; }
        public byte BadgeLevel { get; set; }
        public int BadgeRoom { get; set; }
        public string ChouqinName { get { return Level == 1 ? "初级酬勤" : (Level == 2 ? "中级酬勤" : "高级酬勤"); } }
    }
}
