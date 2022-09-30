using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Coursework
{
    [Serializable]
    public class User
    {
        [XmlElement("ID")]
        public int ID { get; set; }
        [XmlElement("Score")]
        public int score { get; set; }

        public User() {}

        public User(int ID)
        {
            this.ID = ID;
        }

        public void setScore(int score)
        {
            this.score = score;
        }

        public int getScore()
        {
            return score;
        }

        public int getID()
        {
            return ID;
        }
    }
}
