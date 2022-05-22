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

namespace TetrisSharp.Scenes
{
    internal sealed class GameScene : Scene, IGameScene
    {
        private const string ScoreTextPattern = "Rows Removed: {0}";
        private const float KeyDelay = 0.08F;
        private readonly BlockGenerator _blockGenerator = new();
        private readonly TimeSpan _fallingInterval = TimeSpan.FromMilliseconds(500);
        private readonly GameBoard _gameBoard = new(Constants.NumberOfTilesX, Constants.NumberOfTilesY);
        private readonly Texture2D[] _tileTextures = new Texture2D[Constants.TileTextureCount];
        private BackgroundMusic _bgm;
        private SoundEffect _bgmEffect;
        private Block _block;
        private Sound _collisionSound;
        private SoundEffect _collisionSoundEffect;
        private TimeSpan _fallingTimeCounter = TimeSpan.Zero;
        private int _fixedTileSize;
        private Texture2D _fixedTileTexture;
        private Texture2D _gameBoardTexture;
        private Sound _gameoverSound;
        private SoundEffect _gameoverSoundEffect;
        private Sound _rowRemovingSound;
        private SoundEffect _rowRemovingSoundEffect;
        private Texture2D _scoreBoardTexture;
        private float _timeSinceLastKeyPress = 0;
        private bool _isGameOver = false;
        private Text _scoreText;
        private SpriteFont _font;
        private int _score;


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

            // Draw block
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

            _scoreText = new Text(string.Format(ScoreTextPattern, _score), this, _font, Color.LightYellow, new Vector2(25 * Constants.NumberOfTilesX + 5, 5))
            {
                CollisionDetective = false
            };

            Add(_scoreText);

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

            _bgmEffect = contentManager.Load<SoundEffect>("sounds\\bgm");
            _bgm = new(new[] { _bgmEffect }, 0.2F);

            Add(_bgm);

            _rowRemovingSoundEffect = contentManager.Load<SoundEffect>("sounds\\remove_row");
            _rowRemovingSound = new(_rowRemovingSoundEffect, 0.1F);

            _collisionSoundEffect = contentManager.Load<SoundEffect>("sounds\\merge");
            _collisionSound = new(_collisionSoundEffect, 0.1F);

            _gameoverSoundEffect = contentManager.Load<SoundEffect>("sounds\\gameover");
            _gameoverSound = new(_gameoverSoundEffect, 0.2F);

            _blockGenerator.LoadFromFile("blocks.xml");
            for (var i = 0; i < Constants.TileTextureCount; i++)
            {
                _tileTextures[i] = contentManager.Load<Texture2D>($"textures\\tile_{i + 1}");
            }

            _fixedTileTexture = contentManager.Load<Texture2D>($"textures\\tile_fixed");
            _fixedTileSize = _fixedTileTexture.Width;

            _block = _blockGenerator.CreateBlock(this, _tileTextures);
            Add(_block);
        }

        public override void Update(GameTime gameTime)
        {
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
                else if (Keyboard.GetState().IsKeyDown(Keys.J))
                {
                    _block.Rotate();
                }

                _timeSinceLastKeyPress = 0;
            }

            _scoreText.Value = string.Format(ScoreTextPattern, _score);
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

                _collisionSound.Stop();
                _rowRemovingSound.Stop();
                _gameoverSound.Stop();
                _bgm.Stop();
                _collisionSoundEffect.Dispose();
                _rowRemovingSoundEffect.Dispose();
                _gameoverSoundEffect.Dispose();
                _bgmEffect.Dispose();
            }
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

                _score += rows;

                Remove(_block);
                _block = _blockGenerator.CreateBlock(this, _tileTextures);
                if (_block.CollisionsWithBoard())
                {
                    _bgm.Stop();
                    _gameoverSound.Play();
                    _isGameOver = true;
                }

                Add(_block);
            }
        }
    }
}
