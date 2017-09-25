using System;
using System.Linq;
using System.Security.Cryptography;

namespace PDR.Domain.Services.GeneratePassword
{
    public static class GeneratePassword
    {
        private const string PasChars = "1234567890abcdefghijklmopkqrstuvwxyzQAZWSXEDCRFVTGBYHNUJMIKLOP";
        
        public static string Generate(int length = 8, bool cryptoRnd = true)
        {
            var result = string.Empty;
            var systemRandom = new Random();
            for (var i = 0; i <= length; i++)
            {
                result += Convert.ToChar(PasChars[cryptoRnd ? Rnd(PasChars.Length - 1) : systemRandom.Next(PasChars.Length - 1)]);
            }

            return result;
        }

        private static int Rnd(int numSides)
        {
            if (numSides > 0)
            {
                var rndBytes = new byte[(int)(Math.Log(numSides, 256)) + 1];
                (new RNGCryptoServiceProvider()).GetBytes(rndBytes);
                var rnd = rndBytes.Aggregate(1, (current, t) => current * (Convert.ToInt32(t) + 1));

                if (rnd > numSides)
                {
                    rnd = rnd % numSides + 1;
                }

                return rnd;
            }

            return 0;
        }
    }
}
