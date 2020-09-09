using System;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Media;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SpaceInvaders.Properties;

namespace SpaceInvaders
{
    class Utils
    {
        public static FontFamily SpaceInvadersFamily;

        /// <summary>
        /// Source: https://stackoverflow.com/questions/1297264/using-custom-fonts-on-a-label-on-winforms
        /// </summary>
        public static void InitSpaceInvadersFont()
        {
            //Create your private font collection object.
            PrivateFontCollection pfc = new PrivateFontCollection();

            //Select your font from the resources.
            //My font here is "Digireu.ttf"
            int fontLength = Properties.Resources.space_invaders.Length;

            // create a buffer to read in to
            byte[] fontdata = Properties.Resources.space_invaders;

            // create an unsafe memory block for the font data
            System.IntPtr data = Marshal.AllocCoTaskMem(fontLength);

            // copy the bytes to the unsafe memory block
            Marshal.Copy(fontdata, 0, data, fontLength);

            // pass the font to the font collection
            pfc.AddMemoryFont(data, fontLength);

            SpaceInvadersFamily = pfc.Families[0];
        }
        public static void ChangeFontToSpaceInvaders(Control c)
        {
            if (SpaceInvadersFamily == null)
                InitSpaceInvadersFont();
            
            c.Font = new Font(SpaceInvadersFamily, c.Font.Size);
        }

        public static bool IsPointInRect(PointF p, RectangleF r)
        {
            if (p.X >= r.X && p.X <= r.Right)
            {
                if (p.Y >= r.Y && p.Y <= r.Bottom)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Used for tank missiles.
        /// </summary>
        /// <param name="missile">Bottom position of missile.</param>
        /// <returns>Top location of tank missile.</returns>
        public static PointF TopOfTankMissile(PointF missile)
        {
            return new PointF(missile.X, missile.Y - Constants.TankMissileLength);
        }

        /// <summary>
        /// Used for invader missiles.
        /// </summary>
        /// <param name="missile">Top position of missile.</param>
        /// <returns>Bottom location of missile.</returns>
        public static PointF BottomOfInvaderMissile(PointF missile)
        {
            return new PointF(missile.X, missile.Y + Resources.lightning.Height);
        }

        /// <summary>
        /// Plays wav sound stream.
        /// </summary>
        /// <param name="shoot">Stream of music to play.</param>
        public static void Play(UnmanagedMemoryStream shoot)
        {
            SoundPlayer sp = new SoundPlayer(shoot);
            sp.Play();
        }

        /// <summary>
        /// Pythagoras theorem.
        /// </summary>
        /// <returns>Distance of pt1 and pt2.</returns>
        public static float Distance(PointF pt1, PointF pt2)
        {
            return (float) Math.Sqrt(Math.Pow(pt1.X - pt2.X, 2) + Math.Pow(pt1.Y - pt2.Y, 2));
        }
    }
}
