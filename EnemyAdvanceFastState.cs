using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework
{
    class EnemyAdvanceFastState: State
    {
        public EnemyAdvanceFastState()
        {
            Name = "Fast";
        }
        public override void Enter(object owner)
        {
            Enemy enemy = owner as Enemy;
            // Set how fast the enemy moves
            enemy.EnemyMoveSpeed = 70f / enemy.level;         
        }
        public override void Exit(object owner)
        {
            Enemy enemy = owner as Enemy;
        }
        public override void Execute(object owner, GameTime gameTime){ }
    }
}
