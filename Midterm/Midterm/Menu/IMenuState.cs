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
    public interface IMenuState
    {
        void Initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics);
        void RegisterCommands();
        void LoadContent(ContentManager contentManager);
        void ProcessInput(GameTime gameTime);
        MenuStateEnum Update(GameTime gameTime);
        void Draw(GameTime gameTime);
    }
}
