using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Coursework
{
    class SpriteSetAnimation : Animation
    {
        // Collection of images used for animation
        Texture2D[] sprites;


        public void Initialize(Texture2D[] textures, Vector2 position, int frametime, Color color, float scale, bool looping, SpriteEffects spriteEffects, float rotation, float layerDepth)
        {
            //Call the base constructor
            base.Initialize(position, frametime, color, scale, looping, spriteEffects, rotation, layerDepth);

            //Save the set of textures
            this.sprites = textures;
            this.frameCount = textures.Length;
            if (frameCount > 0)
            {
                this.FrameWidth = textures[0].Width * scale;
                this.FrameHeight = textures[0].Height * scale;
            }
        }

        // Draw the Animation Strip
        public override void Draw(SpriteBatch spriteBatch)
        {
            // Only draw the animation when we are active
            if (Active)
            {

                Vector2 drawPosition;
                drawPosition.X = Position.X - FrameWidth / 2;
                drawPosition.Y = Position.Y - FrameHeight / 2;
                spriteBatch.Draw(sprites[currentFrame], drawPosition, null, color, rotation, Vector2.Zero, scale, spriteEffects, layerDepth);
                    
            }
        }

    }
}
