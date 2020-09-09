using System.Collections.Generic;
using System.Drawing;

namespace SpaceInvaders
{
    class Barrier
    {
        //Collection of columns of barrier
        public List<Column> Columns { get; set; } = new List<Column>();

        //Max Y location of drawing square, used when barriers needs to be destroyed because invaders are too close.
        public float TopY { get; set; } = float.MaxValue;
    }

    class Column
    {
        public List<RectangleF> Rectangles { get; set; } = new List<RectangleF>();
    }
}
