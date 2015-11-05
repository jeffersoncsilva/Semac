﻿using System;
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

        private bool pegouBandeira;
        private bool pegouPowerUp;

        public float GetX { get { return this.posLocalX; } }
        public float GetY { get { return this.posLocalY; } }
        public float GetTam { get { return this.widthJogador; } }
        public bool PegouBand { get { return this.pegouBandeira; } set { this.pegouBandeira = value; } }
        public bool PegouPowerUp { get { return this.pegouPowerUp; } set { this.pegouPowerUp = value; } }

        /// <summary>
        /// Metodo construtor. Cria o objeto com seu respectivo tamanho.
        /// </summary>
        /// <param name="wi"></param>
        /// <param name="hei"></param>
        public JogadorInimigo(float wi, float hei, float wS, float hS, int tipo)
        {
            this.widthJogador = wi;
            this.heightJogador = hei;
            this.widthScreen = wS;
            this.heightScreen = hS;
            DefinePosicaoInicial(tipo);
        }


        private void DefinePosicaoInicial(int tipo)
        {
            //lado esquerdo.
            if (tipo == 1)
            {
                this.posLocalX = ((AreaPlayers.CalcPercet(10, this.widthScreen)) / 2) - (this.widthJogador / 2);
            }
            else
            {
                //lado direito.
                this.posLocalX = (((AreaPlayers.CalcPercet(10, this.widthScreen)) / 2) - (this.widthJogador / 2) + AreaPlayers.CalcPercet(90, this.widthScreen));
            }
            this.posLocalY = (this.heightScreen / 2) - (this.heightJogador / 2);
        }


        /// <summary>
        /// Define a posição em coordenadas universal.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetPosicao(float[] newPos)
        {
            //this.posUniverX = x;
            //this.posUniverY = y;
            //ConvertPos();

            posLocalX = newPos[0];//representa x
            posLocalY = newPos[1];//representa y
        }

        
        /// <summary>
        /// Desenha o jogador na posição indicada.
        /// </summary>
        /// <param name="g"></param>
        public void Draw(Graphics g)
        {
            //Console.WriteLine("PD: {0} | {1}", this.posLocalX, this.posLocalY);
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


        /// <summary>
        /// Verifica se o player esta dentro da area recebida. (Area de jogo do player local).
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        public bool Dentro(PosRect area)
        {
            if (this.posLocalX + this.widthJogador > area.x &&
              this.posLocalX > area.x &&
              this.posLocalX + this.widthJogador < area.largura &&
              this.posLocalX < area.largura &&
              this.posLocalY + this.heightJogador > area.y &&
              this.posLocalY > area.y &&
              this.posLocalY + this.heightJogador < area.altura &&
              this.posLocalY < area.altura)
            {
                return true;
            }
            else
                return false;
        }


    }

}