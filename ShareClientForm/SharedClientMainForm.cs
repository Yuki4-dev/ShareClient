using System;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShareClient.Component;
using ShareClient.Model;
using SharedClientForm;
using SharedClientForm.Component;

namespace SharedDisplayForm
{
    public partial class SharedClientMainForm : Form
    {
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1);
        private readonly SettingForm settingForm = new SettingForm();
        private readonly IConnectionManager clientConnection = new ConnectionManager();
        private DisplayImageReciver reciver;
        private DisplayImageSender sender;

        private readonly Parameter reciveParam;
        private readonly Parameter sendParam;
        private readonly System.Windows.Forms.Timer sendByteTimer = new System.Windows.Forms.Timer();
        private readonly System.Windows.Forms.Timer reciveByteTimer = new System.Windows.Forms.Timer();

        private delegate ConnectionResponse DelegateConnect();

        public SharedClientMainForm()
        {
            InitializeComponent();

            SettingBtn.DropVisual = b => b.BackColor = Color.LightGray;
            SettingBtn.CloseVisual = b => b.BackColor = BackColor;

            //ImageCmb.SelectedIndex = 2;

            ClientHostTextBox.Text = "127.0.0.1";
            ClientPortTextBox.Text = "2002";
            ServerPortTextBox.Text = "2002";
            ClientRudioBtn.Checked = true;

            reciveParam = SpeedMeter.Parameter("受信");
            sendParam = SpeedMeter.Parameter("送信");
            SpeedMeter.Add(reciveParam);
            SpeedMeter.Add(sendParam);

            sendByteTimer.Interval = 1000;
            sendByteTimer.Tick += SendByteTimer_Tick;
            sendByteTimer.Start();

            reciveByteTimer.Interval = 1000;
            reciveByteTimer.Tick += ReciveByteTimer_Tick;
            reciveByteTimer.Start();

            clientConnection = new ConnectionManager();

            Activated += SharedDisplayForm_Activated;
            FormClosing += (s, e) =>
            {
                Stop();
                sendByteTimer.Dispose();
                reciveByteTimer.Dispose();
            };
        }

        private void SharedDisplayForm_Activated(object sender, EventArgs e)
        {
            settingForm.Hide();
            SettingBtn.IsDrop = false;
        }

        private void ReciveByteTimer_Tick(object sender, EventArgs e)
        {
            if (reciver == null)
            {
                return;
            }

            SetSpeed(reciver.ClientManager.DataSize.Sum(), reciveParam);
            reciver.ClientManager.DataSizeClear();
        }

        private void SendByteTimer_Tick(object sender, EventArgs e)
        {
            if (this.sender == null)
            {
                return;
            }

            SetSpeed(this.sender.ClientManager.DataSize.Sum(), sendParam);
            this.sender.ClientManager.DataSizeClear();
        }

        private void SetSpeed(int size, Parameter parameter)
        {
            parameter.SetSpeed($"{string.Format("{0:n1} KB", size / 1024f)}/s");
        }

        private void SpeedBtn_IsDropChanged(object sender, System.EventArgs e)
        {
            SpeedMeter.Visible = SpeedBtn.IsDrop;
            if (SpeedBtn.IsDrop)
            {
                SpeedMeter.BringToFront();
            }
        }

        private void SettingBtn_IsDropChanged(object sender, System.EventArgs e)
        {
            if (SettingBtn.IsDrop)
            {
                settingForm.Location = new Point(Location.X + SettingBtn.Location.X, Location.Y + SettingBtn.Location.Y + 60);
                settingForm.Show();
            }
            else
            {
                settingForm.Hide();
            }
        }

        private async void ClientWork(string title, IntPtr hWnd)
        {
            var ip = ClientHostTextBox.Text;
            var port = int.Parse(ClientPortTextBox.Text);

            Connection connection = null;
            bool throwEx = false;
            try
            {
                var iPEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
                var spec = new ShareClientSpec();

                connection = await clientConnection.ConnectAsync(iPEndPoint, spec, GetMeta());
            }
            catch (Exception ex)
            {
                throwEx = true;
                PushMessage(ex.Message);
            }
            finally
            {
                if (clientConnection.Status == ClientStatus.Open)
                {
                    PushMessage("接続しました。");
                    sender = new DisplayImageSender(connection, new DisplayImageCaputure(hWnd, settingForm.WindowWidth), settingForm.FlameLate);
                    sender.Sender.ShareClientClosed += (_, __) => PushMessage("切断しました。");
                    sender.Start();
                }
                else
                {
                    if (!throwEx)
                    {
                        PushMessage("接続を拒否されました。");
                    }
                }
            }
        }

        private byte[] GetMeta()
        {
            var str = Encoding.UTF8.GetBytes(settingForm.UserName);
            var meta = new byte[str.Length + 4 + 4];
            Array.Copy(str, meta, str.Length);
            Array.Copy(BitConverter.GetBytes(settingForm.FlameLate), 0, meta, str.Length, 4);
            Array.Copy(BitConverter.GetBytes(settingForm.WindowWidth), 0, meta, str.Length + 4, 4);

            return meta;
        }

        private void ClientRudioBtn_CheckedChanged(object sender, EventArgs e)
        {
            var rb = (RadioButton)sender;
            if (rb.Checked)
            {
                ClientHostTextBox.Enabled = true;
                ClientPortTextBox.Enabled = true;
                ServerPortTextBox.Enabled = false;
            }
        }

        private void ServerRudioBtn_CheckedChanged(object sender, EventArgs e)
        {
            var rb = (RadioButton)sender;
            if (rb.Checked)
            {
                ClientHostTextBox.Enabled = false;
                ClientPortTextBox.Enabled = false;
                ServerPortTextBox.Enabled = true;
            }
        }

        private async void StartBtn_Click(object sender, EventArgs e)
        {

            if (ClientRudioBtn.Checked)
            {
                var sds = new DisplaySelectForm(ClientWork);
                sds.ShowDialog(this);
            }
            else
            {
                var port = int.Parse(ServerPortTextBox.Text);
                Connection connection = null;

                try
                {
                    var iPEndPoint = new IPEndPoint(IPAddress.Any, port);
                    connection = await clientConnection.AcceptAsync(iPEndPoint, AcceptCallback);
                }
                catch (Exception ex)
                {
                    PushMessage(ex.Message);
                }
                finally
                {
                    if (clientConnection.Status == ClientStatus.Open)
                    {
                        PushMessage("接続しました。");
                        reciver = new DisplayImageReciver(connection, settingForm.FlameLate, PictureArea);
                        reciver.Reciver.ShareClientClosed += (_, __) => PushMessage("切断されました。");
                        reciver.Start();
                    }
                    else
                    {
                        PushMessage("切断しました。");
                    }
                }
            }
        }

        private ConnectionResponse AcceptCallback(IPEndPoint iPEndPoint, ConnectionData connection)
        {
            return (ConnectionResponse)Invoke(new DelegateConnect(() => {
                bool result = false;
                var c = new ConnectForm(iPEndPoint, connection);
                c.ConnectCallBack = () => result = true;
                c.ShowDialog(this);

                return new ConnectionResponse(result, connection);
            }));
        }

        private async void PushMessage(string msg)
        {
            await semaphore.WaitAsync();

            var l = new Label() {
                Width = 300,
                Height = 100,
                Text = msg,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Yu Gothic UI", 20, FontStyle.Bold),
                BackColor = Color.Gray,
            };
            l.Location = new Point(ClientSize.Width - l.Width, ClientSize.Height - (l.Height + 50));
            Controls.Add(l);
            l.BringToFront();

            await Task.Delay(3000);

            Controls.Remove(l);
            l.Dispose();

            semaphore.Release();
        }

        private void StopBtn_Click(object sender, EventArgs e)
        {
            Stop();
        }

        private void Stop()
        {
            clientConnection.Cancel();
            sender?.Dispose();
            reciver?.Dispose();
        }
    }
}
