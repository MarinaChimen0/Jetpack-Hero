using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using System.Threading;

namespace Coursework
{
    class GameFinishedState: State
    {
        public GameFinishedState()
        {
            Name = "Finished";
        }
        public override void Enter(object owner)
        {
            Game game = owner as Game;
            //Saves user score and serializes it
            game.user.setScore(game.score);
            game.usersList.Add(game.user);
            UserCollection uc = new UserCollection();
            uc.Users = game.usersList;
            XmlSerializer ser = new XmlSerializer(typeof(UserCollection));
            //Inilize the stream 
            game.stream = new FileStream("Content/Score/sc.xml", FileMode.Open, FileAccess.Write);
            ser.Serialize(new StreamWriter(game.stream), uc);
            game.stream.Close();
            game.currentOverlay = game.winOverlay;
            //Sleeps so it doest go until the start 
            Thread.Sleep(1000);
        }
        public override void Exit(object owner)
        {
            Game game = owner as Game;
            game.enter = false;
        }
        public override void Execute(object owner, GameTime gameTime)
        {
        }
    }
}
