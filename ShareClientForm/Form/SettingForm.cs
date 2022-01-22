using System.Drawing.Imaging;
using System.Windows.Forms;

namespace SharedClientForm
{
    public partial class SettingForm : Form
    {
        private readonly ImageFormat[] _Formats = new ImageFormat[] { ImageFormat.Jpeg, ImageFormat.Png, ImageFormat.Gif };

        public ImageFormat ImageFormat => (ImageFormat)ImageCmb.SelectedItem;
        public string UserName => textBox1.Text;
        public int FlameLate => (int)numericUpDown1.Value;
        public int WindowWidth => (int)numericUpDown2.Value;

        public SettingForm()
        {
            InitializeComponent();

            ImageCmb.Items.AddRange(_Formats);
            ImageCmb.SelectedIndex = 0;
        }
    }
}
