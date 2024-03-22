﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Midterm
{
    public class Timer
    {
        float m_totalTimeMilliseconds;
        float m_remainingTime;
        public Timer(float totalTimeMilliseconds)
        {
            m_totalTimeMilliseconds = totalTimeMilliseconds;
            m_remainingTime = totalTimeMilliseconds;
        }

        public void Update(GameTime gameTime)
        {
            m_remainingTime -= gameTime.ElapsedGameTime.Milliseconds;
        }
        public float GetRemainingTime() { return m_remainingTime; }
        public bool HasExpired() { return m_remainingTime <= 0; }
        public double GetDisplayTime() { return (int)(m_remainingTime / 1000) + 1; }
    }
}
