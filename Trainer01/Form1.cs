using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace Trainer01
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();            
        }
                
        int finalAdress = 0;
        int max_hearth = 157286400;
        int hearth = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            //se o game nao estiver aberto
            if (!Memoria.CheckProcesso("game"))
            {
                label2.Text = "Abra o game!";
                return;
            }

            if (finalAdress == 0)
            {
                Memoria.Iniciar("game");
                //pega a adress do game;
                int baseAdress = Memoria.PegarBase("game", "game");

                //captura a adress estatica do jogo, atráves dessa adress capturo e modifico valor da vida e outros valores trocando a offset.
                finalAdress = Memoria.Ler<int>((baseAdress + 0x0045D20C), 4);
                checkBox1.Enabled = button1.Enabled = finalAdress != 0;
            }
            
            //vida atual do leon
            hearth = Memoria.Ler<int>(finalAdress + 0x94, 4);
            int porcentVida = (hearth) / (max_hearth / 100);
            label2.Text = "Vida: " + porcentVida + "%";
            
            if (hearth < max_hearth && checkBox1.Checked)
            {
                //escreve na memoria. vida + 1% da vida, regenera a cada segundo
                Memoria.Escrever(hearth + 1572864, ((IntPtr)finalAdress + 0x94).ToInt32());
            }
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            Memoria.Escrever(max_hearth, ((IntPtr)finalAdress + 0x94).ToInt32());
        }
    }
}

