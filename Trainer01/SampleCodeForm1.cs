using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trainer01
{
    class SampleCodeForm1
    {
        private void button1_click()
        {         
            //code1();
            //ou
            //code2();        

        }
        private void code1()
        {
            //se o processo ta em execucao
            Process[] processes = Process.GetProcessesByName("game");
            if (processes.Length > 0)
            {
                IntPtr BaseAddress = IntPtr.Zero;
                //capturo o processo
                Process MYproc = processes[0];

                //captura o modulo do processo
                foreach (ProcessModule module in MYproc.Modules)
                {
                    if (module.ModuleName.Contains("game"))
                    {
                        //captura a adress do modulo do processo
                        BaseAddress = module.BaseAddress;
                    }
                }

                if (BaseAddress != IntPtr.Zero)
                {
                    //write cheat where
                    VAMemory memory = new VAMemory("game");
                    //captura a adress do "game.exe"+0045D20C ou 0085D20C
                    int finalAdress = memory.ReadInt32(BaseAddress + 0x0045D20C);
                    //captura o valor da vida atraves da offset 0x94
                    int hearth = memory.ReadInt32((IntPtr)finalAdress + 0x94);

                    //valor da vida 100%
                    int max_hearth = 157286400;
                    //sobrescreve na memoria o valor da vida
                    memory.WriteInt32((IntPtr)finalAdress + 0x94, max_hearth);
                }
            }
        }

        private void code2()
        {
            Memoria.Iniciar("game");
            //pega a adress do game
            int baseAdress = Memoria.PegarBase("game", "game");
            //captura a adress estatica do jogo, atráves dessa adress capturo e modifico valor da vida e outros valores trocando a offset.
            int finalAdress = Memoria.Ler<int>((baseAdress + 0x0045D20C), 4);

            //captura o valor da vida atraves da offset 0x94
            int hearth = Memoria.Ler<int>(finalAdress + 0x94, 4);

            //valor da vida 100%
            int max_hearth = 157286400;
            //escreve na memoria.
            Memoria.Escrever(max_hearth, ((IntPtr)finalAdress + 0x94).ToInt32());
        }
    }
}


/*  
//adress statica
"game.exe"+0045D20C 
ou
0085D20C

//offset
94 = vida

Adress statica short

copiar o point > "game.exe"+0045D20C
Botao Direito em cima > Browse this memory region
Botao Direito > Go To Adress > Colar "game.exe"+0045D20C
Add to Code List > From: 0085D20C (ESSE É O ADRESS)

 */
