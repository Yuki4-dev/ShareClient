using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SharedClientForm.Component
{
    public class DisplayImageCaputure
    {
        private readonly IntPtr windowHandle;
        private readonly int windowWidth;
        private readonly InterpolationMode interpolationMode;

        public DisplayImageCaputure(IntPtr hWnd, int width, InterpolationMode mode = InterpolationMode.Default)
        {
            windowHandle = hWnd;
            windowWidth = width;
            interpolationMode = mode;
        }

        public bool TryGetWindowImage(out Image sendImage)
        {

            if (BmpHelper.TryGetWindow(windowHandle, out Bitmap windowBmp))
            {
                if(windowWidth > 0)
                {
                    sendImage = BmpHelper.ResizeBmp(windowBmp, windowWidth, interpolationMode);
                }
                else
                {
                    sendImage = windowBmp;
                }
                return true;
            }

            sendImage = null;
            return false;
        }
    }
}