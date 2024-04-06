using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Midterm
{
    public enum MenuStateEnum
    {
        MainMenu,
        NewGame,
        Controls,
        HighScores,
        Credits,
        Exit
    }
    public class Menu : GameStateView
    {

        private Dictionary<MenuStateEnum, IMenuState> m_stateDict;
        private Dictionary<MenuStateEnum, GameStateEnum> m_menuGameMap;
        protected MenuStateEnum m_currentMenu, m_nextMenu;

        private Color m_titleColor = Color.White;
        private Color m_menuColor = Color.DarkGray;
        private Color m_selectedColor = Color.LightGray;

        private GraphicsDevice m_graphicsDevice;
        private ContentManager m_content;

        protected Texture2D rectangleTexture;
        protected Texture2D backgroundTexture;
        protected SpriteFont roboto;

        public Menu(GameStateEnum myState, Dictionary<MenuStateEnum, GameStateEnum> menuGameMap) : base(myState)
        {
            m_menuGameMap = menuGameMap;
            m_currentMenu = MenuStateEnum.MainMenu;
            (string, MenuStateEnum)[] menuArray = new (string, MenuStateEnum)[]
            {
                ("New Game", MenuStateEnum.NewGame),
                //("Controls", MenuStateEnum.Controls),
                ("High Scores", MenuStateEnum.HighScores),
                ("Credits", MenuStateEnum.Credits),
                ("Exit", MenuStateEnum.Exit)
            };
            m_stateDict = new Dictionary<MenuStateEnum, IMenuState>()
            {
                { MenuStateEnum.MainMenu, new MainMenu(MenuStateEnum.MainMenu, MenuStateEnum.Exit, "Catch!", menuArray, m_titleColor, m_menuColor, m_selectedColor) },
                //{ MenuStateEnum.Controls, new ControlsView(MenuStateEnum.Controls, m_titleColor, m_menuColor, m_selectedColor) },
                { MenuStateEnum.HighScores, new HighScore(MenuStateEnum.HighScores, m_titleColor, m_menuColor) },
                {MenuStateEnum.Credits, new Credits(MenuStateEnum.Credits, m_titleColor, m_menuColor) },
            };
        }

        public Menu(GameStateEnum myState, Dictionary<MenuStateEnum, GameStateEnum> menuGameMap, Color titleColor, Color menuColor, Color selectedColor) : this(myState, menuGameMap)
        {
            m_titleColor = titleColor;
            m_menuColor = menuColor;
            m_selectedColor = selectedColor;
        }
        public override void Initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics)
        {
            m_graphicsDevice = graphicsDevice;
            foreach (IMenuState state in m_stateDict.Values)
            {
                state.Initialize(graphicsDevice, graphics);
            }
            base.Initialize(graphicsDevice, graphics);
        }
        public override void LoadContent(ContentManager contentManager)
        {
            m_content = contentManager;
            foreach (IMenuState state in m_stateDict.Values)
            {
                state.LoadContent(contentManager);
            }
        }
        public override void RegisterCommands()
        {
            // No dedicated commands for the menu wrapper
        }
        public override void ProcessInput(GameTime gameTime)
        {
            // No dedicated input for menu wrapper
        }
        public override GameStateEnum Update(GameTime gameTime)
        {
            m_nextMenu = m_currentMenu;
            m_nextMenu = m_stateDict[m_currentMenu].Update(gameTime);
            // If the menu state is in the menuGameMap, return the value
            if (m_menuGameMap.ContainsKey(m_nextMenu))
            {
                return m_menuGameMap[m_nextMenu];
            }
            // Else stay in the menu
            else
            {
                // Initialize and load new menu state
                if (m_nextMenu != m_currentMenu)
                {
                    m_stateDict[m_nextMenu].Initialize(m_graphicsDevice, m_graphics);
                    m_stateDict[m_nextMenu].LoadContent(m_content);
                    m_currentMenu = m_nextMenu;
                }
                return m_myState;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            m_spriteBatch.Begin();

            m_stateDict[m_currentMenu].Draw(gameTime);

            m_spriteBatch.End();
        }
    }
}
