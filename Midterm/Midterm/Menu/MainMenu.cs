using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Midterm
{
    internal class MainMenu : MenuStateView
    {
        protected string m_title;
        protected (string, MenuStateEnum)[] m_menuArray;
        protected int m_selectedIndex;
        private Timer m_inputDelayTimer;
        private MenuStateEnum m_prevState;
        public MainMenu(MenuStateEnum myState, MenuStateEnum prevState, string title, (string, MenuStateEnum)[] menuArray, Color titleColor, Color menuColor, Color selectedColor) : base(myState, titleColor, menuColor,selectedColor)
        {
            m_prevState = prevState;
            m_title = title;
            m_menuArray = menuArray;
            m_selectedIndex = 0;
        }

        public override void Initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics)
        {
            base.Initialize(graphicsDevice, graphics);
            m_inputDelayTimer = new Timer(500);
        }
        public override void RegisterCommands()
        {
            m_keyboard.registerCommand(Keys.Up, true, MenuUp);
            m_keyboard.registerCommand(Keys.Down, true, MenuDown);
            m_keyboard.registerCommand(Keys.Enter, true, MenuSelect);
            m_keyboard.registerCommand(Keys.Escape, true, (GameTime gameTime, float value) => { m_nextState = m_prevState; });
        }

        #region Input Handler Functions
        private void MenuUp(GameTime gameTime, float value)
        {
            m_selectedIndex--;
            if (m_selectedIndex < 0) m_selectedIndex = m_menuArray.Length - 1;
        }

        private void MenuDown(GameTime gameTime, float value)
        {
            m_selectedIndex++;
            if (m_selectedIndex >= m_menuArray.Length) m_selectedIndex = 0;
        }

        protected virtual void MenuSelect(GameTime gameTime, float value)
        {
            m_nextState = m_menuArray[m_selectedIndex].Item2;
        }
        #endregion
        public override void ProcessInput(GameTime gameTime)
        {
            m_keyboard.Update(gameTime);
        }
        public override MenuStateEnum Update(GameTime gameTime)
        {
            m_nextState = m_myState;
            if (m_inputDelayTimer.HasExpired()) this.ProcessInput(gameTime);
            else m_inputDelayTimer.Update(gameTime);
            return m_nextState;
        }

        public override void Draw(GameTime gameTime)
        {
            m_spriteBatch.Begin();

            // Render background
            m_spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, m_graphics.PreferredBackBufferWidth, m_graphics.PreferredBackBufferHeight), null, Color.White, 0, new Vector2(), SpriteEffects.None, 0);

            // Render backdrop for text
            m_spriteBatch.Draw(rectangleTexture, new Rectangle(m_graphics.PreferredBackBufferWidth / 4, 100, m_graphics.PreferredBackBufferWidth / 2, m_graphics.PreferredBackBufferHeight - 200), new Color(Color.Black, 0.5f));

            // Render text
            m_spriteBatch.DrawString(roboto, m_title, new Vector2(m_graphics.PreferredBackBufferWidth / 2 - 100, 100), m_titleColor, 0f, new Vector2(), 2f, SpriteEffects.None, 0);
            for (int i = 0; i < m_menuArray.Length; i++)
            {
                Color textColor = m_menuColor;
                if (i == m_selectedIndex) textColor = m_selectedColor;
                m_spriteBatch.DrawString(roboto, m_menuArray[i].Item1, new Vector2(m_graphics.PreferredBackBufferWidth / 2 - 100, 150 + i * 50), textColor);
            }

            m_spriteBatch.End();
        }
    }
}
