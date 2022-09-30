using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Coursework
{
    class Laser: Collidable
    {
        //Size of the screen
        //public Rectangle titleSafeArea;
        // laser animation.
        public SpriteSetAnimation LaserAnimation;
        // the speed the laser travels
        float laserMoveSpeed = 400f;
        // position of the laser
        public Vector2 Position;
        // The damage the laser deals.
        public int Damage = 10;
        // set the laser to active
        public bool Active;
        // Orientation
        public Vector2 Orientation;
        // the width of the laser image.
        public float Width
        {
            get { return LaserAnimation.FrameWidth; }
        }
        // the height of the laser image.
        public float Height
        {
            get { return LaserAnimation.FrameHeight; }
        }
        public void Initialize(SpriteSetAnimation animation, Vector2 position, int damage, Vector2 orientation)
        {
            Damage = damage;
            LaserAnimation = animation;
            Position = position;
            Orientation = orientation;
            boundingRectangle = new Rectangle((int)Position.X, (int)Position.Y, (int)Width, (int)Height);
            Active = true;
        }
        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position.X -= elapsed*laserMoveSpeed * Orientation.X;
            Position.Y += elapsed*laserMoveSpeed * Orientation.Y;
            boundingRectangle.X = (int)Position.X;
            boundingRectangle.Y = (int)Position.Y;
            LaserAnimation.Position = Position;
            LaserAnimation.Update(gameTime);
            // If has past the screen 
            if (Position.X < -Width)
            {
                // By setting the Active flag to false, the game will remove this objet from the
                // active game list
                Active = false;
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            LaserAnimation.Draw(spriteBatch);
        }

        public override bool CollisionTest(Collidable obj)
        {
            if (obj != null)
            {
                return BoundingRectangle.Intersects(obj.BoundingRectangle);
            }
            return false;
        }

        public override void OnCollision(Collidable obj)
        {
            Player pl = obj as Player;
            if (pl != null)
            {
                Active = false;
            }
        }
    }
}
