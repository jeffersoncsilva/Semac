using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PegaBandeira
{
    /*
 * Trabalho desenvolvido na integração das disciplinas
 * Programação Concorrente 
 * Oficina de Jogos Multiplayer 
 * Redes de Computadores
 * 
 * Alunos:
 * Jefferson C. Silva
 * Vinicios Coelho
 */
    class Obstaculo : ElementoJogo
    {
        private int coluna;
        private int fileira;
        private int bloco;

        public int GetColuna { get { return this.coluna; } }
        public int GetFileira { get { return this.fileira; } }
        public int GetBloco { get { return this.bloco; } }


        Image barreira;


        public Obstaculo(float x, float y, float tX, float tY, int coluna, int fileira, int bloco)
        {
            this.xInicial = x;
            this.yInicial= y;
            this.xAtual = this.xInicial;
            this.yAtual = this.yInicial;
            this.tamX = tX;
            this.tamY = tY;
            this.mostrarInicial = true;
            this.mostrarAtual = this.mostrarInicial;
            this.coluna = coluna;
            this.fileira = fileira;
            this.bloco = bloco;

            barreira = ResizeImage.ScaleImage(Properties.Resources.blocoBarreia, tamX, tamY);

        }

        public void Draw(Graphics g)
        {
            if (this.mostrarAtual)
            {
                g.DrawImage(barreira, xAtual, yAtual);
                //g.FillRectangle(new SolidBrush(Color.Red), this.xAtual, this.yAtual, this.tamX, this.tamY);
            }
        }
    }
}