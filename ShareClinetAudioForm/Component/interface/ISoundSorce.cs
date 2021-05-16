using System;
using System.Collections.Generic;
using System.Text;

namespace ShareClinetAudioForm
{
    public interface ISoundSorce : IDisposable
    {
        public event EventHandler ClosedSourse;
        public event EventHandler StopedSound;
        public bool IsListenning { get; }
        public void StartListen(ISoundListener soundListener);
        public void StopListen();
    }
}
