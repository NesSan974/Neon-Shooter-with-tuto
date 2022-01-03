using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Bloom_Sample;

using System;


/*

https://gamedevelopment.tutsplus.com/tutorials/search/xna

*/



namespace neonShooter
{
    public class Game1 : Game
    {
        public static GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public static ParticleManager<ParticleState> ParticleManager { get; private set; }
        RenderTarget2D renderTarget;

        const int maxGridPoints = 1600;
        Vector2 gridSpacing;
        internal static Grid Grid;




        private BloomFilter _bloomFilter;


        private int _width = 1300;
        private int _height = 800;






        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;

            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
            _graphics.ApplyChanges();


            Instance = this;
        }





        protected override void Initialize()
        {


            _graphics.PreferredBackBufferHeight = _height;
            _graphics.PreferredBackBufferWidth = _width;

            _graphics.ApplyChanges();
            // TODO: Add your initialization logic here

            gridSpacing = new Vector2((float)Math.Sqrt(Viewport.Width * Viewport.Height / maxGridPoints));
            Grid = new Grid(Viewport.Bounds, gridSpacing);


            renderTarget = new RenderTarget2D(
                                                GraphicsDevice, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight,
                                                false, GraphicsDevice.PresentationParameters.BackBufferFormat,
                                                DepthFormat.Depth24);



            base.Initialize();
            EntityManager.Add(PlayerShip.Instance);


            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.6f;
            MediaPlayer.Play(Sound.Music);




            ParticleManager = new ParticleManager<ParticleState>(1024 * 20, ParticleState.UpdateParticle);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            Art.Load(Content);
            Sound.Load(Content);

            //Load our Bloomfilter!
            _bloomFilter = new BloomFilter();
            _bloomFilter.Load(GraphicsDevice, Content, _width, _height);

            _bloomFilter.BloomPreset = BloomFilter.BloomPresets.SuperWide;
            _bloomFilter.BloomStrengthMultiplier = 1f;
            _bloomFilter.BloomThreshold = 0f; // jsp ce que c'est, mais ca rend stylé



        }


        protected override void UnloadContent()
        {
            _bloomFilter.Dispose();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            Input.Update();

            Grid.Update();

            EnemySpawner.Update();

            EntityManager.Update();

            ParticleManager.Update();


            base.Update(gameTime);
        }

        public static GameTime gt { get; set; }

        protected override void Draw(GameTime gameTime)
        {
            gt = gameTime;


            _graphics.GraphicsDevice.SetRenderTarget(renderTarget);


            GraphicsDevice.Clear(Color.Black);


            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);


            Grid.Draw(_spriteBatch);

            EntityManager.Draw(_spriteBatch);

            _spriteBatch.Draw(Art.Pointer, Input.MousePosition, Color.White);

            ParticleManager.Draw(_spriteBatch);



            _spriteBatch.End();


            int w = _width;
            int h = _height;

            //Default 
            //_bloomFilter.BloomUseLuminance = true;

            Texture2D bloom = _bloomFilter.Draw(renderTarget, w, h);

            _graphics.GraphicsDevice.SetRenderTarget(null);



            GraphicsDevice.Clear(Color.Transparent);

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);


            if (PlayerStatus.IsGameOver)
            {
                string text = "Game Over\n" +
                    "Your Score: " + PlayerStatus.Score + "\n" +
                    "High Score: " + PlayerStatus.HighScore;

                Vector2 textSize = Art.Font.MeasureString(text);
                _spriteBatch.DrawString(Art.Font, text, ScreenSize / 2 - textSize / 2, Color.White);
            }





            _spriteBatch.DrawString(Art.Font, "Lives: " + PlayerStatus.Lives, new Vector2(5), Color.White);
            DrawRightAlignedString("Score: " + PlayerStatus.Score, 5);
            DrawRightAlignedString("Multiplier: " + PlayerStatus.Multiplier, 35);


            _spriteBatch.Draw(renderTarget, Vector2.Zero, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0f);
            _spriteBatch.Draw(bloom, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), Color.White);


            _spriteBatch.End();


            base.Draw(gameTime);
        }

        private void DrawRightAlignedString(string text, int y)
        {
            var textWidth = Art.Font.MeasureString(text).X;
            _spriteBatch.DrawString(Art.Font, text, new Vector2(ScreenSize.X - textWidth - 5, y), Color.White);
        }

        public static Game1 Instance { get; private set; }
        public static Viewport Viewport { get { return Instance.GraphicsDevice.Viewport; } }
        public static Vector2 ScreenSize { get { return new Vector2(Viewport.Width, Viewport.Height); } }





    }



}
