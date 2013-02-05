using System;
using Windows.ApplicationModel.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace RetroSeriesTennis
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch SpriteBatch;
        Texture2D MenuScreen;

        enum ScreenState{Title, Action, Pause,Victory}    //Used to determine if paused, at a menu etc
        ScreenState CurrentScreen;

        public static SoundEffect BallCollisionBorderSound;  
        public static SoundEffect BallCollisionPaddleSound;

        public static int ScreenWidth; //Used to determine height and width of screen
        public static int ScreenHeight;

        const int   GAP_WALL_PADDLE = 100; //Gap between side of screen and paddle
        const float BALL_SERVING_VELOCITY = 7f; //initial speed of ball
        const float PADDLE_SPEED = 12f; //speed of paddles
        const float SPEED_POST_COLLISION = 4.5f; //added speed of ball if hit by a moving paddle
        const int   WINNING_SCORE = 21;  //The score needed to win

        Paddle Paddle1;
        Paddle Paddle2;
        Ball Ball; //game objects

        SpriteFont RetroFont; //Font used for various things
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            TouchPanel.EnabledGestures = GestureType.FreeDrag;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            ScreenHeight = GraphicsDevice.Viewport.Height;
            ScreenWidth  = GraphicsDevice.Viewport.Width;
            Paddle1 = new Paddle();
            Paddle2 = new Paddle();
            Ball = new Ball();


            base.Initialize();
        }


          /// <summary>
          /// LoadContent will be called once per game and is the place to load
          /// all of your content.
          /// </summary>
          /// 

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.

            SpriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            Paddle1.Texture = Content.Load<Texture2D>("PongPaddle");
            Paddle2.Texture = Content.Load<Texture2D>("PongPaddle");
            Ball.Texture    = Content.Load<Texture2D>("PongBall");
            MenuScreen      = Content.Load<Texture2D>("PongMenu");

            CurrentScreen = ScreenState.Title; //Initial state of game is the title screen

            RetroFont = Content.Load<SpriteFont>("RetroFont");
            BallCollisionBorderSound = Content.Load<SoundEffect>("BallCollisionBorder");
            BallCollisionPaddleSound = Content.Load<SoundEffect>("BallCollisionPaddle");

            //These are the initial positions of the paddle, relative to the screen size
            Paddle1.Position = new Vector2(GAP_WALL_PADDLE, ScreenHeight / 2 - Paddle1.Texture.Height / 2);
            Paddle2.Position = new Vector2((ScreenWidth - Paddle2.Texture.Width - GAP_WALL_PADDLE), (ScreenHeight / 2 - Paddle2.Texture.Height / 2));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>


        //Method run when one player reaches 21 points
          
        protected override void Update(GameTime gameTime)
        {

            ScreenHeight = GraphicsDevice.Viewport.Height;
            ScreenWidth = GraphicsDevice.Viewport.Width;
            Rectangle ScreenSize = new Rectangle(0, 0, ScreenWidth, ScreenHeight);

            if (CurrentScreen == ScreenState.Title)   //If the screen is the title screen
            {
                String query = Input.GetKeyEntryMenu();
                if (query == "Enter")
                {
                    CurrentScreen = ScreenState.Action;
                    Ball.Serve(BALL_SERVING_VELOCITY);
                }

            }
            else if (CurrentScreen == ScreenState.Pause) //If the game is paused

            {
                if (Input.isGamePaused() == "Unpause")
                {
                    CurrentScreen = ScreenState.Action;
                }
            }
            else if (CurrentScreen == ScreenState.Victory) //If a game just ended

            {
                String query = Input.GetKeyEntryMenu();
                if (query == "Enter")
                {
                    Paddle1.Score = 0;
                    Paddle2.Score = 0;
                    CurrentScreen = ScreenState.Action;
                }
            }
            else if (CurrentScreen == ScreenState.Action) //If the game is being played

            {
                Paddle2.Position.X = (ScreenWidth - Paddle2.Texture.Width - GAP_WALL_PADDLE);
                Ball.Move(Ball.Velocity);

                Vector2 paddle1Velocity = Input.GetKeyboardInputDirection(PlayerIndex.One) * PADDLE_SPEED;
                Vector2 paddle2Velocity = Input.GetKeyboardInputDirection(PlayerIndex.Two) * PADDLE_SPEED;

                Paddle1.Move(paddle1Velocity);
                Paddle2.Move(paddle2Velocity);

                if (Input.isGamePaused() == "Pause")
                {
                    CurrentScreen = ScreenState.Pause;
                }

                Input.ProcessTouchInput(out paddle1Velocity, out paddle2Velocity);
                Paddle1.Move(paddle1Velocity);
                Paddle2.Move(paddle2Velocity);                

                if (Sprite.CheckPadlleHitsBall(Paddle1, Ball))
                {
                    Ball.Velocity.X = Math.Abs(Ball.Velocity.X);
                    Ball.Velocity += paddle1Velocity * SPEED_POST_COLLISION;
                    BallCollisionPaddleSound.Play();
                }

                if (Sprite.CheckPadlleHitsBall(Paddle2, Ball))
                {
                    Ball.Velocity.X = -Math.Abs(Ball.Velocity.X);
                    Ball.Velocity += paddle2Velocity * SPEED_POST_COLLISION;
                    BallCollisionPaddleSound.Play();

                }

                if (Ball.Position.X + Ball.Texture.Width < 0)
                {
                    Ball.Serve(BALL_SERVING_VELOCITY);
                    Paddle2.Score++;

                    if (Paddle2.Score == WINNING_SCORE)
                    {
                        CurrentScreen = ScreenState.Victory;
                    }
                }

                if (Ball.Position.X + Ball.Texture.Width > ScreenWidth)
                {
                    Ball.Serve(BALL_SERVING_VELOCITY);
                    Paddle1.Score++;
                    if (Paddle1.Score == WINNING_SCORE)
                    {
                        CurrentScreen = ScreenState.Victory;
                    }
                }
                base.Update(gameTime);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            SpriteBatch.Begin();


            if (CurrentScreen == ScreenState.Title)
            {
                Rectangle ScreenSize = new Rectangle(0, 0, ScreenWidth, ScreenHeight);
                SpriteBatch.Draw(MenuScreen, ScreenSize, Color.White);
              
            }
            if (CurrentScreen == ScreenState.Pause)
            {
                SpriteBatch.DrawString(RetroFont, "Press U To Unpause",
                    new Vector2(ScreenWidth / 2 - RetroFont.MeasureString("Press U To Unpause")
                .X / 2, ScreenHeight / 2), Color.White);
            }
            if (CurrentScreen == ScreenState.Victory)
            {

                if (Paddle1.Score ==WINNING_SCORE)  //If Player 1 won
                {
                    SpriteBatch.DrawString(RetroFont, "Player 1 wins!",
                                          new Vector2(0, 0), Color.White);

                    SpriteBatch.DrawString(RetroFont, "Press enter to restart",
                                          new Vector2(0, ScreenHeight / 2), Color.White);

                }

                if (Paddle2.Score == WINNING_SCORE) //If Player 2 Won
                {
                    SpriteBatch.DrawString(RetroFont, "Player 2 wins!",
                                          new Vector2(0, 0), Color.White);

                    SpriteBatch.DrawString(RetroFont, "Press enter to restart",
                                          new Vector2(0, ScreenHeight / 2), Color.White);

                }
            }
        
            if (CurrentScreen == ScreenState.Action)
            {

                
                SpriteBatch.DrawString(RetroFont, Paddle1.Score + " - " + Paddle2.Score,
                                       new Vector2(ScreenWidth / 2 - RetroFont.MeasureString(Paddle1.Score + " " + Paddle2.Score)
                                       .X / 2, 0), Color.White);
                Paddle1.Draw(SpriteBatch);
                Paddle2.Draw(SpriteBatch);
                Ball.Draw(SpriteBatch);
            }
            SpriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
