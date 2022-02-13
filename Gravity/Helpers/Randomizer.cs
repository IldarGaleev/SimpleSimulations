using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gravity.Helpers
{
    public static class Randomizer
    {
        private static Random random= new Random();

        public static Vector2d GetVector()
        {
            double x = (0.5 - random.NextDouble()) * 2;
            double y = (0.5 - random.NextDouble()) * 2;
            return new Vector2d(x, y);
        }
    }
}
