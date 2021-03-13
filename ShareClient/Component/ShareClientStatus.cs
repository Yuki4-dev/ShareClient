using System;

namespace ShareClient.Component
{
    public abstract class ShareClientStatus : IClientStatus
    {
        private readonly object lockObj = new object();
        public ClientStatus Status { get; private set; } = ClientStatus.Init;

        protected void init()
        {
            CheckClose();
            Status = ClientStatus.Init;
        }

        protected void Connect()
        {
            CheckClose();
            lock (lockObj)
            {
                if (Status == ClientStatus.Connect)
                {
                    throw new InvalidOperationException(Status.ToString());
                }
                Status = ClientStatus.Connect;
            }
        }

        protected void Open()
        {
            CheckClose();
            lock (lockObj)
            {
                if (Status == ClientStatus.Open)
                {
                    throw new InvalidOperationException(Status.ToString());
                }
                Status = ClientStatus.Open;
            }
        }

        public void Close()
        {
            lock (lockObj)
            {
                if (Status != ClientStatus.Close)
                {
                    Status = ClientStatus.Close;
                    CloseClient();
                }
            }
        }

        private void CheckClose()
        {
            if (Status == ClientStatus.Close)
            {
                throw new InvalidOperationException(Status.ToString());
            }
        }

        public void Dispose()
        {
            Close();
        }

        protected abstract void CloseClient();
    }

}
