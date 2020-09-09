using System;
using System.Drawing;

namespace SpaceInvaders
{
    class Constants
    {
        //length of tank missile
        public static readonly int TankMissileLength = 20;

        //how many pixels invaders move when drawingTimer ticks
        public static readonly int InvaderMovingLength = 15;

        //slowest moving frequency of invaders
        public static readonly int SlowestMovingFrequencyMs =  750;

        //for how long should be explosion seen
        public static readonly int ExplosionInterval = 300;

        //size of square forming barriers
        public static readonly int BarrierSquareSize = 5;

        //max height of all invaders models
        public static readonly int MaxInvaderHeight = 23;

        //slowest speed that invaders can shoot
        public static readonly int InvadersSlowestShootingSpeedMs = 1000;

        //coefficient affecting Y position of invaders based on current level of game
        public static readonly double InvadersPositionCoeff = 0.144444;

        //coefficient to multiply form dimension with when form is resized, used for purposes of setting scoreTableUC location
        public static readonly PointF ScoreTableLocation = new PointF(0.5f, 0.56f);

        //coefficient to move scoreTableUC when form is resized
        public static readonly int ScoreTableLocationShift = 220;

        //time tank needs to reload
        public static readonly int TankShotTimeoutMs = 1000;

        //how many pixels tank move on each tick
        public static readonly int TankMovingDistancePx = 5;

        //length of immunity that tank gets
        public static readonly int ImmunityLengthMs = 3000;

        //mystery's Y location is fixed
        public static readonly int MysteryYLocation = 80;
        
        //tank Y shift from bottom of game board
        public static readonly int TankBottomYShift = 127;

        //green line Y shift from bottom of game board
        public static readonly int GreenLineYShift = 100;

        //how many pixels missiles move on each tick
        public static readonly int MissilesMovingDistance = 10;

        //height distance coefficient of invaders
        public static readonly float InvadersDistanceCoeff = 1.3f;

        //range of possible delays for mystery to appear
        public static Tuple<int, int> MysteryShouldAppearRange = new Tuple<int, int>(10000, 20000);

        //range of possible mystery speeds
        public static Tuple<int,int> MysterySpeedRange = new Tuple<int, int>(3, 10);

        //dimensions of rectangles forming the bottom green rectangle
        public static Tuple<int,int> GreenBottomLineRectangleSize = new Tuple<int, int>(10,6);

    }
}
