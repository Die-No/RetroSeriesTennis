using System;
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
        enum ScreenState{Title, Action, Pause,Victory}
        ScreenState CurrentScreen;
        public static SoundEffect BallCollisionBorderSound;
        public static SoundEffect BallCollisionPaddleSound;
        public static int ScreenWidth;
        public static int ScreenHeight;
        const int GAP_WALL_PADDLE = 100;
        const float BALL_SERVING_VELOCITY = 7f;
        const float PADDLE_SPEED = 10f;
        const float SPEED_POST_COLLISION = 4.5f;

        Paddle Paddle1;
        Paddle Paddle2;
        Ball Ball;
        SpriteFont RetroFont;
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
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.

            SpriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            Paddle1.Texture = Content.Load<Texture2D>("PongPaddle");
            Paddle2.Texture = Content.Load<Texture2D>("PongPaddle");
            Ball.Texture    = Content.Load<Texture2D>("PongBall");
            MenuScreen      = Content.Load<Texture2D>("PongMenu");

            CurrentScreen = ScreenState.Title;

            RetroFont = Content.Load<SpriteFont>("RetroFont");
            BallCollisionBorderSound = Content.Load<SoundEffect>("BallCollisionBorder");
            BallCollisionPaddleSound = Content.Load<SoundEffect>("BallCollisionPaddle");

            Paddle1.Position = new Vector2(GAP_WALL_PADDLE, ScreenHeight / 2 - Paddle1.Texture.Height / 2);
            Paddle2.Position = new Vector2(ScreenWidth - Paddle2.Texture.Width - GAP_WALL_PADDLE, ScreenHeight / 2 - Paddle2.Texture.Height / 2);
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

        public void VictoryScreen(String Paddle)
        {
            CurrentScreen = ScreenState.Victory;

            if (Paddle == "Paddle1")
            {
                SpriteBatch.DrawString(RetroFont, "Player 1 wins!",
                                      new Vector2(0, 0), Color.White);

                SpriteBatch.DrawString(RetroFont, "Press enter to restart",
                                      new Vector2(0, ScreenHeight/2), Color.White);
            
            }

            if (Paddle == "Paddle2")
            {
                SpriteBatch.DrawString(RetroFont, "Player 2 wins!",
                                      new Vector2(0, 0), Color.White);

                SpriteBatch.DrawString(RetroFont, "Press enter to restart",
                                      new Vector2(0, ScreenHeight / 2), Color.White);

            }
        }
        protected override void Update(GameTime gameTime)
        {
            if (CurrentScreen == ScreenState.Title)
            {
                String query = Input.GetKeyEntryMenu();
                if (query == "Enter")
                {
                    CurrentScreen = ScreenState.Action;
                    Ball.Serve(BALL_SERVING_VELOCITY);
                }

            }
            else if (CurrentScreen == ScreenState.Pause)

            {
                if (Input.isGamePaused() == "Unpause")
                {
                    CurrentScreen = ScreenState.Action;
                }
            }
            else if (CurrentScreen == ScreenState.Victory)

            {
                String query = Input.GetKeyEntryMenu();
                if (query == "Enter")
                {
                    Paddle1.Score = 0;
                    Paddle2.Score = 0;
                    CurrentScreen = ScreenState.Action;
                }
            }
            else if (CurrentScreen == ScreenState.Action)

            {
                // TODO: Add your update logic here
                Vector2 paddle1TouchVelocity;
                Vector2 paddle2TouchVelocity;

                ScreenHeight = GraphicsDevice.Viewport.Height;
                ScreenWidth =  GraphicsDevice.Viewport.Width;
                Rectangle ScreenSize = new Rectangle(0, 0, ScreenWidth, ScreenHeight);
                Ball.Move(Ball.Velocity);

                Vector2 paddle1Velocity = Input.GetKeyboardInputDirection(PlayerIndex.One) * PADDLE_SPEED;
                Vector2 paddle2Velocity = Input.GetKeyboardInputDirection(PlayerIndex.Two) * PADDLE_SPEED;

                Paddle1.Move(paddle1Velocity);
                Paddle2.Move(paddle2Velocity);

                if (Input.isGamePaused() == "Pause")
                {
                    CurrentScreen = ScreenState.Pause;
                }

                Input.ProcessTouchInput(out paddle1TouchVelocity, out paddle2TouchVelocity);
                Paddle1.Move(paddle1TouchVelocity);
                Paddle2.Move(paddle2TouchVelocity);

                if (paddle1TouchVelocity.Y > 0)
                {
                    paddle1Velocity = paddle1TouchVelocity;
                }

                if (paddle2TouchVelocity.Y > 0)
                {
                    paddle2Velocity = paddle2TouchVelocity;
                }

                if (paddle1Velocity.Y != 0)
                {
                    paddle1Velocity.Normalize();
                }

                if (paddle2Velocity.Y != 0)
                {
                    paddle2Velocity.Normalize();

                }

                if (Sprite.CheckPadlleHitsBall(Paddle1, Ball))
                {
                    Ball.Velocity.X = Math.Abs(Ball.Velocity.X);
                    Ball.Velocity += paddle1Velocity * SPEED_POST_COLLISION;
                    BallCollisionPaddleSound.Play();
                }

                if (Sprite.CheckPadlleHitsBall(Paddle2, Ball))
                {
                    Ball.Velocity.X = -Math.Abs(Ball.Velocity.X);
                    Ball.Velocity += paddle1Velocity * SPEED_POST_COLLISION;
                    BallCollisionPaddleSound.Play();

                }

                if (Ball.Position.X + Ball.Texture.Width < 0)
                {
                    Ball.Serve(BALL_SERVING_VELOCITY);
                    Paddle2.Score++;
                }

                if (Ball.Position.X + Ball.Texture.Width > ScreenWidth)
                {
                    Ball.Serve(BALL_SERVING_VELOCITY);
                    Paddle1.Score++;
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

            if (CurrentScreen == ScreenState.Action)
            {

                if (Paddle1.Score == 21)
                {
                    VictoryScreen("Paddle1");
                }

                else if (Paddle2.Score == 21)
                {
                    VictoryScreen("Paddle2");
                }

                else SpriteBatch.DrawString(RetroFont, Paddle1.Score + " - " + Paddle2.Score,
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
