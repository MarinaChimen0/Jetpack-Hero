using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Coursework
{
    class Enemy: Collidable
    {
        //FSM
        FSM fsm;
        //Control for the FSM: check if the enemy has passed the half of the screen
        bool halfScreen;

        //Length of the sprite set
        int spriteSetLength = 4;

        //Parameters for the animation
        int frameTime = 50;
        Color color = Color.White;
        SpriteEffects spriteEffects = SpriteEffects.FlipHorizontally;
        float rotation = 0;

        //List of lasers created by the enemy
        public List<Laser> LaserBeams;
        // govern how fast a laser can fire.
        TimeSpan laserSpawnTime;
        TimeSpan previousLaserSpawnTime;
        const float SECONDS_IN_MINUTE = 60f;
        float RATE_OF_FIRE = 15f;

        //Scale applied to the main texture and the animation
        float scale = 1.2f;

        // Texture that represents the enemy
        Texture2D Texture;
        // Animation representing the enemy movement
        public SpriteSetAnimation EnemyAnimation;
        //Animation that represents a spaceship laser
        public Texture2D[] LaserAnimation;

        //The level of difficulty of enemy
        public int level;
        // The position of the enemy ship relative to the top left corner of the screen
        public Vector2 Position;
        // The state of the Enemy Ship
        public bool Active;
        // The hit points of the enemy, if this goes to zero the enemy dies
        public int Health;
        // The amount of damage the enemy inflicts on the player ship
        public int Damage;
        // The amount of score the enemy will give to the player
        public int Value;
        // Get the width of the enemy ship
        public float Width
        {
            get { return Texture.Width*scale + EnemyAnimation.FrameWidth; }
        }
        // Get the height of the enemy ship
        public float Height
        {
            get { return Texture.Height * scale; }
        }
        // The speed at which the enemy moves
        public float EnemyMoveSpeed;
        //Level in which the enemy is being played
        Level playedOn;

        public void Initialize(ContentManager content, String path, int level, Vector2 position, Level playedOn)
        {
            // Load the enemy spaceship texture
            Texture = content.Load<Texture2D>(path + "ship");

            //Load the motor animation
            EnemyAnimation = new SpriteSetAnimation();
            Texture2D[] sprites = new Texture2D[spriteSetLength];
            for (int i = 1; i <= sprites.Length; i++)
            {
                sprites[i - 1] = content.Load<Texture2D>(path + "motor"+ i);

            }
            EnemyAnimation.Initialize(sprites, new Vector2(position.X+scale*Texture.Width/2, position.Y), frameTime, color, scale, true, spriteEffects, rotation, 0);

            //Load the laser sprite animations, to create the animations later
            LaserAnimation = new Texture2D[3];
            for (int i = 1; i <= LaserAnimation.Length; i++)
            {
                LaserAnimation[i - 1] = content.Load<Texture2D>(path + "shoot" + i);

            }
            LaserBeams = new List<Laser>();

            // Set the position of the enemy
            Position = position;
            // Save the level of difficulty
            this.level = level;
            // We initialize the enemy to be active so it will be update in the game
            Active = true;
            // Set the health of the enemy
            Health = 20*level;
            // Set the amount of damage the enemy can do
            Damage = 5*level;
            //Set rate of fire
            RATE_OF_FIRE = RATE_OF_FIRE * level;
            laserSpawnTime = TimeSpan.FromSeconds(SECONDS_IN_MINUTE / RATE_OF_FIRE);
            previousLaserSpawnTime = TimeSpan.Zero;
            // Set the score value of the enemy
            Value = 100*level;
            //Set the bounding rectangle
            boundingRectangle = new Rectangle((int)Position.X, (int)Position.Y, (int)Width/2, (int)Height/4);
            //Set the leven in which is being used
            this.playedOn = playedOn;

            // Initialise the FSM
            halfScreen = false;
            fsm = new FSM(this);
            // Create the states
            EnemyAdvanceFastState fast = new EnemyAdvanceFastState();
            EnemyAdvanceSlowState slow = new EnemyAdvanceSlowState();
            // Create the transitions between the states
            fast.AddTransition(new Transition(slow, () => halfScreen));
            // Add the created states to the FSM
            fsm.AddState(fast);
            fsm.AddState(slow);
            // Set the starting state of the FSM
            fsm.Initialise("Fast");
        }

        protected void AddLaser()
        {
            Vector2[] orientations = { new Vector2(1.0f, 1.0f), new Vector2(1.0f, 0.0f), new Vector2(1.0f, -1.0f) };
            for (int i = 0; i < 3; i++)
            {
                SpriteSetAnimation laserAnimation = new SpriteSetAnimation();
                // initlize the laser animation
                laserAnimation.Initialize(LaserAnimation, new Vector2(Position.X - Texture.Width / 2, Position.Y), frameTime, color, scale, true, spriteEffects, rotation, 0);
                Laser laser = new Laser();
                // init the laser
                laser.Initialize(laserAnimation, laserAnimation.Position, Damage, orientations[i]);
                LaserBeams.Add(laser);
            }
        }


        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            // The enemy always moves to the left so decrement it's x position
            Position.X -= EnemyMoveSpeed* elapsed;
            boundingRectangle.X = (int)Position.X;
            // Update the position of the Animation
            EnemyAnimation.Position = new Vector2(Position.X + scale * Texture.Width / 2, Position.Y);
            // Update Animation
            EnemyAnimation.Update(gameTime);
            // If the enemy is past the screen or its health reaches 0 then deactivate it
            if (Position.X < -Width || Health <= 0)
            {
                // By setting the Active flag to false, the game will remove this objet from the
                // active game list
                Active = false;
            }
            //If the enemy is past the half the screen
            if(Position.X < (playedOn.TitleSafeArea.X + playedOn.TitleSafeArea.Width / 2))
            {
                halfScreen = true;
            }
            // govern the rate of fire for our lasers
            if (gameTime.TotalGameTime - previousLaserSpawnTime > laserSpawnTime)
            {
                previousLaserSpawnTime = gameTime.TotalGameTime;
                // Add the laer to our list.
                AddLaser();
            }
            //Update the FSM
            fsm.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the ship
            Vector2 drawPosition;
            drawPosition.X = Position.X - Texture.Width * scale / 2;
            drawPosition.Y = Position.Y - Texture.Height * scale / 2;
            spriteBatch.Draw(Texture, drawPosition, null, color, rotation, Vector2.Zero, scale, spriteEffects, 0);
            // Draw the animation
            EnemyAnimation.Draw(spriteBatch);
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
            Shoot shoot = obj as Shoot;
            if (shoot != null)
            {
                Health = Health - shoot.Damage;
                if (Health <= 0)
                {
                    //The enemy disappears and the score is updated
                    playedOn.Score = playedOn.Score + Value;
                    Active = false;
                }
            }
        }
    }
}
