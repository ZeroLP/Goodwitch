using System.Drawing;

namespace SpaceInvaders
{
    public static class ExtensionMethods
    {
        public static void DrawInvader(this Graphics g, Invader invader)
        {
            g.DrawImage(invader.ActiveLook(), invader.Location);
        }
    }
}
