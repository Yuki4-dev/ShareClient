using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SharedClientForm.Component
{
    public class DisplayImageCaputure
    {
        private readonly IntPtr _WindowHandle;
        private readonly int _WindowWidth;
        private readonly InterpolationMode _Mode;

        public DisplayImageCaputure(IntPtr hWnd, int width, InterpolationMode mode = InterpolationMode.Default)
        {
            _WindowHandle = hWnd;
            _WindowWidth = width;
            _Mode = mode;
        }

        public bool TryGetWindowImage(out Image sendImage)
        {
            if (BmpHelper.TryGetWindow(_WindowHandle, out Bitmap windowBmp))
            {
                if (_WindowWidth > 0)
                {
                    sendImage = BmpHelper.ResizeBmp(windowBmp, _WindowWidth, _Mode);
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