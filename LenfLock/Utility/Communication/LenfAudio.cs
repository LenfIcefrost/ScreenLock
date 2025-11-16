using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LenfLock.Communication {
    internal class LenfAudio {
        const int MMSYSERR_NOERROR = 0;
        const int CALLBACK_FUNCTION = 0x30000;
        const int WIM_DATA = 0x3C0;
        bool startFailed = false;

        [DllImport("winmm.dll", SetLastError = true)]
        static extern int waveInOpen(out IntPtr hWaveIn, int uDeviceID, WaveFormat lpFormat, WaveInProc dwCallback, IntPtr dwInstance, int dwFlags);

        [DllImport("winmm.dll")]
        static extern int waveInStart(IntPtr hWaveIn);

        [DllImport("winmm.dll")]
        static extern int waveInStop(IntPtr hWaveIn);

        [DllImport("winmm.dll")]
        static extern int waveInClose(IntPtr hWaveIn);

        [DllImport("winmm.dll")]
        static extern int waveInPrepareHeader(IntPtr hWaveIn, ref WAVEHDR lpWaveInHdr, int uSize);

        [DllImport("winmm.dll")]
        static extern int waveInAddBuffer(IntPtr hWaveIn, ref WAVEHDR lpWaveInHdr, int uSize);

        private WAVEHDR waveHeader;

        [StructLayout(LayoutKind.Sequential)]
        public class WaveFormat {
            public short wFormatTag;
            public short nChannels;
            public int nSamplesPerSec;
            public int nAvgBytesPerSec;
            public short nBlockAlign;
            public short wBitsPerSample;
            public short cbSize;

            public WaveFormat(int rate, int bits, int channels) {
                wFormatTag = 1; // PCM
                nChannels = (short)channels;
                nSamplesPerSec = rate;
                wBitsPerSample = (short)bits;
                nBlockAlign = (short)(channels * (bits / 8));
                nAvgBytesPerSec = nSamplesPerSec * nBlockAlign;
                cbSize = 0;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WAVEHDR {
            public IntPtr lpData;
            public uint dwBufferLength;
            public uint dwBytesRecorded;
            public IntPtr dwUser;
            public uint dwFlags;
            public uint dwLoops;
            public IntPtr lpNext;
            public IntPtr reserved;
        }

        delegate void WaveInProc(IntPtr hdrvr, int uMsg, IntPtr dwUser, IntPtr wavhdr, IntPtr dwParam2);
        private WaveInProc callback;

        private IntPtr waveInHandle;
        private byte[] audioBuffer = new byte[1024];

        public void StartRecording() {
            WaveFormat format = new WaveFormat(44100, 16, 1);
            callback = new WaveInProc(WaveInCallback);
            int result = waveInOpen(out waveInHandle, -1, format, callback, IntPtr.Zero, CALLBACK_FUNCTION);

            if (result == MMSYSERR_NOERROR) {
                waveHeader = new WAVEHDR();
                waveHeader.lpData = Marshal.AllocHGlobal(audioBuffer.Length);
                waveHeader.dwBufferLength = (uint)audioBuffer.Length;
                waveHeader.dwBytesRecorded = 0;

                waveInPrepareHeader(waveInHandle, ref waveHeader, Marshal.SizeOf(waveHeader));
                waveInAddBuffer(waveInHandle, ref waveHeader, Marshal.SizeOf(waveHeader));

                waveInStart(waveInHandle);
            } else {
                startFailed = true;
                //MessageBox.Show("麥克風獲取發生錯誤 Code : " + result);
            }
        }

        private void WaveInCallback(IntPtr hdrvr, int uMsg, IntPtr dwUser, IntPtr wavhdr, IntPtr dwParam2) {
            if (uMsg == WIM_DATA) {
                Marshal.Copy(waveHeader.lpData, audioBuffer, 0, (int)waveHeader.dwBytesRecorded);

                double volume = CalculateVolume(audioBuffer);
                if (QuestionData.instance.Audio.Active && volume > QuestionData.instance.Audio.Filter) {
                    MainInterface.instance.Invoke((MethodInvoker)delegate {
                        MainInterface.instance.show();
                    });
                }

                waveInAddBuffer(waveInHandle, ref waveHeader, Marshal.SizeOf(waveHeader));
            }
        }

        private double CalculateVolume(byte[] buffer) {
            int bytesRecorded = buffer.Length;
            int sampleCount = bytesRecorded / 2;
            double sum = 0;

            for (int i = 0; i < sampleCount; i++) {
                short sample = BitConverter.ToInt16(buffer, i * 2);
                sum += (sample * sample);
            }

            double rms = Math.Sqrt(sum / sampleCount);
            double volume = rms / 32768.0 * 100;

            return volume; // 0 ~ 100
        }

        public void StopRecording() {
            if (startFailed) return;
            waveInStop(waveInHandle);
            waveInClose(waveInHandle);

            Marshal.FreeHGlobal(waveHeader.lpData);
        }
    }
}
