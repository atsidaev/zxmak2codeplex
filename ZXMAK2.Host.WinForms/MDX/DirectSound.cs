/* 
 *  Copyright 2008-2018 Alex Makeev
 * 
 *  This file is part of ZXMAK2 (ZX Spectrum virtual machine).
 *
 *  ZXMAK2 is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  ZXMAK2 is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with ZXMAK2.  If not, see <http://www.gnu.org/licenses/>.
 *
 *  Description: Sound player
 *  Date: 10.07.2018
 */
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ZXMAK2.Host.Interfaces;
using ZXMAK2.Host.WinForms.Tools;
using ZXMAK2.DirectX;
using ZXMAK2.DirectX.DirectSound;
using ZXMAK2.DirectX.Native;



namespace ZXMAK2.Host.WinForms.Mdx
{
    public sealed unsafe class DirectSound : IHostSound
    {
        #region Fields

        private readonly DirectSound8 _device;
        private readonly SecondarySoundBuffer _soundBuffer;
        private readonly ConcurrentQueue<uint[]> _fillQueue = new ConcurrentQueue<uint[]>();
        private readonly ConcurrentQueue<uint[]> _playQueue = new ConcurrentQueue<uint[]>();
        private readonly AutoResetEvent _fillEvent = new AutoResetEvent(true);
        private readonly AutoResetEvent _frameEvent = new AutoResetEvent(false);
        private readonly AutoResetEvent _cancelEvent = new AutoResetEvent(false);
        private readonly int _sampleRate;
        private readonly int _bufferSize;
        private readonly int _bufferCount;
        private readonly uint _zeroValue;

        private Thread _wavePlayThread;
        private bool _isFinished;
        private uint? _lastSample;

        #endregion Fields


        public DirectSound(
            Form form,
            int sampleRate,
            int bufferCount)
        {
            if ((sampleRate % 50) != 0)
            {
                throw new ArgumentOutOfRangeException("sampleRate", "Sample rate must be a multiple of 50!");
            }
            _sampleRate = sampleRate;
            var bufferSize = sampleRate / 50;
            for (int i = 0; i < bufferCount; i++)
            {
                _fillQueue.Enqueue(new uint[bufferSize]);
            }
            _bufferSize = bufferSize;
            _bufferCount = bufferCount;
            _zeroValue = 0;

            var hWnd = form != null ? form.Handle : IntPtr.Zero;
            _device = new DirectSound8();
            _device.SetCooperativeLevel(hWnd, DSSCL.PRIORITY);

            // we always use 16 bit stereo (uint per sample)
            const short channels = 2;
            const short bitsPerSample = 16;
            const int sampleSize = 4; // channels * (bitsPerSample / 8);
            var wf = new WaveFormat(_sampleRate, bitsPerSample, channels);

            // Create a buffer
            var bufferDesc = new DSBUFFERDESC();
            bufferDesc.dwBufferBytes = _bufferSize * sampleSize * _bufferCount;
            bufferDesc.dwFlags =
                DSBCAPS_FLAGS.CTRLVOLUME |
                DSBCAPS_FLAGS.CTRLPOSITIONNOTIFY |
                DSBCAPS_FLAGS.GLOBALFOCUS |
                DSBCAPS_FLAGS.GETCURRENTPOSITION2;
            bufferDesc.Format = wf;
            _soundBuffer = new SecondarySoundBuffer(_device, bufferDesc);

            var posNotify = new DSBPOSITIONNOTIFY[_bufferCount];
            for (int i = 0; i < posNotify.Length; i++)
            {
                posNotify[i] = new DSBPOSITIONNOTIFY();
                posNotify[i].dwOffset = i * _bufferSize * sampleSize;
                posNotify[i].WaitHandle = _fillEvent;
            }
            _soundBuffer.SetNotificationPositions(posNotify);

            _wavePlayThread = new Thread(WavePlayThreadProc);
            _wavePlayThread.IsBackground = true;
            _wavePlayThread.Name = "WavePlay";
            _wavePlayThread.Priority = ThreadPriority.Highest;
            _wavePlayThread.Start();
        }

        public void Dispose()
        {
            if (_wavePlayThread == null)
            {
                return;
            }
            try
            {
                _isFinished = true;
                Thread.MemoryBarrier();
                _fillEvent.Set();
                _cancelEvent.Set();
                _wavePlayThread.Join();

                if (_soundBuffer != null)
                {
                    if (_soundBuffer.Status == DSBSTATUS.PLAYING)
                    {
                        _soundBuffer.Stop();
                    }
                    _soundBuffer.Dispose();
                }
                if (_device != null)
                {
                    _device.Dispose();
                }
                if (_fillEvent != null)
                {
                    _fillEvent.Dispose();
                }
                if (_frameEvent != null)
                {
                    _frameEvent.Dispose();
                }
                if (_cancelEvent != null)
                {
                    _cancelEvent.Dispose();
                }
            }
            finally
            {
                _wavePlayThread = null;
            }
        }


        #region WavePlay

        private unsafe void WavePlayThreadProc()
        {
            try
            {
                _soundBuffer.Play(0, DSBPLAY_FLAGS.LOOPING);
                const int sampleSize = 4;
                var rawBufferLength = _bufferSize * sampleSize;
                var hBuffer = Marshal.AllocHGlobal(rawBufferLength);
                NativeHelper.INITBLK((void*)hBuffer, 0, rawBufferLength);
                try
                {
                    var lastWrittenBuffer = -1;
                    do
                    {
                        _fillEvent.WaitOne();
                        var nextIndex = (lastWrittenBuffer + 1) % _bufferCount;
                        var playPos = _soundBuffer.PlayPosition % (_bufferCount * rawBufferLength);
                        var playingIndex = playPos / rawBufferLength;
                        for (var i = nextIndex; i != playingIndex && !_isFinished; i = ++i % _bufferCount)
                        {
                            if (!OnBufferRequest((uint*)hBuffer, _bufferSize))
                            {
                                NativeHelper.INITBLK((void*)hBuffer, 0, rawBufferLength);
                            }
                            var writePos = i * rawBufferLength;
                            _soundBuffer.Write(writePos, (void*)hBuffer, rawBufferLength, DSBLOCK.None);
                            lastWrittenBuffer = i;
                        }
                    } while (!_isFinished);
                }
                finally
                {
                    Marshal.FreeHGlobal(hBuffer);
                    _soundBuffer.Stop();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private bool OnBufferRequest(uint* pBuffer, int sampleCount)
        {
            uint[] source;
            if (!_playQueue.TryDequeue(out source))
            {
                var sample = _lastSample.HasValue ? _lastSample.Value : _zeroValue;
                // TODO: native optimization?
                for (var i = 0; i < sampleCount; i++)
                {
                    pBuffer[i] = sample;
                }
                return true;
            }
            try
            {
                fixed (uint* psrc = source)
                {
                    NativeHelper.CPBLK(pBuffer, psrc, sampleCount * 4);
                    //NativeMethods.CopyMemory(pBuffer, psrc, sampleCount * 4);
                    _lastSample = pBuffer[sampleCount - 1];
                }
                return true;
            }
            finally
            {
                _fillQueue.Enqueue(source);
                _frameEvent.Set();
            }
        }

        private uint[] LockBuffer()
        {
            uint[] buffer;
            if (_fillQueue.TryDequeue(out buffer))
            {
                return buffer;
            }
            return null;
        }

        private void UnlockBuffer(uint[] buffer)
        {
            _playQueue.Enqueue(buffer);
        }

        public double GetLoadLevel()
        {
            return _playQueue.Count / (double)_bufferCount;
        }

        #endregion WavePlay


        #region IHostSound

        public int SampleRate
        {
            get { return _sampleRate; }
        }

        public bool IsSyncSupported 
        {
            get { return true; }
        }

        public bool IsSynchronized { get; set; }

        private void WaitFrame()
        {
            _frameEvent.Reset();
            _cancelEvent.Reset();
            Thread.MemoryBarrier();
            if (_playQueue.Count == 0)
            {
                return;
            }
            WaitHandle.WaitAny(new[] { _frameEvent, _cancelEvent }, 40);
        }

        public void CancelWait()
        {
            _cancelEvent.Set();
        }

        public void PushFrame(IFrameInfo info, IFrameSound frame)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            if (frame == null)
            {
                throw new ArgumentNullException("frame");
            }
            if (IsSynchronized)
            {
                WaitFrame();
            }
            var buffer = LockBuffer();
            if (buffer == null)
            {
                return;
            }
            var srcBuffer = frame.GetBuffer();
            Array.Copy(srcBuffer, buffer, buffer.Length);
            UnlockBuffer(buffer);
        }

        #endregion IHostSound
    }
}
