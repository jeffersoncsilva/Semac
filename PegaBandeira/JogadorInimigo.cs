using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PegaBandeira
{
    class JogadorInimigo
    {
        private float posUniverX;
        private float posUniverY;
        private float posLocalX;
        private float posLocalY;
        private float widthJogador;
        private float heightJogador;

        private float widthScreen;
        private float heightScreen;

        /// <summary>
        /// Metodo construtor. Cria o objeto com seu respectivo tamanho.
        /// </summary>
        /// <param name="wi"></param>
        /// <param name="hei"></param>
        public JogadorInimigo(float wi, float hei, float wS, float hS)
        {
            this.widthJogador = wi;
            this.heightJogador = hei;
            this.widthScreen = wS;
            this.heightScreen = hS;
        }



        /// <summary>
        /// Define a posição em coordenadas universal.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetPosicao(float x, float y)
        {
            this.posUniverX = x;
            this.posUniverY = y;
        }

        
        /// <summary>
        /// Desenha o jogador na posição indicada.
        /// </summary>
        /// <param name="g"></param>
        public void Draw(Graphics g)
        {
            ConvertPos();
            g.FillRectangle(new SolidBrush(Color.Red), this.posLocalX, this.posLocalY, this.widthJogador, this.heightJogador);
        }


        /// <summary>
        /// Converte a posição do jogador de coordenadas universais para coordenadas "normais".
        /// </summary>
        private void ConvertPos()
        {
            this.posLocalX = this.posUniverX * this.widthScreen;
            this.posLocalY = this.posUniverY * this.heightScreen;
        }
    }
}