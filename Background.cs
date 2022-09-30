using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Coursework
{
    class Background
    {
        // Image used to display the static background
        Texture2D mainBackground;
        // Parallaxing Layers
        ParallaxingBackground[] bgLayers;
        //Screen size
        int screenWidth;
        int screenHeight;

        public void Initialize(ContentManager content, String path, int screenWidth, int screenHeight, int layers)
        {
            //Initialise the class attributes
            bgLayers = new ParallaxingBackground[layers];
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;

            //Load the main background
            mainBackground = content.Load<Texture2D>(path + "mainbackground");

            //Load the layers
            for (int i = 1; i <= bgLayers.Length; i++)
            {
                bgLayers[i - 1] = new ParallaxingBackground();
                bgLayers[i - 1].Initialize(content.Load<Texture2D>(path + "bgLayer"+i), screenWidth, screenHeight, -i*50);
            }
        }

        public void Update(GameTime gametime)
        {
            // Update the parallaxing background
            foreach (ParallaxingBackground bgLayer in bgLayers)
            {
                bgLayer.Update(gametime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //Draw the Main Background Texture
            spriteBatch.Draw(mainBackground, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
            // Draw the moving background
            foreach(ParallaxingBackground bgLayer in bgLayers)
            {
                bgLayer.Draw(spriteBatch);
            }
        }

    }
}
