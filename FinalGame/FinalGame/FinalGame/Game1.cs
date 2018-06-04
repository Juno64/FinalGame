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
                            GenerateBall();
                            gameState = GameState.one;
                            LoadNextLevel(Convert.ToInt32(gameState));
                        }
                    }
                    break;
                case GameState.winScreen:
                    if (oldMouse.LeftButton == ButtonState.Pressed && mouse.LeftButton == ButtonState.Pressed)
                    {
                        if (winRect.Contains(cursorPoint))
                        {
                            GenerateBall();
                            gameState = GameState.one;
                            LoadNextLevel(Convert.ToInt32(gameState));
                            lives = 3;
                        }
                    }
                    break;
                case GameState.loseScreen:
                    if (oldMouse.LeftButton == ButtonState.Pressed && mouse.LeftButton == ButtonState.Pressed)
                    {
                        if (loseRect.Contains(cursorPoint))
                        {
                            GenerateBall();
                            gameState = GameState.one;
                            LoadNextLevel(Convert.ToInt32(gameState));
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
                                /*
                                if (ball.X <= brickRect.X && ball.X + ballWidth >= brickRect.X)
                                {
                                    ballVelocity.X *= -1;
                                }
                                if (ball.X <= brickRect.X + brickRect.Width && ball.X + ballWidth >=
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
                                //line of ball = m(x - x1) + y1, m = yvel/xvel, x1 and y1 = position coords
                                bool collisionOccurred = false;
                                bool isLineVertical;
                                int slope = 0; //arbitrary default
                                if(ballVelocity.X != 0)
                                {
                                    slope = Convert.ToInt32(ballVelocity.Y / ballVelocity.X);
                                    isLineVertical = false;
                                }
                                else
                                {
                                    isLineVertical = true;
                                }
                                int[] yValsOfIntersection = { -1, -1, -1, -1}; //0- left vertical, 1- right vertical, 2- upper horizontal, 3- lower horizontal
                                //left vertical
                                if (!isLineVertical)
                                {
                                    int yIntersect0 = Convert.ToInt32(slope * (brickRect.Left - ball.Center.X) + ball.Center.Y);
                                    if (yIntersect0 > brickRect.Top && yIntersect0 < brickRect.Bottom && 
                                        ((ball.Top <= yIntersect0 && ball.Top - ballVelocity.Y > yIntersect0 ) || (ball.Bottom >= yIntersect0 && ball.Bottom - ballVelocity.Y < yIntersect0)))
                                    {
                                        yValsOfIntersection[0] = yIntersect0;
                                        collisionOccurred = true;
                                    }
                                    //if line is vertical, collision with left vertical edge is impossible.

                                    //right vertical
                                    int yIntersect1 = Convert.ToInt32(slope * (brickRect.Right - ball.Center.X) + ball.Center.Y);
                                    if (yIntersect1 > brickRect.Top && yIntersect1 < brickRect.Bottom && 
                                        ((ball.Top <= yIntersect1 && ball.Top - ballVelocity.Y > yIntersect1) || (ball.Bottom >= yIntersect1 && ball.Bottom - ballVelocity.Y < yIntersect1)))
                                    {
                                        yValsOfIntersection[1] = yIntersect1;
                                        collisionOccurred = true;
                                    }
                                    //if line is vertical, collision with right vertical edge is impossible.

                                    //upper horizontal
                                    int xIntersect2 = Convert.ToInt32(brickRect.Top / slope - ball.Center.Y / slope + ball.Center.X);
                                    int yIntersect2 = brickRect.Top;
                                    if (xIntersect2 > brickRect.Left && xIntersect2 < brickRect.Right && ball.Bottom >= yIntersect2 && ball.Bottom - ballVelocity.Y < yIntersect2)
                                    {
                                        yValsOfIntersection[2] = brickRect.Top;
                                        collisionOccurred = true;
                                    }

                                    //lower horizontal
                                    int xIntersect3 = Convert.ToInt32(brickRect.Bottom / slope - ball.Center.Y / slope + ball.Center.X);
                                    int yIntersect3 = brickRect.Bottom;
                                    if (xIntersect3 > brickRect.Left && xIntersect3 < brickRect.Right && ball.Bottom <= yIntersect3 && ball.Bottom - ballVelocity.Y > yIntersect3)
                                    {
                                        yValsOfIntersection[3] = brickRect.Bottom;
                                        collisionOccurred = true;
                                    }
                                    if (ballVelocity.Y < 0)
                                    {
                                        for (int k = 0; k < yValsOfIntersection.Length; k++)
                                        {
                                            if (yValsOfIntersection[k] != -1)
                                            {
                                                yValsOfIntersection[k] *= -1;
                                            }
                                        }
                                    }
                                    int indexOfFirstIntersectedEdge = -1; //if this variable remains at -1, then no intersection has occurred.
                                    int yValueOfIntersectionWithFirstIntersectedEdge = int.MaxValue;
                                    for (int k = 0; k < yValsOfIntersection.Length; k++)
                                    {
                                        if (yValsOfIntersection[k] != -1)
                                        {
                                            if (yValsOfIntersection[k] < yValueOfIntersectionWithFirstIntersectedEdge)
                                            {
                                                indexOfFirstIntersectedEdge = k;
                                                yValueOfIntersectionWithFirstIntersectedEdge = yValsOfIntersection[k];
                                            }
                                        }
                                    }
                                    if (indexOfFirstIntersectedEdge == 0 || indexOfFirstIntersectedEdge == 1)
                                    {
                                        ballVelocity.X *= -1;
                                    }
                                    else if (indexOfFirstIntersectedEdge == 2 || indexOfFirstIntersectedEdge == 3)
                                    {
                                        ballVelocity.Y *= -1;
                                    }
                                    if (indexOfFirstIntersectedEdge != -1)
                                    {
                                        Console.WriteLine("collision with edge " + indexOfFirstIntersectedEdge + " at brick " + i + ", " + j + " at y value " + yValueOfIntersectionWithFirstIntersectedEdge);
                                    }
                                }
                                else if (isLineVertical)
                                {
                                    if(ball.Left >= brickRect.Left && ball.Right >= brickRect.Right && ((ball.Bottom >= brickRect.Top && ball.Bottom - ballVelocity.Y < brickRect.Top) ||
                                        (ball.Top >= brickRect.Bottom && ball.Top - ballVelocity.Y > brickRect.Bottom)))
                                    {
                                        ballVelocity.Y *= -1;
                                        collisionOccurred = true;
                                        Console.WriteLine("vertical collision" + " at brick " + i + ", " + j);
                                    }
                                }
                                if (collisionOccurred)
                                {
                                    debug = true;
                                    double vectorAngle = Math.Atan(ballVelocity.Y / ballVelocity.X);
                                    ballVelocity.X = Convert.ToInt32((280 - brickRect.Y) * Math.Cos(vectorAngle) / 25);
                                    ballVelocity.Y = Convert.ToInt32((280 - brickRect.Y) * Math.Sin(vectorAngle) / 25);
                                    bricks[i, j] = null;
                                    bricksLeft--;
                                    Console.WriteLine(bricksLeft);
                                    if (bricksLeft <= 0)
                                    {
                                        gameState++;
                                        LoadNextLevel(Convert.ToInt32(gameState));
                                        GenerateBall();
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
                            GenerateBall();
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

        void LoadNextLevel(int levelNum)
        {
            if (gameState != GameState.winScreen)
            {
                string path = string.Format("../../../../FinalGameContent/Levels/{0}.txt", levelNum);
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
                        if (charArr[j] != ' ')
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
        }

        void GenerateBall()
        {
            ball = new Rectangle(3 * paddleWidth / 2, windowHeight - 3 * paddleHeight / 2, ballWidth, ballHeight);
            ballVelocity = new Vector2(4, -4);
        }
        
    }
}
