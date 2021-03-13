using System;
using System.Drawing;
using System.Windows.Forms;

namespace SharedClientForm
{
    public class DropButton : Button
    {
        public event EventHandler IsDropChanged;

        public Action<DropButton> CloseVisual { get; set; } = b => b.Text = "<";
        public Action<DropButton> DropVisual { get; set; } = b => b.Text = "▼";

        private bool _IsDrop = false;
        public bool IsDrop {
            get => _IsDrop;
            set => SetIsDrop(value);
        }

        public DropButton()
        {
            FlatAppearance.BorderSize = 0;
            FlatStyle = FlatStyle.Flat;
            ForeColor = Color.Gray;
            Size = new Size(20, 24);
            CloseVisual.Invoke(this);
            Click += DropButton_Click;
        }

        private void DropButton_Click(object sender, EventArgs e)
        {
            IsDrop = !_IsDrop;
        }

        private void SetIsDrop(bool value)
        {
            if (_IsDrop == value)
            {
                return;
            }
            _IsDrop = value;

            if (IsDrop)
            {
                DropVisual?.Invoke(this);
            }
            else
            {
                CloseVisual?.Invoke(this);
            }

            IsDropChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
