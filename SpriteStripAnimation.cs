using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Coursework
{
    class SpriteStripAnimation : Animation
    {
        // The image representing the collection of images used for animation
        Texture2D spriteStrip;
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              
        // The area of the image strip we want to display
        Rectangle sourceRect = new Rectangle();

        // The area where we want to display the image strip in the game
        Rectangle destinationRect = new Rectangle();

        public void Initialize(Texture2D texture, int frameWidth, int frameHeight, Vector2 position, int frameCount, int frametime, Color color, float scale, bool looping, SpriteEffects spriteEffects, float rotation, float layerDepth)
        {
            //Call the base constructor
            base.Initialize(position, frametime, color, scale, looping, spriteEffects, rotation, layerDepth);

            // Keep a local copy of the values passed in
            spriteStrip = texture;
            this.FrameWidth = frameWidth;
            this.FrameHeight = frameHeight;
            this.frameCount = frameCount;
        }

        public override void Update(GameTime gameTime)
        {
            //Call the base update
            base.Update(gameTime);

            // Grab the correct frame in the image strip by multiplying the currentFrame index by the Frame width
             sourceRect = new Rectangle((int)(currentFrame * FrameWidth), 0, (int)FrameWidth, (int)FrameHeight);

            // Grab the correct frame in the image strip by multiplying the currentFrame index by the frame width
             destinationRect = new Rectangle((int)Position.X - (int)(FrameWidth * scale) / 2, (int)Position.Y
            - (int)(FrameHeight * scale) / 2, (int)(FrameWidth * scale), (int)(FrameHeight * scale));
        }


        // Draw the Animation Strip
        public override void Draw(SpriteBatch spriteBatch)
        {
            // Only draw the animation when we are active
            if (Active)
            {
                spriteBatch.Draw(spriteStrip, destinationRect, sourceRect, color, rotation, Vector2.Zero, spriteEffects, layerDepth);
            }
        }

    }
}
