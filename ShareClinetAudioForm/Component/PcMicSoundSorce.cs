using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareClinetAudioForm
{
    public class PcMicSoundSorce : ISoundSorce
    {
        public bool IsListenning { get; private set; } = false;

        public event EventHandler ClosedSourse;
        public event EventHandler StopedSound;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate void DelegateWaveInProc(IntPtr hwi, uint uMsg, IntPtr dwInstance, IntPtr dwParam1, IntPtr dwParam2);
        private readonly IntPtr _PtrWaveInProc;
        private IntPtr lpWaveData = IntPtr.Zero;
        private int dwheaderBytesize;

        private NativeMethods.WaveHdr wh;
        private NativeMethods.WaveFormatEx wf;

        public PcMicSoundSorce()
        {
            _PtrWaveInProc = Marshal.GetFunctionPointerForDelegate(new DelegateWaveInProc(WaveInProc));
        }

        public void StartListen(ISoundListener soundListener)
        {
        }

        public void StopListen()
        {
        }

        public void Dispose()
        {
        }

        public bool TryWaveInOpen(out IntPtr hwi)
        {
            wf = new NativeMethods.WaveFormatEx();
            wf.wFormatTag = NativeMethods.WAVE_FORMAT_PCM;
            wf.cbSize = 0;
            wf.nChannels = 1;
            wf.nSamplesPerSec = 11025;
            wf.wBitsPerSample = 8;
            wf.nBlockAlign = (short)(wf.wBitsPerSample / 8 * wf.nChannels);
            wf.nAvgBytesPerSec = wf.nSamplesPerSec * wf.nBlockAlign;

            hwi = IntPtr.Zero;
            var result = WaveInOpen(ref hwi, ref wf, _PtrWaveInProc);
            if (result != NativeMethods.MMSYSERR_NOERROR)
            {
                return false;
            }

            int dwRecordSecond = 5;
            dwheaderBytesize = wf.nAvgBytesPerSec * dwRecordSecond;
            lpWaveData = Marshal.AllocHGlobal(dwheaderBytesize);

            wh = new NativeMethods.WaveHdr();
            wh.lpData = lpWaveData;
            wh.dwBufferLength = dwheaderBytesize;
            //wh.dwFlags = 0;

            var cdwh = Marshal.SizeOf(wh);
            //var cdwh = 32;

            NativeMethods.waveInPrepareHeader(hwi, ref wh, cdwh);
            //NativeMethods.waveInReset(hwi);
            NativeMethods.waveInAddBuffer(hwi, ref wh, cdwh);
            NativeMethods.waveInStart(hwi);

            return true;
        }


        public void WaveInStop(ref IntPtr hwi)
        {
            Marshal.FreeHGlobal(lpWaveData);
            NativeMethods.waveInStop(hwi);
            NativeMethods.waveInReset(hwi);
            NativeMethods.waveInClose(hwi);
        }

        private int WaveInOpen(ref IntPtr hwi, ref NativeMethods.WaveFormatEx wf, IntPtr dwCallback, int fdwOpen = NativeMethods.CALLBACK_FUNCTION)
        {
            return NativeMethods.waveInOpen(ref hwi, NativeMethods.WAVE_MAPPER, ref wf, dwCallback, 0, fdwOpen);
        }

        private void WaveInProc(IntPtr hwi, uint uMsg, IntPtr dwInstance, IntPtr dwParam1, IntPtr dwParam2)
        {
            switch (uMsg)
            {
                case NativeMethods.WIM_OPEN:
                    MessageBox.Show("Open");
                    break;
                case NativeMethods.WIM_DATA:
                    MessageBox.Show("Data");
                    var wf1 = Marshal.PtrToStructure<NativeMethods.WaveHdr>(dwParam1);

                    var wh = new WaveHeader()
                    {
                        FileSize = WaveHeader.HeaderSize + dwheaderBytesize,
                        FormatChunkSize = 16,
                        FormatID = NativeMethods.WAVE_FORMAT_PCM,
                        Channel = 1,
                        SampleRate = wf.nSamplesPerSec,
                        BytePerSec = wf.nAvgBytesPerSec,
                        BlockSize = wf.nBlockAlign,
                        BitPerSample = 8,
                        DataChunkSize = dwheaderBytesize
                    };

                    var data = new byte[wh.FileSize];
                    Array.Copy(wh.ToBytes(),0, data, 0, WaveHeader.HeaderSize);
                    Marshal.Copy(lpWaveData, data, WaveHeader.HeaderSize, dwheaderBytesize);
                    using (var fs = new FileStream(@"C:\Users\sanak\Desktop\新しいフォルダー\1.wav", FileMode.Create))
                    using (var sw = new BinaryWriter(fs))
                    {
                        //sw.Write(string.Join("",data.Select(x => x.ToString())));
                        sw.Write(data);
                    }
                    break;
                case NativeMethods.WIM_CLOSE:
                    MessageBox.Show("Close");
                    break;
                default:
                    MessageBox.Show("Def");
                    break;
            }
        }
    }
}
