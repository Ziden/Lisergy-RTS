using System;
using System.Collections.Generic;
using System.Text;

namespace Game
{
    /*

        - Receber unit statica via network, e ao receber desempacota certinho do outro lado 
          Um metodo tipo "post-deserialize" q coloca o bagui no jogo

          Precisava de um metodo default de "entrar" no jogo talvez, pra units buildings





     */
    public class Log
    {
        public static Action<string> Debug = Console.WriteLine;
        public static Action<string> Info = Console.WriteLine;
        public static Action<string> Error = Console.Error.WriteLine;
    }
}
