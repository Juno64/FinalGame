using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace FinalGame
{
    class Brick
    {
        public Color BrickColor {get; set;}
        public Rectangle BrickRectangle { get; set; }
        public int Hp { get; set; }
        public Brick(Color theColor, Rectangle theRectangle, int hitPoints)
        {
            BrickColor = theColor;
            BrickRectangle = theRectangle;
            Hp = hitPoints;
        }
    }
}
