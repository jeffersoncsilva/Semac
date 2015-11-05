using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PegaBandeira
{
    class Interface
    {
        
        private PosRect hud;
        private Label nomeJog;
        private int altura;
        private int largura;
        private Label[] labels;

        public Interface(int larg, int alt)
        {
            this.altura = alt;
            this.largura = larg;
            hud = new PosRect();
            hud.largura = larg;
            hud.altura = AreaPlayers.CalcPercet(10, alt);
            hud.x = 0;
            hud.y = 0;
            nomeJog = new Label();
            nomeJog.Text = "Teste.";
            nomeJog.Location = new Point(15, 10);
        }

        public Label GetLabel { get { return this.nomeJog; } }


        public void ConfgLabels(Label[] labs)
        {
            //label de nome do jogador.
            labs[0].Location = new Point((int)AreaPlayers.CalcPercet(1.5f, this.altura), (int)AreaPlayers.CalcPercet(1, this.largura));
            labs[0].AutoSize = false;
            labs[0].Size = new Size(150, 20);
            labs[0].Text = "Jogador: ";

            //LABEL DO PLACAR DO JOGO.
            labs[1].Location =  new Point((int)AreaPlayers.CalcPercet(60, this.altura), (int)AreaPlayers.CalcPercet(0.5f, this.largura));
            labs[1].AutoSize = false;
            labs[1].Size = new Size(80, 20);
            labs[1].Text = "Placar: 0 x 0";
            labs[1].TextAlign = ContentAlignment.MiddleCenter;

            //  LABEL DE TEMPO RESTANTE DA PARTIDA
            labs[2].Location = new Point((int)AreaPlayers.CalcPercet(57, this.altura), (int)AreaPlayers.CalcPercet(3f, this.largura));
            labs[2].AutoSize = false;
            labs[2].Size = new Size(150, 30);
            labs[2].Text = CampoBatalha.TIMEOUTSTRING + CampoBatalha.TIMEOUT + " s";
            labs[2].TextAlign = ContentAlignment.MiddleCenter;

            //LABEL DE TEMPO RESTANTE DE INATIVIDADE DA PARTIDA.
            labs[3].Location = new Point((int)AreaPlayers.CalcPercet(110, this.altura), (int)AreaPlayers.CalcPercet(1.8f, this.largura));
            labs[3].AutoSize = false;
            labs[3].Size = new Size(120, 30);
            labs[3].Text = "INATIVO!!  Volta em: ... segundos. ";
            labs[3].TextAlign = ContentAlignment.MiddleCenter;
            labs[3].Visible = false;

            //LABEL QUE E MOSTRADA QUANDO O JOGO ACABA.
            labs[4].AutoSize = false;
            labs[4].Size = new Size(600, 300);
            //int x = (int)(this.largura / 2) - (labs[4].Size.Width / 2);
            //int y = (int)(this.altura / 2) - (labs[4].Size.Width/2);
            //calcula primeiro o x, pega a largura total divide no meio e diminoi a metade da largura do label para poder centralizar
            //igual para Y.
            labs[4].Location = new Point(((int)(this.largura / 2) - (labs[4].Size.Width / 2)), ((int)(this.altura / 2) - (labs[4].Size.Height / 2)));
            labs[4].TextAlign = ContentAlignment.MiddleCenter;
            //labs[4].Text = "Fim de jogo. O jogador vencedor foi o (fulano). Sinto muito mas parece que voce foi o perdedor. Clique no boltão para poder voltar ao menu principal.";
            labs[4].Visible = false;

            //labs[4].Location = new Point((int));

            //Armazena os labels em um vetor para manipulalos depois.
            this.labels = labs;
        }

        /// <summary>
        /// Ativa o label que mostra o tempo de inatividade do jogador.
        /// </summary>
        public void AtivaLabelInativo(bool valor)
        {
            //label 3 do vetor e o label que ira mostra o tempo que o jogador ficara ativo.
            //ativa ou desativa a visibilidade do label dependendo do que for enviado.
            this.labels[3].Visible = valor;
        }


        public void Draw(Graphics g)
        {
            SolidBrush pen = new SolidBrush(Color.Red);
            g.FillRectangle(new SolidBrush(Color.LightGray), hud.x, hud.y, hud.largura, hud.altura);
        }


    }
    public class PosRect
    {
        public float x;
        public float y;
        public float largura;
        public float altura;
    }
}