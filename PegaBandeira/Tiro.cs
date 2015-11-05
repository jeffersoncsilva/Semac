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
        public static int id_Tiro = 1;

        private float velTiro;
        public bool colidiu;
        private char dir;
        private Thread tMov;
        private int _id;

        public float GetX { get { return this.xAtual; } }
        public float GetY { get { return this.yAtual; } }
        public char GetDirecao { get { return this.dir; } }
        public int GetId { get { return this._id; } }


        /// <summary>
        /// Cria o tiro local.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="tam"></param>
        /// <param name="largTela"></param>
        /// <param name="direcao"></param>
        /// <param name="id"></param>
        public Tiro(float x, float y, float tam, float largTela, char direcao, int id)
        {
            DefineTamTiro(tam);
            DefinePosTiro(x, y, tam);
            this.velTiro = AreaPlayers.CalcPercet(0.25f, largTela);
            this._id = id;
            this.dir = direcao;
            colidiu = true;
        }


        /// <summary>
        /// Construtor do tiro do inimigo.
        /// </summary>
        /// <param name="dados"></param>
        /// <param name="tam"></param>
        /// <param name="larTela"></param>
        public Tiro(float[] posicao, float tam, float largTela, char direcao, int id)
        {
            this.xAtual = posicao[0];
            this.yAtual = posicao[1];
            this.dir = direcao;
            this._id = id;
            DefineTamTiro(tam);
            this.velTiro = AreaPlayers.CalcPercet(0.25f, largTela);
        }


        public void PodeIr()
        {
            Vai();
            colidiu = false;
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
                Thread.Sleep(15);
                if(!colidiu)
                    colidiu = SaiuTela();
            }
        }


        public bool Colisao(Rectangle rect)
        {
            return (this.xAtual + this.tamX > rect.X &&
                    this.xAtual < rect.X + rect.Width &&
                    this.yAtual + this.tamX > rect.Y &&
                    this.yAtual < rect.Y + rect.Height);
        }


        public bool SaiuTela()
        {
            return (this.xAtual > CampoBatalha.LARGURA || this.xAtual < 0 ||
                    this.yAtual > CampoBatalha.ALTURA || this.yAtual < 0);
        }


    }
}