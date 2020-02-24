using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectV4._0
{
    class ParallaxingBackground
    {
        Texture2D _texture;
        
        Vector2[] _positions;

        int _speed;
        int _screenHeight;
        int _screenWidth;

        public void Initialize(ContentManager content, String texturePath, int screenWidth, int screenHeight, int speed)
        {
            _screenHeight = screenHeight;
            _screenWidth = screenWidth;

            _texture = content.Load<Texture2D>(texturePath);

            _speed = speed;

            //Добавление ширины текстуры для движения слоев
            int numOfTiles = (int) (Math.Ceiling(_screenWidth / (float) _texture.Width) + 1);
            _positions = new Vector2[numOfTiles];

            //Начальная позиция
            for (int i = 0; i < _positions.Length; i++)
            {
                _positions[i] = new Vector2(i * _texture.Width, 0);
            }
        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < _positions.Length; i++)
            {
                _positions[i].X += _speed;
            }

            for (int i = 0; i < _positions.Length; i++)
            {
                if (_speed <= 0)
                {
                    //Перемещение текстур из-за экрана вышедших за предел, в другоую сторону экрана для движения
                    if (_positions[i].X <= -_texture.Width)
                    {
                        WrapTextureToLeft(i);
                    }
                }
                else
                {
                    if (_positions[i].X >= _texture.Width * (_positions.Length - 1))
                    {
                        WrapTextureToRight(i);
                    }
                }
            }
        }

        private void WrapTextureToLeft(int index)
        {
            int prevTexture = index - 1;
            if (prevTexture < 0)
                prevTexture = _positions.Length - 1;

            _positions[index].X = _positions[prevTexture].X + _texture.Width;
        }

        private void WrapTextureToRight(int index)
        {
            int nextTexture = index + 1;
            if (nextTexture == _positions.Length)
                nextTexture = 0;

            _positions[index].X = _positions[nextTexture].X - _texture.Width;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < _positions.Length; i++)
            {
                var rectBg = new Rectangle((int) _positions[i].X, (int) _positions[i].Y,
                                           _texture.Width,
                                           _screenHeight);
                spriteBatch.Draw(_texture, rectBg, Color.White);
            }
        }
    }
}
