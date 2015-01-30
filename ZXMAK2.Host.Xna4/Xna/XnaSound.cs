﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna.Framework.Audio;
using ZXMAK2.Host.Interfaces;


namespace ZXMAK2.Host.Xna4.Xna
{
    public unsafe class XnaSound : IHostSound, IDisposable
    {
        private readonly DynamicSoundEffectInstance m_soundEffect;
        private readonly AutoResetEvent m_frameEvent = new AutoResetEvent(false);
        private readonly ManualResetEvent m_waitEvent = new ManualResetEvent(true);
        private readonly int m_bufferLength;
        private readonly Queue<byte[]> m_playQueue = new Queue<byte[]>();
        private readonly Queue<byte[]> m_fillQueue = new Queue<byte[]>();
        private bool m_isCancel;


        public XnaSound(int sampleRate, int channelCount)
        {
            const int frameRate = 50;
            m_bufferLength = (sampleRate / frameRate) * channelCount * 2;

            m_soundEffect = new DynamicSoundEffectInstance(
                sampleRate, 
                channelCount == 1 ? AudioChannels.Mono : AudioChannels.Stereo);
            
            var needSize = m_soundEffect.GetSampleSizeInBytes(
                TimeSpan.FromMilliseconds(20));
            var bufferCount = needSize * 2 / m_bufferLength;
            if (bufferCount < 2)
            {
                bufferCount = 2;
            }
            for (var i = 0; i < bufferCount; i++)
            {
                m_fillQueue.Enqueue(new byte[m_bufferLength]);
            }
            m_soundEffect.BufferNeeded += SoundEffect_OnBufferNeeded;
        }

        public void Dispose()
        {
            CancelWait();
            m_waitEvent.Dispose();
        }

        public void Start()
        {
            m_soundEffect.Play();
        }

        public void Stop()
        {
            m_soundEffect.Stop();
        }

        
        #region IHostSound

        public void WaitFrame()
        {
            m_waitEvent.Reset();
            try
            {
                if (m_isCancel)
                {
                    return;
                }
                lock (m_soundEffect)
                {
                    if (m_fillQueue.Count > 0)
                    {
                        return;
                    }
                }
                m_frameEvent.WaitOne(100);
            }
            finally
            {
                m_waitEvent.Set();
            }
        }

        public void CancelWait()
        {
            m_isCancel = true;
            Thread.MemoryBarrier();
            m_waitEvent.WaitOne();
            m_isCancel = false;
            Thread.MemoryBarrier();
        }

        public void PushFrame(uint[][] frameBuffers)
        {
            var buffer = LockBuffer();
            if (buffer == null)
            {
                return;
            }
            Mix(buffer, frameBuffers);
            UnlockBuffer(buffer);
        }

        #endregion IHostSound


        #region Private

        private byte[] LockBuffer()
        {
            lock (m_soundEffect)
            {
                if (m_fillQueue.Count == 0)
                {
                    return null;
                }
                return m_fillQueue.Dequeue();
            }
        }

        private void UnlockBuffer(byte[] buffer)
        {
            lock (m_soundEffect)
            {
                m_soundEffect.SubmitBuffer(buffer);
                m_playQueue.Enqueue(buffer);
            }
        }

        private void SoundEffect_OnBufferNeeded(object sender, EventArgs e)
        {
            m_frameEvent.Set();
            lock (m_soundEffect)
            {
                if (m_playQueue.Count == 0)
                {
                    return;
                }
                m_fillQueue.Enqueue(m_playQueue.Dequeue());
            }
        }

        private void Mix(byte[] dst, uint[][] bufferArray)
        {
            fixed (byte* bptr = dst)
            {
                var uiptr = (uint*)bptr;
                for (var i = 0; i < dst.Length / 4; i++)    // clean buffer
                {
                    uint value1 = 0;
                    uint value2 = 0;
                    if (bufferArray.Length > 0)
                    {
                        for (int j = 0; j < bufferArray.Length; j++)
                        {
                            value1 += bufferArray[j][i] >> 16;
                            value2 += bufferArray[j][i] & 0xFFFF;
                        }
                        value1 /= (uint)bufferArray.Length;
                        value2 /= (uint)bufferArray.Length;
                    }
                    uiptr[i] = (value1 << 16) | value2;
                }
            }
        }

        #endregion Private
    }
}
