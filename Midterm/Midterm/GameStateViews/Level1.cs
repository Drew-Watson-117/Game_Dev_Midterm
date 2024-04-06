using Lunar_Lander;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace Midterm
{
    internal class Level1 : LevelStateView
    {

        #region Class Members
        PlayerHand m_playerHand;
        DroppingHand m_droppingHand;

        Rod m_rod;
        int m_rodHeight = 100;
        int m_minHeight;
        float m_rodReduction = 0.9f;
        bool m_rodPlayerCollision;

        int m_handSize = 80;

        Score m_score;
        int m_scoreMultiplier = 2;
        long m_scoreIncrease = 100;
        float m_gravity = 0.004f;

        Timer m_redirectTimer;

        // High score list and persister
        ScorePersister m_persister = new ScorePersister("HighScores.json");

        //Particle System and Renderer
        ParticleSystem m_particleSystem;
        ParticleSystemRenderer m_particleSystemRenderer;

        ContentManager m_content;
        Texture2D m_closedHandTexture, m_openHandTexture, m_rodTexture, m_panelTexture;
        SoundEffect m_catchSound, m_missSound;
        #endregion

        public Level1(GameStateEnum myState) : base(myState)
        {
        }

        public override void Initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics)
        {
            m_persister.Load();
            m_score = new Score(0,0);

            m_minHeight = m_rodHeight / 4;
            m_rod = new Rod(new Vector2(440, 100), m_gravity, 10, m_rodHeight);
            m_droppingHand = new DroppingHand(new Vector2(400, 100), m_handSize, m_handSize, m_rod);
            m_playerHand = new PlayerHand(new Vector2(400, 500), m_handSize, m_handSize);
            m_rodPlayerCollision = false;

            m_particleSystem = new ParticleSystem(new ParticleEffect[]
            {
                new CaughtEffect(m_rod, 10, 5, 0.15f, 0.05f, 1000, 200)
            });
            m_particleSystemRenderer = new ParticleSystemRenderer(m_particleSystem);

            base.Initialize(graphicsDevice, graphics);
        }

        public void ReInitialize()
        {
            m_rod = new Rod(new Vector2(440, 100), m_gravity, 10, m_rodHeight);
            m_droppingHand = new DroppingHand(new Vector2(400, 100), m_handSize, m_handSize, m_rod);
            m_playerHand = new PlayerHand(new Vector2(400, 500), m_handSize, m_handSize);
            m_rodPlayerCollision = false;

            m_particleSystem = new ParticleSystem(new ParticleEffect[]
            {
                new CaughtEffect(m_rod, 10, 5, 0.15f, 0.05f, 1000, 200)
            });
            m_particleSystemRenderer = new ParticleSystemRenderer(m_particleSystem);
            this.LoadContent(m_content);

            this.RegisterCommands();
        }
        public override void LoadContent(ContentManager contentManager)
        {
            m_content = contentManager;
            m_closedHandTexture = contentManager.Load<Texture2D>("closedHand");
            m_openHandTexture = contentManager.Load<Texture2D>("openHand");
            m_rodTexture = contentManager.Load<Texture2D>("rod");
            m_panelTexture = contentManager.Load<Texture2D>("panel");
            m_rod.SetTexture(m_rodTexture);
            m_droppingHand.SetClosedTexture(m_closedHandTexture);
            m_droppingHand.SetOpenTexture(m_openHandTexture);
            m_playerHand.SetClosedTexture(m_closedHandTexture);
            m_playerHand.SetOpenTexture(m_openHandTexture);
            m_particleSystemRenderer.LoadContent(contentManager);
            m_missSound = contentManager.Load<SoundEffect>("miss");
            m_catchSound = contentManager.Load<SoundEffect>("catch");
            base.LoadContent(contentManager);
        }
        public override void RegisterCommands()
        {
            //Dictionary<string, Keys> keysDict = m_controls.ControlsDict;
            //m_keyboard.registerCommand(keysDict["Control 1"], true, Win);
            //m_keyboard.registerCommand(keysDict["Control 2"], true, Lose);
            m_keyboard.registerCommand(Keys.Space, true, m_playerHand.Close);
            base.RegisterCommands();
        }

        public override WinState CheckWinState(GameTime gameTime)
        {
            if (m_rod.GetState() == RodState.Caught)
            {
                m_catchSound.Play();
                m_redirectTimer = new Timer(1000);
                // Increase score;
                m_score = new Score(m_score.Value + m_scoreIncrease, m_score.Poles + 1);
                m_scoreIncrease *= m_scoreMultiplier;
                // Reduce rod size
                m_rodHeight = (int)(m_rodHeight * m_rodReduction) > m_minHeight ? (int)(m_rodHeight * m_rodReduction) : m_minHeight;
                return WinState.Won;
            }
            else if (m_rod.GetState() == RodState.Missed)
            {
                m_missSound.Play();
                m_redirectTimer = new Timer(5000);

                // Write High Score
                m_highScores = m_persister.getHighScores();
                if (m_highScores != null)
                {
                    if (m_highScores.Count >= 5)
                    {
                        for (int i = 0; i < m_highScores.Count; i++)
                        {
                            if (m_score > m_highScores[i])
                            {
                                m_highScores[i] = m_score;
                                break;
                            }
                        }
                    }
                    else
                    {
                        m_highScores.Add(m_score);
                    }
                    m_highScores.Sort();
                }
                else
                {
                    m_highScores = new List<Score>() { m_score };
                }
                m_persister.Save(m_highScores);

                return WinState.Lost;
            }
            else
            {
                return WinState.None;
            }
        }

        #region Update Functions
        public override GameStateEnum MainUpdate(GameTime gameTime)
        {
            m_particleSystem.Update(gameTime);

            m_droppingHand.Update(gameTime);
            m_rod.Update(gameTime);
            if (m_rod.HasCollided(m_playerHand)) { m_rodPlayerCollision = true; }
            // Detecting if the player caught the rod
            if (m_rod.HasCollided(m_playerHand) && m_playerHand.IsClosed())
            {
                m_rod.SetState(RodState.Caught);
            }
            // Detecting if the player missed the rod
            else if (m_playerHand.IsClosed() && !m_rod.HasCollided(m_playerHand) || m_rodPlayerCollision && !m_rod.HasCollided(m_playerHand))
            {
                m_rod.SetState(RodState.Missed);
            }
            return m_myState;
        }
        public override GameStateEnum WonUpdate(GameTime gameTime)
        {
            m_particleSystem.Update(gameTime);

            m_redirectTimer.Update(gameTime);
            if (m_redirectTimer.HasExpired())
            {
                // Create new rod and upper hand, set update function to MainUpdate and draw function to MainDraw

                ReInitialize();

                m_winState = WinState.None;
            }
            return m_myState;
        }
        public override GameStateEnum LostUpdate(GameTime gameTime)
        {
            m_particleSystem.Update(gameTime);

            m_redirectTimer.Update(gameTime);
            if (m_redirectTimer.HasExpired())
            {
                return GameStateEnum.Menu;
            }
            else
            {
                return m_myState;
            }
        }
        #endregion

        #region Draw Functions
        public override void MainDraw(GameTime gameTime)
        {

            m_rod.Draw(m_spriteBatch);
            m_droppingHand.Draw(m_spriteBatch);
            m_playerHand.Draw(m_spriteBatch);
            DrawScore();
            m_particleSystemRenderer.Draw(m_spriteBatch);
        }
        public override void WonDraw(GameTime gameTime)
        {
            m_rod.Draw(m_spriteBatch);
            m_droppingHand.Draw(m_spriteBatch);
            m_playerHand.Draw(m_spriteBatch);
            DrawScore();
            m_particleSystemRenderer.Draw(m_spriteBatch);
        }
        public override void LostDraw(GameTime gameTime)
        {
            m_rod.Draw(m_spriteBatch);
            m_droppingHand.Draw(m_spriteBatch);
            m_playerHand.Draw(m_spriteBatch);
            m_particleSystemRenderer.Draw(m_spriteBatch);

            m_spriteBatch.Begin();
            m_spriteBatch.Draw(rectangleTexture, new Rectangle(0, 0, m_graphics.PreferredBackBufferWidth, m_graphics.PreferredBackBufferHeight), new Color(Color.Black, 0.5f));
            m_spriteBatch.DrawString(roboto, "Game Over -- Press ESC to Open Pause Menu", new Vector2(m_graphics.PreferredBackBufferWidth / 2 - roboto.MeasureString("Game Over -- Press ESC to Open Pause Menu").X / 2, m_graphics.PreferredBackBufferHeight / 2), Color.White);
            m_spriteBatch.DrawString(roboto, "Final Score: " + m_score.Value.ToString(), new Vector2(m_graphics.PreferredBackBufferWidth / 2 - roboto.MeasureString("Final Score: " + m_score.Value.ToString()).X / 2, m_graphics.PreferredBackBufferHeight / 2 + 50), Color.White);
            m_spriteBatch.DrawString(roboto, "Total Poles Caught: " + m_score.Poles.ToString(), new Vector2(m_graphics.PreferredBackBufferWidth / 2 - roboto.MeasureString("Total Poles Caught: " + m_score.Poles.ToString()).X / 2, m_graphics.PreferredBackBufferHeight / 2 + 70), Color.White);

            m_spriteBatch.End();
        }

        public void DrawScore()
        {
            int startX = 0;
            int startY = m_graphics.PreferredBackBufferHeight - 500;
            int width = 300;
            int height = 120;
            m_spriteBatch.Begin();
            m_spriteBatch.Draw(m_panelTexture, new Rectangle(startX, startY, width, height), new Color(Color.Black, 0.9f));
            m_spriteBatch.DrawString(roboto, "Score:", new Vector2(startX + 50, startY + 20), Color.White);
            m_spriteBatch.DrawString(roboto, "Points: " + m_score.Value.ToString(), new Vector2(startX + 50, startY + 60), Color.White);
            m_spriteBatch.DrawString(roboto, "Poles Caught: " + m_score.Poles.ToString(), new Vector2(startX + 50, startY + 80), Color.White);
            m_spriteBatch.End();
        }
        #endregion
    }
}
