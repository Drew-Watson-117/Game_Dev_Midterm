using Lunar_Lander;
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
    internal class HighScore : MenuStateView
    {
        private List<Score> m_highScores;
        private ScorePersister m_persister;
        public HighScore(MenuStateEnum myState, Color titleColor, Color menuColor) : base(myState, titleColor, menuColor, Color.White)
        {
            m_persister = new ScorePersister("HighScores.json");
        }

        public override void Initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics)
        {
            m_keyboard = new KeyboardInput();
            m_nextState = m_myState;
            RegisterCommands();
            m_highScores = null;
            m_persister.Load();
            base.Initialize(graphicsDevice, graphics);
        }

        public override void RegisterCommands()
        {
            m_keyboard.registerCommand(Keys.Escape, true, (gameTime, value) => { m_nextState = MenuStateEnum.MainMenu; });
        }


        public override void ProcessInput(GameTime gameTime)
        {
            m_keyboard.Update(gameTime);
        }

        public override MenuStateEnum Update(GameTime gameTime)
        {
            ProcessInput(gameTime);
            m_highScores = m_persister.getHighScores();

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
            m_spriteBatch.DrawString(roboto, "High Scores", new Vector2(m_graphics.PreferredBackBufferWidth / 2 - 100, 100), m_titleColor, 0f, new Vector2(), 2f, SpriteEffects.None, 0);
            if (m_highScores == null)
            {
                m_spriteBatch.DrawString(roboto, "No High Scores to Display", new Vector2(m_graphics.PreferredBackBufferWidth / 2 - 100, 150), m_menuColor);
            }
            else
            {
                for (int i = m_highScores.Count - 1; i >= 0; i--)
                {
                    // TODO: CHANGE THE DISPLAY BASED ON THE CLASS MEMBERS OF SCORE
                    string displayString = " Rank: " + 
                        (m_highScores.Count - i).ToString() + 
                        "\n   Score: " + m_highScores[i].Value + 
                        "\n   Poles Caught: " + m_highScores[i].Poles +
                        "\n   Date: " + m_highScores[i].TimeStamp;
                    float yHeight = roboto.MeasureString(displayString).Y;
                    m_spriteBatch.DrawString(roboto, displayString, new Vector2(m_graphics.PreferredBackBufferWidth / 2 - 100, 160 + (m_highScores.Count - 1 - i) * (yHeight+10)), m_menuColor);
                }
            }

            m_spriteBatch.End();
        }
    }
}
