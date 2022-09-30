using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.IO;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Input;

namespace Coursework
{
    class Level: IDisposable
    {
        //Screen area
        public Rectangle TitleSafeArea;

        //General attributes
        public bool levelSucceded;
        public bool levelFailed;

        // texture to hold the shoot.
        Texture2D shootTexture;

        //Animation of the powerup path
        string powerupAnimation;

        //Time until loading the next enemies
        int timeEnemies = int.MaxValue;

        //Time until loading the next powerups
        int timePowerUps = int.MaxValue;

        //Loader
        Loader loader;

        //File lines
        List<string> lines = new List<string>();

        //Index of line read
        int iLine = 0;

        // Entities in the level.
        public Player Player
        {
            get { return player; }
        }
        Player player; 

        //Active enemies
        private List<Enemy> enemies = new List<Enemy>();
        //Next enemies to add
        private List<Enemy> nextEnemies = new List<Enemy>();

        //Active PowerUps
        private List<PowerUp> powerups = new List<PowerUp>();
        //Next powerups to add
        private List<PowerUp> nextPowerUps = new List<PowerUp>();

        //List of laser beams
        List<Laser> laserBeams;

        //List of player shoots
        List<Shoot> shootBeams;

        // Background
        Background bg;

        // Level game state.
        CollisionManager collisionManager;
        private Random random = new Random(354668); // Arbitrary, but constant seed
        public int Score
        {
            get { return score; }
            set { score = value; }
        }
        int score;

        // Level content.        
        public ContentManager Content
        {
            get { return content; }
        }
        ContentManager content;

        double timePassed;

        /// <summary>
        /// Constructs a new level.
        /// </summary>
        /// <param name="serviceProvider">
        /// The service provider that will be used to construct a ContentManager.
        /// </param>
        /// <param name="fileStream">
        /// A stream containing the tile data.
        /// </param>
        public Level(IServiceProvider serviceProvider, Stream fileStream, int levelIndex, Rectangle titleSafeArea)
        {
            this.TitleSafeArea = titleSafeArea;
            collisionManager = new CollisionManager();

            //Initialise loader
            loader = new Loader(fileStream);

            // Create a new content manager to load content used just by this level.
            content = new ContentManager(serviceProvider, "Content");

            // Initialize the player class
            player = new Player();

            // Load the player resources
            Vector2 playerPosition = new Vector2(TitleSafeArea.X, TitleSafeArea.Y + TitleSafeArea.Height / 2);
            player.Initialize(Content, "Graphics/", playerPosition);
            collisionManager.AddCollidable(player);

            //Init the powerup animation path
            powerupAnimation = "Graphics/PowerUp/Heart";

            // Load the parallaxing background
            bg = new Background();
            bg.Initialize(Content, "Graphics/SpaceBackground/", TitleSafeArea.Width, TitleSafeArea.Height, 2);


            // Initialize the enemies list
            enemies = new List<Enemy>();
            nextEnemies = new List<Enemy>();

            // Initialize the powerups list
            powerups = new List<PowerUp>();
            nextPowerUps = new List<PowerUp>();

            lines = loader.ReadLinesFromTextFile();
            ReadLines();

            // init our shoot
            shootTexture = Content.Load<Texture2D>("Graphics\\playerShoot");
            shootBeams = new List<Shoot>();

            // Init ships laser
            laserBeams = new List<Laser>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileStream">
        /// A stream containing the data.
        /// </param>
        private void ReadLines()
        {
            //Calculates the next time
            int time = int.Parse(lines[iLine]);
            iLine++;
            //Load the enemies and power ups
            for (int i = 0; i < lines[iLine].Length - 1; i=i+2) {
                int quantity = (int)char.GetNumericValue(lines[iLine][i]);
                char type = lines[iLine][i+1];
                if(type == 'P')
                {
                    timePowerUps = time;
                } else
                {
                    timeEnemies = time;
                }
                LoadType(quantity, type);
            }
            iLine++;
        }

        private void LoadType(int quantity, char type)
        {
            float y;
            switch(type)
            {
                // Various enemies
                case 'A':
                    y = TitleSafeArea.Height / (quantity+1);
                    for(int i = 0; i < quantity; i++)
                    {
                        AddEnemy(1, y *(float)(i+1));
                    }
                    break;
                   
                case 'B':
                    y = TitleSafeArea.Height / (quantity + 1);
                    for (int i = 0; i < quantity; i++)
                    {
                        AddEnemy(2, y * (float)(i + 1));
                    }
                    break;

                case 'C':
                    y = TitleSafeArea.Height / (quantity + 1);
                    for (int i = 0; i < quantity; i++)
                    {
                        AddEnemy(3, y * (float)(i + 1));
                    }
                    break;

                case 'D':
                    y = TitleSafeArea.Height / (quantity + 1);
                    for (int i = 0; i < quantity; i++)
                    {
                        AddEnemy(4, y * (float)(i + 1));
                    }
                    break;

                case 'E':
                    y = TitleSafeArea.Height / (quantity + 1);
                    for (int i = 0; i < quantity; i++)
                    {
                        AddEnemy(5, y * (float)(i + 1));
                    }
                    break;

                case 'F':
                    y = TitleSafeArea.Height / (quantity + 1);
                    for (int i = 0; i < quantity; i++)
                    {
                        AddEnemy(6, y * (float)(i + 1));
                    }
                    break;

                //PowerUps
                case 'P':
                    y = TitleSafeArea.Height / (quantity + 1);
                    for (int i = 0; i < quantity; i++)
                    {
                        AddPowerUp(y * (float)(i + 1));
                    }
                    break;

                // Unknown tile type character
                default:
                    throw new NotSupportedException(String.Format("Unsupported type character."));
            }
        }

        private void AddEnemy(int type, float y)
        {
            // Randomly generate the position of the enemy
            Vector2 position = new Vector2(TitleSafeArea.Width, y);
            // Create an enemy
            Enemy enemy = new Enemy();
            // Initialize the enemy
            enemy.Initialize(Content, "Graphics/Enemy" + type + "/", type, position, this);
            // Add the enemy to the active enemies list
            nextEnemies.Add(enemy);
        }


        private void AddPowerUp(float y)
        {
            // Randomly generate the position
            Vector2 position = new Vector2(TitleSafeArea.Width, y);
            // Create the power up
            PowerUp pu = new PowerUp();
            // Initialize the powerup
            pu.Initialize(Content, powerupAnimation, position);
            // Add the enemy to the active enemies list
            nextPowerUps.Add(pu);
        }

        public void FireShoots(eButtonState buttonState, Vector2 amount)
        {
            Player.Shoot(buttonState, amount);
            if (buttonState == eButtonState.DOWN)
            {
                AddShoot();
            }
        }

        protected void AddShoot()
        {
            SpriteStripAnimation shootAnimation = new SpriteStripAnimation();
            // initlize the laser animation
            shootAnimation.Initialize(shootTexture, shootTexture.Width/2, shootTexture.Height, Player.Position, 2, 15, Color.White, 1.5f, true, SpriteEffects.None, (float)Math.PI / 2.0f, 0);
            Shoot shoot = new Shoot();
            // Get the starting postion of the laser.
            var shootPosition = player.Position;
            // Adjust the position slightly to match the muzzle of the cannon.
            shootPosition.X += player.Width/2;
            // init the laser
            shoot.Initialize(shootAnimation, shootPosition, TitleSafeArea);
            shootBeams.Add(shoot);
            collisionManager.AddCollidable(shoot);
            /* todo: add code to create a laser. */
            // laserSoundInstance.Play();
        }

        private void UpdateShootBeams(GameTime gameTime)
        {
            // Update the Projectiles
            for (int i = shootBeams.Count - 1; i >= 0; i--)
            {
                shootBeams[i].Update(gameTime);
                if (shootBeams[i].Active == false)
                {
                    collisionManager.RemoveCollidable(shootBeams[i]);
                    shootBeams.RemoveAt(i);
                }
            }
        }

        protected void AddLaser(Laser l)
        {
            //Adds laser to the collision manager
            laserBeams.Add(l);
            collisionManager.AddCollidable(l);
        }

        private void UpdateEnemies(GameTime gameTime)
        {
            // Update the Enemies
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                enemies[i].Update(gameTime);
                if (enemies[i].Active == false)
                {
                    collisionManager.RemoveCollidable(enemies[i]);
                    enemies.RemoveAt(i);
                } else
                {
                    for(int l = 0; l < enemies[i].LaserBeams.Count; l++)
                    {
                        AddLaser(enemies[i].LaserBeams[l]);
                    }
                    enemies[i].LaserBeams.Clear();
                }
            }

            //When elements have been loaded, if enemies have all been killed
            if (iLine == lines.Count)
            {
                if (enemies.Count == 0 && nextEnemies.Count ==0)
                {
                    levelSucceded = true;
                }
            }
        }

        private void UpdatePowerUps(GameTime gameTime)
        {
            // Update the powerups
            for (int i = powerups.Count - 1; i >= 0; i--)
            {
                powerups[i].Update(gameTime);
                if (powerups[i].Active == false)
                {
                    collisionManager.RemoveCollidable(powerups[i]);
                    powerups.RemoveAt(i);
                }
            }
        }

        private void UpdatePlayer(GameTime gameTime)
        {
            player.Update(gameTime);

            // Make sure that the player does not go out of bounds
            player.Position.X = MathHelper.Clamp(player.Position.X, player.Width / 2, TitleSafeArea.Width - player.Width / 2);
            player.Position.Y = MathHelper.Clamp(player.Position.Y, player.Height / 2, TitleSafeArea.Height - player.Height / 2);
        }

        private void UpdateLaserBeams(GameTime gameTime)
        {
            // Update the Projectiles
            for (int i = laserBeams.Count - 1; i >= 0; i--)
            {
                laserBeams[i].Update(gameTime);
                if (laserBeams[i].Active == false)
                {
                    collisionManager.RemoveCollidable(laserBeams[i]);
                    laserBeams.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime)
        {
            if(player.Active == false)
            {
                levelFailed = true;
            }

            // Calculates the time that has passed
            double seconds = gameTime.ElapsedGameTime.TotalSeconds;
            timePassed = timePassed + seconds;
            if (timeEnemies <= timePassed)
            {

                //Adds the next enemies to the list of active enemies
                for (int i = 0; i < nextEnemies.Count; i++)
                {
                    collisionManager.AddCollidable(nextEnemies[i]);
                    enemies.Add(nextEnemies[i]);
                }
                //Clears the list of next enemies to load
                nextEnemies.Clear();
                timeEnemies = int.MaxValue;
                if (iLine  == lines.Count)
                {
                    //All elements have been loaded, if enemies have all been killed
                    if(enemies.Count == 0)
                    {
                        levelSucceded = true;
                    }
                }
                else
                {
                    //Read the next elements
                    ReadLines();
                }
            }

            if (timePowerUps <= timePassed)
            {

                //Adds the next powerups to the list of active powerups
                for (int i = 0; i < nextPowerUps.Count; i++)
                {
                    collisionManager.AddCollidable(nextPowerUps[i]);
                    powerups.Add(nextPowerUps[i]);
                }
                //Clears the list of next enemies to load
                nextPowerUps.Clear();
                timePowerUps = int.MaxValue;
                if (iLine == lines.Count)
                {
                    //All elements have been loaded, if enemies have all been killed
                    if (enemies.Count == 0)
                    {
                        levelSucceded = true;
                    }
                }
                else
                {
                    //Read the next elements
                    ReadLines();
                }
            }

            //Update the collision manager
            collisionManager.Update();

            // Update the parallaxing background
            bg.Update(gameTime);

            //Update the player
            UpdatePlayer(gameTime);

            // Update the enemies
            UpdateEnemies(gameTime);

            //Update the powerups
            UpdatePowerUps(gameTime);

            // update shootsbeams
            UpdateShootBeams(gameTime);

            // update laserbeams
            UpdateLaserBeams(gameTime);


        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //Draw the background
            bg.Draw(spriteBatch);

            // Draw the Player
            player.Draw(spriteBatch);

            // Draw the Enemies
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].Draw(spriteBatch);
            }

            // Draw the PowerUps
            for (int i = 0; i < powerups.Count; i++)
            {
                powerups[i].Draw(spriteBatch);
            }

            //Draw the shoots
            // Draw the lasers.
            foreach (var s in shootBeams)
            {
                s.Draw(spriteBatch);
            }

            // Draw the lasers.
            foreach (var l in laserBeams)
            {
                l.Draw(spriteBatch);
            }
        }

        /// <summary>
        /// Unloads the level content.
        /// </summary>
        public void Dispose()
        {
            Content.Unload();
        }
    }
}
