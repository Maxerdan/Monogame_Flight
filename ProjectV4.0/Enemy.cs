using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectV4._0.Objects
{
    public class Enemy
    {
        //Анимация врага
        public Animation EnemyAnimation;

        //Местоположение врага
        public Vector2 Position;

        //Состояние
        public bool Active;

        //Здоровье врага    
        public int Health;

        //Урон наносимый игроку при столкновении
        public int Damage;

        //Очки за врага
        public int Value;

        //Ширина врага
        public int Width
        {
            get { return EnemyAnimation.FrameWidth; }
        }

        //Высота
        public int Height
        {
            get { return EnemyAnimation.FrameHeight; }
        }

        public float enemyMoveSpeed;

        public void Initialize(Animation animation,
            Vector2 position)
        {
            //Текстура врага
            EnemyAnimation = animation;

            //Местоположение
            Position = position;
            
            Active = true;
            
            Health = 10;

            //Урон наносимый игроку
            Damage = 10;

            //Скорость
            enemyMoveSpeed = 10;

            //Очки
            Value = 100;
        }

        public void Update(GameTime gameTime)
        {
            //Постоянное движение влево
            Position.X -= enemyMoveSpeed;

            //Обновление местоположения
            EnemyAnimation.Position = Position;

            //Анимации
            EnemyAnimation.Update(gameTime);

            //Убрать врага если добрался до экрана или умер
            if (Position.X < -Width || Health <= 0)
            {
                Active = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            EnemyAnimation.Draw(spriteBatch);
        }
    }
}
