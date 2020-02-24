using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectV4._0
{
    public class Animation
    {
        //Спрайтлист
        Texture2D spriteStrip;

        //Для вывода изображения
        float scale;

        //Время последнего обновления
        int elapsedTime;

        //Время показа frame'ов
        int frameTime;

        //Количество frame'ов
        int frameCount;

        //Индекс текущего frame'а
        int currentFrame;

        //Цвет - конст
        Color color;

        //Размер спрайта
        Rectangle sourceRect = new Rectangle();

        //Спрайт для вывода
        Rectangle destinationRect = new Rectangle();

        //Ширина спрайта
        public int FrameWidth;

        //Высота спрайта
        public int FrameHeight;

        //Состояние
        public bool Active;

        //Цикл(будет ли повторятся анимация)
        public bool Looping;
        
        public Vector2 Position;

        public void Initialize(Texture2D texture, Vector2 position, int frameWidth, int frameHeight, int frameCount, int frametime, Color color, float scale, bool looping)
        {
            this.color = color;
            FrameWidth = frameWidth;
            FrameHeight = frameHeight;
            this.frameCount = frameCount;
            frameTime = frametime;
            this.scale = scale;

            Looping = looping;
            Position = position;
            spriteStrip = texture;

            elapsedTime = 0;
            currentFrame = 0;

            //Запуск анимации
            Active = true;
        }

        public void Update(GameTime gameTime)
        {
            //Если игра не активна - не обновлять ее
            if(Active == false) return;

            elapsedTime += (int) gameTime.ElapsedGameTime.TotalMilliseconds;

            if (elapsedTime > frameTime)
            {
                currentFrame++;

                if (currentFrame == frameCount)
                {
                    currentFrame = 0;
                    if (Looping == false)
                        Active = false;
                }
                elapsedTime = 0;
            }

            //Выбрать нужный frame
            sourceRect = new Rectangle(currentFrame * FrameWidth, 0, FrameWidth, FrameHeight);

            //Выбрать frame для вывода
            destinationRect = new Rectangle((int) Position.X,
                                            (int) Position.Y,
                                            (int) (FrameWidth * scale),
                                            (int) (FrameHeight * scale));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Active)
                spriteBatch.Draw(spriteStrip, destinationRect, sourceRect, color);
        }
    }
}
