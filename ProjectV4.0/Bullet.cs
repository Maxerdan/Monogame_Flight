using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectV4._0.Objects
{
    public class Bullet 
    {

        //Анимация пули
        public Animation BulletAnimation;

        //Скорость
        float laserMoveSpeed = 30f;

        //Местоположение
        public Vector2 Position;

        //Урон
        int Damage = 10;

        //Состояние
        public bool Active;
        
        //Ширина
        public int Width
        {
            get { return BulletAnimation.FrameWidth; }
        }

        //Высота
        public int Height
        {
            get { return BulletAnimation.FrameHeight; }

        }

        public void Initialize(Animation animation, Vector2 position)
        {
            BulletAnimation = animation;
            Position = position;
            Active = true;
        }

        public void Update(GameTime gameTime)
        {
            Position.X += laserMoveSpeed;

            BulletAnimation.Position = Position;
            BulletAnimation.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            BulletAnimation.Draw(spriteBatch);
        }
    }
}
