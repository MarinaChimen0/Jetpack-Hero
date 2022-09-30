using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Coursework
{
    [Serializable()]
    [XmlRoot("UserCollection")]
    public class UserCollection
    {
        [XmlArray("Users"), XmlArrayItem(typeof(User), ElementName = "User")]
        public List<User> Users { get; set; }

        public UserCollection() {}

        public UserCollection(List<User> users) { this.Users = users; }
    }
}
