using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;

namespace FinalGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        const int windowWidth = 800;
        const int windowHeight = 600;
        const int paddleSpeed = 12;
        const double maxAngle = Math.PI / 2;
        Rectangle paddle;
        int paddleWidth;
        int paddleHeight;
        Rectangle ball;
        int ballWidth;
        int ballHeight;
        Rectangle startRect;
        Rectangle loseRect;
        Rectangle winRect;
        Texture2D paddleTexture;
        Texture2D ballTexture;
        Texture2D startTexture;
        Texture2D loseTexture;
        Texture2D winTexture;
        Texture2D brickTexture;
        Vector2 ballVelocity;
        Brick[,] bricks;
        int lives;
        int bricksLeft;
        MouseState mouse;
        MouseState oldMouse;
        Point cursorPoint;
        KeyboardState keyboard;
        enum GameState
        {
            titleScreen,
            one,
            two,
            three,
            four,
            five,
            six,
            seven,
            eight,
            nine,
            winScreen,
            loseScreen
        }
        GameState gameState;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = windowWidth;
            graphics.PreferredBackBufferHeight = windowHeight;
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
            IsMouseVisible = true;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            gameState = GameState.titleScreen;
            paddleWidth = windowWidth / 8;
            paddleHeight = windowHeight / 60;
            paddle = new Rectangle(0, windowHeight - paddleHeight, paddleWidth, paddleHeight);
            ballWidth = windowWidth / 80;
            ballHeight = windowHeight / 60;
            ballVelocity = new Vector2();
            startRect = new Rectangle(windowWidth / 2 - 200, 2 * windowHeight / 3 - 50, 400, 100);
            loseRect = new Rectangle(windowWidth / 2 - 100, windowHeight / 2 - 25, 200, 50);
            winRect = new Rectangle(windowWidth / 2 - 150, windowHeight / 2 - 30, 300, 60);
            bricks = new Brick[10, 15]; // bricks are therefore 80 x 40
            lives = 3;

            paddleTexture = Content.Load<Texture2D>("paddleTexture");
            ballTexture = Content.Load<Texture2D>("ballTexture");
            startTexture = Content.Load<Texture2D>("startTexture");
            winTexture = Content.Load<Texture2D>("winTexture");
            loseTexture = Content.Load<Texture2D>("loseTexture");
            brickTexture = Content.Load<Texture2D>("brickTexture");
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
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            mouse = Mouse.GetState();
            cursorPoint = new Point(mouse.X, mouse.Y);
            keyboard = Keyboard.GetState(PlayerIndex.One);
            switch (gameState)
            {
                case GameState.titleScreen:
                    if (oldMouse.LeftButton == ButtonState.Pressed && mouse.LeftButton == ButtonState.Pressed)
                    {
                        if(startRect.Contains(cursorPoint))
                        {
                            generateBall();
                            gameState = GameState.one;
                            loadNextLevel();
                        }
                    }
                    break;
                case GameState.winScreen:
                    if (oldMouse.LeftButton == ButtonState.Pressed && mouse.LeftButton == ButtonState.Pressed)
                    {
                        if (winRect.Contains(cursorPoint))
                        {
                            generateBall();
                            gameState = GameState.one;
                            lives = 3;
                        }
                    }
                    break;
                case GameState.loseScreen:
                    if (oldMouse.LeftButton == ButtonState.Pressed && mouse.LeftButton == ButtonState.Pressed)
                    {
                        if (loseRect.Contains(cursorPoint))
                        {
                            generateBall();
                            gameState = GameState.one;
                            lives = 3;
                        }
                    }
                    break;
                default:
                    bool debug = false;
                    if(keyboard.IsKeyDown(Keys.A) && paddle.X - paddleSpeed >= 0)
                    {
                        paddle.X -= paddleSpeed;
                    }
                    if (keyboard.IsKeyDown(Keys.D) && paddle.X + paddleSpeed <= windowWidth - paddleWidth)
                    {
                        paddle.X += paddleSpeed;
                    }
                    if (ball.X < 0 || ball.X + ballWidth > windowWidth)
                    {
                        ballVelocity.X *= -1;
                    }
                    if (ball.Y < 0)
                    {
                        ballVelocity.Y *= -1;
                    }
                    for (int i = 0; i < bricks.GetLength(0); i++)
                    {
                        for (int j = 0; j < bricks.GetLength(1); j++)
                        {
                            if (bricks[i, j] != null)
                            {
                                Rectangle brickRect = bricks[i, j].BrickRectangle;
                                if(ball.Intersects(brickRect))
                                {
                                    /*
                                    if (ball.X <= brickRect.X && ball.X + ballWidth >= brickRect.X)
                                    {
                                        ballVelocity.X *= -1;
                                    }
                                    if (ball.X <= brickRect.X + brickRect.Width && b
                                     * 
                                     * all.X + ballWidth >=
                                        brickRect.X + brickRect.Width)
                                    {
                                        ballVelocity.X *= -1;
                                    }
                                    if (ball.Y <= brickRect.Y && ball.Y + ballHeight >= brickRect.Y)
                                    {
                                        ballVelocity.Y *= -1;
                                    }
                                    if (ball.Y <= brickRect.Y + brickRect.Height && ball.Y + ballHeight >=
                                        brickRect.Y + brickRect.Height)
                                    {
                                        ballVelocity.Y *= -1;
                                    }
                                    */
                                    if (ballVelocity.X > 0)
                                    {

                                    }
                                    double vectorAngle = Math.Atan(ballVelocity.Y / ballVelocity.X);
                                    ballVelocity.X = Convert.ToInt32((280 - bricks[i, j].BrickRectangle.Y) * Math.Cos(vectorAngle) / 25);
                                    ballVelocity.Y = Convert.ToInt32((280 - bricks[i, j].BrickRectangle.Y) * Math.Sin(vectorAngle) / 25);
                                    bricks[i, j] = null;
                                    bricksLeft--;
                                    Console.WriteLine(bricksLeft);
                                    if (bricksLeft <= 0)
                                    {
                                        gameState++;
                                        loadNextLevel();
                                        generateBall();
                                    }
                                }
                            }
                        }
                    }
                    if (paddle.Intersects(ball))
                    {
                        Console.WriteLine("ASDSADSA");
                        double refAngle = maxAngle * ((paddle.X + paddleWidth / 2) - (ball.X + ballWidth / 2)) / paddleWidth;
                        ballVelocity = new Vector2
                            (Convert.ToInt32(Convert.ToDouble(ballVelocity.Length()) * Math.Cos(3 * Math.PI / 2 - refAngle)),
                             Convert.ToInt32(Convert.ToDouble(ballVelocity.Length()) * Math.Sin(3 * Math.PI / 2 - refAngle)));
                        
                    }
                    else if (ball.Y > windowHeight)
                    {
                        lives--;
                        if (lives > 0)
                        {
                            generateBall();
                        }
                    }
                    ball.X += Convert.ToInt32(ballVelocity.X);
                    ball.Y += Convert.ToInt32(ballVelocity.Y);
                    if (debug)
                    {
                        Console.WriteLine("VELO: {0} || {1}", ballVelocity.X, ballVelocity.Y);
                        Console.WriteLine("POS: {0} <> {1}", ball.X, ball.Y);
                    }
                    if (lives <= 0)
                    {
                        gameState = GameState.loseScreen;
                    }
                    break;
            }
            oldMouse = mouse;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            switch (gameState)
            {
                case GameState.titleScreen:
                    spriteBatch.Draw(startTexture, startRect, Color.White);
                    break;
                case GameState.winScreen:
                    spriteBatch.Draw(winTexture, winRect, Color.White);
                    break;
                case GameState.loseScreen:
                    spriteBatch.Draw(loseTexture, loseRect, Color.White);
                    break;
                default:
                    spriteBatch.Draw(paddleTexture, paddle, Color.White);
                    spriteBatch.Draw(ballTexture, ball, Color.White);
                    for (int i = 0; i < bricks.GetLength(0); i++)
                    {
                        for (int j = 0; j < bricks.GetLength(1); j++)
                        {
                            if (bricks[i, j] != null)
                            {
                                spriteBatch.Draw(brickTexture, bricks[i, j].BrickRectangle,
                                    bricks[i, j].BrickColor);
                            }
                        }
                    }
                    break;
            }
            base.Draw(gameTime);
            spriteBatch.End();
        }

        void loadNextLevel()
        {
            string path = string.Format("../../../../FinalGameContent/Levels/{0}.txt", Convert.ToInt32(gameState));
            StreamReader file = new StreamReader(path);
            List<string> lines = new List<string>();
            string line = file.ReadLine();
            while (line != null)
            {
                lines.Add(line);
                line = file.ReadLine();
            }
            int bricksHorizontal = bricks.GetLength(0);
            int bricksVertical = bricks.GetLength(1);
            bricksLeft = 0;
            for (int i = 0; i < lines.Count; i++) //lines.Count should be less than or equal to 15.
            {
                char[] charArr = lines[i].ToCharArray();
                for (int j = 0; j < charArr.Length; j++) //charArr.Length should be 10
                {
                    Rectangle generatedBrickRectangle = new Rectangle(j * windowWidth / bricksHorizontal,
                                    i * windowHeight / bricksVertical, windowWidth / bricksHorizontal,
                                    windowHeight / bricksVertical);
                    if (charArr[j] != null)
                    {
                        bricksLeft++;
                        if (charArr[j] == 'A')
                        {
                            bricks[i, j] = new Brick(Color.Red, generatedBrickRectangle);
                        }
                        else if (charArr[j] == 'B')
                        {
                            bricks[i, j] = new Brick(Color.Blue, generatedBrickRectangle);
                        }
                        else if (charArr[j] == 'C')
                        {
                            bricks[i, j] = new Brick(Color.Green, generatedBrickRectangle);
                        }
                    }
                }
            }
        }

        void generateBall()
        {
            ball = new Rectangle(3 * paddleWidth / 2, windowHeight - 3 * paddleHeight / 2, ballWidth, ballHeight);
            ballVelocity = new Vector2(4, -4);
        }

    }
}
