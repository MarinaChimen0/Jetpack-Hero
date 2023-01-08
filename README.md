# Computer-Games-Architecture
This project was implemented for the Computer Games Architecture module in the MSc in Computer Games Technology that I completed in 2020. Our task was to build our own game engine using MonoGame and C# with Visual Studio, and produce a demo of a casual 2D game from it. The demo produced was a space side scrolling shooter in 2D, in which the player is a jetpack astronaut that has to defend himself of the spaceship enemies.

![Captura de pantalla 2021-01-27 225227](https://user-images.githubusercontent.com/113347414/211168394-47f25ca4-1819-4a27-9f8d-7303b003aa72.png)

[See a video demonstration clicking here](https://youtu.be/cNMMWl-FAWU)


- Use of resource management strategy to load assets

The resource management strategy applied for loading the assets has been the use of the Content Pipeline from the XNA framework. In first place, all the graphic files have been saved in the Content file using the Monogame Pipeline Tool, where all the related sprites have been set in different folders. Then, in the classes of the game, the path for these assets have been saved for the right element, so the assets and the elements match. The assets are loaded using this path with the ContentManager and they get saved until the game does not need them.

Example of the Enemy class, in which the Initialize method loads different sprites using the variable content as the ContentManager instance to conform a spaceship:

```
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
```

- An event-driven architecture to cntrol of the game character

For implementing this, an event driven architecture has been created in order to provide a different layer of abstraction between the keyboard input and the player response. Instead of having to check if an event has occurred, the event is fired and gets received by the listener class.

The classes that have implemented this functionality are clearly divided by the ones that constitute the game-engine, and the ones that are just using the game-engine for generating the game. In the first place there are:

-ButtonStates: A simple enumeration class that defines the states that a button may have (down, up, pressed).

-KeyboardEventArgs: Class that stores a key and both the current and previous keyboard state, used by CommandManager to send the state of the keyboard related to a specific key.

-InputListener: This class acts as a listener for the keyboard inputs. It stores the previous keyboard state, gets the new one, and check within its key list if it has to fire an event, which is delegated by the own class so it can be handled in another one.

-CommandManager: This class is the responsible of initialising the input listener, registering on it the possible input actions (by adding them to the input listener) and storing the relations between the possible input actions and the game actions.

On the other hand, the game classes that implement this functionality of the game-engine are:

-Game: In the class that represents the Game, the command manager for the game is created, and the pairs of keys and game actions get bind at the InitializeBindings function:

```
commandManager = new CommandManager();
commandManager.AddKeyboardBinding(Keys.Up, level.Player.Up);
commandManager.AddKeyboardBinding(Keys.Down, level.Player.Down);
commandManager.AddKeyboardBinding(Keys.Right, level.Player.Right);
commandManager.AddKeyboardBinding(Keys.Left, level.Player.Left);
commandManager.AddKeyboardBinding(Keys.Space, level.FireShoots);
```           

-Player: In the class that represents the player, these events get handled by the functions that have been added to the command manager in the Game class, as it has been shown above: Up, Down, Right, Left and FireShoots.   

     
 
-	Use of basic brute force techniques for Collision detection

A game-engine should have an implementation for detecting (and handling) collisions, since in almost every game the contact between the elements is relevant for the resolution of it. For this reason, the game-engine created has a collision detection and response system. It is based on having a base class, Collidable, from which all the classes that represents objects that have the characteristic of colliding, have to inherit. 

This and the rest of the game-engine classes used for detecting collisions are listed below:

-Collidable: This class delimits the fields and methods that an element that is collidable needs. For the first aspect, a collidable needs a rectangle that delimits the element (BoundingRectangle in the class). For the second one, discussing just about the detection, a CollisionTest function that returns true if the collidable has collided with other collidable.

-Collision: This class represents a collision that has taken place between two collidable objects. It contains an additional class for comparing two collidables, so it can check if two collidable are the same object.

-CollisionManager: This class is the responsible for checking the collisions between the collidable objects that has stored in a list. The collisions generated get stored in a hash map. It also contains a method for removing the collidables from the list.

In consequence, in the game part classes of the project, the following ones inherit from Collidable: Enemy, Laser, Player, Shoot and PowerUp.

![image](https://user-images.githubusercontent.com/113347414/211203324-e3344fc4-d3fe-4b2c-bc24-597f9986d914.png)

All these classes have the same CollsionTest method implemented:

```
public override bool CollisionTest(Collidable obj)
          {
            if (obj != null)
            {
                return BoundingRectangle.Intersects(obj.BoundingRectangle);
            }
            return false;
}
```  

Moreover, in the class Level, a collision manager is declared, initialized, and updated to detect the collisions produced between the different elements of the level. As an example, the AddLaser function of this class:

```  
protected void AddLaser(Laser l)
          {
            //Adds laser to the collision manager
            laserBeams.Add(l);
            collisionManager.AddCollidable(l);
   }
```  

-	Moving and animating game elements with frame-rate independent game loop control

All the game elements in the game (the player, the enemies, the lasers, ...) are animated. For creating the animation, the game-engine has an Animation abstract class with two different implementations: SpriteSetAnimation and SpriteStripAnimation. The reason behind this design decision is when the search for asset sprites took place, some assets for animations came with all the different frames in a strip in a sprite, meanwhile other assets came as folders with the animation frames separated in different sprites.

![image](https://user-images.githubusercontent.com/113347414/211203799-06d92320-62ab-496b-8f39-d9050cd847b0.png)

Both of these implementations are frame-rate independent of the game loop control, because for getting updated they use a variable that gets incremented with the time that has passed since the last time their Update function was called:

```
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
// If the currentFrame is equal to frameCount reset currentFrame to         zero
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
```

The movement of the elements is also frame-rate independent of the game loop control. In the case of the player, it is independent since the position will be changed when the arrow keys are pressed because of the event-driven architecture used. In the case of the other elements, this is achieved by using the elapsed time again, multiplying the speed by it. As an example, the change of the position in the Update function of Enemy class:

```
float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            // The enemy always moves to the left so decrement it's x position
            Position.X -= EnemyMoveSpeed* elapsed;
```

There is another element in the game that is animated: the background. The Background class consist on a 2D static texture as the main background and an array of animated layers, of the class ParallaxingBackground. In this class, as in the previous ones, the multiplication of the layer speed by the elapsed time has been used to maintain the animation frame-rate independent:

```
float elapsed = (float)gametime.ElapsedGameTime.TotalSeconds;
           // Update the positions of the background
           for (int i = 0; i < positions.Length; i++)
           {
                // Update the position of the screen by adding the speed
                positions[i].X += elapsed*speed;
```

-	Configurable game world with positions/attributes of game elements/opponents using a data-driven approach. 

In order to make the construction of levels with different configurations easy, the approach chosen for the game engine has been the data driven design, in which the core engine logic must be separated from the game functionality. The type of files used for saving levels configurations is text file.

Taking these two points in consideration, a Loader class for this kind of files has been added to the game engine. The Loader class contains a single attribute, the Stream from which the content is going to be read, and two different reading methods. The ReadTextFileComplete function reads the text file and saves it completely in an only string. The ReadLinesFromTextFile function reads the file per lines and returns a List of string, in which every element is a line.

The configuration chosen for representing the game elements in the text file has been the following: per every two lines, the first is a number that represents the time until loading the element and the second represents the elements to load, writing in first place the number of elements and then the type. The type is specified by a character:

A: Enemy of difficulty 1 
B: Enemy of difficulty 2
C: Enemy of difficulty 3 
D: Enemy of difficulty 4 
E: Enemy of difficulty 5 
F: Enemy of difficulty 6

P: Health power up

Example of level file:

5
2A1B
15
1P
20
1D2C

With the text format clear, in the Level class this kind of files are used in combination with the Loader class that the engine part provides. In the constructor, it uses the loader to save all the lines of the file in a list. The level is going to do a dynamical load of the elements, since they have to appear when a certain amount of time has passed. 

This is done by the method ReadLines that process two lines to get the time and the game elements that must load, saving them in the nextEnemies or nextPowerUps list. Then, in the Update function when the time that has passed since starting the level is equal to the time for loading the elements, the elements in those arrays would be saved in the enemies or powerups lists, and therefore finally set as active in their correspondent update functions.


-	Collision response based removal of game elements, separating collision detection and collision response code. 

The way of managing the collisions response in the game-engine classes is the following:

-Collidable: This class provide the collidable objects with a method called OnCollision, that the inherited classes will have to implement with their own response to the collision.

-Collision: The Collision class has a Resolve method that calls to the OnCollision method of the collidable that has being part of a collision.

-CollisionManager: The collisions manager has a ResolveCollisions method that calls to the Resolve method of all the collisions saved on its hash map.

![image](https://user-images.githubusercontent.com/113347414/211203970-aa704512-a4aa-4321-b0e1-003aedeea029.png) 

In the game, every class that inherits from Collidable has its own OnCollision method, depending on what they should do when colliding with other collidables. As an example, the method implemented for the class Player, in which if the player loses all his health he will be removed:

```
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
```

-	Scoring system using event listeners

The score system used for the game is the following: when the player kills an enemy, receives a punctuation depending on the difficulty of the spaceship, that gets added to the score already achieved on that level. This score and the one already saved from the previous levels is showed to the user all the time in the screen. If the user pass the level, the punctuation of the level gets added to the user game punctuation. 

In the game code this works in the following manner. First in the Enemy class, taking profit of the OnCollision method, if an Enemy has a collision with an object of the type Shoot (the class that represents the fire of the player) and it loses all his health in the collision, the score of the level in which is the enemy will get uploaded:
 
```
public override void OnCollision(Collidable obj)
          {
            Shoot shoot = obj as Shoot;
            if (shoot != null)
            {
                Health = Health - shoot.Damage;
                if (Health <= 0)
                {
                    playedOn.Score = playedOn.Score + Value;
                    Active = false;
                }
            }
}
```

In the Update method of the Game class, the final score gets updated when the level has been passed:

```
 level.Update(gameTime);
           if (level.levelSucceded)
     score = score + level.Score;
```

This score will get serialized in the game state GameFinishedState.

- High-score table using serialization.

For serializing the data of the players score a XML file has been used. For implementing the serialization in XML, two serializable classes has been added to the game engine part:

´´´
[Serializable]
    	    public class User
           {
        	[XmlElement("ID")]
        	public int ID { get; set; }
        	[XmlElement("Score")]
    public int score { get; set; }

[Serializable()]
    	    [XmlRoot("UserCollection")]
 	    public class UserCollection
    	    {
[XmlArray("Users"), XmlArrayItem(typeof(User), ElementName = "User")]
    public List<User> Users { get; set; }
```

The User class represents the user that is using or has used the game. The ID is set by the game based on the number of users that have played, so if the game has been played 5 times, the user ID will be 6. The score is the score achieved when the user has passed all the levels. The UserCollection represents a list of elements of the class User. 

In the Game class there is a variable for the user (User) and for the user list (usersList). In the GameStartState game state, the xml is deserialize with the following method of the Game class:

```
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
```

The list is saved in usersList and the current User initialised. When the player arrives to the GameFinishedState game state, the list gets appended the new user and serialize the list. 

```
//Saves user score and serializes it
            game.user.setScore(game.score);
            game.usersList.Add(game.user);
            UserCollection uc = new UserCollection();
            uc.Users = game.usersList;
            XmlSerializer ser = new XmlSerializer(typeof(UserCollection));
            //Inilize the stream 
game.stream = new FileStream("Content/Score/sc.xml", FileMode.Open, FileAccess.Write);
            ser.Serialize(new StreamWriter(game.stream), uc);
            game.stream.Close();
            game.currentOverlay = game.winOverlay;
```

Example of fila generated:

```
<?xml version="1.0" encoding="UTF-8"?>
<UserCollection xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
<Users>
<User>
<ID>1</ID>
<Score>300</Score>
<ID>2</ID>
<Score>2800</Score>
		       </User>
</Users>
</UserCollection>
```

With the same list that has been used for generating the XML, the score is printed in the scoreboard.
 
- Start-screen (containing intro and keyboard controls) and game over screen (with score and restart options) using state pattern and FSM with game loop

The game contain another two overlays, one for the start-screen that contains the name of the game and the keyboard controls, and the other one for the game over screen, that asks the player to press enter to restart the level.
For controlling in which state is the game, how it is the flow between those states and in consequence, which overlay has to load, a Finite State Machine (FSM) system has been implemented. The system itself is part of the game-engine, in fact as it shown at point 3) in this part, it has been used in this game for a different purpose. 
The system is constituted by the following classes:
-State: This class represents a state in which an object can be. It contains another class, Transition, that defines a transition between two states if a condition is satisfied. The State class contains a method for adding transitions to it and moreover, three abstract methods for being implemented in the classes that inherits from it: Exit that is called when the FSM is going to move to a different state,  Enter called when the FSM enters in the state and Execute that is executed when the system is updated meanwhile is in the state.
-FSM: It represents the FSM system and its parameters: the owner of the FSM, the list of states and the current state. The system allows the user to create and initialise it in a concrete state and to add more.
With the system ready, to use it as part of the game it is needed to create the states of the game as new classes that inherit from State class, so these new classes have the fields and methods from State: GameFinishedState, GameLevelState, GameOverState, GameStartState.
 
 ![image](https://user-images.githubusercontent.com/113347414/211204224-0f036933-6aa2-4966-befe-b67ec46a98d9.png)
 
Finally, in the game class, a FSM object is declared, initialized and updated so it can controls the state of the game and what has to be shown. Before FSM is initialized, the transitions between the game states are added, having as condition a bool that represents if the player has pressed the enter or a bool that represents if  the player has succeeded or lost the previous level, and if it was the last one:

```
     // Create the transitions between the states
            start.AddTransition(new Transition(levelState, () => enter));
levelState.AddTransition(new Transition(levelState, () =>                 (level.levelSucceded && (levelIndex < (numberOfLevels-1)))));
levelState.AddTransition(new Transition(over, () => level.levelFailed));
levelState.AddTransition(new Transition(finished, () => (level.levelSucceded&&(levelIndex==(numberOfLevels-1)))));
            over.AddTransition(new Transition(levelState, () => enter));
            finished.AddTransition(new Transition(start, () => enter));

            // Add the created states to the FSM
            fsm.AddState(start);
            fsm.AddState(levelState);
            fsm.AddState(over);
     fsm.AddState(finished);
```

Creating the following state machine:
![image](https://user-images.githubusercontent.com/113347414/211204294-64349418-847a-49d8-a899-bc4b7f68edd5.png)

-	Power-ups using event-listeners 

A class PowerUp has been created for helping the player in the difficult moments of the game, since it is going to increase the player health by 50 points. The PoweUp class inherits from the class Collidable, so this approach is the one that has been used to increment the health on the player. In the Player class:

```
public override void OnCollision(Collidable obj)
          {
            // Cast the object as a laser
            Laser laser = obj as Laser;
            if (laser != null)
            {
                //Other code
            } 
            // Cast the object as a Power Up
            PowerUp pu = obj as PowerUp;
            if(pu != null)
                Health = Health + pu.Health;
  }

```
On the other hand, the use of a base class for all the game objects didn’t seem to me like the best implementation in this case, because the Level class loads dynamically the objects so it needs to know exactly of which type they are.

-	NPC opponents using FSM control of game objects 

The FSM system implemented has been used with another different purpose apart from setting the overlays. It has been used to give artificial intelligence to the non-playable characters of the game, the spaceships, so they will behave in different way depending on which state they are.

The implementation of the engine part has been already explained. To use it in the IA of the enemies, it is needed to create the states of the enemy as new classes that inherit from State class, so these new classes have the fields and methods from State: EnemyAdvanceFastState, EnemyAdvanceSlowState.

![image](https://user-images.githubusercontent.com/113347414/211204443-a753cac3-0009-42f5-899b-8a3b21a93009.png)
 
When the FSM enters in those states is going to set the speed of the spaceship. The EnemyAdvanceFastState is the initial state and sets a fast speed, until the spaceship reaches the half of the window. In this moment the state will change to EnemyAdvanceSlowState that will set a really low speed, so the spaceships continue being a problem for the enemy, instead of getting out of the window.

```
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
```

Creating the following state machine:
 
![image](https://user-images.githubusercontent.com/113347414/211204482-ad45ddde-7c88-4e1b-8eb2-3bfedc6a8579.png)


Assets sources:

‐	City University Moodle (2020) INM379 Computer Games Architectures (PRD2 A 2019/20) https://moodle.city.ac.uk/pluginfile.php/1667352/mod_resource/content/0/ShooterGameContent.zip?forcedownload=1 [Accessed 25 Mar 2019]
‐	Mobile Game Graphics (2019) Download Jetpack Hero Game Kit https://mobilegamegraphics.itch.io/jetpack-hero-game-kit/download/eyJleHBpcmVzIjoxNTg2NzA1Njc0LCJpZCI6MzE0MzgyfQ%3d%3d.5Blp0brmKxYavqvMMiWinFvsIJw%3d [Accessed 25 Mar 2019]
‐	ansimuz (2018) Space Background https://ansimuz.itch.io/space-background?download [Accessed 25 Mar 2019]
‐	CraftPix.net 2D Game Assets (2019) Enemy Spaceship 2D Sprites Pixel Art https://opengameart.org/content/enemy-spaceship-2d-sprites-pixel-art [Accessed 25 Mar 2019]


