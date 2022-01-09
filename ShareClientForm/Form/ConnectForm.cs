using ShareClient.Model;
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
            textbobConnection.AppendText($" NAME：【{Encoding.UTF8.GetString(name)}】");
            textbobConnection.AppendText(Environment.NewLine);
            textbobConnection.AppendText($" IP：【{iPEndPoint.Address}】");
            textbobConnection.AppendText(Environment.NewLine);
            var nameLen = name.Length;
            textbobConnection.AppendText($" FlameLate：【{BitConverter.ToInt32(meta[nameLen..^4].ToArray())}】");
            textbobConnection.AppendText(Environment.NewLine);
            nameLen += 4;
            textbobConnection.AppendText($" WidowWidth：【{BitConverter.ToInt32(meta[nameLen..meta.Length].ToArray())}】");
            textbobConnection.AppendText(Environment.NewLine);
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
