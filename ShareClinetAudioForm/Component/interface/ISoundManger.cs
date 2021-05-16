using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ShareClinetAudioForm
{
    public interface ISoundManger
    {
        public event EventHandler OutPutSound;
        public SoundLevelAnalyzer SoundLevelAnalyzer { get; }
        public void LoadSound(ISoundSorce soundSorce);
        public Stream GetOutPutSoundStream();
    }
}
