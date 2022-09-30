using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework
{
    class PowerUp: Collidable
    {
        // laser animation.
        public SpriteSetAnimation animation;
        //Parameters for the animation
        int frameTime = 50;
        Color color = Color.White;
        SpriteEffects spriteEffects = SpriteEffects.None;
        float rotation = 0;
        float scale = 0.35f;
        // the speed the laser travels
        float moveSpeed = 250f;//5
        // position of the laser
        public Vector2 Position;
        //origin y
        float y;
        // The damage the laser deals.
        public int Health = 50;
        // set the laser to active
        public bool Active;

        // the width of the laser image.
        public float Width
        {
            get { return animation.FrameWidth; }
        }
        // the height of the laser image.
        public float Height
        {
            get { return animation.FrameHeight; }
        }

        public void Initialize(ContentManager content, String path, Vector2 position)
        {
            //Load the heart animation
            animation = new SpriteSetAnimation();
            Texture2D[] sprites = new Texture2D[3];
            for (int i = 1; i <= sprites.Length; i++)
            {
                sprites[i - 1] = content.Load<Texture2D>(path + i);

            }
            animation.Initialize(sprites, new Vector2(position.X + scale * sprites[0].Width / 2, position.Y), frameTime, color, scale, true, spriteEffects, rotation, 0);
            Position = position;
            y = position.Y;
            boundingRectangle = new Rectangle((int)Position.X, (int)Position.Y, (int)Width, (int)Height);
            Active = true;
        }
        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position.X -= moveSpeed * elapsed;
            Position.Y = y + moveSpeed * elapsed;
            boundingRectangle.X = (int)Position.X;
            boundingRectangle.Y = (int)Position.Y;
            animation.Position = Position;
            animation.Update(gameTime);
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
            animation.Draw(spriteBatch);
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
