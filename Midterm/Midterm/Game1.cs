using Midterm;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Midterm
{

    public enum GameStateEnum
    {
        Menu,
        Level1,
        Exit,
    }

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Dictionary<GameStateEnum, IGameState> m_stateDict;
        private GameStateEnum m_currentState, m_nextState;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // If no controls exist, create new controls
            ControlsPersister controlsPersister = new ControlsPersister("gameControls.json");
            controlsPersister.Load();
            var controls = controlsPersister.GetControls();
            if (controls == null)
            {
                controls = new Controls(new Dictionary<string, Keys>() 
                    {
                        { "Control 1", Keys.Up},
                        { "Control 2", Keys.Down},
                    });
                controlsPersister.Save(controls);
            }

            Menu menu = new Menu(GameStateEnum.Menu, new Dictionary<MenuStateEnum, GameStateEnum>() { { MenuStateEnum.NewGame, GameStateEnum.Level1 }, { MenuStateEnum.Exit, GameStateEnum.Exit } } );

            m_stateDict = new Dictionary<GameStateEnum, IGameState>()
            {
                { GameStateEnum.Menu, menu },
                { GameStateEnum.Level1, new Level1(GameStateEnum.Level1) },
            };
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 950;
            _graphics.ApplyChanges();

            // Initialize all states and set initial state
            m_currentState = GameStateEnum.Menu;
            m_nextState = m_currentState;
            foreach (IGameState state in m_stateDict.Values)
            {
                state.Initialize(GraphicsDevice, _graphics);
            }


            base.Initialize();
        }

        protected override void LoadContent()
        {
            //_spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load Content for all States
            foreach (IGameState state in m_stateDict.Values)
            {
                state.LoadContent(Content);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            // Run update for current game state, get next game state
            m_nextState = m_stateDict[m_currentState].Update(gameTime);
            // If next state is exit, quit
            if (m_nextState == GameStateEnum.Exit)
            {
                this.Exit();
            }
            // Conduct state change
            if (m_currentState != m_nextState && m_nextState != GameStateEnum.Exit)
            {
                m_stateDict[m_nextState].Initialize(GraphicsDevice, _graphics);
                m_stateDict[m_nextState].LoadContent(Content);
                m_currentState = m_nextState;
            }


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            m_stateDict[m_currentState].Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}