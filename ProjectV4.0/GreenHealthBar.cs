using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectV4._0
{
    class GreenHealthBar
    {
        Texture2D GreenHealth;

        Rectangle GreenRectangle;

        int ActualHealth;

        int GreenScale;

        int fullWidth = 500;
        
        public void Initialize(Texture2D greenHealth, int actualHealth)
        {
            GreenHealth = greenHealth;
            ActualHealth = actualHealth;
            GreenRectangle = new Rectangle(0, 0, greenHealth.Width, greenHealth.Height);
        }

        public void Update(GameTime gameTime, int actualHealth)
        {
            GreenScale = actualHealth / 10;
            GreenRectangle.Width = (GreenScale) * (fullWidth / 10);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GreenHealth, GreenRectangle, GreenRectangle, Color.White);
        }
    }
}
