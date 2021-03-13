using System.Windows.Forms;

namespace SharedClientForm
{
    public partial class SettingForm : Form
    {
        public string UserName => textBox1.Text;
        public int FlameLate => (int)numericUpDown1.Value;
        public int WindowWidth => (int)numericUpDown2.Value;

        public SettingForm()
        {
            InitializeComponent();
        }
    }
}
