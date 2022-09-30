using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.IO;

namespace Coursework
{
    class GameStartState : State
    {
        public GameStartState()
        {
            Name = "Start";
        }
        public override void Enter(object owner)
        {
            Game game = owner as Game;
            game.score = 0;
            game.currentOverlay = game.startOverlay;
            //Deserialize the list 
            game.stream = (FileStream)TitleContainer.OpenStream("Content/Score/sc.xml");
            game.usersList = game.DeserializeList(new StreamReader(game.stream)).Users;
            //Creates the user
            game.user = new User(game.usersList.Count + 1);
            //Sets enter as false
            if (game.enter)
            {
                game.enter = false;
            }
        }
        public override void Exit(object owner)
        {
            Game game = owner as Game;
            game.currentOverlay = null;
            game.enter = false;
        }
        public override void Execute(object owner, GameTime gameTime){}

    }
}
