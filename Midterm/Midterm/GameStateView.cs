using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Midterm
{
    public abstract class GameStateView : IGameState
    {
        protected GraphicsDeviceManager m_graphics;
        protected SpriteBatch m_spriteBatch;
        protected GameStateEnum m_myState, m_nextState;
        protected KeyboardInput m_keyboard;

        public GameStateView(GameStateEnum myState)
        {
            m_myState = myState;
            m_nextState = myState;
        }
        public virtual void Initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics)
        {
            m_keyboard = new KeyboardInput();
            m_graphics = graphics;
            m_spriteBatch = new SpriteBatch(graphicsDevice);
            this.RegisterCommands();
        }
        public abstract void RegisterCommands();

        protected void Nothing(GameTime gameTime, float value) { }
        public abstract void LoadContent(ContentManager contentManager);
        public abstract void ProcessInput(GameTime gameTime);
        public abstract GameStateEnum Update(GameTime gameTime);
        public abstract void Draw(GameTime gameTime);
    }
}
