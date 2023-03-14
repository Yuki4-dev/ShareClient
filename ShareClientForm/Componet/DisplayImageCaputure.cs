using ShareClientForm.Module;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ShareClientForm.Componet
{
    public class DisplayImageCapture
    {
        private readonly IntPtr _WindowHandle;
        private readonly int _WindowWidth;
        private readonly InterpolationMode _Mode;

        public DisplayImageCapture(IntPtr hWnd, int width, InterpolationMode mode = InterpolationMode.Default)
        {
            _WindowHandle = hWnd;
            _WindowWidth = width;
            _Mode = mode;
        }

        public bool TryGetWindowImage(out Image sendImage)
        {
            if (BmpHelper.TryGetWindow(_WindowHandle, out Bitmap windowBmp))
            {
                sendImage = _WindowWidth > 0 ? BmpHelper.ResizeBmp(windowBmp, _WindowWidth, _Mode) : (Image)windowBmp;
                return true;
            }

            sendImage = null;
            return false;
        }
    }
}