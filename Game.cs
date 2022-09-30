using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Soap;
using System.Xml.Serialization;

namespace Coursework
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        // Resources for drawing.
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Global content.
        public SpriteFont hudFont;
        public Texture2D startOverlay;
        public Texture2D winOverlay;
        public Texture2D diedOverlay;
        public Texture2D currentOverlay;

        // Meta-level game state.
        public bool enter;
        private int levelIndex = -1;
        Level level;
        public int score;

        //FSM
        FSM fsm;

        //User of the game
        public User user;

        //List of previous users
        public List<User> usersList;

        //Stream for the seriliazed data
        public FileStream stream;

        // The number of levels in the Levels directory of our content.We assume that
        // levels in our content are 0-based and that all numbers under this constant
        // have a level file present. This allows us to not need to check for the file
        // or handle exceptions, both of which can add unnecessary time to level loading.
        private const int numberOfLevels = 3;

        CommandManager commandManager;

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            commandManager = new CommandManager();
            enter = false;

            // Initialise the FSM
            fsm = new FSM(this);

            // Create the states
            GameStartState start = new GameStartState();
            GameLevelState levelState = new GameLevelState();
            GameOverState over = new GameOverState();
            GameFinishedState finished = new GameFinishedState();

            // Create the transitions between the states
            start.AddTransition(new Transition(levelState, () => enter));
            levelState.AddTransition(new Transition(levelState, () => (level.levelSucceded && (levelIndex < (numberOfLevels-1)))));
            levelState.AddTransition(new Transition(over, () => level.levelFailed));
            levelState.AddTransition(new Transition(finished, () => (level.levelSucceded&&(levelIndex==(numberOfLevels-1)))));
            over.AddTransition(new Transition(levelState, () => enter));
            finished.AddTransition(new Transition(start, () => enter));

            // Add the created states to the FSM
            fsm.AddState(start);
            fsm.AddState(levelState);
            fsm.AddState(over);
            fsm.AddState(finished);
        
        }
        
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>

        /// <summary>
        /// LoadConte nt will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            // Load fonts
            hudFont = Content.Load<SpriteFont>("Fonts/gameFont");

            // Load overlay textures
            startOverlay = Content.Load<Texture2D>("Graphics/Overlays/Overlay_1");
            diedOverlay = Content.Load<Texture2D>("Graphics/Overlays/Overlay_2");
            winOverlay = Content.Load<Texture2D>("Graphics/Overlays/Overlay_3");

            // Set the starting state of the FSM
            fsm.Initialise("Start");

            commandManager = new CommandManager();
            commandManager.AddKeyboardBinding(Keys.Enter, Continue);
            commandManager.AddKeyboardBinding(Keys.Escape, StopGame);
        }

        private void InitializeBindings()
        {
            commandManager = new CommandManager();
            commandManager.AddKeyboardBinding(Keys.Escape, StopGame);
            commandManager.AddKeyboardBinding(Keys.Up, level.Player.Up);
            commandManager.AddKeyboardBinding(Keys.Down, level.Player.Down);
            commandManager.AddKeyboardBinding(Keys.Right, level.Player.Right);
            commandManager.AddKeyboardBinding(Keys.Left, level.Player.Left);
            commandManager.AddKeyboardBinding(Keys.Space, level.FireShoots);
            commandManager.AddKeyboardBinding(Keys.Enter, Continue);
        }

        public void StopGame(eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == eButtonState.DOWN)
            {
                Exit();
            }
        }

        public void Continue(eButtonState buttonState, Vector2 amount)
        {

            if (buttonState == eButtonState.DOWN)
            {
                enter = true;
            }

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            commandManager.Update();
            // update our level, passing down the GameTime along with all of our input states
            if (level != null)
            {
                level.Update(gameTime);
                if (level.levelSucceded)
                    score = score + level.Score;
            }
            fsm.Update(gameTime);
            base.Update(gameTime);
        }

        public void LoadNextLevel()
        {
            // move to the next level
            levelIndex = (levelIndex + 1) % (numberOfLevels);

            // Unloads the content for the current level before loading the next one.
            if (level != null)
            {
                level.Dispose();
            }

            
            // Load the level.
            string levelPath = string.Format("Content/Levels/Level{0}.txt", levelIndex+1);
            using (Stream fileStream = TitleContainer.OpenStream(levelPath))
                level = new Level(Services, fileStream, levelIndex, GraphicsDevice.Viewport.TitleSafeArea);
            InitializeBindings();
        }

        public void ReloadCurrentLevel()
        {
            --levelIndex;
            LoadNextLevel();
        }

        /// <summary>
        /// Draws the game from background to foreground.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            if(level != null)
                level.Draw(gameTime, spriteBatch);

            DrawHud();

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void DrawHud()
        {
            Rectangle titleSafeArea = GraphicsDevice.Viewport.TitleSafeArea;
            Vector2 hudLocation = new Vector2(titleSafeArea.X, titleSafeArea.Y);
            Vector2 center = new Vector2(titleSafeArea.X + titleSafeArea.Width / 2.0f,
                                         titleSafeArea.Y + titleSafeArea.Height / 2.0f);

            // Draw time remaining. Uses modulo division to cause blinking when the
            // player is running out of time.

            // Draw level and score
            if (level != null)
            {
                string levelString = "LEVEL " + ((levelIndex+1)%(numberOfLevels+1));
                DrawShadowedString(hudFont, levelString, hudLocation, Color.Yellow);
                float scoreHeight = hudFont.MeasureString(levelString).Y;
                DrawShadowedString(hudFont, "SCORE: " + (score+level.Score).ToString(), hudLocation + new Vector2(0.0f, titleSafeArea.Height - scoreHeight), Color.Yellow);
            }

            if (currentOverlay != null)
            {
                // Draw status message.
                Vector2 statusSize = new Vector2(currentOverlay.Width, currentOverlay.Height);
                spriteBatch.Draw(currentOverlay, center - statusSize / 2, Color.White);
                if (currentOverlay.Equals(winOverlay))
                {
                    if(level != null)
                    {
                        level.Dispose();
                    }
                    DrawScoreTable();
                }
            }

        }

        private void DrawScoreTable()
        {
            Rectangle titleSafeArea = GraphicsDevice.Viewport.TitleSafeArea;
            Vector2 lineLocation = new Vector2(titleSafeArea.X + 60, titleSafeArea.Y + 140);

            //Sorts the list
            if (usersList.Count > 1)
            {
                usersList.Sort((x, y) => y.getScore().CompareTo(x.getScore()));
            }

            //Prints it
            float scoreHeight = 0.0f;
            string space = "";
            for (int j = 0; j < titleSafeArea.Width / 10f; j++)
            {
                space = space + " ";
            }
            for (int i = 0; i<usersList.Count; i++)
            {
                string scoreString = "PLAYER "+ usersList[i].getID()+space+ usersList[i].getScore();
                if (usersList[i].getID() == usersList.Count)
                {
                    DrawShadowedString(hudFont, scoreString, lineLocation + new Vector2(0.0f, scoreHeight), Color.Yellow);
                }
                else
                {
                    DrawShadowedString(hudFont, scoreString, lineLocation + new Vector2(0.0f, scoreHeight), Color.Pink);
                }
                scoreHeight = scoreHeight+hudFont.MeasureString(scoreString).Y;
                if(i == 6)
                {
                    break;
                }
            }

            //Reset the user
            user = null;
        }

        public UserCollection DeserializeList(StreamReader reader)
        {
            //Creates the object and the list
            UserCollection list = new UserCollection();
            list.Users = new List<User>();
            //If the file is not empty
            if (reader.Peek() != -1) {
                //Reads the file
                XmlSerializer serializer = new XmlSerializer(typeof(UserCollection));
                list = (UserCollection)serializer.Deserialize(reader);
            }
            //Closes the reader
            reader.Close();
            //Returns the list
            return list;
        }

        private void DrawShadowedString(SpriteFont font, string value, Vector2 position, Color color)
        {
            spriteBatch.DrawString(font, value, position + new Vector2(1.0f, 1.0f), Color.Black);
            spriteBatch.DrawString(font, value, position, color);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
  

    }
}
