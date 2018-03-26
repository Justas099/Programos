using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intelektika
{
    static class Extensions
    {
        public static double ToRadians(this double val)
        {
            return (Math.PI / 180) * val;
        }
    }
}
