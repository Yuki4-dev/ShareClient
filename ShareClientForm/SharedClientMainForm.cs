﻿using ShareClient.Model.Connect;
using ShareClient.Model.ShareClient;
using ShareClientForm.Componet;
using SharedClientForm;
using SharedClientForm.Component;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SharedDisplayForm
{
    public partial class SharedClientMainForm : Form
    {
        private readonly SemaphoreSlim _Semaphore = new SemaphoreSlim(1);
        private readonly SettingForm _SettingForm = new SettingForm();
        private readonly Parameter _ReciveParam;
        private readonly Parameter _SendParam;
        private readonly ImageShareAlgorithm _ShareAlgorithm = new ImageShareAlgorithm();
        private readonly System.Windows.Forms.Timer _SpeedTimer = new System.Windows.Forms.Timer();

        public SharedClientMainForm()
        {
            InitializeComponent();

            SettingBtn.DropVisual = b => b.BackColor = Color.LightGray;
            SettingBtn.CloseVisual = b => b.BackColor = BackColor;

            ClientHostTextBox.Text = "127.0.0.1";
            ClientPortTextBox.Text = "2002";
            ServerPortTextBox.Text = "2002";
            ClientRudioBtn.Checked = true;

            _ReciveParam = SpeedMeter.Parameter("受信");
            _SendParam = SpeedMeter.Parameter("送信");
            SpeedMeter.Add(_ReciveParam);
            SpeedMeter.Add(_SendParam);

            _SpeedTimer.Interval = 1000;
            _SpeedTimer.Tick += SpeedTimer_Tick;
            _SpeedTimer.Start();
        }

        private void SharedClientMainForm_Activated(object sender, EventArgs e)
        {
            SettingBtn.IsDrop = false;
        }

        private void SharedClientMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Stop();
            _SpeedTimer.Dispose();
        }

        private void SpeedTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                SetSpeed(_ShareAlgorithm.ReciveManager.GetRecieveDataSize(), _ReciveParam);
                _ShareAlgorithm.ReciveManager.RecieveDataSizeClear();
                SetSpeed(_ShareAlgorithm.SendManager.GetSendDataSize(), _SendParam);
                _ShareAlgorithm.SendManager.SendDataSizeClear();
            }
            catch (Exception ex)
            {
                PushMessage(ex.Message);
            }
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

        private void RudioBtn_CheckedChanged(object sender, EventArgs e)
        {
            if (ClientRudioBtn.Checked)
            {
                ClientHostTextBox.Enabled = true;
                ClientPortTextBox.Enabled = true;
                ServerPortTextBox.Enabled = false;
            }
            else if (ServerRudioBtn.Checked)
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
                return;
            }

            try
            {
                var iPEndPoint = new IPEndPoint(IPAddress.Any, int.Parse(ServerPortTextBox.Text));
                var connection = await _ShareAlgorithm.AcceptAsync(iPEndPoint, AcceptCallback);
                if (connection != null)
                {
                    PushMessage("接続しました。");
                    _ShareAlgorithm.Recieve(connection, _SettingForm.FlameLate, PictureArea, () => PushMessage("切断されました。"));
                }
                else
                {
                    PushMessage("切断しました。");
                }
            }
            catch (Exception ex)
            {
                PushMessage(ex.Message);
            }
        }

        private async void ClientWork(string title, IntPtr hWnd)
        {
            try
            {
                var iPEndPoint = new IPEndPoint(IPAddress.Parse(ClientHostTextBox.Text), int.Parse(ClientPortTextBox.Text));
                var connection = await _ShareAlgorithm.ConnectAsync(iPEndPoint, new ConnectionData(new ShareClientSpec(), GetMeta()));
                if (connection != null)
                {
                    PushMessage("接続しました。");
                    _ShareAlgorithm.Send(connection,
                                                new DisplayImageCaputure(hWnd, _SettingForm.WindowWidth),
                                                _SettingForm.FlameLate,
                                                _SettingForm.ImageFormat,
                                                () => PushMessage("切断しました。"));
                }
                else
                {
                    PushMessage("接続を拒否されました。");
                }
            }
            catch (Exception ex)
            {
                PushMessage(ex.Message);
                return;
            }
        }

        private ConnectionResponse AcceptCallback(IPEndPoint iPEndPoint, ConnectionData connectionData)
        {
            if (InvokeRequired)
            {
                return Invoke(() => AcceptCallback(iPEndPoint, connectionData));
            }

            bool result = false;
            var c = new ConnectForm(iPEndPoint, connectionData);
            c.ConnectCallback = () => result = true;
            c.ShowDialog(this);

            return new ConnectionResponse(result, connectionData);
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

        private async void PushMessage(string msg)
        {
            await _Semaphore.WaitAsync();

            var l = new Label()
            {
                Width = 400,
                Height = 200,
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
            _ShareAlgorithm.Stop();
        }
    }
}
