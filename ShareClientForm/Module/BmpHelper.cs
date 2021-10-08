using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SharedClientForm
{
    public class BmpHelper
    {
        public static bool TryGetWindowBmp(IntPtr hWnd, out Bitmap windowBmp)
        {
            if (!NativeMethod.GetWindowRect(hWnd, out NativeMethod.RECT rect))
            {
                windowBmp = null;
                return false;
            }

            int width = rect.right - rect.left;
            int height = rect.bottom - rect.top;
            if (width <= 0 || height <= 0)
            {
                windowBmp = null;
                return false;
            }

            windowBmp = new(width, height);
            using var g = Graphics.FromImage(windowBmp);
            var dc = g.GetHdc();
            NativeMethod.PrintWindow(hWnd, dc, 0);
            g.ReleaseHdc(dc);

            return true;
        }

        public static bool TryGetWindow(IntPtr hWnd, out Bitmap windowBmp)
        {
            if (!NativeMethod.GetWindowRect(hWnd, out var rect))
            {
                windowBmp = null;
                return false;
            }

            int width = rect.right - rect.left;
            int height = rect.bottom - rect.top;
            if (width <= 0 || height <= 0)
            {
                windowBmp = null;
                return false;
            }

            var rectangle = new Rectangle(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top);
            windowBmp = new(rectangle.Width, rectangle.Height);

            using var g = Graphics.FromImage(windowBmp);
            g.CopyFromScreen(new Point(rectangle.X, rectangle.Y), new Point(0, 0), rectangle.Size);
            Cursors.Arrow.DrawStretched(g, new Rectangle(Cursor.Position, Cursor.Current.Size));

            return true;
        }

        public static bool TryGetPrimaryWindow(out Bitmap windowBmp)
        {
            windowBmp = new(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            var g = Graphics.FromImage(windowBmp);

            g.CopyFromScreen(new Point(0, 0), new Point(0, 0), windowBmp.Size);
            Cursors.Arrow.DrawStretched(g, new Rectangle(Cursor.Position, Cursor.Current.Size));

            g.Dispose();
            return true;
        }


        public static Bitmap ResizeBmp(Bitmap baseBmp, int width, InterpolationMode mode)
        {
            var height = (int)(baseBmp.Height * (width / (double)baseBmp.Width));
            return ResizeBmp(baseBmp, height, width, mode);
        }

        public static Bitmap ResizeBmp(Bitmap baseBmp, int height, int width, InterpolationMode mode)
        {
            if (baseBmp == null)
            {
                throw new ArgumentNullException(nameof(baseBmp));
            }

            var resizeBmp = new Bitmap(width, height);
            using var g = Graphics.FromImage(resizeBmp);
            g.InterpolationMode = mode;
            g.DrawImage(baseBmp, 0, 0, width, height);

            return resizeBmp;
        }
    }
}
