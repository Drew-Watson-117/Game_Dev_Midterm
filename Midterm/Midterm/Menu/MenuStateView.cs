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
    public abstract class MenuStateView : IMenuState
    {
        protected GraphicsDeviceManager m_graphics;
        protected SpriteBatch m_spriteBatch;
        protected MenuStateEnum m_myState, m_nextState;
        protected KeyboardInput m_keyboard;

        protected Texture2D rectangleTexture;
        protected Texture2D backgroundTexture;
        protected SpriteFont roboto;
        protected Color m_titleColor;
        protected Color m_menuColor;
        protected Color m_selectedColor;
        private MenuStateEnum myState;

        public MenuStateView(MenuStateEnum myState, Color titleColor, Color menuColor, Color selectedColor)
        {
            m_myState = myState;
            m_nextState = myState;
            m_titleColor = titleColor;
            m_menuColor = menuColor;
            m_selectedColor = selectedColor;
        }

        public virtual void Initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics)
        {
            m_keyboard = new KeyboardInput();
            m_graphics = graphics;
            m_spriteBatch = new SpriteBatch(graphicsDevice);
            RegisterCommands();
        }
        public abstract void RegisterCommands();

        public virtual void ReregisterCommands(Keys[] newKeys)
        {

        }

        protected void Nothing(GameTime gameTime, float value) { }
        public virtual void LoadContent(ContentManager contentManager)
        {
            roboto = contentManager.Load<SpriteFont>("roboto");
            rectangleTexture = contentManager.Load<Texture2D>("whiteRectangle");
            backgroundTexture = contentManager.Load<Texture2D>("background");
        }
        public abstract void ProcessInput(GameTime gameTime);
        public abstract MenuStateEnum Update(GameTime gameTime);
        public abstract void Draw(GameTime gameTime);
    }
}
