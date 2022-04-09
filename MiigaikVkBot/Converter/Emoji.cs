using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiigaikVkBot.Converters
{
    public static class Emoji
    {
        public static string Number(int num)
        {
            switch (num)
            {
                case 0:
                    return "0&#8419;";
                case 1:
                    return "1&#8419;";
                case 2:
                    return "2&#8419;";
                case 3:
                    return "3&#8419;";
                case 4:
                    return "4&#8419;";
                case 5:
                    return "5&#8419;";
                case 6:
                    return "6&#8419;";
                case 7:
                    return "7&#8419;";
                case 8:
                    return "8&#8419;";
                case 9:
                    return "9&#8419;";
                default:
                    return "UNRECOGNIZED_NUMBER";
            }
        }

        public static string RedCircle() => "&#128308;";
        public static string LetterI() => "&#8505;";
    }
}