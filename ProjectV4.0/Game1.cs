using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using ProjectV4._0.Objects;

namespace ProjectV4._0
{
    public class Game1 : Game
    {
        //Основные настройки
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //Скорость игрока
        private const float PlayerMoveSpeed = 8;

        Player _player;

        Texture2D _mainBackground;
        ParallaxingBackground _bgLayer1;
        ParallaxingBackground _bgLayer2;
        Rectangle _rectBackground;
        const float Scale = 1f;

        //Полоса здоровья
        GreenHealthBar greenHealthBar;
        Texture2D greenHealth;
        int actualHealth;

        //Состояние клавиатуры
        KeyboardState _currentKeyboardState;
        KeyboardState _prevKeyboardState;

        //Состояние геймпада
        GamePadState _currentGamePadState;
        GamePadState _prevGamePadState;

        //Текстура пули(списка)
        Texture2D bulletTexture;
        List<Bullet> bulletsList;

        //Частота стрельбы
        TimeSpan bulletSpawnTime;
        TimeSpan previousBulletSpawnTime;

        //Частота появления противника
        TimeSpan enemySpawnTime;
        TimeSpan previousSpawnTime;

        //Текстура противника(списка)
        Texture2D enemyTexture;
        List<Enemy> enemies;

        //Рандомный генератор 
        Random random;

        //Текстура взрыва(списка)
        Texture2D explosionTexture;
        List<Explosion> explosions;
        
        //Звуки пули и его экземпляр
        private SoundEffect bulletSound;
        private SoundEffectInstance bulletSoundInstance;

        //Звук взрыва
        private SoundEffect explosionSound;
        private SoundEffectInstance explosionSoundInstance;

        //Музыка в игре
        private Song gameMusic;
        
        //Очки
        private SpriteFont font;
        private int score = 0;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            _player = new Player();

            greenHealthBar = new GreenHealthBar();
            
            _bgLayer1 = new ParallaxingBackground();
            _bgLayer2 = new ParallaxingBackground();

            //Прямоугольник во весь экран для фона
            _rectBackground = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            //Инициализация пули
            bulletsList = new List<Bullet>();
            const float SECONDS_IN_MINUTE = 60f;
            const float RATE_OF_FIRE = 200f;
            bulletSpawnTime = TimeSpan.FromSeconds(SECONDS_IN_MINUTE / RATE_OF_FIRE);
            previousBulletSpawnTime = TimeSpan.Zero;

            //Инициализация листа врагов
            enemies = new List<Enemy>();

            //Определение частоты появления врагов
            enemySpawnTime = TimeSpan.FromSeconds(1.0f);

            random = new Random();

            explosions = new List<Explosion>();
            graphics.IsFullScreen = true;
            base.Initialize();
        }
        
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Загрузка контента для игрока
            Rectangle titleSafeArea = GraphicsDevice.Viewport.TitleSafeArea;
            var playerPosition = new Vector2(titleSafeArea.X, titleSafeArea.Y + titleSafeArea.Height / 2);

            Texture2D playerTexture = Content.Load<Texture2D>("Graphics/shipAnimation");
            Animation playerAnimation = new Animation();
            playerAnimation.Initialize(playerTexture, playerPosition, 115, 69, 8, 30, Color.White, 1, true);

            _player.Initialize(playerAnimation, playerPosition);

            //Уровень здоровья
            greenHealth = Content.Load<Texture2D>("Graphics/greenHealth");
            greenHealthBar.Initialize(greenHealth, actualHealth);

            //Очки
            font = Content.Load<SpriteFont>("Graphics/Score");

            //Загрузка фона
            _bgLayer1.Initialize(Content, "Graphics/bgLayer1", GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, -1);
            _bgLayer2.Initialize(Content, "Graphics/bgLayer2", GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, -2);
            _mainBackground = Content.Load<Texture2D>("Graphics/mainbackground");

            //Загрузка текстуры противника
            enemyTexture = Content.Load<Texture2D>("Graphics/enemyAnimation");

            //Пуля
            bulletTexture = Content.Load<Texture2D>("Graphics/bullet");

            //Спрайт лист взырва
            explosionTexture = Content.Load<Texture2D>("Graphics/explosion");

            //Звуки пули
            bulletSound = Content.Load<SoundEffect>("Sounds/bulletFire");
            bulletSoundInstance = bulletSound.CreateInstance();

            //Звуки взрыва
            explosionSound = Content.Load<SoundEffect>("Sounds/explosion");
            explosionSoundInstance = explosionSound.CreateInstance();

            //Музыка в игре
            gameMusic = Content.Load<Song>("Sounds/gameMusic");

            //Начало игры музыки
            MediaPlayer.Play(gameMusic);
            MediaPlayer.Volume = 0.04f;
        }
        
        protected override void UnloadContent()
        {
            bulletSoundInstance.Dispose();
            explosionSoundInstance.Dispose();
        }
        
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //Ввод
            //Сохранение предыдущего состояния нажатых клавиш
            _prevGamePadState = _currentGamePadState;
            _prevKeyboardState = _currentKeyboardState;
            //Считывание текущего ввода
            _currentKeyboardState = Keyboard.GetState();
            _currentGamePadState = GamePad.GetState(PlayerIndex.One);

            UpdatePlayer(gameTime);
            _bgLayer1.Update(gameTime);
            _bgLayer2.Update(gameTime);

            actualHealth = _player.Health;
            greenHealthBar.Update(gameTime, actualHealth);


            //Обновление:
            //пули
            UpdateBullet(gameTime);

            //врагов
            UpdateEnemies(gameTime);

            //столкновений
            UpdateCollision();

            //взрывов
            UpdateExplosions(gameTime);

            if (_player.Active == false)
                Exit();

            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            //Отрисовка 
            //фона
            spriteBatch.Draw(_mainBackground, _rectBackground, Color.White);
            _bgLayer1.Draw(spriteBatch);
            _bgLayer2.Draw(spriteBatch);

            //игрока
            _player.Draw(spriteBatch);
            
            //здоровья
            greenHealthBar.Draw(spriteBatch);

            //очки
            spriteBatch.DrawString(font, "Score" + score, new Vector2(GraphicsDevice.Viewport.Width - 200, 0), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            //пуль
            foreach (var l in bulletsList)
            {
                l.Draw(spriteBatch);
            }


            //врагов
            foreach (var e in enemies)
            {
                e.Draw(spriteBatch);
            };

            //взрывов
            foreach (var e in explosions)
            {
                e.Draw(spriteBatch);
            };

            spriteBatch.End();

            base.Draw(gameTime);
        }
        
        private void UpdateExplosions(GameTime gameTime)
        {
            for (var e = 0; e < explosions.Count; e++)
            {
                explosions[e].Update(gameTime);

                if (!explosions[e].Active)
                    explosions.Remove(explosions[e]);
            }
        }

        void UpdatePlayer(GameTime gameTime)
        {
            _player.Update(gameTime);

            //Движение по стику
            _player.Position.X += _currentGamePadState.ThumbSticks.Left.X * PlayerMoveSpeed;
            _player.Position.Y += _currentGamePadState.ThumbSticks.Left.Y * PlayerMoveSpeed;

            //Движение по клавиатуре
            if (_currentKeyboardState.IsKeyDown(Keys.Left) || _currentGamePadState.DPad.Left == ButtonState.Pressed)
            {
                _player.Position.X -= +PlayerMoveSpeed;
            }
            if (_currentKeyboardState.IsKeyDown(Keys.Right) || _currentGamePadState.DPad.Right == ButtonState.Pressed)
            {
                _player.Position.X += PlayerMoveSpeed;
            }
            if (_currentKeyboardState.IsKeyDown(Keys.Up) || _currentGamePadState.DPad.Up == ButtonState.Pressed)
            {
                _player.Position.Y -= PlayerMoveSpeed;
            }
            if (_currentKeyboardState.IsKeyDown(Keys.Down) || _currentGamePadState.DPad.Down == ButtonState.Pressed)
            {
                _player.Position.Y += PlayerMoveSpeed;
            }

            if (_currentKeyboardState.IsKeyDown(Keys.Space) || _currentGamePadState.Buttons.X == ButtonState.Pressed)
            {
                FireBullet(gameTime);
            }

            //Ограничение выхода за предел экрана
            _player.Position.X = MathHelper.Clamp(_player.Position.X, 0, GraphicsDevice.Viewport.Width - _player.Width);
            _player.Position.Y = MathHelper.Clamp(_player.Position.Y, 0, GraphicsDevice.Viewport.Height - _player.Height);
        }

        protected void UpdateBullet(GameTime gameTime)
        {
            for (var i = 0; i < bulletsList.Count; i++)
            {
                bulletsList[i].Update(gameTime);

                //Убрать пулю при попадании или выходе за экран
                if (!bulletsList[i].Active || bulletsList[i].Position.X > GraphicsDevice.Viewport.Width)
                {
                    bulletsList.Remove(bulletsList[i]);
                }
            }
        }

        protected void FireBullet(GameTime gameTime)
        {
            //Управление частотой стрельбы
            if (gameTime.TotalGameTime - previousBulletSpawnTime > bulletSpawnTime)
            {
                previousBulletSpawnTime = gameTime.TotalGameTime;
                AddBullet();

                bulletSoundInstance.Play();
                bulletSoundInstance.Volume = 0.1f;
            }

        }

        protected void AddBullet()
        {
            Animation bulletAnimation = new Animation();

            //Инициализация анимации пули
            bulletAnimation.Initialize(bulletTexture,
                _player.Position,
                46,
                16,
                1,
                30,
                Color.White,
                1f,
                true);

            Bullet bullet = new Bullet();

            //Начальная позиция пули
            var bulletPostion = _player.Position;

            bulletPostion.Y += 28;
            bulletPostion.X += 70;

            //Инициализация
            bullet.Initialize(bulletAnimation, bulletPostion);

            bulletsList.Add(bullet);
        }

        protected void UpdateEnemies(GameTime gameTime)
        {
            //Спавн врагов с интервалом 1.5 секунды
            if (gameTime.TotalGameTime - previousSpawnTime > enemySpawnTime)
            {
                previousSpawnTime = gameTime.TotalGameTime;
                AddEnemy();
            }

            for (var i = 0; i < enemies.Count; i++)
            {
                enemies[i].Update(gameTime);
                if (!enemies[i].Active)
                {
                    enemies.Remove(enemies[i]);
                }
            }
        }

        protected void AddEnemy()
        {
            Animation enemyAnimation = new Animation();

            //Инициализация анимации врага
            enemyAnimation.Initialize(enemyTexture, Vector2.Zero, 115, 69, 8, 30, Color.White, 1f, true);

            //Случайная генерация врага по местоположению
            Vector2 position = new Vector2(
                GraphicsDevice.Viewport.Width + enemyTexture.Width / 2,
                random.Next(100, GraphicsDevice.Viewport.Height - 100));

            Enemy enemy = new Enemy();

            enemy.Initialize(enemyAnimation, position);

            //Добавление врагов в список
            enemies.Add(enemy);
        }


        protected void UpdateCollision()
        {
            Rectangle playerRectangle;
            Rectangle enemyRectangle;
            Rectangle bulletRectangle;

            //Прямоугольник игрока
            playerRectangle = new Rectangle(
                (int)_player.Position.X,
                (int)_player.Position.Y,
                _player.Width,
                _player.Height);

            //Обнаружение столкновений между игроком и врагами
            for (var i = 0; i < enemies.Count; i++)
            {
                enemyRectangle = new Rectangle(
                   (int)enemies[i].Position.X,
                   (int)enemies[i].Position.Y,
                   enemies[i].Width,
                   enemies[i].Height);

                //Определение столкновений
                if (playerRectangle.Intersects(enemyRectangle))
                {
                    //Убрать врага
                    enemies[i].Health = 0;

                    //Анимация взрыва
                    AddExplosion(enemies[i].Position);

                    //Урон игроку
                    _player.Health -= enemies[i].Damage;

                    //Условие проигрыша
                    if (_player.Health <= 0)
                    {
                        _player.Active = false;
                    }
                }

                for (var l = 0; l < bulletsList.Count; l++)
                {
                    //Прямоугольник лазера
                    bulletRectangle = new Rectangle(
                        (int)bulletsList[l].Position.X,
                        (int)bulletsList[l].Position.Y,
                        bulletsList[l].Width,
                        bulletsList[l].Height);

                    //Проверка столкновений пули и врага
                    if (bulletRectangle.Intersects(enemyRectangle))
                    {
                        score++;
                        //Анимация взрыва
                        AddExplosion(enemies[i].Position);

                        //Убрать врага
                        enemies[i].Health = 0;

                        //Убрать пулю
                        bulletsList[l].Active = false;
                    }
                }
            }
        }

        protected void AddExplosion(Vector2 enemyPosition)
        {
            Animation explosionAnimation = new Animation();

            explosionAnimation.Initialize(explosionTexture,
                enemyPosition,
                134,
                134,
                12,
                30,
                Color.White,
                1.0f,
                true);

            Explosion explosion = new Explosion();
            explosion.Initialize(explosionAnimation, enemyPosition);

            explosions.Add(explosion);

            explosionSoundInstance.Play();
            explosionSoundInstance.Volume = 0.07f;
        }
    }
}
