using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Coursework
{
    class Player: Collidable
    {
        // Animation representing the player
        public SpriteSetAnimation PlayerAnimation;
        // Animation representing the player boosting
        public SpriteSetAnimation PlayerBoostAnimation;
        // Animation representing the player shooting
        public SpriteSetAnimation PlayerShootAnimation;
        // Animation playinh
        SpriteSetAnimation currentAnimation;
        // Position of the Player relative to the upper left side of the screen
        public Vector2 Position;
        // Movement speed 
        float playerMoveSpeed;
        // State of the player
        public bool Active;
        // Amount of hit points that player has
        public int Health;
        // Get the width of the player 
        public float Width
        {
            get { return PlayerAnimation.FrameWidth; }
        }
        // Get the height of the player 
        public float Height
        {
            get { return PlayerAnimation.FrameHeight; }
        }

        public void Initialize(ContentManager content, String path, Vector2 position)
        {
            //Load the player animation resources
            PlayerAnimation = new SpriteSetAnimation();
            Texture2D[] playerSprites = new Texture2D[8];
            for (int i = 1; i <= playerSprites.Length; i++)
            {
                playerSprites[i - 1] = content.Load<Texture2D>(path+"PlayerFlyAnimation/playerFly" + i);
            }
            PlayerAnimation.Initialize(playerSprites, Vector2.Zero, 30, Color.White, 0.4f, true, SpriteEffects.FlipHorizontally, 0f, 0f);

            PlayerBoostAnimation = new SpriteSetAnimation();
            playerSprites = new Texture2D[7];
            for (int i = 1; i <= playerSprites.Length; i++)
            {
                playerSprites[i - 1] = content.Load<Texture2D>(path + "PlayerBoostAnimation/playerBoost" + i);
            }
            PlayerBoostAnimation.Initialize(playerSprites, Vector2.Zero, 30, Color.White, 0.4f, true, SpriteEffects.FlipHorizontally, 0f, 0f);

            PlayerShootAnimation = new SpriteSetAnimation();
            playerSprites = new Texture2D[8];
            for (int i = 1; i <= playerSprites.Length; i++)
            {
                playerSprites[i - 1] = content.Load<Texture2D>(path + "PlayerShootAnimation/playerShoot" + i);
            }
            PlayerShootAnimation.Initialize(playerSprites, Vector2.Zero, 30, Color.White, 0.4f, true, SpriteEffects.FlipHorizontally, 0f, 0f);

            //Set the current animation
            currentAnimation = PlayerAnimation;
            //Set the bounding rectangle
            boundingRectangle = new Rectangle((int)Position.X, (int)Position.Y, (int)Width-50, (int)Height-20);

            // Set the starting position of the player around the middle of the screen and to the back
            Position = position;
            // Set the player to be active
            Active = true;
            // Set the player health
            Health = 200;
            //Set the player speed
            playerMoveSpeed = 5.0f;
        }

        public void Update(GameTime gameTime)
        {
            currentAnimation.Position = Position;
            currentAnimation.Update(gameTime);
            boundingRectangle.X = (int)Position.X;
            boundingRectangle.Y = (int)Position.Y;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            currentAnimation.Draw(spriteBatch);
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
            // Cast the object as a laser
            Laser laser = obj as Laser;
            if (laser != null)
            {
                //Moves the character position
                Vector2 collisionNormal = Vector2.Normalize(new Vector2(laser.BoundingRectangle.Center.X - BoundingRectangle.Center.X, laser.BoundingRectangle.Center.Y - BoundingRectangle.Center.Y));
                Position.X = Position.X + (-collisionNormal.X*5);
                Position.Y = Position.Y + (-collisionNormal.Y*5);
                //Change player health
                Health -= laser.Damage;
                // If the player health is less than zero we died
                if (Health <= 0)
                    Active = false;
            } 
            // Cast the object as a Power Up
            PowerUp pu = obj as PowerUp;
            if(pu != null)
                Health = Health + pu.Health;
        }

        public void Up(eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == eButtonState.DOWN)
            {
                Position.Y -= playerMoveSpeed*1.6f;
            }
        }

        public void Down(eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == eButtonState.DOWN)
            {
                Position.Y += playerMoveSpeed*1.6f;
            }
        }

        public void Right(eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == eButtonState.DOWN)
            {
                currentAnimation = PlayerBoostAnimation;
                Position.X += playerMoveSpeed;
            } else
            {
                currentAnimation = PlayerAnimation;
            }
        }

        public void Left(eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == eButtonState.DOWN)
            {
                Position.X -= playerMoveSpeed*0.5f;
            }
        }

        public void Shoot(eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == eButtonState.DOWN)
            {
                currentAnimation = PlayerShootAnimation;
            } else
            {
                currentAnimation = PlayerAnimation;
            }
        }

    }
}
