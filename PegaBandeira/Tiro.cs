using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;

namespace PegaBandeira
{
    class Tiro : ElementoJogo
    {

        private float velTiro;
        public bool colidiu;
        private char dir;
        private Thread tMov;

        public Tiro(float x, float y, float tam, float largTela, char direcao)
        {
            DefineTamTiro(tam);
            DefinePosTiro(x, y, tam);
            this.velTiro = AreaPlayers.CalcPercet(0.25f, largTela);
            this.dir = direcao;
            Vai();
        }

        private void DefineTamTiro(float t)
        {
            this.tamX = AreaPlayers.CalcPercet(25f, t);
        }

        private void DefinePosTiro(float x, float y, float tam)
        {
            this.xAtual = x + (tam / 2);
            this.yAtual = y + (tam / 2);
        }

        public void Draw(Graphics g)
        {
            if(!colidiu)
                g.FillEllipse(new SolidBrush(Color.Black), this.xAtual, this.yAtual, this.tamX, this.tamX);
        }


        public void Vai()
        {
            this.tMov = new Thread(() => Movimento());
            this.tMov.Start();
        }


        private void Movimento()
        {            
            while (!colidiu)
            {
                if(this.dir == 'd')
                    this.xAtual += this.velTiro;

                if (this.dir == 'e')
                    this.xAtual -= this.velTiro;

                if (this.dir == 'b')
                    this.yAtual += this.velTiro;

                if (this.dir == 'c')
                    this.yAtual -= this.velTiro; 

                Thread.Sleep(14);
            }
        }

        public bool Colisao(Rectangle rect)
        {
            return (this.xAtual + this.tamX > rect.X &&
                    this.xAtual < rect.X + rect.Width &&
                    this.yAtual + this.tamX > rect.Y &&
                    this.yAtual < rect.Y + rect.Height);
        }

        public bool SaiuTela(int larg, int alt)
        {
            return (this.xAtual > larg || this.xAtual < 0 ||
                    this.yAtual > alt || this.yAtual < 0);
        }
    }
}