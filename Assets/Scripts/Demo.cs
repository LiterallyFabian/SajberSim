using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// https://www.youtube.com/watch?v=YUZsHDB8o2Q
// pls leave the source code alone

public class Demo
{
    public const bool isDemo = false;
    public static readonly DemoNovel[] allowedNovels =
    {
        new DemoNovel("2212412555", "76561198218793769") // openhouse
    };



    public class DemoNovel
    {
        public string id;
        public string by;
        public DemoNovel(string id, string by)
        {
            this.id = id;
            this.by = by;
        }
    }
}


