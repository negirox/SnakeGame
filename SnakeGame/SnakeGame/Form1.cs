using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SnakeGame
{
    public partial class Form1 : Form
    {
        private List<Circle> Snake = new List<Circle>();
        private Circle food = new Circle();
        public Form1()
        {
            InitializeComponent();
            InitSettings();
            //lblGameover.Visible = false;
        }

        private void InitSettings()
        {
            //Initialize Settings.
            new GlobalSettings();
            //Set game speed and start timer
            gameTimer.Interval = 1000 / GlobalSettings.Speed;
            gameTimer.Tick += (UpdateScreen);
            //start timer
            gameTimer.Start();

            //
            StartNewGame();
        }

        private void UpdateScreen(object sender,EventArgs e)
        {
            if (GlobalSettings.GameOver)
            {
                if (Inputs.KeyPressed(Keys.Enter))
                {
                    StartNewGame();
                }
            }
            else
            {
                if (Inputs.KeyPressed(Keys.Right) && GlobalSettings.Direction != Direction.Left)
                    GlobalSettings.Direction = Direction.Right;

                else if (Inputs.KeyPressed(Keys.Left) && GlobalSettings.Direction != Direction.Right)
                    GlobalSettings.Direction = Direction.Left;

                else if (Inputs.KeyPressed(Keys.Up) && GlobalSettings.Direction != Direction.Down)
                    GlobalSettings.Direction = Direction.Up;
                else if (Inputs.KeyPressed(Keys.Down) && GlobalSettings.Direction != Direction.Up)
                    GlobalSettings.Direction = Direction.Down;

                MovePlayer();
            }
            pbCanvas.Invalidate();

        }

       
        private void StartNewGame()
        {
            lblGameover.Visible = false;
            new GlobalSettings();
            Snake.Clear();
            Circle head = new Circle();
            head.X = 10;
            head.Y = 5;
            Snake.Add(head);

            lblScore.Text = GlobalSettings.Score.ToString();
            GenerateFood();
        }

        private void GenerateFood()
        {
            int maxXPos = pbCanvas.Size.Width / GlobalSettings.Width;
            int maxYPos = pbCanvas.Size.Height / GlobalSettings.Height;

            Random random = new Random();
            food = new Circle();
            food.X = random.Next(0, maxXPos);
            food.Y = random.Next(0, maxYPos);
             
        }

        private void pbCanvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;
            if (!GlobalSettings.GameOver)
            {
                
                for(int i = 0; i < Snake.Count; i++)
                {
                    Brush snakeColor;
                    if (i == 0)
                        snakeColor = Brushes.Black;
                    else
                        snakeColor = Brushes.Green;

                    canvas.FillEllipse(snakeColor, 
                        new Rectangle(Snake[i].X * GlobalSettings.Width, 
                        Snake[i].Y * GlobalSettings.Width,
                        GlobalSettings.Width, GlobalSettings.Height));


                    canvas.FillEllipse(Brushes.Chocolate,
                        new Rectangle(food.X * GlobalSettings.Width,
                        food.Y * GlobalSettings.Width,
                        GlobalSettings.Width, GlobalSettings.Height));
                }
            }
            else
            {
                string gameOver = "Game Over \n Your Final Score is:" + GlobalSettings.Score +"\n Press Enter to Start Again.";
                lblGameover.Text = gameOver;
                lblGameover.Visible = true;
            }
        }

        private void MovePlayer()
        {
            for (int i = Snake.Count - 1; i >= 0; i--)
            {
                if (i == 0)
                {
                    switch (GlobalSettings.Direction)
                    {
                        case Direction.Right:
                            Snake[i].X++;
                            break;
                        case Direction.Left:
                            Snake[i].X--;
                            break;
                        case Direction.Up:
                            Snake[i].Y--;
                            break;
                        case Direction.Down:
                            Snake[i].Y++;
                            break;
                    }
                    int maxXPos = pbCanvas.Size.Width / GlobalSettings.Width;
                    int maxYPos = pbCanvas.Size.Height / GlobalSettings.Height;

                    //Detect collission with game borders.
                    if (Snake[i].X < 0 || Snake[i].Y < 0
                        || Snake[i].X >= maxXPos || Snake[i].Y >= maxYPos)
                    {
                        Die();
                    }


                    //Detect collission with body
                    for (int j = 1; j < Snake.Count; j++)
                    {
                        if (Snake[i].X == Snake[j].X &&
                           Snake[i].Y == Snake[j].Y)
                        {
                            Die();
                        }
                    }

                    //Detect collision with food piece
                    if (Snake[0].X == food.X && Snake[0].Y == food.Y)
                    {
                        Eat();
                    }

                }
                else
                {
                    Snake[i].X = Snake[i - 1].X;
                    Snake[i].Y = Snake[i - 1].Y;
                }
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            Inputs.ChangeState(e.KeyCode,true);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            Inputs.ChangeState(e.KeyCode, false);
        }

        private void Eat()
        {
            //Add circle to body
            Circle circle = new Circle
            {
                X = Snake[Snake.Count - 1].X,
                Y = Snake[Snake.Count - 1].Y
            };
            Snake.Add(circle);

            //Update Score
            GlobalSettings.Score += GlobalSettings.Points;
            lblScore.Text = GlobalSettings.Score.ToString();

            GenerateFood();
        }

        private void Die()
        {
            GlobalSettings.GameOver = true;
        }
    }
}
