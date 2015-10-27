﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PegaBandeira
{
    class Player : ElementoJogo
    {
        private float velSemPowerUp;
        private float velComPowerUp;
        private float velocidadeAtual;

        private bool pegouBandeira;
        private bool pegouPowerUp;
        //REPRESENTA A LARGURA E ALTURA DA TELA DE JOGO.
        private float larguraTela;
        private float alturaTela;
        private float inicioTelaY;
        public char direcaoBala;


        public bool PegouBand { get { return this.pegouBandeira; } set { this.pegouBandeira = value; } }
        public bool PegouPowerUp { get { return this.pegouPowerUp; } set { this.pegouPowerUp = value; } }

        public Player(int larg, int alt)
        {
            this.larguraTela = larg;
            this.alturaTela = AreaPlayers.CalcPercet(90, alt);
            DefineAreaDeJogo(larg, alt);
            DefineTamPlayer();
            DefinePosInicial();
            DefineVelocidades();
            
            this.direcaoBala = 'd';
        }


        private void DefineVelocidades()
        {
            this.velSemPowerUp =  AreaPlayers.CalcPercet(1f, this.larguraTela);

            this.velComPowerUp = AreaPlayers.CalcPercet(1.5f, this.larguraTela);

            this.velocidadeAtual = this.velSemPowerUp; //define a velocidade inicial do player.
        }



        #region CONFIGURACAO PLAYER
        private void DefineAreaDeJogo(int larg, int alt)
        {
            this.inicioTelaY = AreaPlayers.CalcPercet(10, this.alturaTela);
        }

        private void DefinePosInicial()
        {
            //Pego a largura da area do jogador, divido no meio, e diminuo o equivalente a metade do tamanho do jogador.
            this.xInicial = ((AreaPlayers.CalcPercet(10, this.larguraTela)) / 2) - (this.tamX / 2);
            //pego a altura da area  de jogo e divido ao meio.
            this.yInicial = this.alturaTela / 2;
            this.xAtual = this.xInicial;
            this.yAtual = this.yInicial;
        }

        private void DefineTamPlayer()
        {
            this.tamX = AreaPlayers.CalcPercet(5f, this.alturaTela);
            this.tamY = this.tamX;
        }

        #endregion


        /// <summary>
        /// Verifica a tecla que foi pressionada, verifica se esta ou nao tendo colisão com algum obj na tela.
        /// </summary>
        /// <param name="e">Resultado de um evento disparado do windows forms. </param>
        /// <param name="lstObs">Lista de obstaculos.</param>
        public void Input(KeyPressEventArgs e, List<Obstaculo> lstObs)
        {
            /*
             * Verifica se está tendo colisão do player com algum obj ou se esta excedento os limites da tela antes de movimenta.
             */
            if (e.KeyChar == 'w' && this.yAtual > inicioTelaY && !HasBlock(lstObs, 'w', this.xAtual, this.yAtual - this.velocidadeAtual))
            {
                this.yAtual -= this.velocidadeAtual;
                this.direcaoBala = 'w';
            }

            if (e.KeyChar == 's' && this.yAtual < this.alturaTela - this.tamX && !HasBlock(lstObs, 's', this.xAtual, this.yAtual + this.velocidadeAtual))
            {
                this.yAtual += this.velocidadeAtual;
                this.direcaoBala = 's';
            }

            if (e.KeyChar == 'a' && this.xAtual > 0 && !HasBlock(lstObs, 'a', this.xAtual - this.velocidadeAtual, this.yAtual))
            {
                this.xAtual -= this.velocidadeAtual;
                this.direcaoBala = 'a';
            }

            if (e.KeyChar == 'd' && this.xAtual < this.larguraTela - this.tamX && !HasBlock(lstObs, 'd', this.xAtual + this.velocidadeAtual, this.yAtual))
            {
                this.xAtual += this.velocidadeAtual;
                this.direcaoBala = 'd';
            }
        }


        public void Draw(Graphics g)
        {
            SolidBrush b = new SolidBrush(Color.Red);
            g.FillRectangle(b, this.xAtual, this.yAtual, this.tamX, this.tamX);
        }


        public bool HasBlock(List<Obstaculo> lobs, char dir, float nX, float nY)
        {
            switch (dir)
            {
                case 'w': 
                    return Colide(lobs, nX, nY);

                case 's':
                    return Colide(lobs, nX, nY);

                case 'a':
                    return Colide(lobs, nX, nY);

                case 'd':
                    return Colide(lobs, nX, nY);
                default:
                    return false;
            }
            
        }


        private bool Colide(List<Obstaculo> obs, float nX, float nY)
        {
            foreach (var v in obs)
                if (Colisao(v.xAtual, v.yAtual, v.tamX, v.tamY, nX, nY) && v.mostrarAtual)
                    return true;

            return false;
        }


        /// <summary>
        /// Verifica se esta havendo colisão com os dados indicados. Colisão por retangulos.
        /// </summary>
        /// <param name="x">Posicao inicial do retangulo em X.</param>
        /// <param name="y">Posicao inicial do retangulo em Y.</param>
        /// <param name="tamX">Largura do retangulo.</param>
        /// <param name="tamY">Altura do retangulo.</param>
        /// <returns></returns>
        public bool Colisao(float x, float y, float tamX, float tamY)
        {
            return (this.xAtual + this.tamX > x &&
                    this.xAtual < x + tamX &&
                    this.yAtual + this.tamX > y &&
                    this.yAtual < y + tamY);
        }


        /// <summary>
        /// Simula uma colisão com a area passada e a uma possivel posição do player.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="tamX"></param>
        /// <param name="tamY"></param>
        /// <param name="nX"></param>
        /// <param name="nY"></param>
        /// <returns></returns>
        public bool Colisao(float x, float y, float tamX, float tamY, float nX, float nY)
        {
            return (nX + this.tamX > x &&
                    nX < x + tamX &&
                    nY + this.tamX > y &&
                    nY < y + tamY);
        }


        /// <summary>
        /// Verifica se o player esta dentro da area recebida. (Area de jogo do player local).
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        public bool Dentro(PosRect area)
        {
            if (this.xAtual + this.tamX > area.x &&
              this.xAtual > area.x &&
              this.xAtual + this.tamX < area.largura &&
              this.xAtual < area.largura &&
              this.yAtual + this.tamY > area.y &&
              this.yAtual > area.y &&
              this.yAtual + this.tamY < area.altura &&
              this.yAtual < area.altura)
            {
                return true;
            }
            else
                return false;
        }


        /// <summary>
        /// Muda a velocidade atual para velocidade com o power up. Quando o jogador pegar o power up, ele muda a velocidade de movimento.
        /// </summary>
        public void MudaVelPwUp()
        {
            this.velocidadeAtual = this.velComPowerUp;
        }


        /// <summary>
        /// Para quando o jogaodr ficar inativo, ele muda a velocidade pra a velocidade normal do jogador quando ele nascer de novo.
        /// </summary>
        public void MudaVelNormal()
        {
            this.velocidadeAtual = this.velSemPowerUp;
        }

    }
}