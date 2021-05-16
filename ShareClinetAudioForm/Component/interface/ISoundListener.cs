using System;
using System.Collections.Generic;
using System.Text;

namespace ShareClinetAudioForm
{
    public interface ISoundListener
    {
        public void Listen(byte[] soundData);
    }
}
