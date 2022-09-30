using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Coursework
{
    abstract class Animation
    {
        // The scale used to display the sprite strip
        public float scale;

        // The time since we last updated the frame
        protected int elapsedTime;

        // The time we display a frame until the next one
        protected int frameTime;

        // The number of frames that the animation contains
        protected int frameCount;

        // The index of the current frame we are displaying
        protected int currentFrame;

        // The color of the frame we will be displaying
        protected Color color;

        // Width of a given frame
        public float FrameWidth;

        // Height of a given frame
        public float FrameHeight;

        // The state of the Animation
        public bool Active;

        // Determines if the animation will keep playing or deactivate after one run
        public bool Looping;

        // Position
        public Vector2 Position;

        //SpriteEffects
        protected SpriteEffects spriteEffects;

        //Rotation
        protected float rotation;

        //Layer depth
        protected float layerDepth;

        public void Initialize(Vector2 position, int frametime, Color color, float scale, bool looping, SpriteEffects spriteEffects, float rotation, float layerDepth)
        {
            // Keep a local copy of the values passed in
            this.color = color;
            this.frameTime = frametime;
            this.scale = scale;
            Looping = looping;
            Position = position;
            this.spriteEffects = spriteEffects;
            this.rotation = rotation;
            this.layerDepth = layerDepth;
            // Set the time to zero
            elapsedTime = 0;
            currentFrame = 0;
            // Set the Animation to active by default
            Active = true;
        }

        public virtual void Update(GameTime gameTime)
        {
            // Do not update the game if we are not active
            if (Active == false) return;
            // Update the elapsed time
            elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            // If the elapsed time is larger than the frame time
            // we need to switch frames
            if (elapsedTime > frameTime)
            {
                // Move to the next frame
                currentFrame++;
                // If the currentFrame is equal to frameCount reset currentFrame to zero
                if (currentFrame == frameCount)
                {
                    currentFrame = 0;
                    // If we are not looping deactivate the animation
                    if (Looping == false)
                        Active = false;
                }
                // Reset the elapsed time to zero
                elapsedTime = 0;
            }

        }


        // Draw the Animation 
        public abstract void Draw(SpriteBatch spriteBatch);

    }
}
