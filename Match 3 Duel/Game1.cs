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
        public bool gemFalling;
        private Random rnd;
        public Gem(int pX, int pY)
        {
            gemX = pX;
            gemY = pY;
            gemPos = new Vector2(gemX, gemY);
            matched = false;
            gemFalling = false;
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
        private float speedX;
        private float speedY;
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
            speedX = tileWidth / 2;
            speedY = tileHeight / 2;
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
            //if (gridFilled == false)
            //{
            //    FillGrid();
            //    gridFilled = true;
            //}
            
            //init grid
            if (EmptySpaces())
            {
                FillGrid();
                MatchGems();
                RemoveMatchedGems();
                ResetGemState();
            }

            FillGrid();
            MatchGems();
            RemoveMatchedGems();
            GemFall();
            while (GemFalling())
            {
                MoveGems(gameTime);
            }
            FillGrid();
            ResetGemState();

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
                    if (gemGrid[i, j] != null)
                    {
                        if ((gemGrid[i, j + 1] != null &&
                            gemGrid[i, j + 2] != null) &&
                            gemGrid[i, j].gemtype == gemGrid[i, j + 1].gemtype &&
                            gemGrid[i, j].gemtype == gemGrid[i, j + 2].gemtype)
                        {
                            //if (gemGrid[i, j].matched == true ||
                            //    gemGrid[i, j + 1].matched == true ||
                            //    gemGrid[i, j + 2].matched == true)
                            //{
                            //    extraMove = true;
                            //}
                            gemGrid[i, j].matched = true;
                            gemGrid[i, j + 1].matched = true;
                            gemGrid[i, j + 2].matched = true;
                        }
                    }

                }
            }
            //checks horizontal 3-match, only need to check up to the 5th column for matching logic, left to right
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (gemGrid[i, j] != null)
                    {
                        if ((gemGrid[i + 1, j] != null &&
                            gemGrid[i + 2, j] != null) &&
                            gemGrid[i, j].gemtype == gemGrid[i + 1, j].gemtype &&
                            gemGrid[i, j].gemtype == gemGrid[i + 2, j].gemtype)
                        {
                            //if (gemGrid[i, j].matched == true ||
                            //    gemGrid[i + 1, j].matched == true ||
                            //    gemGrid[i + 2, j].matched == true)
                            //{
                            //    extraMove = true;
                            //}
                            gemGrid[i, j].matched = true;
                            gemGrid[i + 1, j].matched = true;
                            gemGrid[i + 2, j].matched = true;
                        }
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
            for (int i = 0; i < 7; i++)
            {
                //no need to check bottom row
                for (int j = 3; j >= 0; j--)
                {
                    if (gemGrid[i, j] != null)
                    {
                        if (gemGrid[i, j + 1] == null)
                        {
                            gemGrid[i, j].gemFalling = true;
                        }
                        else
                        {
                            gemGrid[i,j].gemFalling = false;
                        }
                    }
                }
            }
        }
        protected bool GemFalling()
        {
            for (int i = 0; i < 7; i++)
            {
                //no need to check bottom row
                for (int j = 0; j < 5; j++)
                {
                    if (gemGrid[i, j] != null)
                    {
                        if (gemGrid[i, j].gemFalling == true)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        protected void MoveGems(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            for (int i = 0; i < 7; i++)
            {
                for (int j = 3; j >= 0; j--)
                {
                    if (gemGrid[i, j] != null && gemGrid[i, j].gemFalling == true)
                    {
                        if (gemGrid[i, j].gemY > (j + 1) * tileHeight + gridY)
                        {
                            gemGrid[i, j].gemY = (j + 1) *  tileHeight + gridY;
                            gemGrid[i, j].gemFalling = false;
                            gemGrid[i, j + 1] = gemGrid[i, j];
                            gemGrid[i, j] = null;
                        }
                        else
                        {
                            int gemMoveY = (int)(speedY * dt);
                            gemGrid[i, j].gemY += gemMoveY;
                            gemGrid[i, j].gemPos = new Vector2(gemGrid[i, j].gemX, gemGrid[i, j].gemY);
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
                        gemGrid[i, j].gemPos = new Vector2(gemGrid[i, j].gemX, gemGrid[i, j].gemY);
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
            //initialize selection variables
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
                    //IsMouseVisible = false;
                    Mouse.SetPosition(gemGrid[gemRow,gemCol].gemX + tileWidth/2, gemGrid[gemRow,gemCol].gemY + tileHeight/2);
                }
            }
            //calculates possible moves
            if (newState.LeftButton == ButtonState.Pressed && gemSelected == true && gemGrid[gemRow, gemCol] != null)
            {
                gemGrid[gemRow, gemCol].gemPos = new Vector2(gemGrid[gemRow, gemCol].gemX, gemGrid[gemRow, gemCol].gemY);
                mouseDeltaX = newState.X;
                mouseDeltaY = newState.Y;
                Vector2 mousePos = new Vector2(mouseDeltaX, mouseDeltaY);
                Vector2 center = new Vector2(gemGrid[gemRow, gemCol].gemX + tileWidth / 2, gemGrid[gemRow, gemCol].gemY + tileHeight / 2);
                Vector2 direction = mousePos - center;
                float mouseAngle = (float)Math.Atan2(direction.Y, direction.X);
                Vector2 topLeft = new Vector2(gemGrid[gemRow, gemCol].gemX, gemGrid[gemRow, gemCol].gemY);
                Vector2 topRight = new Vector2(gemGrid[gemRow, gemCol].gemX + tileWidth, gemGrid[gemRow, gemCol].gemY);
                Vector2 bottomRight = new Vector2(gemGrid[gemRow, gemCol].gemX + tileWidth, gemGrid[gemRow, gemCol].gemY + tileHeight);
                Vector2 bottomLeft = new Vector2(gemGrid[gemRow, gemCol].gemX, gemGrid[gemRow, gemCol].gemY + tileHeight);
                float topLeftAngle = (float)Math.Atan2(topLeft.Y - center.Y, topLeft.X - center.X);
                float topRightAngle = (float)Math.Atan2(topRight.Y - center.Y, topRight.X - center.X);
                float bottomRightAngle = (float)Math.Atan2(bottomRight.Y - center.Y, bottomRight.X - center.X);
                float bottomLeftAngle = (float)Math.Atan2(bottomLeft.Y - center.Y, bottomLeft.X - center.X);
                ////reinitialize positions
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        if (gemGrid[i, j]!= null) gemGrid[i, j].gemPos = new Vector2(gemGrid[i, j].gemX, gemGrid[i, j].gemY);
                    }
                }
                //upward
                if (gemCol != 0 &&
                    mouseAngle > topLeftAngle &&
                    mouseAngle < topRightAngle)
                {
                    if (mouseDeltaY < gemGrid[gemRow, gemCol].gemY && gemGrid[gemRow, gemCol - 1] != null)
                    {
                        gemGrid[gemRow, gemCol].gemPos = new Vector2(gemGrid[gemRow, gemCol].gemX, gemGrid[gemRow, gemCol].gemY - tileHeight);
                        gemGrid[gemRow, gemCol - 1].gemPos = new Vector2(gemGrid[gemRow, gemCol - 1].gemX, gemGrid[gemRow, gemCol - 1].gemY + tileHeight);
                        moveGem = "UP";
                        //SwapGems(gemGrid[gemRow, gemCol].gemPos, gemGrid[gemRow, gemCol - 1].gemPos, gameTime);
                    }
                }
                //right
                if (gemRow != 6 &&
                    mouseAngle > topRightAngle &&
                    mouseAngle < bottomRightAngle)
                {
                    if (mouseDeltaX > gemGrid[gemRow, gemCol].gemX + tileWidth && gemGrid[gemRow + 1, gemCol] != null)
                    {
                        gemGrid[gemRow, gemCol].gemPos = new Vector2(gemGrid[gemRow, gemCol].gemX + tileWidth, gemGrid[gemRow, gemCol].gemY);
                        gemGrid[gemRow + 1, gemCol].gemPos = new Vector2(gemGrid[gemRow + 1, gemCol].gemX - tileWidth, gemGrid[gemRow + 1, gemCol].gemY);
                        moveGem = "RIGHT";
                    }
                }
                //downward
                if (gemCol != 4 &&
                    mouseAngle > bottomRightAngle &&
                    mouseAngle < bottomLeftAngle)
                {
                    if (mouseDeltaY > gemGrid[gemRow, gemCol].gemY + tileHeight && gemGrid[gemRow, gemCol + 1] != null)
                    {
                        gemGrid[gemRow, gemCol].gemPos = new Vector2(gemGrid[gemRow, gemCol].gemX, gemGrid[gemRow, gemCol].gemY + tileHeight);
                        gemGrid[gemRow, gemCol + 1].gemPos = new Vector2(gemGrid[gemRow, gemCol + 1].gemX, gemGrid[gemRow, gemCol + 1].gemY - tileHeight);
                        moveGem = "DOWN";
                        }
                }
                //left
                if (gemRow != 0)
                {
                    if (mouseAngle > bottomLeftAngle ||
                        mouseAngle < topLeftAngle)
                    {
                        if (mouseDeltaX < gemGrid[gemRow, gemCol].gemX && gemGrid[gemRow - 1, gemCol] != null)
                        {
                            gemGrid[gemRow, gemCol].gemPos = new Vector2(gemGrid[gemRow, gemCol].gemX - tileWidth, gemGrid[gemRow, gemCol].gemY);
                            gemGrid[gemRow - 1, gemCol].gemPos = new Vector2(gemGrid[gemRow - 1, gemCol].gemX + tileWidth, gemGrid[gemRow - 1, gemCol].gemY);
                            moveGem = "LEFT";
                        }
                    }
                }
            }
            //change values in gemGrid
            if (newState.LeftButton == ButtonState.Released && gemSelected == true && gemGrid[gemRow, gemCol] != null)
            {
                int gemtype1 = gemGrid[gemRow, gemCol].gemtype;
                int gemtype2;
                switch (moveGem)
                {
                    case "UP":
                        gemtype2 = gemGrid[gemRow, gemCol - 1].gemtype;
                        gemGrid[gemRow, gemCol].gemtype = gemtype2;
                        gemGrid[gemRow, gemCol - 1].gemtype = gemtype1;
                        break;
                    case "RIGHT":
                        gemtype2 = gemGrid[gemRow + 1, gemCol].gemtype;
                        gemGrid[gemRow, gemCol].gemtype = gemtype2;
                        gemGrid[gemRow + 1, gemCol].gemtype = gemtype1;
                        break;
                    case "DOWN":
                        gemtype2 = gemGrid[gemRow, gemCol + 1].gemtype;
                        gemGrid[gemRow, gemCol].gemtype = gemtype2;
                        gemGrid[gemRow, gemCol + 1].gemtype = gemtype1;
                        break;
                    case "LEFT":
                        gemtype2 = gemGrid[gemRow - 1, gemCol].gemtype;
                        gemGrid[gemRow, gemCol].gemtype = gemtype2;
                        gemGrid[gemRow - 1, gemCol].gemtype = gemtype1;
                        break;
                    default:
                        break;
                }
                gemSelected = false;
                //IsMouseVisible = true;
                moveGem = "";
                //update new positions
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        if (gemGrid[i,j] != null) gemGrid[i, j].gemPos = new Vector2(gemGrid[i, j].gemX, gemGrid[i, j].gemY);
                    }
                }
                //Mouse.SetPosition(mouseStartX, mouseStartY);
                //Mouse.SetPosition(gemGrid[gemRow, gemCol].gemX + tileWidth / 2, gemGrid[gemRow, gemCol].gemY + tileHeight / 2);
            }
            lastState = newState;
        }
        protected void SwapGems(Vector2 gem1Pos, Vector2 gem2Pos, GameTime gameTime)
        {
            Vector2 gem1Start = gem1Pos;
            Vector2 gem2Start = gem2Pos;
            Vector2 gem1End = new Vector2(gem2Pos.X, gem2Pos.Y);
            Vector2 gem2End = new Vector2(gem1Pos.X, gem1Pos.Y);
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 gem1Distance = gem1End * deltaTime;
            Vector2 gem2Distance = gem2End * deltaTime;
            gem1Pos += gem1Distance;
            gem2Pos += gem2Distance;
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
                    //_spriteBatch.Draw(gemTiles[type - 1], new Vector2(gemGrid[i, j].gemX, gemGrid[i,j].gemY), Color.White);
                    if (gemGrid[i, j] != null)
                    {
                        int type = gemGrid[i, j].gemtype;
                        _spriteBatch.Draw(gemTiles[type - 1], gemGrid[i, j].gemPos, Color.White);



                        string coords = gemGrid[i, j].gemX.ToString() + "," + gemGrid[i, j].gemY.ToString() + "                 [" + i.ToString() + "][" + j.ToString() + "]";
                        _spriteBatch.DrawString(arialFont, coords, gemGrid[i, j].gemPos, Color.White);
                        string matchState = gemGrid[i, j].matched.ToString();
                        _spriteBatch.DrawString(arialFont, matchState, new Vector2(gemGrid[i, j].gemX, gemGrid[i, j].gemY + 20), Color.White);

                    }

                    HighlightGem();



                    string turnextra = extraMove.ToString();
                    _spriteBatch.DrawString(arialFont, turnextra, new Vector2(0, 0), Color.White);
                    //MouseState mouseState = Mouse.GetState();
                    //string mouseCoords = "current mous coord: " + mouseState.X.ToString() + "," + mouseState.Y.ToString();
                    //_spriteBatch.DrawString(arialFont, mouseCoords, new Vector2(0,0), Color.White);
                    //string gemCoord = "mouse hovering gem [x,y]: " + ((lastState.X - gridX)/tileWidth).ToString() + "," + ((lastState.Y - gridY)/tileHeight).ToString();
                    //_spriteBatch.DrawString(arialFont, gemCoord, new Vector2(0, 20), Color.White);
                    //string gem2Coord = "selected gem [x,y]: " + gemRow.ToString() + "," + gemCol.ToString();
                    //_spriteBatch.DrawString(arialFont, gem2Coord, new Vector2(0,40), Color.White);
                    //string mouseStartCoords = "mouseStart X,Y: " + (mouseStartX).ToString() + "," + (mouseStartY).ToString();
                    //_spriteBatch.DrawString(arialFont, mouseStartCoords, new Vector2(0, 60), Color.White);
                    //string mouseDeltaCoords = "mouseDelta X,Y: " + (mouseDeltaX).ToString() + "," + (mouseDeltaY).ToString();
                    //_spriteBatch.DrawString(arialFont, mouseDeltaCoords, new Vector2(0, 80), Color.White);
                }
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
        protected void HighlightGem()
        {
            if (gemSelected)
            {
                // set the position, size, and transparency of the rectangle
                int rectangleWidth = tileWidth;
                int rectangleHeight = tileHeight;
                float transparency = 0.5f; // 50% transparency
                Rectangle rectangle = new Rectangle(gemGrid[gemRow,gemCol].gemX, gemGrid[gemRow, gemCol].gemY, rectangleWidth, rectangleHeight);

                // define the fill and border colors
                Color fillColor = new Color(255, 255, 255, 255); // white color with the desired transparency
                Color borderColor = Color.White;

                // draw the rectangle shape with white borders and transparency
                Texture2D pixel = new Texture2D(GraphicsDevice, 1, 1);
                pixel.SetData(new[] { Color.White }); // create a 1x1 white texture
                //_spriteBatch.Draw(pixel, rectangle, fillColor); // draw the filled rectangle
                _spriteBatch.Draw(pixel, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, 1), borderColor); // draw the top border
                _spriteBatch.Draw(pixel, new Rectangle(rectangle.X, rectangle.Y, 1, rectangle.Height), borderColor); // draw the left border
                _spriteBatch.Draw(pixel, new Rectangle(rectangle.X + rectangle.Width - 1, rectangle.Y, 1, rectangle.Height), borderColor); // draw the right border
                _spriteBatch.Draw(pixel, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height - 1, rectangle.Width, 1), borderColor); // draw the bottom border
            }
        }
    }
}