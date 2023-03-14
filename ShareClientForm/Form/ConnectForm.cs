using ShareClient.Model.Connect;
using System;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace SharedClientForm
{
    public partial class ConnectForm : Form
    {

        public Action ConnectCallback { get; set; }

        public ConnectForm(IPEndPoint iPEndPoint, ConnectionData connection)
        {
            InitializeComponent();
            if (connection.MetaData.Length == 0)
            {
                return;
            }

            var meta = connection.MetaData.AsSpan();
            var name = meta[0..^8].ToArray();
            textboxConnection.AppendText($" NAME：【{Encoding.UTF8.GetString(name)}】");
            textboxConnection.AppendText(Environment.NewLine);
            textboxConnection.AppendText($" IP：【{iPEndPoint.Address}】");
            textboxConnection.AppendText(Environment.NewLine);
            var nameLen = name.Length;
            textboxConnection.AppendText($" FlameLate：【{BitConverter.ToInt32(meta[nameLen..^4].ToArray())}】");
            textboxConnection.AppendText(Environment.NewLine);
            nameLen += 4;
            textboxConnection.AppendText($" WidowWidth：【{BitConverter.ToInt32(meta[nameLen..meta.Length].ToArray())}】");
            textboxConnection.AppendText(Environment.NewLine);
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            Close();
            ConnectCallback?.Invoke();
        }

        private void buttoonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }

}
