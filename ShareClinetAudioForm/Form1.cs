using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareClinetAudioForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var src = new PcMicSoundSorce();
            src.TryWaveInOpen(out var pwi);

            FormClosing += (s, e) => src.WaveInStop(ref pwi);
        }
    }
}
