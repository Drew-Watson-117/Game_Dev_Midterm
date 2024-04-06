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
    internal class Pause
    {
        private Controls m_oldControls;
        private (string, GameStateEnum)[] m_menuArray;
        private int m_selectedIndex = 0;
        private GameStateEnum m_levelState;
        private GameStateEnum m_nextGameState;
        private bool isPaused;

        private Color m_titleColor = Color.White;
        private Color m_textColor = Color.DarkGray;
        private Color m_selectedColor = Color.LightGray;

        private KeyboardInput m_keyboard;
        private KeyboardInput m_oldKeyboard;
        private SpriteBatch m_spriteBatch;
        private GraphicsDeviceManager m_graphics;

        private Texture2D rectangleTexture;
        private SpriteFont roboto;

        public Pause(Controls controls, GameStateEnum levelState, KeyboardInput keyboard, GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
        {
            m_oldControls = controls;
            m_levelState = levelState;
            m_nextGameState = levelState;

            m_keyboard = keyboard;
            m_oldKeyboard = keyboard;
            m_graphics = graphics;
            m_spriteBatch = spriteBatch;

            isPaused = false;
            m_menuArray = new (string, GameStateEnum)[]{
                ("Continue", levelState),
                ("Main Menu -- Progress Will Be Lost", GameStateEnum.Menu),
                ("Quit", GameStateEnum.Exit)
            };
        }
        public Pause(Controls controls, GameStateEnum levelState,  Color titleColor, Color textColor, Color selectedColor, KeyboardInput keyboard, GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
        {
            m_oldControls = controls;
            m_levelState = levelState;
            m_nextGameState = levelState;

            m_titleColor = titleColor;
            m_textColor = textColor;
            m_selectedColor = selectedColor;

            m_keyboard = keyboard;
            m_oldKeyboard = keyboard;
            m_graphics = graphics;
            m_spriteBatch = spriteBatch;
      
            isPaused = false;
            m_menuArray = new (string, GameStateEnum)[]{
                ("Continue", levelState),
                ("Main Menu -- Progress Will Be Lost", GameStateEnum.Menu),
                ("Quit", GameStateEnum.Exit)
            };
        }

        private void Nothing(GameTime gameTime, float value) { }

        public void LoadContent(ContentManager contentManager)
        {
            roboto = contentManager.Load<SpriteFont>("roboto");
            rectangleTexture = contentManager.Load<Texture2D>("whiteRectangle");
        }
        public void RegisterCommands()
        {
            foreach (Keys key in m_oldControls.ControlsDict.Values)
            {
                m_keyboard.registerCommand(key, true, Nothing);
            }
            m_keyboard.registerCommand(Keys.Up, true, MenuUp);
            m_keyboard.registerCommand(Keys.Down, true, MenuDown);
            m_keyboard.registerCommand(Keys.Enter, true, MenuSelect);
        }

        private void ReregisterOldCommands()
        {
            m_keyboard = m_oldKeyboard;
        }

        #region Pause Input Handlers
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

        private void MenuSelect(GameTime gameTime, float value)
        {
            GameStateEnum state = m_menuArray[m_selectedIndex].Item2;
            if (state == m_levelState)
            {
                // Exit Pause
                isPaused = false;
            }
            else { m_nextGameState = state; }
        }
        #endregion
        public void ProcessInput(GameTime gameTime)
        {
            m_keyboard.Update(gameTime);
        }
        public GameStateEnum Update(GameTime gameTime)
        {
            m_nextGameState = m_levelState;
            ProcessInput(gameTime);
            // If game is unpaused, reregister control inputs
            if (!isPaused)
            {
                this.ReregisterOldCommands();
            }
            return m_nextGameState;
        }
        public void Draw(GameTime gameTime)
        {

            // Draw Menu in front of terrain
            m_spriteBatch.Begin();
            m_spriteBatch.Draw(rectangleTexture, new Rectangle(0, 0, m_graphics.PreferredBackBufferWidth, m_graphics.PreferredBackBufferHeight), new Color(Color.Black, 0.5f));
            m_spriteBatch.DrawString(roboto, "Game Paused", new Vector2(m_graphics.PreferredBackBufferWidth / 2 - 100, 100), m_titleColor, 0f, new Vector2(), 2f, SpriteEffects.None, 0);
            for (int i = 0; i < m_menuArray.Length; i++)
            {
                Color textColor = m_textColor;
                if (i == m_selectedIndex) textColor = m_selectedColor;
                m_spriteBatch.DrawString(roboto, m_menuArray[i].Item1, new Vector2(m_graphics.PreferredBackBufferWidth / 2 - 100, 150 + i * 50), textColor);
            }
            m_spriteBatch.End();
        }

        public void togglePaused(GameTime gameTime, float value)
        {
            isPaused = !isPaused;
        }
        public bool IsPaused { get { return isPaused; } }
    }
}
