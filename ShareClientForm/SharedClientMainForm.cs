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
        private readonly SemaphoreSlim _Semaphore = new SemaphoreSlim(1);
        private readonly SettingForm _SettingForm = new SettingForm();
        private readonly IConnectionManager _ClientConnection = new ConnectionManager();
        private DisplayImageReciver _Receiver;
        private DisplayImageSender _Sender;

        private readonly Parameter _ReciveParam;
        private readonly Parameter _SendParam;
        private readonly System.Windows.Forms.Timer _SendByteTimer = new System.Windows.Forms.Timer();
        private readonly System.Windows.Forms.Timer _ReciveByteTimer = new System.Windows.Forms.Timer();

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

            _ReciveParam = SpeedMeter.Parameter("受信");
            _SendParam = SpeedMeter.Parameter("送信");
            SpeedMeter.Add(_ReciveParam);
            SpeedMeter.Add(_SendParam);

            _SendByteTimer.Interval = 1000;
            _SendByteTimer.Tick += SendByteTimer_Tick;
            _SendByteTimer.Start();

            _ReciveByteTimer.Interval = 1000;
            _ReciveByteTimer.Tick += ReciveByteTimer_Tick;
            _ReciveByteTimer.Start();

            _ClientConnection = new ConnectionManager();

            Activated += SharedDisplayForm_Activated;
            FormClosing += (s, e) =>
            {
                Stop();
                _SendByteTimer.Dispose();
                _ReciveByteTimer.Dispose();
            };
        }

        private void SharedDisplayForm_Activated(object sender, EventArgs e)
        {
            _SettingForm.Hide();
            SettingBtn.IsDrop = false;
        }

        private void ReciveByteTimer_Tick(object sender, EventArgs e)
        {
            if (_Receiver == null)
            {
                return;
            }

            SetSpeed(_Receiver.ClientManager.DataSize.Sum(), _ReciveParam);
            _Receiver.ClientManager.DataSizeClear();
        }

        private void SendByteTimer_Tick(object sender, EventArgs e)
        {
            if (this._Sender == null)
            {
                return;
            }

            SetSpeed(this._Sender.ClientManager.DataSize.Sum(), _SendParam);
            this._Sender.ClientManager.DataSizeClear();
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
                _SettingForm.Location = new Point(Location.X + SettingBtn.Location.X, Location.Y + SettingBtn.Location.Y + 60);
                _SettingForm.Show();
            }
            else
            {
                _SettingForm.Hide();
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

                connection = await _ClientConnection.ConnectAsync(iPEndPoint, spec, GetMeta());
            }
            catch (Exception ex)
            {
                throwEx = true;
                PushMessage(ex.Message);
            }
            finally
            {
                if (_ClientConnection.Status == ClientStatus.Open)
                {
                    PushMessage("接続しました。");
                    _Sender = new DisplayImageSender(connection, new DisplayImageCaputure(hWnd, _SettingForm.WindowWidth), _SettingForm.FlameLate);
                    _Sender.Sender.ShareClientClosed += (_, __) => PushMessage("切断しました。");
                    _Sender.Start();
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
            var str = Encoding.UTF8.GetBytes(_SettingForm.UserName);
            var meta = new byte[str.Length + 4 + 4];
            Array.Copy(str, meta, str.Length);
            Array.Copy(BitConverter.GetBytes(_SettingForm.FlameLate), 0, meta, str.Length, 4);
            Array.Copy(BitConverter.GetBytes(_SettingForm.WindowWidth), 0, meta, str.Length + 4, 4);

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
                    connection = await _ClientConnection.AcceptAsync(iPEndPoint, AcceptCallback);
                }
                catch (Exception ex)
                {
                    PushMessage(ex.Message);
                }
                finally
                {
                    if (_ClientConnection.Status == ClientStatus.Open)
                    {
                        PushMessage("接続しました。");
                        _Receiver = new DisplayImageReciver(connection, _SettingForm.FlameLate, PictureArea);
                        _Receiver.Reciver.ShareClientClosed += (_, __) => PushMessage("切断されました。");
                        _Receiver.Start();
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
                c.ConnectCallback = () => result = true;
                c.ShowDialog(this);

                return new ConnectionResponse(result, connection);
            }));
        }

        private async void PushMessage(string msg)
        {
            await _Semaphore.WaitAsync();

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

            _Semaphore.Release();
        }

        private void StopBtn_Click(object sender, EventArgs e)
        {
            Stop();
        }

        private void Stop()
        {
            _ClientConnection.Cancel();
            _Sender?.Dispose();
            _Receiver?.Dispose();
        }
    }
}
