using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Match_3_Duel
{
    public class Gem
    {
        public int gemtype;
        public bool matched;
        private Random rnd;
        public Gem()
        {
            matched = false;
            rnd = new Random();
            gemtype = rnd.Next(1, 7);
            if (gemtype == 6)
            {
                gemtype = rnd.Next(0, 3);
                if (gemtype == 0)
                {
                    gemtype = 6;
                }
                else
                {
                    gemtype = rnd.Next(1, 6);
                }
            }
        }
    }
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private int tileWidth;
        private int tileHeight;
        private int gridX;
        private int gridY;
        public Texture2D[] gemTiles = new Texture2D[6];
        MouseState lastState;
        Gem[,] gemGrid;
        bool extraMove;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();

            tileWidth = 179;
            tileHeight = 170;
            gridX = (GraphicsDevice.Viewport.Width - 7 * tileWidth) / 2;
            gridY = (GraphicsDevice.Viewport.Height - 5 * tileHeight) / 2;

            gemGrid = new Gem[7, 5];
            extraMove = false;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            Texture2D gemTileset = Content.Load<Texture2D>("gemTileset");

            for (int i = 0; i < 6; i++)
            {
                int startX = i * tileWidth;
                Rectangle sourceRectangle = new Rectangle(startX, 0, tileWidth, tileHeight);
                Texture2D texture = new Texture2D(GraphicsDevice, sourceRectangle.Width, sourceRectangle.Height);
                Color[] data = new Color[sourceRectangle.Width * sourceRectangle.Height];
                gemTileset.GetData(0, sourceRectangle, data, 0, data.Length);
                texture.SetData(data);
                gemTiles[i] = texture;
            }
        }
        
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            do
            {
                FillGrid();
                MatchGems();
                RemoveMatchedGems();
                GemFall();
                ResetGemState();
            }
            while(EmptySpaces());
            
            base.Update(gameTime);
        }

        protected void FillGrid()
        {
            for (int i = 0; i < 7; i++)
            {
                for (int j = 4; j >= 0; j--)
                {
                    if (gemGrid[i,j] == null)
                    {
                        gemGrid[i,j] = new Gem();
                    }
                }
            }
        }
        protected void MatchGems()
        {
            //checks vertical 3-match, only need to check up to the 3rd row for matching logic, top to bottom
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (gemGrid[i, j] == gemGrid[i, j + 1] &&
                        gemGrid[i, j] == gemGrid[i, j + 2])
                    {
                        if (gemGrid[i, j].matched == true ||
                            gemGrid[i, j + 1].matched == true ||
                            gemGrid[i, j + 2].matched == true)
                        {
                            extraMove = true;
                        }
                        gemGrid[i, j].matched = true;
                        gemGrid[i, j + 1].matched = true;
                        gemGrid[i, j + 2].matched = true;
                    }
                }
            }
            //checks horizontal 3-match, only need to check up to the 5th column for matching logic, left to right
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (gemGrid[i, j] == gemGrid[i + 1, j] &&
                        gemGrid[i, j] == gemGrid[i + 2, j])
                    {
                        if (gemGrid[i, j].matched == true ||
                            gemGrid[i + 1, j].matched == true ||
                            gemGrid[i + 2, j].matched == true)
                        {
                            extraMove = true;
                        }
                        gemGrid[i, j].matched = true;
                        gemGrid[i + 1, j].matched = true;
                        gemGrid[i + 2, j].matched = true;
                    }
                }
            }
        }
        protected void RemoveMatchedGems()
        {
            for (int i = 0; i < 7; i++)
            {
                for (int j = 4; j >= 0; j--)
                {
                    if (gemGrid[i,j] != null)
                    {
                        if (gemGrid[i, j].matched == true)
                        {
                            gemGrid[i, j] = null;
                        }
                    }
                }
            }
        }
        protected void GemFall()
        {
            for (int h = 0; h < 4; h++)
            {
                for (int i = 0; i < 7; i++)
                {
                    //no need to check bottom row
                    for (int j = 3; j >= 0; j--)
                    {
                        //if the block below is empty then move down by 1
                        if (gemGrid[i, j + 1] == null)
                        {
                            gemGrid[i, j + 1] = gemGrid[i, j];
                            gemGrid[i, j] = null;
                        }
                    }
                }
            }
        }
        protected void ResetGemState()
        {
            for (int i = 0;i < 7; i++)
            {
                for (int j=0; j < 5; j++)
                {
                    if (gemGrid[i,j] != null)
                    {
                        gemGrid[i, j].matched = false;
                    }
                }
            }
        }
        protected bool EmptySpaces()
        {
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (gemGrid[i,j] == null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        protected void MouseClicks()
        {
            MouseState newState = Mouse.GetState();
            bool bClick = false;
            if (newState.LeftButton == ButtonState.Pressed && lastState.LeftButton == ButtonState.Released)
            {
                bClick = true;
            }
            lastState = newState;
            bool collideImg = false;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            for (int i =0; i < 7; i++)
            {
                for (int j = 0;j < 5; j++)
                {
                    //if (gemGrid[i, j] != null)
                    //{
                        int type = gemGrid[i, j].gemtype;
                        Vector2 position = new Vector2(i * tileWidth + gridX, j * tileHeight + gridY);
                        _spriteBatch.Draw(gemTiles[type - 1], position, Color.White);
                    //}
                }
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}