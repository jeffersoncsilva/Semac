using System;
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
        public char direcaoJogador;

        private MenuInicial frm_MenuInicio;
        private int vidas = 3;

        public int GetVida { get { return this.vidas; } }

        public bool PegouBand { get { return this.pegouBandeira; } set { this.pegouBandeira = value; } }
        public bool PegouPowerUp { get { return this.pegouPowerUp; } set { this.pegouPowerUp = value; } }
       
        public Player(int larg, int alt, int onde, MenuInicial b)
        {
            this.larguraTela = larg;
            this.alturaTela = alt;
            this.frm_MenuInicio = b;
            DefineAreaDeJogo(larg, alt);
            DefineTamPlayer();
            DefinePosInicial(onde);
            DefineVelocidades();

            this.direcaoJogador = 'd';
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

        private void DefinePosInicial(int onde)
        {
            if (onde == 0)//player na esquerda e inimigo na direita.
            {
                //Pego a largura da area do jogador, divido no meio, e diminuo o equivalente a metade do tamanho do jogador.
                this.xInicial = ((AreaPlayers.CalcPercet(10, this.larguraTela)) / 2) - (this.tamX / 2);
            }
            else//player na direita e inimigo na esquerda.
            {
                //Pego a largura da area do jogador, divido no meio, e diminuo o equivalente a metade do tamanho do jogador, Depois soma onde inicia a area do outro jogador.
                this.xInicial = (((AreaPlayers.CalcPercet(10, this.larguraTela)) / 2) - (this.tamX / 2) + AreaPlayers.CalcPercet(90, this.larguraTela));
                //pego a altura da area  de jogo e divido ao meio.
                
            }
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
             * 
             * Considere o seguinte para as comparações:
             * 1º: verifica a tecla que foi pressionada.
             * 2º: verifica se o player ira sair dos limites da tela, e for sair, impede a movimentação. 
             * 3º: verifica se tem algum bloco no caminho. Se tiver, impede a movimentação.
             */


            if (e.KeyChar == 'w' && this.yAtual - this.velocidadeAtual > inicioTelaY && !HasBlock(lstObs, 'w', this.xAtual, this.yAtual - this.velocidadeAtual)) 
            {
                this.direcaoJogador = 'c';
                EnviaMsgMov(this.xAtual, this.yAtual - this.velocidadeAtual);
               //Movimenta();
            }

            if (e.KeyChar == 's'  && this.yAtual + this.velocidadeAtual < this.alturaTela - this.tamX && !HasBlock(lstObs, 's', this.xAtual, this.yAtual + this.velocidadeAtual))
            {
                this.direcaoJogador = 'b';
                EnviaMsgMov(this.xAtual, this.yAtual + this.velocidadeAtual);
                //Movimenta();
            }

            if (e.KeyChar == 'a' &&  this.xAtual - this.velocidadeAtual > 0 && !HasBlock(lstObs, 'a', this.xAtual - this.velocidadeAtual, this.yAtual))
            {
                this.direcaoJogador = 'e';
                EnviaMsgMov(this.xAtual - this.velocidadeAtual, this.yAtual);
                //Movimenta();
            }

            if (e.KeyChar == 'd' && this.xAtual + this.velocidadeAtual < this.larguraTela - this.tamX && !HasBlock(lstObs, 'd', this.xAtual + this.velocidadeAtual, this.yAtual))
            {
                this.direcaoJogador = 'd';
                EnviaMsgMov(this.xAtual + this.velocidadeAtual, this.yAtual);
                //Movimenta();
            }
           
        }


        private bool SaiDaTela(float pos, float larg)
        {
            return pos + this.velocidadeAtual > larg;
        }


        #region PLAYER_MOV

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
        /// Para quando o jogaodr ficar inativo, ele muda a velocidade pra a velocidade normal do jogador quando ele nascer de novo.
        /// </summary>
        public void MudaVelNormal()
        {
            this.velocidadeAtual = this.velSemPowerUp;
        }
        
        #endregion

        //-------- MESAGENS REFERENTES A COMUNICAÇÃO DE REDES --------



        /// <summary>
        /// Muda a velocidade atual para velocidade com o power up. Quando o jogador pegar o power up, ele muda a velocidade de movimento.
        /// </summary>
        public void MudaVelPwUp()
        {
            this.velocidadeAtual += this.velComPowerUp;
        }



        public void Movimenta()
        {
            switch (direcaoJogador)
            {
                case 'c':
                    this.yAtual -= this.velocidadeAtual;
                    break;
                case 'b':
                    this.yAtual += this.velocidadeAtual;
                    break;
                case 'e':
                    this.xAtual -= this.velocidadeAtual;
                    break;
                case 'd':
                    this.xAtual += this.velocidadeAtual;
                    break;
            }
        }


        private void EnviaMsgMov(float nPX, float nPy)
        {
            //float xUni = this.xAtual;
            //float yUni = this.yAtual;
            string aux = string.Format("{0}|{1}|{2}", nPX, nPy, this.direcaoJogador);
            int v = aux.Length + 5;
            string msg = string.Format("11{0}{1}", v.ToString("000"), aux);
            
            this.frm_MenuInicio.EnviaMsgTcp(msg);
        }




        public void ApplyDamange()
        {
            this.vidas -= 1;            
        }        


        public bool TemVida()
        {
            return this.vidas == 0;
        }


        public override void Reestart()
        {
            xAtual = xInicial;
            yAtual = yInicial;
            mostrarAtual = mostrarInicial;
            this.vidas = 3;
        }


    }
}