﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kitahara
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Nyukanyuryoku());
            //Application.Run(new Sizecolor2());
            //Application.Run(new Syukkasiji());
            //Application.Run(new ZaikoRireki_View());
            //Application.Run(new ZaikoRireki());
        }
    }
}
