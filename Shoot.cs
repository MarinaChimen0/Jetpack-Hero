using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Coursework
{
    class Shoot: Collidable
    {
        //Size of the screen
        Rectangle titleSafeArea;
        // laser animation.
        public Animation ShootAnimation;
        // the speed the laser travels
        float shootMoveSpeed = 1300f;
        // position of the laser
        public Vector2 Position;
        // The damage the laser deals.
        public int Damage = 1;
        // set the laser to active
        public bool Active;
        // Laser beams range.
        int Range;
        // the width of the laser image.
        public float Width
        {
            get { return ShootAnimation.FrameWidth; }
        }
        // the height of the laser image.
        public float Height
        {
            get { return ShootAnimation.FrameHeight; }
        }
        public void Initialize(Animation animation, Vector2 position, Rectangle titleSafeArea)
        {
            this.titleSafeArea = titleSafeArea;
            ShootAnimation = animation;
            Position = position;
            Active = true;
            boundingRectangle = new Rectangle((int)Position.X, (int)Position.Y, (int)Width, (int)Height/2);
        }
        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position.X += shootMoveSpeed * elapsed;
            boundingRectangle.X = (int)Position.X;
            ShootAnimation.Position = Position;
            ShootAnimation.Update(gameTime);
            if (Position.X > titleSafeArea.Width + Height)
            {
                // By setting the Active flag to false, the game will remove this objet from the
                // active game list
                Active = false;
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            ShootAnimation.Draw(spriteBatch);
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
            Enemy enm = obj as Enemy;
            if (enm != null)
            {
                Active = false;
            }
        }
    }
}
