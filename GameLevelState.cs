using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Coursework
{
    class GameLevelState: State
    {
        public GameLevelState()
        {
            Name = "Level";
        }
        public override void Enter(object owner)
        {
            Game game = owner as Game;
            if (game.enter)
            {
                game.currentOverlay = null;
                game.enter = false;
                game.ReloadCurrentLevel();
            } else
            {
                game.LoadNextLevel();
            }
        }
        public override void Exit(object owner)
        {
            Game game = owner as Game;
        }
        public override void Execute(object owner, GameTime gameTime)
        {
            Game game = owner as Game;
            if (game.enter)
            {
                game.enter = false;
            }
        }

    }
}
