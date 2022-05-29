using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using TetrisSharp.Framework;
using TetrisSharp.Framework.Scenes;
using TetrisSharp.Models;
using TetrisSharp.Sprites;
using TetrisSharp.Framework.Sounds;
using Microsoft.Xna.Framework.Audio;
using TetrisSharp.Framework.Components;
using TetrisSharp.Framework.Sprites;

namespace TetrisSharp.Scenes
{
    internal sealed class GameScene : Scene, IGameScene
    {
        private const string HintTextLine1 = "Left - [A], Right - [D], Down - [S]";
        private const string HintTextLine2 = "Rotate - [J], Pause - [Enter]";
        private const string HintTextLine3 = "BGM: {0} [F12]";
        private const float KeyDelay = 0.08F;
        private const string LevelTextPattern = "Level: {0}";
        private const string RowsRemovedTextPattern = "Rows removed: {0}";
        private const string TotalScoreTextPattern = "Total score: {0}";
        private static readonly Random _random = new(DateTime.Now.Millisecond);
        private readonly BlockGenerator _blockGenerator = new();
        private readonly GameBoard _gameBoard = new(Constants.NumberOfTilesX, Constants.NumberOfTilesY);
        private readonly KeyPress _keyEnter = new(Keys.Enter);
        private readonly KeyPress _keyF12 = new(Keys.F12);
        private readonly KeyPress _keyJ = new(Keys.J);
        private readonly Queue<int> _tetrisQueue = new();
        private readonly Texture2D[] _tileTextures = new Texture2D[Constants.TileTextureCount];
        private bool _allowBgm = true;
        private BackgroundMusic _bgm;
        private SoundEffect _bgmEffect;
        private Block _block;
        private Sound _collisionSound;
        private SoundEffect _collisionSoundEffect;
        private TimeSpan _fallingInterval = TimeSpan.FromMilliseconds(1000);
        private TimeSpan _fallingTimeCounter = TimeSpan.Zero;
        private int _fixedTileSize;
        private Texture2D _fixedTileTexture;
        private SpriteFont _font;
        private Texture2D _gameBoardTexture;
        private Sound _gameoverSound;
        private SoundEffect _gameoverSoundEffect;
        private Text _hintTextLine1;
        private Text _hintTextLine2;
        private Text _hintTextLine3;
        private bool _isGameOver = false;
        private Text _labelNextBlock;
        private int _level = 1;
        private Text _levelText;
        private Sound _levelUpSound;
        private SoundEffect _levelUpSoundEffect;
        private Block _nextBlock;
        private bool _paused = false;
        private DumbSprite _pausedSprite;
        private Texture2D _pausedTexture;
        private Sound _rowRemovingSound;
        private SoundEffect _rowRemovingSoundEffect;
        private int _rowsRemoved;
        private Text _rowsRemovedText;
        private Texture2D _scoreBoardTexture;
        private float _timeSinceLastKeyPress = 0;
        private int _totalScore;
        private Text _totalScoreText;
        /// <summary>
        /// Initializes a new instance of the <c>GameScene</c> class.
        /// </summary>
        /// <param name="game">The game.</param>
        public GameScene(IOvowGame game)
            : base(game)
        {

        }

        public GameBoard GameBoard => _gameBoard;

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Draw background and scoreboard
            spriteBatch.Draw(_gameBoardTexture, new Vector2(0, 0), Color.White);
            spriteBatch.Draw(_scoreBoardTexture, new Vector2(25 * Constants.NumberOfTilesX, 0), Color.White);

            // Draw gameboard
            for (var boardY = 0; boardY < Constants.NumberOfTilesY; boardY++)
            {
                for (var boardX = 0; boardX < Constants.NumberOfTilesX; boardX++)
                {
                    if (_gameBoard.BoardMatrix[boardX, boardY] == 1)
                    {
                        var posX = boardX * _fixedTileSize;
                        var posY = boardY * _fixedTileSize;
                        spriteBatch.Draw(_fixedTileTexture, new Vector2(posX, posY), Color.White);
                    }
                }
            }

            base.Draw(gameTime, spriteBatch);
        }

        public override void Enter()
        {
            _bgm.Play();
        }

        public override void Leave()
        {
            _bgm.Stop();
        }

        public override void Load(ContentManager contentManager)
        {
            _font = contentManager.Load<SpriteFont>("fonts\\tetris");

            _labelNextBlock = new Text("Next block:", this, _font, Color.LightYellow, new Vector2(25 * Constants.NumberOfTilesX + 5, 10))
            {
                CollisionDetective = false
            };

            _rowsRemovedText = new Text(string.Format(RowsRemovedTextPattern, _rowsRemoved), this, _font, Color.LightYellow, new Vector2(25 * Constants.NumberOfTilesX + 5, 6 * 25))
            {
                CollisionDetective = false
            };

            _totalScoreText = new Text(string.Format(TotalScoreTextPattern, _totalScore), this, _font, Color.LightYellow, new Vector2(25 * Constants.NumberOfTilesX + 5, 7 * 25))
            {
                CollisionDetective = false
            };

            _levelText = new Text(string.Format(LevelTextPattern, _level), this, _font, Color.LightYellow, new Vector2(25 * Constants.NumberOfTilesX + 5, 8 * 25))
            {
                CollisionDetective = false
            };

            _hintTextLine1 = new Text(HintTextLine1, this, _font, Color.LightYellow, new Vector2(25 * Constants.NumberOfTilesX + 5, Constants.NumberOfTilesY * 25 - 25 * 3))
            {
                CollisionDetective = false
            };

            _hintTextLine2 = new Text(HintTextLine2, this, _font, Color.LightYellow, new Vector2(25 * Constants.NumberOfTilesX + 5, Constants.NumberOfTilesY * 25 - 25 * 2))
            {
                CollisionDetective = false
            };

            _hintTextLine3 = new Text(string.Format(HintTextLine3, _allowBgm ? "On" : "Off"), this, _font, Color.LightYellow, new Vector2(25 * Constants.NumberOfTilesX + 5, Constants.NumberOfTilesY * 25 - 25))
            {
                CollisionDetective = false
            };

            Add(_labelNextBlock);
            Add(_rowsRemovedText);
            Add(_totalScoreText);
            Add(_levelText);
            Add(_hintTextLine1);
            Add(_hintTextLine2);
            Add(_hintTextLine3);

            _gameBoardTexture = new Texture2D(Game.GraphicsDevice, 25 * Constants.NumberOfTilesX, 25 * Constants.NumberOfTilesY);
            var gameBoardColorData = new Color[25 * Constants.NumberOfTilesX * 25 * Constants.NumberOfTilesY];
            for (var i = 0; i < gameBoardColorData.Length; i++)
                gameBoardColorData[i] = Color.Black;
            _gameBoardTexture.SetData(gameBoardColorData);

            _scoreBoardTexture = new Texture2D(Game.GraphicsDevice, Constants.ScoreBoardWidth, 25 * Constants.NumberOfTilesY);
            var scoreBoardColorData = new Color[Constants.ScoreBoardWidth * 25 * Constants.NumberOfTilesY];
            for (var i = 0; i < scoreBoardColorData.Length; i++)
                scoreBoardColorData[i] = Color.Gray;
            _scoreBoardTexture.SetData(scoreBoardColorData);

            _bgmEffect = contentManager.Load<SoundEffect>("sounds\\bgm2");
            _bgm = new(new[] { _bgmEffect }, 0.2F);

            Add(_bgm);

            _rowRemovingSoundEffect = contentManager.Load<SoundEffect>("sounds\\remove_row");
            _rowRemovingSound = new(_rowRemovingSoundEffect, Constants.SoundVolume);

            _collisionSoundEffect = contentManager.Load<SoundEffect>("sounds\\merge");
            _collisionSound = new(_collisionSoundEffect, Constants.SoundVolume);

            _gameoverSoundEffect = contentManager.Load<SoundEffect>("sounds\\gameover");
            _gameoverSound = new(_gameoverSoundEffect, Constants.SoundVolume);

            _levelUpSoundEffect = contentManager.Load<SoundEffect>("sounds\\levelup");
            _levelUpSound = new(_levelUpSoundEffect, Constants.SoundVolume);

            _blockGenerator.LoadFromFile("blocks.xml");
            for (var i = 0; i < Constants.TileTextureCount; i++)
            {
                _tileTextures[i] = contentManager.Load<Texture2D>($"textures\\tile_{i + 1}");
            }

            _fixedTileTexture = contentManager.Load<Texture2D>($"textures\\tile_fixed");
            _fixedTileSize = _fixedTileTexture.Width;

            _tetrisQueue.Enqueue(_random.Next(_blockGenerator.BlockDefinitions.Definitions.Count));
            _tetrisQueue.Enqueue(_random.Next(_blockGenerator.BlockDefinitions.Definitions.Count));

            _pausedTexture = contentManager.Load<Texture2D>($"textures\\paused");
            _pausedSprite = new DumbSprite(this, _pausedTexture, new Vector2(10, 60))
            {
                Visible = false,
                Layer = 5
            };

            Add(_pausedSprite);

            AddBlockToBoard();
        }

        public override void Update(GameTime gameTime)
        {
            KeyPress.Update();

            if (_keyF12.IsPressed)
            {
                _allowBgm = !_allowBgm;
                _hintTextLine3.Value = string.Format(HintTextLine3, _allowBgm ? "On" : "Off");
            }

            if (_keyEnter.IsPressed)
            {
                _paused = !_paused;
            }

            _pausedSprite.Visible = _paused;
            ToggleBgm(_allowBgm && !_paused);

            if (_paused)
            {
                return;
            }

            base.Update(gameTime);

            if (_isGameOver)
            {
                return;
            }

            _fallingTimeCounter += gameTime.ElapsedGameTime;
            if (_fallingTimeCounter >= _fallingInterval)
            {
                CheckCollision();
                _block.Y++;
                _fallingTimeCounter = TimeSpan.Zero;
            }

            var seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _timeSinceLastKeyPress += seconds;

            if (_timeSinceLastKeyPress > KeyDelay)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.A) && _block.CanMoveLeft)
                {
                    _block.X--;
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.D) && _block.CanMoveRight)
                {
                    _block.X++;
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.S))
                {
                    CheckCollision();
                    _block.Y++;

                }

                _timeSinceLastKeyPress = 0;
            }

            if (_keyJ.IsPressed)
            {
                _block.Rotate();
            }

            _rowsRemovedText.Value = string.Format(RowsRemovedTextPattern, _rowsRemoved);
            _totalScoreText.Value = string.Format(TotalScoreTextPattern, _totalScore);
            _levelText.Value = string.Format(LevelTextPattern, _level);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var texture in _tileTextures)
                {
                    texture.Dispose();
                }

                _font.Texture.Dispose();

                _fixedTileTexture.Dispose();
                _gameBoardTexture.Dispose();
                _scoreBoardTexture.Dispose();

                _levelUpSound.Stop();
                _collisionSound.Stop();
                _rowRemovingSound.Stop();
                _gameoverSound.Stop();
                _bgm.Stop();

                _levelUpSoundEffect.Dispose();
                _collisionSoundEffect.Dispose();
                _rowRemovingSoundEffect.Dispose();
                _gameoverSoundEffect.Dispose();
                _bgmEffect.Dispose();
            }
        }

        private void AddBlockToBoard()
        {
            if (_nextBlock != null)
            {
                Remove(_nextBlock);
            }

            var index = _tetrisQueue.Dequeue();
            _block = _blockGenerator.CreateBlock(this, _tileTextures, index: index);
            Add(_block);

            var nextIndex = _tetrisQueue.Peek();
            _nextBlock = _blockGenerator.CreateBlock(this, _tileTextures, nextIndex, Constants.NumberOfTilesX + 3, 2);
            Add(_nextBlock);

            if (_block.CollisionsWithBoard())
            {
                _bgm.Stop();
                _gameoverSound.Play();
                _isGameOver = true;
            }

            _tetrisQueue.Enqueue(_random.Next(_blockGenerator.BlockDefinitions.Definitions.Count));
        }

        private void CheckCollision()
        {
            if (_block.CollisionsWithBoard())
            {
                _gameBoard.Merge(_block.CurrentRotation,
                    (int)_block.X,
                    (int)_block.Y,
                    () => _collisionSound.Play());

                var rows = _gameBoard.CleanupFilledRows(row =>
                {
                    _rowRemovingSound.Play();
                });

                _rowsRemoved += rows;
                _totalScore += rows switch
                {
                    1 => 10,
                    2 => 20,
                    3 => 50,
                    4 => 100,
                    _ => 0
                };

                if (_rowsRemoved > 0)
                {
                    var calculatedLevel = _rowsRemoved / Constants.RowsForLevelUp + 1;
                    if (calculatedLevel > _level)
                    {
                        _level = calculatedLevel;
                        _bgm.Pause();
                        _levelUpSound.Play();
                        var fallingIntervalMillsec = 1000 - (_level - 1) * 50;
                        if (fallingIntervalMillsec <= 50)
                        {
                            fallingIntervalMillsec = 50;
                        }

                        _fallingInterval = TimeSpan.FromMilliseconds(fallingIntervalMillsec);
                    }
                }

                Remove(_block);

                AddBlockToBoard();
            }
        }

        private void ToggleBgm(bool shouldPlay)
        {
            if (shouldPlay && _bgm.State == SoundState.Playing)
            {
                return;
            }

            if (shouldPlay && _bgm.State != SoundState.Playing && _levelUpSound.State == SoundState.Stopped)
            {
                _bgm.Resume();
            }
            else if (_bgm.State != SoundState.Paused)
            {
                _bgm.Pause();
            }
        }
    }
}
