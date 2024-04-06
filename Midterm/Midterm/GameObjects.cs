using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace Midterm
{
    public enum RodState
    {
        Held,
        Released,
        Caught,
        Missed
    }
    internal class Rod
    {
        Vector2 m_position;
        float m_verticalMomentum;
        float m_gravity;
        RodState m_state;
        int m_width;
        int m_height;
        Rectangle m_rectangle;

        Texture2D m_texture;
        public Rod(Vector2 pos, float gravity, int width, int height) 
        {
            m_position = pos;
            m_gravity = gravity;
            m_width = width;
            m_height = height;
            m_verticalMomentum = 0;
            m_rectangle = new Rectangle((int)m_position.X, (int)m_position.Y, width, height);
            m_state = RodState.Held;
        }

        public void SetState(RodState state)
        {
            m_state = state;
        }
        public RodState GetState() { return m_state; }
        public Rectangle GetRectangle() { return m_rectangle; }
        public bool HasCollided(PlayerHand player)
        {
            Rectangle other = player.GetRectangle();
            return m_rectangle.X < other.X + other.Width &&
               m_rectangle.X + m_rectangle.Width > other.X &&
               m_rectangle.Y < other.Y + other.Height &&
               m_rectangle.Y + m_rectangle.Height > other.Y;
        }

        public void Release()
        {
            m_state = RodState.Released;
        }

        public void SetTexture(Texture2D texture)
        {
            m_texture = texture;
        }
        public void Update(GameTime gameTime)
        {
            if (m_state == RodState.Released) 
            {
                m_verticalMomentum += m_gravity;
                m_position.Y += m_verticalMomentum * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                m_rectangle = new Rectangle((int)m_position.X, (int)m_position.Y, m_width, m_height);
            }

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(m_texture, m_rectangle, Color.White);
            spriteBatch.End();
        }

        
    }

    internal class DroppingHand
    {
        Rectangle m_rectangle;
        bool m_isClosed;
        int m_width;
        int m_height;
        Vector2 m_position;

        Rod m_rod;
        Timer m_releaseTimer;

        Texture2D m_openTexture;
        Texture2D m_closedTexture;
        public DroppingHand(Vector2 position, int width, int height, Rod rod) 
        {
            m_isClosed = true;
            m_width = width;
            m_height = height;
            m_position = position;
            m_rectangle = new Rectangle((int)m_position.X, (int)m_position.Y, m_width, m_height);
            m_rod = rod;
            Random rand = new Random();
            m_releaseTimer = new Timer(rand.Next(500, 2501));
        }

        public void SetClosedTexture(Texture2D texture)
        {
            m_closedTexture = texture;
        }
        public void SetOpenTexture(Texture2D texture)
        {
            m_openTexture = texture;
        }
        public void Open()
        {
            m_isClosed = false;
            m_rod.Release();
        }

        public void Update(GameTime gameTime)
        {
            if (m_isClosed)
            {
                m_releaseTimer.Update(gameTime);
                if (m_releaseTimer.HasExpired())
                {
                    Open();
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch) 
        {
            spriteBatch.Begin();
            if (m_isClosed)
            {
                spriteBatch.Draw(m_closedTexture, m_rectangle, Color.White);
            }
            else
            {
                spriteBatch.Draw(m_openTexture, m_rectangle, Color.White);
            }
            spriteBatch.End();
        }
    }
    internal class PlayerHand
    {
        Rectangle m_rectangle;
        bool m_isClosed;
        int m_width;
        int m_height;
        Vector2 m_position;

        Texture2D m_openTexture;
        Texture2D m_closedTexture;
        
        public PlayerHand(Vector2 position, int width, int height) 
        {
            m_isClosed = false;
            m_width = width;
            m_height = height;
            m_position = position;
            m_rectangle = new Rectangle((int)m_position.X, (int)m_position.Y, m_width, m_height);
        }

        public bool IsClosed() { return m_isClosed; }

        public void SetClosedTexture(Texture2D texture)
        {
            m_closedTexture = texture;
        }
        public void SetOpenTexture(Texture2D texture)
        {
            m_openTexture = texture;
        }

        public void Close(GameTime gameTime, float value)
        {
            m_isClosed = true;
        }

        public Rectangle GetRectangle() { return m_rectangle; }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            if (m_isClosed)
            {
                spriteBatch.Draw(m_closedTexture, m_rectangle, Color.White);
            }
            else
            {
                spriteBatch.Draw(m_openTexture, m_rectangle, Color.White);
            }
            spriteBatch.End();
        }


    }
}
