using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Input;
using System.Threading;


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
        public char proxDirecao;
        private char direcaoAnterior;
        private MenuInicial frm_MenuInicio;
        private int vidas = 3;

        //armazena as imagens para fazer a animação.
        private Image img_Atual;

        private Image srt_Esquerda;
        private Image srt_Direita;
        private Image srt_Cima;
        private Image srt_Baixo;

        //variavel para saber o tempo decorrido desde o ultimo movimento
        long timeUltMov;



        //para poder rotacionar as imagens
        //private float anguloAtual;
        //private float proxAngulo;
        //obj locker para nao dar ero
        static object _locker = new object();

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
            LoadImages(onde);
            this.proxDirecao = 'd';
            this.direcaoAnterior = 'd';
            timeUltMov = DateTime.Now.Millisecond;
        }


        //Carrega as imagens do player.
        private void LoadImages(int onde)
        {
            if (onde == 0)
            {
                //são as sprites do jogador de cor vermelha - sprites do jogador 0 Servidor.
                srt_Esquerda = ResizeImage.ScaleImage(Properties.Resources.srptEsquerda, tamX, tamX);
                srt_Direita = ResizeImage.ScaleImage(Properties.Resources.srptDireita, tamX, tamX);
                srt_Cima = ResizeImage.ScaleImage(Properties.Resources.srptCima, tamX, tamX);
                srt_Baixo = ResizeImage.ScaleImage(Properties.Resources.srptBaixo, tamX, tamX);
                img_Atual = srt_Direita;
            }
            else
            {
                srt_Esquerda = ResizeImage.ScaleImage(Properties.Resources.srptEsquerda2, tamX, tamX);
                srt_Direita = ResizeImage.ScaleImage(Properties.Resources.srptDireita2, tamX, tamX);
                srt_Cima = ResizeImage.ScaleImage(Properties.Resources.srptCima2, tamX, tamX);
                srt_Baixo = ResizeImage.ScaleImage(Properties.Resources.srptBaixo2, tamX, tamX);
                img_Atual = srt_Esquerda;
            }
            
        }


        private void DefineVelocidades()
        {
            this.velSemPowerUp =  AreaPlayers.CalcPercet(1f, this.larguraTela) / 60;

            this.velComPowerUp = AreaPlayers.CalcPercet(1.5f, this.larguraTela) / 60;

            this.velocidadeAtual = this.velSemPowerUp; //define a velocidade inicial do player.
        }


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


        /// <summary>
        /// Verifica a tecla que foi pressionada, verifica se esta ou nao tendo colisão com algum obj na tela.
        /// </summary>
        /// <param name="e">Resultado de um evento disparado do windows forms. </param>
        /// <param name="lstObs">Lista de obstaculos.</param>
        public bool Input(KeyPressEventArgs e, List<Obstaculo> lstObs, JogadorInimigo enemy)
        {
            /*
             * Verifica se está tendo colisão do player com algum obj ou se esta excedento os limites da tela antes de movimenta.
             * 
             * Considere o seguinte para as comparações:
             * 1º: verifica a tecla que foi pressionada.
             * 2º: verifica se o player ira sair dos limites da tela, e for sair, impede a movimentação. 
             * 3º: verifica se tem algum bloco no caminho. Se tiver, impede a movimentação.
             * 
             * ------------- VERIFICA SE O PLAYER IRA COMLIDIR COM O OUTRO PLAYER
             * 
             * Se esse player se colidir com o outro player, ele retorna um valor negativo e o programa envia para o outro programa
             * que havera uma colisão entre os dois jogadores, e interrompe a movimentação entre os dois jogadores.
             */

            //if (System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.Up))
            //{
            //    Console.WriteLine("key pressed");
            //}
            //if (e.KeyChar == 'w' && this.yAtual - this.velocidadeAtual > inicioTelaY && !HasBlock(lstObs, 'w', this.xAtual, this.yAtual - this.velocidadeAtual)) 
            if(Keyboard.IsKeyDown(Key.W) && this.yAtual - this.velocidadeAtual > inicioTelaY && !HasBlock(lstObs, 'w', this.xAtual, this.yAtual - this.velocidadeAtual))
            {
                if (ColisaoInimigo(enemy, this.xAtual, this.yAtual - this.velocidadeAtual))
                    return false;

                this.proxDirecao = 'c';
                EnviaMsgMov(this.xAtual, this.yAtual - this.velocidadeAtual);  
            }

            //if (e.KeyChar == 's' && this.yAtual + this.velocidadeAtual < this.alturaTela - this.tamX && !HasBlock(lstObs, 's', this.xAtual, this.yAtual + this.velocidadeAtual) )
            if (Keyboard.IsKeyDown(Key.S) && this.yAtual + this.velocidadeAtual < this.alturaTela - this.tamX && !HasBlock(lstObs, 's', this.xAtual, this.yAtual + this.velocidadeAtual) )
            {
                if (ColisaoInimigo(enemy, this.xAtual, this.yAtual + this.velocidadeAtual))
                    return false;

                this.proxDirecao = 'b';
                EnviaMsgMov(this.xAtual, this.yAtual + this.velocidadeAtual);                
            }


            if (Keyboard.IsKeyDown(Key.A) && this.xAtual - this.velocidadeAtual > 0 && !HasBlock(lstObs, 'a', this.xAtual - this.velocidadeAtual, this.yAtual) )
            {
                if (ColisaoInimigo(enemy, this.xAtual - this.velocidadeAtual, this.yAtual))
                    return false;

                this.proxDirecao = 'e';
                EnviaMsgMov(this.xAtual - this.velocidadeAtual, this.yAtual);                
            }


            if (Keyboard.IsKeyDown(Key.D) && this.xAtual + this.velocidadeAtual < this.larguraTela - this.tamX && !HasBlock(lstObs, 'd', this.xAtual + this.velocidadeAtual, this.yAtual))
            {
                if (ColisaoInimigo(enemy, this.xAtual + this.velocidadeAtual, this.yAtual))
                    return false;

                this.proxDirecao = 'd';
                EnviaMsgMov(this.xAtual + this.velocidadeAtual, this.yAtual);                
            }

            return true;           
        }


        /// <summary>
        /// Verifica a tecla que foi pressionada, verifica se esta ou nao tendo colisão com algum obj na tela.
        /// </summary>
        /// <param name="e">Resultado de um evento disparado do windows forms. </param>
        /// <param name="lstObs">Lista de obstaculos.</param>
        [STAThread]
        public bool Input(List<Obstaculo> lstObs, JogadorInimigo enemy)
        {
            /*
             * Verifica se está tendo colisão do player com algum obj ou se esta excedento os limites da tela antes de movimenta.
             * 
             * Considere o seguinte para as comparações:
             * 1º: verifica a tecla que foi pressionada.
             * 2º: verifica se o player ira sair dos limites da tela, e for sair, impede a movimentação. 
             * 3º: verifica se tem algum bloco no caminho. Se tiver, impede a movimentação.
             * 
             * ------------- VERIFICA SE O PLAYER IRA COMLIDIR COM O OUTRO PLAYER
             * 
             * Se esse player se colidir com o outro player, ele retorna um valor negativo e o programa envia para o outro programa
             * que havera uma colisão entre os dois jogadores, e interrompe a movimentação entre os dois jogadores.
             */

            //if (System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.Up))
            //{
            //    Console.WriteLine("key pressed");
            //}
            //if (e.KeyChar == 'w' && this.yAtual - this.velocidadeAtual > inicioTelaY && !HasBlock(lstObs, 'w', this.xAtual, this.yAtual - this.velocidadeAtual)) 
            
            if (Keyboard.IsKeyDown(Key.W) && this.yAtual - this.velocidadeAtual > inicioTelaY && !HasBlock(lstObs, 'w', this.xAtual, this.yAtual - this.velocidadeAtual))
            {
                if (ColisaoInimigo(enemy, this.xAtual, this.yAtual - this.velocidadeAtual))
                    return false;

                this.proxDirecao = 'c';
                EnviaMsgMov(this.xAtual, this.yAtual - this.velocidadeAtual);
            }

            //if (e.KeyChar == 's' && this.yAtual + this.velocidadeAtual < this.alturaTela - this.tamX && !HasBlock(lstObs, 's', this.xAtual, this.yAtual + this.velocidadeAtual) )
            if (Keyboard.IsKeyDown(Key.S) && this.yAtual + this.velocidadeAtual < this.alturaTela - this.tamX && !HasBlock(lstObs, 's', this.xAtual, this.yAtual + this.velocidadeAtual))
            {
                if (ColisaoInimigo(enemy, this.xAtual, this.yAtual + this.velocidadeAtual))
                    return false;

                this.proxDirecao = 'b';
                EnviaMsgMov(this.xAtual, this.yAtual + this.velocidadeAtual);
            }


            if (Keyboard.IsKeyDown(Key.A) && this.xAtual - this.velocidadeAtual > 0 && !HasBlock(lstObs, 'a', this.xAtual - this.velocidadeAtual, this.yAtual))
            {
                if (ColisaoInimigo(enemy, this.xAtual - this.velocidadeAtual, this.yAtual))
                    return false;

                this.proxDirecao = 'e';
                EnviaMsgMov(this.xAtual - this.velocidadeAtual, this.yAtual);
            }


            if (Keyboard.IsKeyDown(Key.D) && this.xAtual + this.velocidadeAtual < this.larguraTela - this.tamX && !HasBlock(lstObs, 'd', this.xAtual + this.velocidadeAtual, this.yAtual))
            {
                if (ColisaoInimigo(enemy, this.xAtual + this.velocidadeAtual, this.yAtual))
                    return false;

                this.proxDirecao = 'd';
                EnviaMsgMov(this.xAtual + this.velocidadeAtual, this.yAtual);
            }

            return true;
        }


        private bool SaiDaTela(float pos, float larg)
        {
            return pos + this.velocidadeAtual > larg;
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
            Rectangle r1 = new Rectangle((int)this.xAtual, (int)this.yAtual, (int)this.tamX, (int)this.tamY);//retangulo do player
            Rectangle r2 = new Rectangle((int)area.x, (int)area.y, (int)area.largura, (int)area.altura);//retandulo da area
            bool inside = false;

            if (r1.X > r2.X && r1.Y > r2.Y && r1.X + r1.Width < r2.X + r2.Width && r1.Y + r1.Height < r2.Y + r2.Height)
                inside = true;
            else
                inside = false;

            return inside;
        }


        /// <summary>
        /// Para quando o jogaodr ficar inativo, ele muda a velocidade pra a velocidade normal do jogador quando ele nascer de novo.
        /// </summary>
        public void MudaVelNormal()
        {
            this.velocidadeAtual = this.velSemPowerUp;
        }


        //-------- MESAGENS REFERENTES A COMUNICAÇÃO DE REDES --------

        public bool ColisaoInimigo(JogadorInimigo jogIni, float nX, float nY)
        {
            /*
             * Verifica se o player local ira se colidir com o outro player. 
             * Se for colidir, retorna um valor verdadeiro.
            */
            return Colisao(jogIni.GetX, jogIni.GetY, jogIni.GetTam, jogIni.GetTam, nX, nY);            
        }


        /// <summary>
        /// Muda a velocidade atual para velocidade com o power up. Quando o jogador pegar o power up, ele muda a velocidade de movimento.
        /// </summary>
        public void MudaVelPwUp()
        {
            this.velocidadeAtual += this.velComPowerUp;
        }


        public void Draw(Graphics g)
        {
            Rectangle r = new Rectangle((int)xAtual, (int)yAtual, (int)img_Atual.Width, (int)img_Atual.Height);
            g.DrawImage(img_Atual, r);
        }


        public void Movimenta()
        {
            /*
             * Verificar se o proximo movimento do jogador esta na mesma direção
             * que o movimento anteriro. se nao tiver, rotaciona o player e depois movimenta o jogador.
            */
            
            switch (proxDirecao)
            {
                case 'c':
                    img_Atual = srt_Cima;
                    this.yAtual -= CalculaMovimento();
                    timeUltMov = DateTime.Now.Millisecond;
                    break;
                case 'b':
                    img_Atual = srt_Baixo;
                    this.yAtual += CalculaMovimento();
                    timeUltMov = DateTime.Now.Millisecond;
                    break;
                case 'e':
                    img_Atual = srt_Esquerda;
                    this.xAtual -= CalculaMovimento();
                    timeUltMov = DateTime.Now.Millisecond;
                    break;
                case 'd':
                    img_Atual = srt_Direita;
                    this.xAtual += CalculaMovimento();
                    timeUltMov = DateTime.Now.Millisecond;
                    break;
            }
        }


        private float CalculaMovimento()
        {
            long ta = DateTime.Now.Millisecond;
            long tc = ta - timeUltMov;
            return (tc / 1000) * velocidadeAtual;
        }



        #region
        private void EnviaMsgMov(float nPX, float nPy)
        {
            float[] posSend = ConvertDispositivoNormal(xAtual, yAtual);
            string aux = string.Format("{0}|{1}|{2}", posSend[0], posSend[1], this.proxDirecao);
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
            this.velocidadeAtual = this.velSemPowerUp;
            ForcaEnvioPos();
        }


        public void ForcaEnvioPos()
        {
            EnviaMsgMov(this.xAtual, this.yAtual);
        }


        /*
        * Converte de coordenadas normalizadas para coordenadas da tela.
        */
        public float[] ConvertNormalDispositivo(float x, float y)
        {
            float[] pos = new float[2];

            pos[0] = x * this.larguraTela;//representa X
            pos[1] = y * this.alturaTela;//representa Y

            return pos;
        }


        /*
         * Converte de coordenadas do dispositivo para coordenadas normalizadas.
         */
        public float[] ConvertDispositivoNormal(float x, float y)
        {
            float[] pos = new float[2];

            pos[0] = x / this.larguraTela;//representa X
            pos[1] = y / this.alturaTela;//representa Y

            return pos;
        }

        #endregion

        //Verifica se o jogador esta virado para a mesma direção.
        private bool DirecaoCorreta(char dir)
        {
            return dir.Equals(direcaoAnterior);
        }
    }
}