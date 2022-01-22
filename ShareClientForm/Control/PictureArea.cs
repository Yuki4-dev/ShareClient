﻿using System.Drawing;
using System.Windows.Forms;

namespace SharedClientForm
{
    public interface IPictureArea
    {
        public void PaintDefault();
        public void PaintPicture(Image img);
    }

    public class PictureArea : Control, IPictureArea
    {
        protected readonly PictureBox _MainPicture = new PictureBox();

        public Image DefaultPicture { get; set; }

        private delegate void DelegateSetImage();

        public PictureArea()
        {
            _MainPicture.TabStop = false;
            _MainPicture.Dock = DockStyle.Fill;
            _MainPicture.SizeMode = PictureBoxSizeMode.CenterImage;

            Controls.Add(_MainPicture);
        }

        public void PaintPicture(Image img)
        {
            SetImage(img);
        }

        public void PaintDefault()
        {
            SetImage(DefaultPicture);
        }

        protected virtual void SetImage(Image img)
        {
            if (_MainPicture.InvokeRequired)
            {
                _MainPicture.Invoke(new DelegateSetImage(() => SetImage(img)));
                return;
            }
            var oldImage = _MainPicture.Image;
            _MainPicture.Image = img;
            Size = img.Size;
            oldImage?.Dispose();
        }
    }
}
