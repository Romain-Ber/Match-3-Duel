using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Match_3_Duel
{
    public class Gem
    {
        public int gemX;
        public int gemY;
        public Vector2 gemPos;
        public int gemtype;
        public bool matched;
        private Random rnd;
        public Gem(int pX, int pY)
        {
            gemX = pX;
            gemY = pY;
            gemPos = new Vector2(gemX, gemY);
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
        Gem[,] gemGrid;
        private bool extraMove;
        private MouseState lastState;
        private int mouseStartX;
        private int mouseStartY;
        private int mouseDeltaX;
        private int mouseDeltaY;
        private int gemRow;
        private int gemCol;
        private bool gemSelected;
        private string moveGem;

        private bool gridFilled;
        public SpriteFont arialFont;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            tileWidth = 179;
            tileHeight = 170;
            gridX = (GraphicsDevice.Viewport.Width - 7 * tileWidth) / 2;
            gridY = (GraphicsDevice.Viewport.Height - 5 * tileHeight) / 2;

            gemGrid = new Gem[7, 5];
            extraMove = false;

            gemSelected = false;
            moveGem = "";

            gridFilled = false;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            Texture2D gemTileset = Content.Load<Texture2D>("gemTileset");
            arialFont = Content.Load<SpriteFont>("arial");

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
            if (gridFilled == false)
            {
                FillGrid();
                gridFilled = true;
            }
            //do
            //{
            //    FillGrid();
            //    MatchGems();
            //    RemoveMatchedGems();
            //    GemFall();
            //    ResetGemState();
            //}
            //while(EmptySpaces());
            MouseClicks();
            
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
                        gemGrid[i,j] = new Gem(i * tileWidth + gridX, j * tileHeight + gridY);
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
            if (newState.LeftButton == ButtonState.Pressed && lastState.LeftButton == ButtonState.Released)
            {
                mouseStartX = newState.X;
                mouseStartY = newState.Y;
                if (mouseStartX > gridX &&
                    mouseStartX < GraphicsDevice.Viewport.Width - gridX &&
                    mouseStartY > gridY &&
                    mouseStartY < GraphicsDevice.Viewport.Height - gridY)
                {
                    gemRow = (int)((mouseStartX - gridX) / tileWidth);
                    gemCol = (int)((mouseStartY - gridY) / tileHeight);
                    gemSelected = true;
                }
            }
            if (newState.LeftButton == ButtonState.Pressed && gemSelected == true)
            {
                gemGrid[gemRow, gemCol].gemPos = new Vector2(gemGrid[gemRow, gemCol].gemX, gemGrid[gemRow, gemCol].gemY);
                mouseDeltaX = newState.X;
                mouseDeltaY = newState.Y;
                //toward the right
                if (mouseDeltaX > gemGrid[gemRow, gemCol].gemX &&
                    mouseDeltaY > gemGrid[gemRow, gemCol].gemY &&
                    mouseDeltaY < gemGrid[gemRow, gemCol].gemY + tileHeight &&
                    gemRow < 6)
                {
                    if (mouseDeltaX - mouseStartX < tileWidth / 3)
                    {
                        gemGrid[gemRow, gemCol].gemPos = new Vector2(gemGrid[gemRow, gemCol].gemX + tileWidth, gemGrid[gemRow, gemCol].gemY);
                        gemGrid[gemRow + 1, gemCol].gemPos = new Vector2(gemGrid[gemRow + 1, gemCol].gemX - tileWidth, gemGrid[gemRow + 1, gemCol].gemY);
                        moveGem = "RIGHT";
                    }
                    else
                    {
                        gemGrid[gemRow, gemCol].gemPos = new Vector2(gemGrid[gemRow, gemCol].gemX, gemGrid[gemRow, gemCol].gemY);
                        gemGrid[gemRow + 1, gemCol].gemPos = new Vector2(gemGrid[gemRow + 1, gemCol].gemX, gemGrid[gemRow + 1, gemCol].gemY);
                        moveGem = "";
                    }
                }
                //toward the left
                if (mouseDeltaX < gemGrid[gemRow, gemCol].gemX &&
                    mouseDeltaY > gemGrid[gemRow, gemCol].gemY &&
                    mouseDeltaY < gemGrid[gemRow, gemCol].gemY + tileHeight &&
                    gemRow > 0)
                {
                    if (mouseStartX - mouseDeltaX < tileWidth / 3)
                    {
                        gemGrid[gemRow, gemCol].gemPos = new Vector2(gemGrid[gemRow, gemCol].gemX - tileWidth, gemGrid[gemRow, gemCol].gemY);
                        gemGrid[gemRow - 1, gemCol].gemPos = new Vector2(gemGrid[gemRow + 1, gemCol].gemX + tileWidth, gemGrid[gemRow + 1, gemCol].gemY);
                        moveGem = "LEFT";
                    }
                    else
                    {
                        gemGrid[gemRow, gemCol].gemPos = new Vector2(gemGrid[gemRow, gemCol].gemX, gemGrid[gemRow, gemCol].gemY);
                        gemGrid[gemRow - 1, gemCol].gemPos = new Vector2(gemGrid[gemRow - 1, gemCol].gemX, gemGrid[gemRow - 1, gemCol].gemY);
                        moveGem = "";
                    }
                }
                //downward
                if (mouseDeltaY > gemGrid[gemRow, gemCol].gemY &&
                    mouseDeltaX > gemGrid[gemRow, gemCol].gemX &&
                    mouseDeltaX < gemGrid[gemRow, gemCol].gemX + tileWidth &&
                    gemCol < 4)
                {
                    if (mouseDeltaY - mouseStartY < tileHeight / 3)
                    {
                        gemGrid[gemRow, gemCol].gemPos = new Vector2(gemGrid[gemRow, gemCol].gemX, gemGrid[gemRow, gemCol].gemY + tileHeight);
                        gemGrid[gemRow, gemCol + 1].gemPos = new Vector2(gemGrid[gemRow, gemCol + 1].gemX, gemGrid[gemRow, gemCol + 1].gemY - tileHeight);
                        moveGem = "DOWN";
                    }
                    else
                    {
                        gemGrid[gemRow, gemCol].gemPos = new Vector2(gemGrid[gemRow, gemCol].gemX, gemGrid[gemRow, gemCol].gemY);
                        gemGrid[gemRow, gemCol + 1].gemPos = new Vector2(gemGrid[gemRow, gemCol + 1].gemX, gemGrid[gemRow, gemCol + 1].gemY);
                        moveGem = "";
                    }
                }
                //upward
                if (mouseDeltaY < gemGrid[gemRow, gemCol].gemY &&
                    mouseDeltaX > gemGrid[gemRow, gemCol].gemX &&
                    mouseDeltaX < gemGrid[gemRow, gemCol].gemX + tileWidth &&
                    gemCol > 0)
                {
                    if (mouseStartY - mouseDeltaY < tileHeight / 3)
                    {
                        gemGrid[gemRow, gemCol].gemPos = new Vector2(gemGrid[gemRow, gemCol].gemX, gemGrid[gemRow, gemCol].gemY - tileHeight);
                        gemGrid[gemRow, gemCol - 1].gemPos = new Vector2(gemGrid[gemRow, gemCol - 1].gemX, gemGrid[gemRow, gemCol - 1].gemY + tileHeight);
                        moveGem = "UP";
                    }
                    else
                    {
                        gemGrid[gemRow, gemCol].gemPos = new Vector2(gemGrid[gemRow, gemCol].gemX, gemGrid[gemRow, gemCol].gemY);
                        gemGrid[gemRow, gemCol - 1].gemPos = new Vector2(gemGrid[gemRow, gemCol - 1].gemX, gemGrid[gemRow, gemCol - 1].gemY);
                        moveGem = "";
                    }
                }
            }

            if (newState.LeftButton == ButtonState.Released && gemSelected == true && moveGem != "")
            {
                Gem oGem = gemGrid[gemRow, gemCol];
                switch (moveGem)
                {
                    case "RIGHT":
                        gemGrid[gemRow, gemCol] = gemGrid[gemRow + 1, gemCol];
                        gemGrid[gemRow + 1, gemCol] = oGem;
                        break;
                    case "LEFT":
                        gemGrid[gemRow, gemCol] = gemGrid[gemRow - 1, gemCol];
                        gemGrid[gemRow - 1, gemCol] = oGem;
                        break;
                    case "DOWN":
                        gemGrid[gemRow, gemCol] = gemGrid[gemRow, gemCol + 1];
                        gemGrid[gemRow, gemCol + 1] = oGem;
                        break;
                    case "UP":
                        gemGrid[gemRow, gemCol] = gemGrid[gemRow, gemCol - 1];
                        gemGrid[gemRow, gemCol - 1] = oGem;
                        break;
                    default:
                        break;
                }
                moveGem = "";
                gemSelected = false;
                gemRow = -1;
                gemCol = -1;
            }
            lastState = newState;

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
                    int type = gemGrid[i, j].gemtype;
                    _spriteBatch.Draw(gemTiles[type - 1], gemGrid[i,j].gemPos, Color.White);

                    string coords = gemGrid[i, j].gemX.ToString() + "," + gemGrid[i, j].gemY.ToString();
                    _spriteBatch.DrawString(arialFont, coords, gemGrid[i, j].gemPos, Color.White);
                    MouseState mouseState = Mouse.GetState();
                    string mouseCoords = "current mous coord: " + mouseState.X.ToString() + "," + mouseState.Y.ToString();
                    _spriteBatch.DrawString(arialFont, mouseCoords, new Vector2(0,0), Color.White);
                    string gemCoord = "mouse hovering gem [x,y]: " + ((lastState.X - gridX)/tileWidth).ToString() + "," + ((lastState.Y - gridY)/tileHeight).ToString();
                    _spriteBatch.DrawString(arialFont, gemCoord, new Vector2(0, 25), Color.White);
                    string gem2Coord = "selected gem [x,y]: " + gemRow.ToString() + "," + gemCol.ToString();
                    _spriteBatch.DrawString(arialFont, gem2Coord, new Vector2(0,50), Color.White);
                }
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}