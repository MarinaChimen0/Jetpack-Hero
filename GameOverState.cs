using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Coursework
{
    class GameOverState: State
    {
        public GameOverState()
        {
            Name = "Start";
        }
        public override void Enter(object owner)
        {
            Game game = owner as Game;
            game.currentOverlay = game.diedOverlay;
            game.enter = false;
        }
        public override void Exit(object owner)
        {
        }
        public override void Execute(object owner, GameTime gameTime)
        {
        }
    }
}
