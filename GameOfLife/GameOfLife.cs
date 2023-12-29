using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace GameOfLife
{
    public class GameOfLife : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private GraphicsDevice _device;
        private SpriteBatch _spriteBatch;
        private readonly Random _random = new();
        private const int _cellSize = 24;
        private const int _viewportWidth = 1920;
        private const int _viewportHeight = 1080;
        private const int _columns = _viewportWidth / _cellSize;
        private const int _rows = _viewportHeight / _cellSize;
        private const int _numberStartingLivingCells = 400;
        private Texture2D _deadCellTexture;
        private Texture2D _liveCellTexture;
        private int[,] _board = new int[_columns, _rows];
        private readonly int[,] _tempBoard = new int[_columns, _rows];

        public GameOfLife()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Window.Title = "Conway's Game of Life";

            _graphics.IsFullScreen = false;
            _graphics.PreferredBackBufferWidth = _viewportWidth;
            _graphics.PreferredBackBufferHeight = _viewportHeight;
            _graphics.ApplyChanges();

            _device = _graphics.GraphicsDevice;

            _InitializeCellTextures();
            _InitializeBoards();

            base.Initialize();
        }

        private void _InitializeCellTextures()
        {
            _deadCellTexture = new Texture2D(_device, _cellSize, _cellSize);
            Color[] _deadColors = new Color[_cellSize * _cellSize];
            for (int i = 0; i < _deadColors.Length; i++)
            {
                _deadColors[i] = Color.Black;
            }
            _deadCellTexture.SetData<Color>(_deadColors);

            _liveCellTexture = new Texture2D(_device, _cellSize, _cellSize);
            Color[] _liveColors = new Color[_cellSize * _cellSize];
            for (int i = 0; i < _liveColors.Length; i++)
            {
                _liveColors[i] = Color.White;
            }
            _liveCellTexture.SetData<Color>(_liveColors);
        }

        private void _InitializeBoards()
        {
            for (int y = 0; y < _rows; y++)
            {
                for (int x = 0; x < _columns; x++)
                {
                    _board[x, y] = 0;
                    _tempBoard[x, y] = 0;
                }
            }

            for (int i = 0; i < _numberStartingLivingCells; i++)
            {
                int x = _random.Next(0, _columns);
                int y = _random.Next(0, _rows);
                _board[x, y] = 1;
                _tempBoard[x, y] = 1;
            }
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            _UpdateBoard();

            base.Update(gameTime);
        }

        private void _UpdateBoard()
        {
            for (int y = 0; y < _rows; y++)
            {
                for (int x = 0; x < _columns; x++)
                {
                    // check neighbors
                    int _livingNeighbors = 0;
                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            if (i == 0 && j == 0) { continue; }
                            else
                            {
                                int _neighborX = x + i;
                                int _neighborY = y + j;
                                if (_neighborX > 0 && _neighborX < _columns && _neighborY > 0 && _neighborY < _rows)
                                {
                                    if (_board[_neighborX, _neighborY] == 1)
                                    {
                                        _livingNeighbors++;
                                    }
                                }
                            }
                        }
                    }

                    // enforce rules
                    _tempBoard[x, y] = _board[x, y] == 1 ? (_livingNeighbors is < 2 or > 3 ? 0 : 1) : (_livingNeighbors == 3 ? 1 : 0);
                }
            }

            _board = _tempBoard.Clone() as int[,];
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            for (int y = 0; y < _rows; y++)
            {
                for (int x = 0; x < _columns; x++)
                {
                    Texture2D _texture = _board[x, y] == 0 ? _deadCellTexture : _liveCellTexture;
                    _spriteBatch.Draw(_texture, new Rectangle(x * _cellSize, y * _cellSize, _cellSize, _cellSize), Color.White);
                }
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
