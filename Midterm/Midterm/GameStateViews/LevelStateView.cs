using Lunar_Lander;
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
    public enum WinState
    {
        None,
        Won,
        Lost,
        Paused
    }
    internal abstract class LevelStateView : IGameState
    {

        protected GraphicsDeviceManager m_graphics;
        protected SpriteBatch m_spriteBatch;
        protected GameStateEnum m_myState, m_nextState;
        protected KeyboardInput m_keyboard;
        protected ControlsSubscriber m_controlsSubscriber;
        protected Controls m_controls;
        protected WinState m_winState;

        private Pause pause;

        public delegate GameStateEnum UpdateFunction(GameTime gameTime);
        protected UpdateFunction m_updateFunction;
        public delegate void DrawFunction(GameTime gameTime);
        protected DrawFunction m_drawFunction;

        protected List<Score> m_highScores;

        protected Texture2D rectangleTexture;
        protected Texture2D backgroundTexture;
        protected SpriteFont roboto;

        protected LevelStateView(GameStateEnum myState)
        {
            m_myState = myState;
            m_nextState = myState;
        }

        public virtual void Initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics)
        {
            m_keyboard = new KeyboardInput();
            m_graphics = graphics;
            m_spriteBatch = new SpriteBatch(graphicsDevice);
            m_controlsSubscriber = new ControlsSubscriber("gameControls.json");
            m_controlsSubscriber.Load();
            m_controls = m_controlsSubscriber.GetControls();
            m_winState = WinState.None;

            m_updateFunction = MainUpdate;
            m_drawFunction = MainDraw;
            pause = new Pause(m_controls, GameStateEnum.Level1, m_keyboard, m_graphics, m_spriteBatch);
            this.RegisterCommands();
        }
        public virtual void RegisterCommands()
        {
            m_keyboard.registerCommand(Keys.Escape, true, pause.togglePaused);
        }

        protected void Nothing(GameTime gameTime, float value) { }
        public virtual void LoadContent(ContentManager contentManager)
        {
            roboto = contentManager.Load<SpriteFont>("roboto");
            rectangleTexture = contentManager.Load<Texture2D>("whiteRectangle");
            backgroundTexture = contentManager.Load<Texture2D>("background");
            pause.LoadContent(contentManager);
        }
        public virtual void ProcessInput(GameTime gameTime)
        {
            m_keyboard.Update(gameTime);
        }

        public abstract WinState CheckWinState(GameTime gameTime);
        public virtual GameStateEnum Update(GameTime gameTime)
        {
            if (pause.IsPaused)
            {
                m_nextState = pause.Update(gameTime);
                pause.RegisterCommands();
            }
            else
            {
                ProcessInput(gameTime);
                m_nextState = m_updateFunction(gameTime);
                if (m_winState == WinState.Won)
                {
                    m_updateFunction = WonUpdate;
                    m_drawFunction = WonDraw;
                }
                else if (m_winState == WinState.Lost)
                {
                    m_updateFunction = LostUpdate;
                    m_drawFunction = LostDraw;
                }
                else
                {
                    m_updateFunction = MainUpdate;
                    m_drawFunction = MainDraw;
                    m_winState = CheckWinState(gameTime);
                }
            }
            return m_nextState;
        }

        #region Update Functions
        public abstract GameStateEnum MainUpdate(GameTime gameTime);
        public abstract GameStateEnum WonUpdate(GameTime gameTime);
        public abstract GameStateEnum LostUpdate(GameTime gameTime);

        #endregion
        public virtual void Draw(GameTime gameTime)
        {
            // Render background
            m_spriteBatch.Begin();
            m_spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, m_graphics.PreferredBackBufferWidth, m_graphics.PreferredBackBufferHeight), null, Color.White, 0, new Vector2(), SpriteEffects.None, 0);
            m_spriteBatch.End();

            // Call draw function
            if (pause.IsPaused)
            {
                pause.Draw(gameTime);
            }
            else
            {
                m_drawFunction(gameTime);
            }
        }

        #region Draw Functions
        public abstract void MainDraw(GameTime gameTime);
        public abstract void WonDraw(GameTime gameTime);
        public abstract void LostDraw(GameTime gameTime);

        #endregion

    }
}
