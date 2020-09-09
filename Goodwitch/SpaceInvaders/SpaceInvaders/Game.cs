using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Timers;
using System.Windows.Forms;
using SpaceInvaders.Properties;
using Timer = System.Timers.Timer;

namespace SpaceInvaders
{
    class Game : IDisposable
    {
        public Form1 Form { get; }

        /// <summary>
        /// Number of available lifes.
        /// </summary>
        public int Life { get; private set; } = 5;


        /// <summary>
        /// Current level playing.
        /// </summary>
        public int Level
        {
            get => lvl;
            private set
            {
                PutTextIntoControl(value, Form.actualLevelLabel);
                lvl = value;
            }
        }
        private int lvl = 0;



        /// <summary>
        /// Current score.
        /// </summary>
        public int Score
        {
            get => score;
            private set
            {
                PutTextIntoControl(value, Form.actualScoreLabel);
                score = value;
            }
        }
        private int score = 0;



        //Collection of tank missiles
        private List<PointF> tankMissiles = new List<PointF>();

        //Collection of all invaders' explosions
        private List<PointF> explosions = new List<PointF>();

        //Collection of all invaders' explosions used for drawing (thread safe)
        private List<PointF> explosionsSafe = new List<PointF>();

        //Collection of invaders' missiles
        private List<PointF> invadersMissiles = new List<PointF>();

        //Collection of invaders' missiles used for drawing (thread safe)
        private List<PointF> invadersMissilesSafe = new List<PointF>();

        //Collection of invaders
        private List<Invader> invaders = new List<Invader>();

        //Collection of barriers
        private List<Barrier> barriers = new List<Barrier>();

        //Contains bottom green line
        private List<RectangleF> bottomLine = new List<RectangleF>();

        //Constains location of red ufo (mystery)
        private PointF? mysteryLocation = null;

        //Explosion position of killed mystery
        private PointF? mysteryExplosion;

        //Current position of tank
        private PointF tankLocation;

        //Ticks when explosion should be deleted
        private Timer mysteryExplosionTimer = new Timer();

        //Main game loop
        private Timer drawingTimer;

        //Ticks when new mystery should appear
        private Timer mysteryShouldAppearTimer;

        //Ticks when invaders should make a move
        private Timer invadersShouldMoveTimer;

        //Ticks when invader explosion should be removed
        private Timer explosionsTimer = new Timer();

        //Ticks when invaders should shoot
        private Timer invadersShouldShootTimer = new Timer();

        //Ticks after showing tank explosion, respawns the tank and ensures than the tank is immune for couple of seconds
        private Timer tankExplosionTimer = new Timer(800);

        //Ticks when immunity of tank should turn off
        private Timer immunityTimer = new Timer(3000);

        //Tank has reloaded and its ready to shoot again
        private Timer tankCanShootTimer = new Timer();

        //Flag stating whether it is possible to shoot with tank (tank needs time to reload)
        private bool canTankShoot = true;

        //Left arrow key pressed
        private bool keyLArrow = false;

        //Right arrow key pressed
        private bool keyRArrow = false;

        //Space key pressed
        private bool keySpace = false;

        //Direction of invaders' movement
        private bool invadersMovingRight = true;

        //Locking invaders' movement when changing direction
        private bool lckMovement = false;

        //locking the drawingTimer, so it cant be called more than once concurrently
        private bool lockingToken = false;

        //Should be tank drawn or its ruins?
        private bool shouldDrawTank = true;

        //Is immunity enabled?
        private bool immunity = false;

        //Ensures not to resume the game when it was paused by user and tank explosion is shown
        private bool forcePause = false;

        //is game paused by using P key?
        private bool paused = false;

        //is game over?
        private bool endGame = false;

        //only one invader left, enable special mode
        private bool enableExtraSpeed = false;

        //Cheat
        private bool enemyShootingDisabled = false;

        //was barrier destroyed played
        private bool barrierDestroyedPlayed = false;

        //randomly generated speed of currently appearing mystery
        private int mysterySpeed;

        //When invaders' movement direction change, their movement accelerates by shorteningMs
        private int movementAccelerationMs = 50;

        //Y value of bottom green line, calculated based on window dimension
        private int greenLineY;

        //Probability of selecting invaders shooter intelligently (shooter that can hit tank)
        private double shootingPrecision = 0.3;

        //when moving down interval is slowed down and then restored
        private double invadersShouldMoveOriginalInterval;

        //y location of invader with highest y location
        private float bottomInvaderY = -1;

        //barrier y location
        private float barrierY;

        //radius of splash damage when barrier is hit
        private static float splashRadius;

        //used to adjust the immunityTimer interval if game was paused during tank immunity
        private Stopwatch immunityStopwatch = new Stopwatch();

        //Random is used for various purposes
        private Random random = new Random();
        public Game(Form1 f)
        {
            Form = f;

            //main drawing timer
            drawingTimer = new Timer(15) { Enabled = true };
            drawingTimer.Elapsed += drawingTimer_Elapsed;

            //this should solve problem with moving the tank
            Form.KeyPreview = true;

            //setting up events
            Form.Paint += Form_Paint;
            Form.Resize += Form_Resize;
            Form.KeyDown += FormOnKeyDown;
            Form.KeyUp += FormOnKeyUp;
            Form.Deactivate += FormOnDeactivate;

            //setting various variables
            greenLineY = Form.Height - Constants.GreenLineYShift;
            barrierY = 0.65f * Form.Height;

            //creating barriers
            GenerateBarrier();

            CreateBottomLine();

            tankLocation = new PointF(Form.Width / 2f - Resources.tank.Width / 2f, Form.Height - Constants.TankBottomYShift);

            //setting up radius of splash damage (used when barrier is hit)
            splashRadius = 0.01058f * (Form.Width + Form.Height);

            //setting up mystery
            mysteryShouldAppearTimer = new Timer(random.Next(Constants.MysteryShouldAppearRange.Item1, Constants.MysteryShouldAppearRange.Item2)) { Enabled = true };
            mysteryShouldAppearTimer.Elapsed += MysteryShouldAppearTimerOnElapsed;


            //setting up timers
            tankCanShootTimer.Elapsed += TankCanShootTimerOnElapsed;

            immunityTimer.Elapsed += ImmunityTimerOnElapsed;

            explosionsTimer.Elapsed += ExplosionsTimerOnElapsed;
            mysteryExplosionTimer.Elapsed += MysteryExplosionTimer_Elapsed;

            invadersShouldMoveTimer = new Timer(Constants.SlowestMovingFrequencyMs) { Enabled = false };
            invadersShouldMoveTimer.Elapsed += InvadersShouldMoveTimerElapsed;

            tankExplosionTimer.Elapsed += TankExplosionTimerElapsed;
            invadersShouldShootTimer.Elapsed += InvadersShouldShootTimerOnElapsed;

            //begin lvl 1
            NextLevel();


        }

        /// <summary>
        /// Pause game when game loses its focus.
        /// </summary>
        private void FormOnDeactivate(object sender, EventArgs e)
        {
            if (!endGame)
            {
                paused = true;
                PauseGameWMsg();
            }
        }

        /// <summary>
        /// Filling up collection with new barriers.
        /// </summary>
        private void GenerateBarrier()
        {
            barrierDestroyedPlayed = false;
            int w = (int)(0.2 * Form.Width);
            int h = (int)(0.1 * Form.Height);
            float stopMovingTopX = (1 / 6f) * w;
            float barrierY = 0.7f * Form.Height;
            for (int i = 0; i < 3; i++)
            {
                Barrier b = new Barrier();
                if (i == 0)
                    CreateBarrier(new PointF(Form.Width * 0.11f, barrierY), stopMovingTopX, h, 1, ref b);

                if (i == 1)
                    CreateBarrier(new PointF(Form.Width * 0.4f, barrierY), stopMovingTopX, h, 1, ref b);

                if (i == 2)
                    CreateBarrier(new PointF(Form.Width * 0.7f, barrierY), stopMovingTopX, h, 1, ref b);

                barriers.Add(b);
            }
        }

        /// <summary>
        /// Update associated label for Properties (Score, Level)
        /// </summary>
        /// <param name="value">Value that should be put into label</param>
        /// <param name="l">Which label should be updated.</param>
        private void PutTextIntoControl(int value, Label l)
        {
            if (l.InvokeRequired)
                l.Invoke(new Action(() => l.Text = value.ToString()));
            else
                l.Text = value.ToString();
        }

        /// <summary>
        /// Ticks when invaders should make a move.
        /// </summary>
        private void InvadersShouldMoveTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (invaders.Count == 0) return;

            drawingTimer.Stop();      
            drawingTimer.Enabled = false;
            if (invadersMovingRight)
            {
                Invader right = RightInvader();
                if (right.Location.X + 2 * right.Width + Constants.InvaderMovingLength < Form.Width)
                {
                    invaders.ForEach(x => x.MoveRight());

                }
                if (right.Location.X + 2 * right.Width + Constants.InvaderMovingLength > Form.Width)
                {
                    if (!lckMovement)
                    {
                        PrepareForMovingDown();
                        return;
                    }

                    invadersShouldMoveTimer.Interval = invadersShouldMoveOriginalInterval;
                    MovingDown();
                    lckMovement = false;
                }
            }
            else
            {
                Invader left = LeftInvader();
                if (left.Location.X - Constants.InvaderMovingLength > 0)
                    invaders.ForEach(x => x.MoveLeft());

                if (left.Location.X - Constants.InvaderMovingLength < 0)
                {
                    if (!lckMovement)
                    {
                        PrepareForMovingDown();
                        return;
                    }

                    invadersShouldMoveTimer.Interval = invadersShouldMoveOriginalInterval;
                    MovingDown();
                    lckMovement = false;
                }
            }
            drawingTimer.Enabled = true;
        }

        /// <summary>
        /// Setting various things before invaders can move down.
        /// </summary>
        private void PrepareForMovingDown()
        {
            invadersShouldMoveOriginalInterval = invadersShouldMoveTimer.Interval;
            invadersShouldMoveTimer.Interval = 500;
            lckMovement = true;
            drawingTimer.Enabled = true;
        }

        /// <summary>
        /// Move down invader according to lvl. Accelerates invaders and their shooting.
        /// </summary>
        private void MovingDown()
        {
            invadersMovingRight = !invadersMovingRight;
            invaders.ForEach(x => x.MoveDown(Level));
            if (!enableExtraSpeed)
            {
                double newInterval = invadersShouldMoveTimer.Interval - movementAccelerationMs * (0.7 + Level * 0.3);
                invadersShouldMoveTimer.Interval = Math.Max(newInterval, 100);
                movementAccelerationMs = (int)(movementAccelerationMs * 1.15f);
                if (invadersShouldShootTimer.Interval > 420)
                    invadersShouldShootTimer.Interval -= 30;
            }
            UpdateBottomInvader();
        }

        /// <summary>
        /// Used to check whether direction of invaders should be changed.
        /// </summary>
        /// <returns>Invader with highest x location.</returns>
        private Invader RightInvader()
        {
            Invader f = invaders[0];
            for (int i = 1; i < invaders.Count; i++)
            {
                if (invaders[i].Location.X > f.Location.X)
                {
                    f = invaders[i];
                }
            }

            return f;
        }


        /// <summary>
        /// Used to check whether direction of invaders should be changed.
        /// </summary>
        /// <returns>Invader with lowest x location.</returns>
        private Invader LeftInvader()
        {
            Invader f = invaders[0];
            for (int i = 1; i < invaders.Count; i++)
            {
                if (invaders[i].Location.X < f.Location.X)
                    f = invaders[i];

            }

            return f;
        }

        /// <summary>
        /// Releasing keys.
        /// </summary>
        private void FormOnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                keyLArrow = false;
            }

            if (e.KeyCode == Keys.Right)
            {
                keyRArrow = false;
            }

            if (e.KeyCode == Keys.Space)
            {
                keySpace = false;
            }
            e.Handled = true;
            e.SuppressKeyPress = true;
        }

        /// <summary>
        /// Already reloaded, tank can shoot again.
        /// </summary>
        private void TankCanShootTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            canTankShoot = true;
            tankCanShootTimer.Enabled = false;
        }

        /// <summary>
        /// Ticks when new mystery should appear.
        /// </summary>
        private void MysteryShouldAppearTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            Utils.Play(Resources.ufo_lowpitch);
            mysteryLocation = new PointF(Form.Width - mysterySpeed, Constants.MysteryYLocation);
            Timer s = (Timer)sender;
            mysterySpeed = random.Next(Constants.MysterySpeedRange.Item1, Constants.MysterySpeedRange.Item2);
            s.Interval = random.Next(Constants.MysteryShouldAppearRange.Item1, Constants.MysteryShouldAppearRange.Item2);
        }

        /// <summary>
        /// Keys are pressed.
        /// </summary>
        private void FormOnKeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Left)
                keyLArrow = true;



            if (e.KeyCode == Keys.Right)
                keyRArrow = true;


            if (e.KeyCode == Keys.Space || e.KeyCode == Keys.X)
                keySpace = true;

            e.Handled = true;
            e.SuppressKeyPress = true;
        }

        /// <summary>
        /// In fact an event that pauses a game when its being minimalized.
        /// </summary>
        private void Form_Resize(object sender, EventArgs e)
        {
            if (Form.WindowState == FormWindowState.Minimized)
                PauseGameWMsg();

        }

        /// <summary>
        /// Creating bottom green line.
        /// </summary>
        private void CreateBottomLine()
        {
            bottomLine.Clear();
            for (int i = 0; i < Form.Width; i += Constants.GreenBottomLineRectangleSize.Item1)
            {
                RectangleF rect = new RectangleF(i, greenLineY, Constants.GreenBottomLineRectangleSize.Item1, Constants.GreenBottomLineRectangleSize.Item2);
                bottomLine.Add(rect);
            }
        }

        private void Form_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.HighSpeed;

            //drawing bottom green line
            e.Graphics.FillRectangles(Brushes.LawnGreen, bottomLine.ToArray());

            //drawing lifes left
            e.Graphics.DrawString(Life.ToString(), new Font(Utils.SpaceInvadersFamily, 28), Brushes.White, 10, Form.Height - 85);

            for (int i = 80; i <= 50 * Life; i += 50)
                e.Graphics.DrawImage(Resources.tank, new PointF(i, Form.Height - 80));

            //drawing tank
            if (shouldDrawTank)
            {
                if (immunity)
                {
                    e.Graphics.DrawImage(Resources.immune_tank, tankLocation);
                }
                else
                    e.Graphics.DrawImage(Resources.tank, tankLocation);
            }
            else
            {
                e.Graphics.DrawImage(Resources.tank_destroyed, tankLocation);
            }

            //drawing mystery
            if (mysteryLocation.HasValue)
                e.Graphics.DrawImage(Resources.mystery, mysteryLocation.Value);

            //drawing tank shoots
            for (int i = 0; i < tankMissiles.Count; i++)
            {
                e.Graphics.DrawLine(new Pen(Color.LawnGreen, 3f), tankMissiles[i].X, tankMissiles[i].Y - Constants.TankMissileLength, tankMissiles[i].X, tankMissiles[i].Y);
            }

            //drawing invaders missiles
            for (int i = 0; i < invadersMissilesSafe.Count; i++)
                e.Graphics.DrawImage(Resources.lightning, invadersMissilesSafe[i]);
            


            //drawing mystery explosion
            if (mysteryExplosion != null)
                e.Graphics.DrawImage(Resources.mystery_boom, mysteryExplosion.Value);
            

            //drawing normal explosion
            for (int i = 0; i < explosionsSafe.Count; i++)
                e.Graphics.DrawImage(Resources.boom, explosionsSafe[i]); 



            //draw all invaders
            foreach (var invader in invaders)
            {
                e.Graphics.DrawInvader(invader);
            }

            //drawing barriers
            foreach (Barrier b in barriers)
                e.Graphics.FillRectangles(Brushes.LawnGreen, b.Columns.SelectMany(x => x.Rectangles).ToArray());
        }

        /// <summary>
        /// Actual logic of generating barrier.
        /// </summary>
        /// <param name="origin">PointF to start generating barrier from.</param>
        /// <param name="w">How many pixels should I draw to right (x location).</param>
        /// <param name="h">Height of current column that is drawn.</param>
        /// <param name="partN">Which part of barrier is being drawn.</param>
        /// <param name="b">Reference to barrier created.</param>
        private void CreateBarrier(PointF origin, float w, float h, int partN, ref Barrier b)
        {
            PointF startPt = origin;
            do
            {
                Column c = new Column();
                PointF movingY = startPt;
                do
                {
                    c.Rectangles.Add(new RectangleF(movingY.X, movingY.Y, Constants.BarrierSquareSize, Constants.BarrierSquareSize));

                    if (movingY.Y < b.TopY)
                        b.TopY = movingY.Y;

                    movingY = PointF.Add(movingY, new Size(0, Constants.BarrierSquareSize));

                } while (movingY.Y < origin.Y + h);

                if (partN == 2)
                    h -= Constants.BarrierSquareSize;
                else if (partN == 4)
                    h += Constants.BarrierSquareSize;

                if (partN == 1)
                    startPt = PointF.Add(startPt, new Size(Constants.BarrierSquareSize, -Constants.BarrierSquareSize));

                else if (partN == 5)
                    startPt = PointF.Add(startPt, new Size(Constants.BarrierSquareSize, Constants.BarrierSquareSize));

                else if (partN == 2 || partN == 4 || partN == 3)
                    startPt = PointF.Add(startPt, new Size(Constants.BarrierSquareSize, 0));


                b.Columns.Add(c);

            } while (startPt.X < origin.X + w);


            if (partN < 5)
            {
                partN++;
                if (partN == 2)
                    CreateBarrier(startPt, w, h, partN, ref b);
                if (partN == 3)
                    CreateBarrier(startPt, 2f * w, h, partN, ref b);
                if (partN == 4)
                    CreateBarrier(startPt, 0.6f * w, h, partN, ref b);
                if (partN == 5)
                    CreateBarrier(startPt, w, b.Columns.First().Rectangles.Last().Y - origin.Y, partN, ref b);
            }
        }

        /// <summary>
        /// Main game loop.
        /// </summary>
        private void drawingTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (lockingToken)
                return;
            lockingToken = true;

            //Invalidating screen
            if (Form.InvokeRequired)
                Form.Invoke(new Action(() =>
                {
                    Form.Refresh();
                }));
            //moving mystery
            if (mysteryLocation.HasValue)
            {
                //collision with mystery
                RectangleF m = new RectangleF(mysteryLocation.Value.X, mysteryLocation.Value.Y, Resources.mystery.Width, Resources.mystery.Height);
                foreach (var shoot in tankMissiles)
                {
                    PointF topOfMissle = Utils.TopOfTankMissile(shoot);
                    if (Utils.IsPointInRect(topOfMissle, m))
                    {
                        Utils.Play(Resources.ufo_highpitch);
                        tankMissiles.Remove(shoot);
                        mysteryExplosion = mysteryLocation;
                        mysteryLocation = null;
                        ShowMysteryExplosion();
                        break;
                    }
                }

                //deleting unvisible mystery
                if (mysteryLocation != null)
                    if (mysteryLocation.Value.X + Resources.mystery.Width < 0)
                    {
                        mysteryLocation = null;
                    }

                //moving mystery at his speed
                if (mysteryLocation != null)
                    mysteryLocation = new PointF(mysteryLocation.Value.X - mysterySpeed, mysteryLocation.Value.Y);
            }

            //moving tank
            if (keyLArrow && tankLocation.X > 0 && shouldDrawTank)
                tankLocation.X -= Constants.TankMovingDistancePx;


            if (keyRArrow && tankLocation.X + Resources.tank.Width + 12 < Form.Width && shouldDrawTank)
                tankLocation.X += Constants.TankMovingDistancePx;

            //tank is shooting
            if (canTankShoot && keySpace)
            {
                canTankShoot = false;
                tankCanShootTimer.Enabled = true;
                tankCanShootTimer.Interval = Constants.TankShotTimeoutMs;
                tankMissiles.Add(new PointF(tankLocation.X + Resources.tank.Width / 2f, tankLocation.Y - 2));
                Utils.Play(Resources.shoot);
            }
            invadersMissilesSafe = invadersMissiles.ToList();
            explosionsSafe = explosions.ToList();

            //tank missile collision with invaders
            for (int i = invaders.Count - 1; i >= 0; i--)
            {
                Invader curr = invaders[i];
                RectangleF currInvader = new RectangleF(invaders[i].Location.X, invaders[i].Location.Y, invaders[i].Width, invaders[i].Height);
                for (int j = tankMissiles.Count - 1; j >= 0; j--)
                {
                    PointF tS = tankMissiles[j];
                    if (Utils.IsPointInRect(Utils.TopOfTankMissile(tankMissiles[j]), currInvader))
                    {
                        //collision
                        Score += curr.ScoreGain;
                        ShowNormalExplosion(new PointF(currInvader.X, currInvader.Y));
                        tankMissiles.RemoveAll(x => x == tS);

                        invaders.Remove(curr);
                        --i;
                        Utils.Play(Resources.invaderkilled);
                        if (i < 0) break;
                        SmallShootingSpeedIncrease();

                        if (Math.Abs(currInvader.Y - bottomInvaderY) < 0.1)
                            UpdateBottomInvader();
                    }
                }
            }

            //collision of invaders shoot
            for (int i = invadersMissiles.Count - 1; i >= 0; --i)
            {
                PointF bottom = Utils.BottomOfInvaderMissile(invadersMissiles[i]);

                //collision with bottom green line
                if (Math.Abs(bottom.Y - greenLineY) < 5)
                {
                    for (int j = bottomLine.Count - 1; j >= 0; j--)
                    {
                        if (bottom.X > bottomLine[j].X && bottom.X < bottomLine[j].Right)
                            bottomLine.RemoveAt(j);

                    }
                }
                //tank collision with invaders shoot 
                if (!immunity)
                { //if tank was not reswapned moment ago
                    RectangleF invaderMissile = new RectangleF(invadersMissiles[i].X, invadersMissiles[i].Y, Resources.lightning.Width, Resources.lightning.Height);
                    RectangleF tank = new RectangleF(tankLocation.X, tankLocation.Y, Resources.tank.Width, Resources.tank.Height);
                    if (invaderMissile.IntersectsWith(tank))
                    {
                        if (Life - 1 > 0)
                        {
                            immunity = true;
                            immunityStopwatch.Restart();
                        }

                        if (Life - 1 == 0) //gameover
                        {
                            Life--;
                            End();
                        }
                        else //life down
                        {
                            Life--;
                            ShowTankExplosion();
                        }

                        invadersMissiles.RemoveAt(i); //remove invader shoot if collision occured
                        i--;
                        if (i < 0)
                            break;
                    }
                }

                //collision of tank and invader missiles
                for (int j = tankMissiles.Count - 1; j >= 0 && i >= 0; j--)
                {
                    PointF top = Utils.TopOfTankMissile(tankMissiles[j]);
                    if (top.X >= bottom.X && top.X <= bottom.X + Resources.lightning.Width)
                    {
                        if (top.Y - bottom.Y < 11)
                        {
                            tankMissiles.RemoveAt(j); //remove both missiles
                            invadersMissiles.RemoveAt(i);
                            i--;
                            if (i < 0)
                                break;

                        }
                    }
                }

                //collision of barrier and invader missiles
                for (int j = 0; j < barriers.Count && i >= 0; j++)
                {
                                                                             //column idx of impact
                    if (CollisionWithBarrier(invadersMissiles[i], barriers[j], out var cIdx, true))
                    {
                        invadersMissiles.RemoveAt(i);
                        PointF topOfC = barriers[j].Columns[cIdx].Rectangles.First().Location;
                        SplashDamage(topOfC, barriers[j], cIdx, true);
                        i--;
                        if (i < 0)
                            break;

                    }
                }
            }


            //collision of tank missiles and barrier
            for (int j = 0; j < barriers.Count; ++j)
            {
                for (int i = tankMissiles.Count - 1; i >= 0; i--)
                {
                    if (CollisionWithBarrier(tankMissiles[i], barriers[j], out var columnIdx, false))
                    {
                        tankMissiles.RemoveAt(i);
                        PointF bottomOfC = barriers[j].Columns[columnIdx].Rectangles.Last().Location;
                        SplashDamage(bottomOfC, barriers[j], columnIdx, false);

                    }
                }
            }



            //remove unvisible shoots
            for (int i = tankMissiles.Count - 1; i >= 0; i--)
            {
                if (tankMissiles[i].Y < 0)
                    tankMissiles.RemoveAt(i);

            }

            for (int i = invadersMissiles.Count - 1; i >= 0; i--)
            {
                if (invadersMissiles[i].Y > Form.Height)
                    invadersMissiles.RemoveAt(i);

            }

            //only one invader left, special mode
            if (invaders.Count == 1 && !enableExtraSpeed)
            {
                enableExtraSpeed = true;
                invadersShouldShootTimer.Interval = 350;
                invadersShouldMoveTimer.Interval = 100;
            }

            //level cleared
            if (invaders.Count == 0)
            {
                NextLevel();
            }
            
            //moving tank shoots
            for (int i = 0; i < tankMissiles.Count; i++)
                tankMissiles[i] = new PointF(tankMissiles[i].X, tankMissiles[i].Y - Constants.MissilesMovingDistance);

            //moving invaders shoots
            for (int i = 0; i < invadersMissiles.Count; i++)
                invadersMissiles[i] = new PointF(invadersMissiles[i].X, invadersMissiles[i].Y + Constants.MissilesMovingDistance);

            //barriers were destoryed earlier by invaders missiles
            if (!barriers.Any() && !barrierDestroyedPlayed)
            {
                Utils.Play(Resources.barrier_destroyed);
                barrierDestroyedPlayed = true;
            }

            //Invaders are too close. Barriers need to be destroyed.
            if (bottomInvaderY + Constants.MaxInvaderHeight >= barrierY && barriers.Any())
            {
                barriers.Clear();
                Utils.Play(Resources.barrier_destroyed);
                barrierDestroyedPlayed = true;
            }

            //Invaders got to the tank location, gameover.
            if (bottomInvaderY + Constants.MaxInvaderHeight >= tankLocation.Y)
                End();

            lockingToken = false;
        }

        /// <summary>
        /// When invader is killed shooting speed is slightly increased.
        /// </summary>
        private void SmallShootingSpeedIncrease()
        {
            if (invadersShouldShootTimer.Interval - 10 < 420)
                invadersShouldShootTimer.Interval = 420;
            else invadersShouldShootTimer.Interval -= 5;
        }

        /// <summary>
        /// Stoping all threads when game has ended.
        /// </summary>
        private void DisableAllTimers()
        {
            foreach (FieldInfo fieldInfo in GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
            {
                if (fieldInfo.FieldType == typeof(Timer))
                {
                    Timer t = (Timer)fieldInfo.GetValue(this);
                    t.Stop();
                    t.Enabled = false;
                }
            }
        }

        /// <summary>
        /// Gameover.
        /// </summary>
        public void End()
        {
            endGame = true;
            DisableAllTimers();
            Form.Invoke(new Action(() => Form.Refresh()));
            Form.Invoke(new Action(() => Form.ShowGameOver()));
            Dispose();
        }

        /// <summary>
        /// Checking whether collision with barrier happened.
        /// </summary>
        /// <param name="missile">Which missile to check</param>
        /// <param name="barrier">Which barrier to check.</param>
        /// <param name="columnIdx">Column idx of barrier where collision happenend.</param>
        /// <param name="fromInvader">whether the missile comes from the invader or tank.</param>
        /// <returns></returns>
        private bool CollisionWithBarrier(PointF missile, Barrier barrier, out int columnIdx, bool fromInvader)
        {
            columnIdx = -1;
            PointF btm;
            btm = fromInvader ? Utils.BottomOfInvaderMissile(missile) : Utils.TopOfTankMissile(missile);
            PointF f;
            PointF l;
            if (fromInvader)
            {
                f = barrier.Columns.First().Rectangles.First().Location;
                l = barrier.Columns.Last().Rectangles.First().Location;
            }
            else
            {
                f = barrier.Columns.First().Rectangles.Last().Location;
                l = barrier.Columns.Last().Rectangles.Last().Location;
            }

            if (btm.X >= f.X - Constants.MissilesMovingDistance && btm.X <= l.X + Constants.MissilesMovingDistance)
            {
                for (int i = 0; i < barrier.Columns.Count; i++)
                {
                    float cmp;
                    cmp = fromInvader ? barrier.Columns[i].Rectangles.First().X : barrier.Columns[i].Rectangles.Last().X;
                    if (barrier.Columns[i].Rectangles.Any() &&
                        Math.Abs(btm.X - cmp) < Constants.MissilesMovingDistance)
                    {
                        PointF topOfC;
                        topOfC = fromInvader ? barrier.Columns[i].Rectangles.First().Location : barrier.Columns[i].Rectangles.Last().Location;

                        if (Math.Abs(topOfC.Y - btm.Y) < Constants.MissilesMovingDistance)
                        {
                            columnIdx = i;
                            return true;
                        }
                    }

                }
            }

            return false;

        }
        /// <summary>
        /// Splash damaging barrier.
        /// </summary>
        /// <param name="impactPoint">Reference point</param>
        /// <param name="barrier">barrier to damage</param>
        /// <param name="columnIdx">idx of column</param>
        /// <param name="fromInvader">whether invader missile collided with barrier or it was tank missile</param>
        private void SplashDamage(PointF impactPoint, Barrier barrier, int columnIdx, bool fromInvader)
        {
            double m;
            int j = columnIdx;
            bool second = false;
            do
            {
                if (second)
                    j = columnIdx - 1;

                PointF cmp;
                if (fromInvader)
                    cmp = barrier.Columns[j].Rectangles.First().Location;
                else
                    cmp = barrier.Columns[j].Rectangles.Last().Location;

                while (j >= 0 && j < barrier.Columns.Count && barrier.Columns[j].Rectangles.Any() && Utils.Distance(cmp,
                           impactPoint) < splashRadius)
                {
                    m = j == columnIdx ? 1.0 : 0.7;
                    int k; //square idx in column
                    if (fromInvader)
                        k = 0;
                    else k = barrier.Columns[j].Rectangles.Count - 1;
                    while (j >= 0 && k >= 0 && k < barrier.Columns[j].Rectangles.Count && Utils.Distance(
                               barrier.Columns[j].Rectangles[k].Location, impactPoint) <
                           splashRadius)
                    {
                        if (random.NextDouble() <= m)
                        {
                            barrier.Columns[j].Rectangles.RemoveAt(k);
                            if (barrier.Columns[j].Rectangles.Count == 0)
                            {
                                barrier.Columns.RemoveAt(j);
                                if (barrier.Columns.Count == 0)
                                {
                                    barriers.Remove(barrier);
                                }
                                if (columnIdx > j)
                                    columnIdx--;
                                j--;
                            }
                            if (fromInvader)
                                k--;
                        }

                        if (fromInvader)
                            k++;
                        else k--;
                    }

                    if (!second)
                        j++; //which column
                    else j--;
                }

                second = !second && (columnIdx != 0) && (columnIdx != barrier.Columns.Count - 1);
            } while (second);


        }

        /// <summary>
        /// Ticks after showing tank explosion, respawns the tank and ensures than the tank is immune for couple of seconds.
        /// </summary>
        private void TankExplosionTimerElapsed(object sender, ElapsedEventArgs e)
        {
            tankExplosionTimer.Enabled = false;
            tankLocation.X = random.Next((int)(Form.Width * 0.1), (int)(Form.Width * 0.9));
            shouldDrawTank = true;
            tankCanShootTimer.Enabled = true;
            canTankShoot = true;
            ResumeGame();
            immunityTimer.Interval = Constants.ImmunityLengthMs;
            immunityTimer.Enabled = true;

        }

        /// <summary>
        /// Ticks when immunity of tank should turn off.
        /// </summary>
        private void ImmunityTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            immunityTimer.Enabled = false;
            immunity = false;
        }

        /// <summary>
        /// Tank hit. Showing explosion.
        /// </summary>
        private void ShowTankExplosion()
        {
            PauseWOMsg();
            tankExplosionTimer.Enabled = true;
            shouldDrawTank = false;
            canTankShoot = false;
            tankCanShootTimer.Enabled = false;

            Form.Invoke(new Action(() => Form.Refresh()));
            Utils.Play(Resources.tank_explosion);

        }


        /// <summary>
        /// Generating next lvl.
        /// </summary>
        private void NextLevel()
        {
            invadersMissiles.Clear();
            invadersMovingRight = true;
            enableExtraSpeed = false;
            drawingTimer.Enabled = false;
            invadersShouldMoveTimer.Enabled = false;
            Level++;
            invadersShouldMoveTimer.Interval = Constants.SlowestMovingFrequencyMs;
            movementAccelerationMs = 30 + Level * 20;
            GenerateInvadersForLevel();
            invadersShouldShootTimer.Enabled = true;
            invadersShouldShootTimer.Interval = Constants.InvadersSlowestShootingSpeedMs;
            drawingTimer.Enabled = true;
            invadersShouldMoveTimer.Enabled = true;
            UpdateBottomInvader();
        }


        /// <summary>
        /// Ticks when invaders should shoot
        /// </summary>
        private void InvadersShouldShootTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            int numberOfShooters = Math.Min(random.Next(1, 3), invaders.Count);
            List<Invader> nClosest = NClosestInvaders(numberOfShooters);
            List<Invader> shooters = new List<Invader>();
            while (shooters.Count < numberOfShooters)
            {
                if (random.NextDouble() < shootingPrecision) //aim for tank
                {
                    shooters.Add(nClosest.First());
                    nClosest.RemoveAt(0);
                }
                else //random
                {
                    int idx = random.Next(0, invaders.Count - 1);
                    shooters.Add(invaders[idx]);
                }
            }
            shooters.ForEach(x => MakeInvaderShootMissile(x));
        }

        /// <summary>
        /// Function taking invaders that can hit tank.
        /// </summary>
        /// <returns>n invaders with closest x location to tank.</returns>
        private List<Invader> NClosestInvaders(int n)
        {
            var cpy = invaders.ToArray().ToList();
            cpy.Sort(InvaderComparator);
            return cpy.Take(n).ToList();
        }

        /// <summary>
        /// Comparison functor.
        /// </summary>
        /// <returns>
        /// -1 if invaders are in the sorted order
        /// 1 if invaders are not in the sorted order
        /// 0 if invaders are of the same instance
        /// </returns>
        private int InvaderComparator(Invader i1, Invader i2)
        {
            float diff1 = Math.Abs(tankLocation.X - i1.Location.X);
            float diff2 = Math.Abs(tankLocation.X - i2.Location.X);

            if (diff1 < diff2)
                return -1;
            if (diff1 > diff2)
                return 1;

            return 0;
        }

        /// <summary>
        /// Make particular invader shoot a missile.
        /// </summary>
        /// <param name="invader">Invader that is going to shoot.</param>
        private void MakeInvaderShootMissile(Invader invader)
        {
            PointF toAdd = PointF.Add(invader.Location, new Size(10, invader.Height));
            invadersMissiles.Add(toAdd);
        }

        /// <summary>
        /// Generating invaders for level.
        /// </summary>
        private void GenerateInvadersForLevel()
        {
            int b = Math.Min((int)(Form.Height * Constants.InvadersPositionCoeff * Level), 380);

            for (float x = 40; x < 0.6f * Form.Width; x += Constants.InvadersDistanceCoeff * Resources.bottom1.Width)
            {
                invaders.Add(new TopInvader(new PointF(x - Resources.top1.Width / 2f, b)));
                invaders.Add(new MiddleInvader(new PointF(x - Resources.middle1.Width / 2f, b + Constants.InvadersDistanceCoeff * Resources.bottom1.Height)));
                invaders.Add(new MiddleInvader(new PointF(x - Resources.middle1.Width / 2f, b + 2 * Constants.InvadersDistanceCoeff * Resources.bottom1.Height)));
                invaders.Add(new BottomInvader(new PointF(x - Resources.bottom1.Width / 2f, b + 3 * Constants.InvadersDistanceCoeff * Resources.bottom1.Height)));
            }
            UpdateBottomInvader();
        }

        /// <summary>
        /// Invader with highest y location killed. Finding the new one.
        /// </summary>
        private void UpdateBottomInvader()
        {
            foreach (Invader invader in invaders)
            {
                if (invader.Location.Y > bottomInvaderY)
                    bottomInvaderY = invader.Location.Y;

            }
        }

        /// <summary>
        /// Normal explosion effect.
        /// </summary>
        /// <param name="invaderLocation"></param>
        private void ShowNormalExplosion(PointF invaderLocation)
        {
            explosions.Add(invaderLocation);
            explosionsTimer.Enabled = true;
            explosionsTimer.Interval = Constants.ExplosionInterval;

        }

        /// <summary>
        /// Explosion should dissapear.
        /// </summary>
        private void ExplosionsTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            drawingTimer.Stop();
            drawingTimer.Enabled = false;
            explosions.RemoveAt(0);
            explosionsTimer.Enabled = explosions.Any();
            drawingTimer.Enabled = mysteryShouldAppearTimer.Enabled;
        }

        /// <summary>
        /// Mystery explosion effect.
        /// </summary>
        private void ShowMysteryExplosion()
        {
            mysteryExplosionTimer.Enabled = true;
            mysteryExplosionTimer.Interval = Constants.ExplosionInterval;
        }

        /// <summary>
        /// Mystery explosion effect ending.
        /// </summary>
        private void MysteryExplosionTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            mysteryExplosion = null;
            mysteryExplosionTimer.Enabled = false;
            Score += (mysterySpeed - 2) * random.Next(20, 30);
        }

        /// <summary>
        /// Cleaning up.
        /// </summary>
        public void Dispose()
        {
            drawingTimer?.Dispose();
            mysteryShouldAppearTimer?.Dispose();
            invadersShouldMoveTimer?.Dispose();
            mysteryExplosionTimer?.Dispose();
            tankCanShootTimer?.Dispose();
            explosionsTimer?.Dispose();
            invadersShouldShootTimer.Dispose();
            Form.Paint -= Form_Paint;
            Form.Resize -= Form_Resize;
            Form.KeyDown -= FormOnKeyDown;
            Form.KeyUp -= FormOnKeyUp;
        }

        /// <summary>
        /// Cheating for making the game easier.
        /// </summary>
        /// <param name="keyData">Key pressed.</param>
        public void CheatKey(Keys keyData)
        {
            if (keyData == Keys.H) //add life
            {
                Life++;
            }
            else if (keyData == Keys.S) //add score
            {
                Score += 100;
            }
            else if (keyData == Keys.P) //pause game
            {
                if (paused)
                {
                    ResumeGame();
                    paused = false;
                }
                else
                {
                    paused = true;
                    PauseGameWMsg();
                }
            }
            else if (keyData == Keys.M) //disable invaders shooting
            {
                enemyShootingDisabled = !enemyShootingDisabled;
                invadersShouldShootTimer.Enabled = !invadersShouldShootTimer.Enabled;
            }
            else if (keyData == Keys.B) //regenerate barriers
            {
                barriers.Clear();
                GenerateBarrier();
            }
            else if (keyData == Keys.D) //delete one invader
            {
                invaders.RemoveAt(random.Next(0, invaders.Count - 1));
                SmallShootingSpeedIncrease();
            }
        }

        /// <summary>
        /// Pause game without showing Pause MSG
        /// </summary>
        private void PauseWOMsg()
        {
            drawingTimer.Enabled = false;
            mysteryShouldAppearTimer.Enabled = false;
            invadersShouldMoveTimer.Enabled = false;
            immunityTimer.Enabled = false;
            invadersShouldShootTimer.Enabled = false;
            tankCanShootTimer.Enabled = false;
        }

        /// <summary>
        /// Pause game and show Pause MSG
        /// </summary>
        private void PauseGameWMsg()
        {

            PauseWOMsg();
            Form.pauseLabel.Visible = true;
            forcePause = tankExplosionTimer.Enabled;
            tankExplosionTimer.Enabled = false;
            immunityStopwatch.Stop();
        }


        /// <summary>
        /// Resume game.
        /// </summary>
        private void ResumeGame()
        {
            drawingTimer.Enabled = true;
            mysteryShouldAppearTimer.Enabled = true;
            invadersShouldMoveTimer.Enabled = true;
            tankCanShootTimer.Enabled = true;

            tankExplosionTimer.Enabled = !shouldDrawTank;

            if (immunity && paused)
            {
                if(immunityTimer.Interval - immunityStopwatch.ElapsedMilliseconds > 0)
                    immunityTimer.Interval -= immunityStopwatch.ElapsedMilliseconds; 

                immunityStopwatch.Restart();
            }

            immunityTimer.Enabled = immunity;


            invadersShouldShootTimer.Enabled = !enemyShootingDisabled;
            Form.pauseLabel.Invoke(new Action(() => Form.pauseLabel.Visible = false));
            forcePause = false;
        }
    }
}
