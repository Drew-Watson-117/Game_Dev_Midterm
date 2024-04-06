using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Midterm
{
    internal class CaughtEffect : ParticleEffect
    {
        Rod m_rod;
        Timer m_emissionTimer;
        public CaughtEffect(Rod rod, int sizeMean, int sizeStdDev, float speedMean, float speedStdDev, float lifetimeMean, float lifetimeStdDev) : base("particle", sizeMean, sizeStdDev, speedMean, speedStdDev, lifetimeMean, lifetimeStdDev)
        {
            m_rod = rod;
            m_emissionTimer = new Timer(100);
        }

        public override void Update(GameTime gameTime)
        {
            // Update existing particles
            List<long> removeMe = new List<long>();
            foreach (Particle p in m_particles.Values)
            {
                if (!p.update(gameTime))
                {
                    removeMe.Add(p.name);
                }
            }

            // Remove dead particles
            foreach (long key in removeMe)
            {
                m_particles.Remove(key);
            }

            // Add some new particles around the rod
            if (m_rod.GetState() == RodState.Caught && !m_emissionTimer.HasExpired())
            {
                m_emissionTimer.Update(gameTime);
                for (int i = 0; i < 8; i++)
                {
                    Rectangle rodRect = m_rod.GetRectangle();
                    int yValue = m_random.Next(rodRect.Top, rodRect.Bottom);
                    int xValue = rodRect.Center.X;
                    var particle = Create(new Vector2(xValue, yValue), m_random.nextCircleVector());
                    m_particles.Add(particle.name, particle);
                }
            }
        }
    }
}
