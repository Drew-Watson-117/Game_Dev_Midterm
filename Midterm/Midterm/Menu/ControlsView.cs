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
    internal class ControlsView : MenuStateView
    {
        private (string, Keys)[] m_menuArray;
        private int m_selectedIndex;
        private ControlsPersister m_controlsPersister;
        private Controls m_oldControls, m_newControls;

        public delegate MenuStateEnum UpdateFunction(GameTime gameTime);
        UpdateFunction m_updateFunction;
        public delegate void DrawFunction(GameTime gameTime);
        DrawFunction m_drawFunction;

        private Color m_flashingColorPermanent = Color.Black;
        private Color m_flashingColor;

        private Timer m_delayInputTimer;
        private Timer m_flashTimer;
        public ControlsView(MenuStateEnum myState, Color titleColor, Color menuColor, Color selectedColor) : base(myState, titleColor, menuColor, selectedColor)
        {
            m_controlsPersister = new ControlsPersister("realControls.json");
        }

        public override void Initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics)
        {
            m_controlsPersister.Load();
            m_oldControls = m_controlsPersister.GetControls();
            m_newControls = m_oldControls;
            m_menuArray = BuildArray(m_newControls.ControlsDict);
            m_selectedIndex = 0;

            m_updateFunction = MainUpdate;
            m_drawFunction = MainDraw;
            m_nextState = m_myState;
            m_flashingColor = m_menuColor;
            m_delayInputTimer = new Timer(500);

            base.Initialize(graphicsDevice, graphics);
        }

        private (string, Keys)[] BuildArray(Dictionary<string, Keys> dict)
        {
            (string, Keys)[] arr = new (string, Keys)[dict.Count];
            int i = 0;
            foreach (var key in dict.Keys)
            {
                arr[i] = (key, dict[key]);
                ++i;
            }
            return arr;
        }

        public override void RegisterCommands()
        {
            m_keyboard.registerCommand(Keys.Up, true, MenuUp);
            m_keyboard.registerCommand(Keys.Down, true, MenuDown);
            m_keyboard.registerCommand(Keys.Enter, true, MenuSelect);
            m_keyboard.registerCommand(Keys.Escape, true, MenuBack);
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

        private void MenuSelect(GameTime gameTime, float value)
        {
            // Switch update function
            m_updateFunction = RemapUpdate;
            m_drawFunction = RemapDraw;
            m_flashTimer = new Timer(500);
        }

        private void MenuBack(GameTime gameTime, float value)
        {
            m_nextState = MenuStateEnum.MainMenu;
        }
        #endregion



        public override void ProcessInput(GameTime gameTime)
        {
            m_keyboard.Update(gameTime);
        }

        public override MenuStateEnum Update(GameTime gameTime)
        {
            if (m_delayInputTimer.HasExpired())
            {
                return m_updateFunction(gameTime);
            }
            else
            {
                m_delayInputTimer.Update(gameTime);
                return m_myState;
            }
        }

        #region Update Functions
        private MenuStateEnum MainUpdate(GameTime gameTime)
        {
            ProcessInput(gameTime);
            return m_nextState;
        }

        private MenuStateEnum RemapUpdate(GameTime gameTime)
        {
            // Toggle Color of Flashing Text
            m_flashTimer.Update(gameTime);
            if (m_flashTimer.HasExpired())
            {
                if (m_flashingColor == m_menuColor) m_flashingColor = m_flashingColorPermanent;
                else if (m_flashingColor == m_flashingColorPermanent) m_flashingColor = m_menuColor;
                m_flashTimer = new Timer(500);
            }

            Keys[] keys = Keyboard.GetState().GetPressedKeys();
            if (keys.Length > 0)
            {
                Keys key = keys[keys.Length - 1];
                if (key != Keys.Escape && key != Keys.Enter)
                {
                    // Get dictionary key for selected index
                    string keyName = m_menuArray[m_selectedIndex].Item1;
                    // Create a new dictionary
                    Dictionary<string, Keys> newDict = m_newControls.ControlsDict;
                    // Reassign keyboard key of the selected dictionary key
                    newDict[keyName] = key;
                    // Set controls dictionary to the new dictionary
                    m_newControls.ControlsDict = newDict;
                    m_oldControls = m_newControls;
                    m_menuArray = BuildArray(newDict);
                    m_controlsPersister.Save(m_newControls);
                    m_updateFunction = MainUpdate;
                    m_drawFunction = MainDraw;
                }
            }
            return m_nextState;
        }

        #endregion
        public override void Draw(GameTime gameTime)
        {
            m_drawFunction(gameTime);
        }

        #region Draw Functions
        public void MainDraw(GameTime gameTime)
        {
            m_spriteBatch.Begin();

            // Render background
            m_spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, m_graphics.PreferredBackBufferWidth, m_graphics.PreferredBackBufferHeight), null, Color.White, 0, new Vector2(), SpriteEffects.None, 0);

            // Render backdrop for text
            m_spriteBatch.Draw(rectangleTexture, new Rectangle(m_graphics.PreferredBackBufferWidth / 4, 100, m_graphics.PreferredBackBufferWidth / 2, m_graphics.PreferredBackBufferHeight - 200), new Color(Color.Black, 0.5f));

            // Render text
            m_spriteBatch.DrawString(roboto, "Controls", new Vector2(m_graphics.PreferredBackBufferWidth / 2 - 100, 100), m_titleColor, 0f, new Vector2(), 2f, SpriteEffects.None, 0);
            for (int i = 0; i < m_menuArray.Length; i++)
            {
                Color textColor = m_menuColor;
                if (i == m_selectedIndex) textColor = m_selectedColor;
                Keys key = m_menuArray[i].Item2;
                m_spriteBatch.DrawString(roboto, m_menuArray[i].Item1 + ":  " + key.ToString(), new Vector2(m_graphics.PreferredBackBufferWidth / 2 - 100, 150 + i * 50), textColor);
            }

            m_spriteBatch.End();
        }

        public void RemapDraw(GameTime gameTime)
        {
            m_spriteBatch.Begin();

            // Render background
            m_spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, m_graphics.PreferredBackBufferWidth, m_graphics.PreferredBackBufferHeight), null, Color.White, 0, new Vector2(), SpriteEffects.None, 0);

            // Render backdrop for text
            m_spriteBatch.Draw(rectangleTexture, new Rectangle(m_graphics.PreferredBackBufferWidth / 4, 100, m_graphics.PreferredBackBufferWidth / 2, m_graphics.PreferredBackBufferHeight - 200), new Color(Color.Black, 0.5f));

            // Render text
            m_spriteBatch.DrawString(roboto, "Controls", new Vector2(m_graphics.PreferredBackBufferWidth / 2 - 100, 100), m_titleColor, 0f, new Vector2(), 2f, SpriteEffects.None, 0);
            for (int i = 0; i < m_menuArray.Length; i++)
            {
                Color textColor = m_menuColor;
                if (i == m_selectedIndex) textColor = m_flashingColor;
                Keys key = m_menuArray[i].Item2;
                m_spriteBatch.DrawString(roboto, m_menuArray[i].Item1 + ":  " + key.ToString(), new Vector2(m_graphics.PreferredBackBufferWidth / 2 - 100, 150 + i * 50), textColor);
            }

            m_spriteBatch.End();
        }
        #endregion
    }
}
