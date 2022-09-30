// ParallaxingBackground.cs
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Coursework
{
    class ParallaxingBackground
    {
        // The image representing the parallaxing background
        Texture2D texture;
        // An array of positions of the parallaxing background
        Vector2[] positions;
        // The speed which the background is moving
        int speed;
        //Background width and height
        int bgHeight;
        int bgWidth;
        //Scale factor
        float scale;

        private void WrapTextureToLeft(int index)
        {
            // If the textures are scrolling to the left, when the tile wraps, it should be put at the
            // one pixel to the right of the tile before it.
            int prevTexture = index - 1;
            if (prevTexture < 0)
                prevTexture = positions.Length - 1;
            positions[index].X = positions[prevTexture].X + (float)texture.Width * scale;
        }

        private void WrapTextureToRight(int index)
        {
            // If the textures are scrolling to the right, when the tile wraps, it should be placed to the left
            // of the tile that comes after it.
            int nextTexture = index + 1;
            if (nextTexture == positions.Length)
                nextTexture = 0;
            positions[index].X = positions[nextTexture].X - (float)texture.Width * scale;
        }

        public void Initialize(Texture2D texture, int screenWidth, int screenHeight, int speed)
        {
            bgHeight = screenHeight;
            bgWidth = screenWidth;
            // Load the background texture we will be using
            this.texture = texture;
            //Calculates the scale
            scale = (float)(bgHeight/texture.Height);
            // Set the speed of the background
            this.speed = speed;
            // If we divide the screen with the texture width then we can determine the number of tiles need.
            // We add 1 to it so that we won't have a gap in the tiling
            int numOfTiles = (int)(Math.Ceiling(screenWidth / (float)texture.Width*scale) + 1);
            positions = new Vector2[numOfTiles];
            // Set the initial positions of the parallaxing background
            for (int i = 0; i < positions.Length; i++)
            {
                // We need the tiles to be side by side to create a tiling effect
                positions[i] = new Vector2((float)(i * texture.Width)*scale, 0);
            }
        }

        public void Update(GameTime gametime)
        {
            float elapsed = (float)gametime.ElapsedGameTime.TotalSeconds;
            // Update the positions of the background
            for (int i = 0; i < positions.Length; i++)
            {
                // Update the position of the screen by adding the speed
                positions[i].X += elapsed*speed;
                // If the speed has the background moving to the left
                if (speed <= 0)
                {
                    // Check the texture is out of view then put that texture at the end of the screen
                    if (positions[i].X <= -(float)texture.Width*scale)
                    {
                        WrapTextureToLeft(i);
                    }
                }
                // If the speed has the background moving to the right
                else
                {
                    // Check if the texture is out of view then position it to the start of the screen
                    if (positions[i].X >= (float)texture.Width * scale * (positions.Length - 1))
                    {
                        WrapTextureToRight(i);
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < positions.Length; i++)
            {
                Rectangle rectBg = new Rectangle((int)positions[i].X, (int)positions[i].Y, bgWidth, bgHeight);
                spriteBatch.Draw(texture, rectBg, Color.White);
            }
        }
    }
}
