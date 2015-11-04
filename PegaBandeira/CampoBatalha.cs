﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace PegaBandeira
{
    public partial class CampoBatalha : Form
    {
        private int vidasOutroPlayer = 3;
        private bool outroJogadorCongelado = false;

        #region
        //constantes para definir o tamanho da tela
        public const int LARGURA = 1024;
        public const int ALTURA = 800;

        public const int TIMEOUT = 120;
        public const string TIMEOUTSTRING = "Tempo resante da partida: \n";
        public const string TIMEENDSTRING = "Acabou a partida. A proxima partida inicia em: ";
        private int tempoRestante;
        private int proxPartida = 2;
        private bool endPartida;
        private bool congelado;
        private bool podeAtirar;
        private static readonly object _lock = new object();

        //necessario para fazer o desenho na tela.
        PictureBox pb;
        Bitmap surface;
        Graphics g;

        private MenuInicial frm_Inicio;

        //essa trhead fica redesenhando os obj na tela.
        private Thread desenha;

        //Referente aos players do jogo.
        private Player player;
        private AreaPlayers areaPlay;
        private Interface hud;
        private JogadorInimigo playerEnemy;

        private Bandeira[] bands;
        private int mB; //armazena o index da minha bandeira.
        private int qualPlayer; // 0 se for o player 1 servidor || 1 se for o player 2 cliente.
        private static List<ElementoJogo> elementosJogo;

        //private Tiro bullet = null;
        private List<Tiro> tiros;
        Tiro newTiro;

        private List<Tiro> tirosInimigos;

        private Thread colisaoBala;

        private List<Obstaculo> listaObs;

        private PowerUp powerUp;

        //o construtor abaixo e somente para teste. Quando testar em rede deve-se utilizar o outro. 
        public CampoBatalha()
        {
            //parte de configuração do formulario.
            InitializeComponent();
            ConfigPicBox();
            ConfigForm();
            ConfGraphics();
            InicializaVariaveis(1);
        }


        public CampoBatalha(MenuInicial m, int tipo)
        {
            InitializeComponent();
            this.frm_Inicio = m;
            ConfigPicBox();
            ConfigForm();
            ConfGraphics();
            InicializaVariaveis(tipo);
        }
        

        private void InicializaVariaveis(int qPlayer)
        {
            elementosJogo = new List<ElementoJogo>();   //cria a lista com todos os elementos do jogo.
            this.podeAtirar = true;                     //define que o jogador pode atirar.
            this.areaPlay = new AreaPlayers(this.pb.Size.Width, this.pb.Size.Height);   //cria a rea de jogo do player.
            //refere-se a HUD do jogo.
            this.hud = new Interface(this.pb.Size.Width, this.pb.Size.Height);      //cria a hud do jogo.
            Label[] lab = new Label[] { this.lbl_NomeJog, this.lbl_Placar, this.lbl_TempoRestante, this.lbl_Inativo };//pega as labels da hud do jogo que ta no form.
            this.hud.ConfgLabels(lab);  //define as posições e tamanhos das labels que tem no jogo.

            this.qualPlayer = qPlayer;
            this.player = new Player(this.pb.Size.Width, this.pb.Size.Height, this.qualPlayer, this.frm_Inicio);
            playerEnemy = new JogadorInimigo(this.player.tamX, this.player.tamY, this.pb.Size.Width, this.pb.Size.Height, this.qualPlayer);

            //thread de desenho.
            this.desenha = new Thread(() => Draw());
            this.desenha.Name = "DesenhaTela";
            this.desenha.Start();

            //timer que controle o tempo de jogo.
            this.tempoRestante = TIMEOUT;
            this.tm_UpdtTempoPartida.Start();

            //cria as bandeiras que ha no jogo.
            this.bands = new Bandeira[2];
            this.bands[0] = new Bandeira(this.areaPlay, 1);
            this.bands[1] = new Bandeira(this.areaPlay, 0);
            this.mB = qPlayer == 1 ? 0 : 1; //dis qual e a minha bandeira.

            //adicionas as bandeiras a lista de lemento de jogo e eo player.
            elementosJogo.Add(bands[0]);
            elementosJogo.Add(bands[1]);

            //cra a lista de tiros.
            this.tiros = new List<Tiro>();
            this.tirosInimigos = new List<Tiro>();

            //cria uma thread que ficara responsavel por verificar se houve alguma colisão do tiro com algum elemento do jogo.
            this.colisaoBala = new Thread(() => ColisaoDaBala());
            this.colisaoBala.Name = "Colicao_Balas";
            this.colisaoBala.Start();

            //cria uma lista onde ficara armazenado os obstaculos. Cria os obstaculos do jogo.
            listaObs = new List<Obstaculo>();
            CriaObstaculos();

            //cria o powerUp do jogo e adiciona a lista de elementos do jogo.
            this.powerUp = new PowerUp(this.pb.Size.Width, this.pb.Size.Height);//cria o powerUp do jogo e adiciona a lista de elementos do jogo.
            elementosJogo.Add(powerUp);

            //atualiza a posição para o outro jogador
            this.player.ForcaEnvioPos();
        }


        #region Configuracao do grafico.
        private void ConfigForm()
        {
            //this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Width = LARGURA;
            this.Height = ALTURA;
            this.Text = "Pega Bandeira - SEMAC";

            //this.WindowState = FormWindowState.Normal;
            //this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            //this.Bounds = Screen.PrimaryScreen.Bounds;


        }

        private void ConfigPicBox()
        {
            this.pb = new PictureBox();
            this.pb.Parent = this;
            this.pb.BackColor = Color.White;
            this.pb.Dock = DockStyle.Fill;
        }

        private void ConfGraphics()
        {
            this.surface = new Bitmap(pb.Size.Width, pb.Size.Height);
            this.pb.Image = surface;
            this.g = Graphics.FromImage(surface);
        }

        private void ResetaGraphics()
        {
            this.pb.Refresh();
            this.g.Clear(Color.White);
        }
        #endregion


        private void CampoBatalha_FormClosing(object sender, FormClosingEventArgs e)
        {
            desenha.Abort();        //paro a thread de desenho.
            //frm_Inicio.Desisto();   //envio a msg de desistencia para o jogador.
            //frm_Inicio.Show();      // mostro o formulario inicial.
        }
#endregion

        private void CampoBatalha_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!endPartida && !congelado)
            {
                /*
                 * Verifio se o player nao ira colidir com algo. 
                 * Nesse caso, vai retornar verdadeiro se o player nao colidir com o outro player. se colidir, ele envia uma msg de colisão,
                 * e impede o movimento do player naquela direção.
                 * Quando for retornado um valor falso, o player nao podera se mover e sera enviada uma msg de colisão para o outro jogador.
                 */
                if (!this.player.Input(e, this.listaObs, this.playerEnemy))
                {
                    //Movimento foi negado pois esta havendo colisão com o outro jogador.
                    ColisaoEntreJogadores();
                }
            }
        }

        #region

        private void VerificaQuemColidiuBandeira(string[] dados)
        {
            /*verifica se e a minha bandeira.
                    Pego os dados recebidos (qual foi a bandeira (obj 2)) e comparo se a bandeira que ta na posição 0
                e a minha bandeira. Se for a minha bandeira, marca meu player como se tiver pegado a bandeira
                e apaga a bandeira. 
                Se nao for a minha bandeira, somente apaga a bandeira para não desenhar mais na tela.
            */
            if (dados[1].Equals(bands[this.mB].GetBandeira))
            {
                this.player.PegouBand = true;
                this.bands[this.mB].Pegada = true;
                this.bands[this.mB].mostrarAtual = false;
            }
            else
            {                
                int b = this.mB == 0 ? 1 : 0;//pra saber qual e a outra bandeira e desativala da tela.
                this.bands[b].mostrarAtual = false;
                this.bands[b].Pegada = false;
            }
        }


        private void VerificaQuemColidiuPowerUp(string[] dados)
        {
            /*
                Verifica quem foi que colidiu. Se for o player local que colidiu, aumenta a sua velocidade, e desativa o power up.
             *  Se tiver sido o player remoto que colidiu, somente desativa o power up.
             *  
             *  Como funciona a comparação:
             *  
             * 1º - Verifico se eu sou o player servidor, e os dados recebidos são do jogador 2, que significa q sou eu.
             * 2º - Verifico se eu sou o player cliente, e os dados recebidos são do jogador 1 que significa q sou eu. 
             * 
             * Sem duvidas q ha uma maneira mais facil, mais vamo deixa essa q ta funcionando.
            */
            if (this.qualPlayer == 0 && dados[0].Equals("J2"))//quer dizer q sou eu e eu sou o servidor.
            {
                this.player.PegouPowerUp = true;
                this.player.MudaVelPwUp();
                this.powerUp.mostrarAtual = false;
            }
            else if (this.qualPlayer == 1 && dados[0].Equals("J1"))//quer dizer q sou eu e eu sou o cliente.
            {
                this.player.PegouPowerUp = true;
                this.player.MudaVelPwUp();
                this.powerUp.mostrarAtual = false;
            }
            else
            {
                this.powerUp.mostrarAtual = false;
            }
        }

       
        private void CampoBatalha_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Application.Exit();

            if (e.KeyCode == Keys.Space && !endPartida && podeAtirar && !PossoAtirar())
            {
                newTiro = new Tiro(this.player.xAtual, this.player.yAtual, this.player.tamX, LARGURA, this.player.direcaoJogador, Tiro.id_Tiro);
                Tiro.id_Tiro += 1;
                podeAtirar = false;
                tm_Bala.Start();
                EnviaMsgTiro(newTiro);
            }
        }


        private void tm_UpdtTempoPartida_Tick(object sender, EventArgs e)
        {
            //quando endPartida for true, a partida ja acabou. e mostra a msg de fim de jogo.
            if (!endPartida)
            {
                if (this.tempoRestante > 0 && !GameEnd())
                {
                    this.tempoRestante--;
                    lbl_TempoRestante.Text = TIMEOUTSTRING + this.tempoRestante + " s.";
                }
                else
                {
                    this.endPartida = true;
                    lbl_TempoRestante.Text = TIMEENDSTRING + proxPartida + " s.";
                    MostraMsgFinal();
                }
            }
            //mostra a msg de fim de jogo, e depois reinica o jogo passado o tempo.
            else
            {
                if (proxPartida > 0)
                {
                    proxPartida--;
                    lbl_TempoRestante.Text = TIMEENDSTRING + proxPartida + " s.";
                }
                else
                {
                    Restart();
                }
            }
        }


        private bool GameEnd()
        {
            if (this.player.PegouBand && this.player.Dentro(this.areaPlay.GetAreaPlayerLocal))
            {
                return true;
            }
            else
                return false;
        }


        //retorna verdadeiro se estiver dentro da area do jogoador adversario, ou se estiver dentro da sua propria area.
        private bool PossoAtirar()
        {
            return (this.player.Colisao(
                                    (int)this.areaPlay.GetAreaPlayerLocal.x, (int)this.areaPlay.GetAreaPlayerLocal.y,
                                    (int)this.areaPlay.GetAreaPlayerLocal.largura, (int)this.areaPlay.GetAreaPlayerLocal.altura)
                                    ||
                                    this.player.Colisao((int)this.areaPlay.GetAreaPlayerRemoto.x, (int)this.areaPlay.GetAreaPlayerRemoto.y,
                                    (int)this.areaPlay.GetAreaPlayerRemoto.largura, (int)this.areaPlay.GetAreaPlayerRemoto.altura)
                                    );
        }


        /// <summary>
        /// Verifico se o player local colidiu como power up ou com a bandeira.
        /// </summary>
        private void VerificaColisaoPlayer()
        {
            //envia a MSG 15 dizendo que o player colidiu com a bandeira se o player nao tive pegado a bandeira.
            if (!this.player.PegouBand)
            {
                if (player.Colisao((int)bands[mB].GetPosX, (int)bands[mB].GetPosY, (int)bands[mB].GetTam, (int)bands[mB].GetTam))
                    ColisaoPlayerBandeira(bands[mB]);
            }

            //Envia MSG 15 dizendo que o player colidiu com o POWER'UP
            if (this.player.Colisao(this.powerUp.xAtual, this.powerUp.yAtual, this.powerUp.tamX, this.powerUp.tamY) && this.powerUp.mostrarAtual)
                ColisaoPlayerPowerUp();
        }


        private void ColisaoDaBala()
        {
            while (true)
            {
                lock (_lock)
                {
                    Rectangle rect;
                    foreach (var t in tiros)
                    {
                        foreach (var o in listaObs)
                        {
                            rect = new Rectangle((int)o.xAtual, (int)o.yAtual, (int)o.tamX, (int)o.tamY);
                            if (t.Colisao(rect) && o.mostrarAtual)
                            {
                                //hove colisão com da bala com o obstaculo.
                                EnviaMsgColisaoObstaculo(o, t);
                                break;
                            }
                        }
                        //colisão do tiro com o player.
                        rect = new Rectangle((int)this.playerEnemy.GetX, (int)this.playerEnemy.GetY, (int)this.playerEnemy.GetTam, (int)this.playerEnemy.GetTam);
                        if (t.Colisao(rect) && !t.colidiu && !this.outroJogadorCongelado)
                        {
                            TiroColidiuPlayerMsg(t);
                            
                        }
                        //colisao do tiro com o power up
                        rect = new Rectangle((int)this.powerUp.xAtual, (int)this.powerUp.yAtual, (int)this.powerUp.tamX, (int)this.powerUp.tamY);
                        if (t.Colisao(rect) && this.powerUp.mostrarAtual)
                            TiroColidiuPowerUp(t);

                        //colisao do tiro com uma das bandeiras
                        for (int i = 0; i < this.bands.Length; i++)
                        {
                            rect = new Rectangle((int)bands[i].xAtual, (int)bands[i].yAtual, (int)bands[i].GetTam, (int)bands[i].GetTam);
                            if (t.Colisao(rect))
                            {
                                TiroColidiuBandeira(bands[i], t);
                            }
                        }
                    }
                }
                Thread.Sleep(20);
            }
        }


        private void Draw()
        {
            while (true)
            {
                try
                {
                    this.Invoke((MethodInvoker)delegate() { ResetaGraphics(); });
                    lock (_lock)
                    {
                        hud.Draw(this.g);

                        this.areaPlay.Draw(this.g);
                        this.bands[0].Draw(this.g);
                        this.bands[1].Draw(this.g);
                        this.player.Draw(this.g);
                        this.playerEnemy.Draw(this.g);

                        this.powerUp.Draw(this.g);

                        foreach (var t in tiros)
                            t.Draw(this.g);
                        foreach (var o in listaObs)
                            o.Draw(this.g);
                        foreach (var t in tirosInimigos)
                            t.Draw(this.g);
                    }
                }
                catch (Exception e)
                {
                    //Console.WriteLine("ERRO: " + e.ToString());
                }
                Thread.Sleep(30);
            }
        }


        private void Restart()
        {
            this.tempoRestante = TIMEOUT;
            this.proxPartida = 5;
            this.endPartida = false;
            lbl_TempoRestante.Text = TIMEOUTSTRING + this.tempoRestante + " s.";
            this.lbl_Resultado.Visible = false;
            this.player.PegouBand = false;

            foreach (var v in elementosJogo)
            {
                v.Reestart();
            }
        }


        private void MostraMsgFinal()
        {
            this.lbl_Resultado.Text = "Fim de jogo.";
            this.lbl_Resultado.Location = new Point(((int)AreaPlayers.CalcPercet(50, this.pb.Size.Width) - 75), (int)AreaPlayers.CalcPercet(50, this.pb.Size.Height) - 15);
            this.lbl_Resultado.AutoSize = false;
            this.lbl_Resultado.Size = new Size(150, 30);
            this.lbl_Resultado.Visible = true;
        }


        private void CriaObstaculos()
        {
            float largAreaJogo = this.pb.Size.Width - AreaPlayers.CalcPercet(20, this.pb.Size.Width);
            float altAreJogo = AreaPlayers.CalcPercet(90, this.pb.Size.Height);

            //ALTURA DO OBS (10 % DO TAMANHO DA ZONA DE COMBATE) -- PEGO O TAMANHO DA ZONA DE COMBATE QUE DE 90 % DO TAMANHO TOTAL DO PICBOX.
            float tY = AreaPlayers.CalcPercet(10f, altAreJogo);
            float tX = AreaPlayers.CalcPercet(1f, largAreaJogo);

            //POSIÇÃO  DOS OBS.
            float y = AreaPlayers.CalcPercet(10, this.pb.Size.Height);   //represnta a posição em y. Sempre sera a mesma.

            //cria o 1ª bloco de obstaculos.
            float x = AreaPlayers.CalcPercet(15, largAreaJogo) + AreaPlayers.CalcPercet(10, this.pb.Size.Width);  //representa a posição em x DO 1º BLOCO. Varia para cada bloco.
            Cria(x, y, tX, tY, 1);//crio a 1 coluna de obstaculos


            //define a nova posição em x do 2ª bloco.
            x = AreaPlayers.CalcPercet(30, largAreaJogo) + AreaPlayers.CalcPercet(10, this.pb.Size.Width);
            Cria(x, y, tX, tY, 2);//crio a 2 coluna de obstaculos


            //define a nova posição em x do 3ª bloco.
            x = AreaPlayers.CalcPercet(70, largAreaJogo) + AreaPlayers.CalcPercet(10, this.pb.Size.Width);
            Cria(x, y, tX, tY, 3);//crio a 3 coluna de obstaculos


            //define a nova posição em x do 4ª bloco.
            x = AreaPlayers.CalcPercet(85, largAreaJogo) + AreaPlayers.CalcPercet(10, this.pb.Size.Width);
            Cria(x, y, tX, tY, 4);//crio a 4 coluna de obstaculos
        }


        private void Cria(float xIni, float yIni, float tX, float tY, int coluna)
        {
            int fileira = 1;
            int bloco = 1;
            float x = xIni;
            float y = yIni;
            Obstaculo obs;
            for (int j = 0; j < 2; j++)
            {
                for (int i = 0; i < 10; i++)
                {
                    obs = new Obstaculo(x, y, tX, tY, coluna, fileira, bloco);
                    listaObs.Add(obs);
                    elementosJogo.Add(obs);
                    y += tY;
                    bloco += 1;
                }
                bloco = 1;
                fileira += 1;
                x += tX;
                y = yIni;
            }
        }


        private void tm_Bala_Tick(object sender, EventArgs e)
        {
            //sempre que atirar, ele ativa. quando acontece, desativa o timer e ativa o gatilho para poder atirar novamente.
            this.podeAtirar = true;
            tm_Bala.Stop();
        }


        /// <summary>
        /// Procura o tiro na lista de tiro.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private Tiro ProcuraOTiro(int id, List<Tiro> tiros)
        {
            foreach (var v in tiros)
                if (v.GetId == id)
                    return v;
            return null;
        }


        /// <summary>
        /// Procura o obstaculo atingido na lista de obstaculo.
        /// </summary>
        /// <param name="col"></param>
        /// <param name="fil"></param>
        /// <param name="bl"></param>
        /// <returns></returns>
        private Obstaculo ProcuraOObs(int col, int fil, int bl)
        {
            foreach (var o in listaObs)
            {
                if (o.GetColuna == col)
                    if (o.GetFileira == fil)
                        if (o.GetBloco == bl)
                            return o;
            }
            return null;
        }


        private Tiro BuscaTiro(int index)
        {
            foreach (var v in this.tirosInimigos)
            {
                if (v.GetId == index)
                {
                    v.colidiu = true;
                    return v;
                }
            }
            return null;
        }


        private void VoltaBandeira()
        {
            if (!bands[0].mostrarAtual)
                bands[0].mostrarAtual = bands[0].mostrarInicial;
            else if (!bands[1].mostrarAtual)
                bands[1].mostrarAtual = bands[1].mostrarInicial;

        }


        private void VoltaPowerUp()
        {
            if (!this.powerUp.mostrarAtual)
                this.powerUp.mostrarAtual = this.powerUp.mostrarInicial;
        }


        //---------------- Tratamento das MSG recebidas -- PUBLICS ---------------------------

        /// <summary>
        /// Quando recebida a msg de movimento autorizado, ele movimenta realiza o jogador na tela.
        /// </summary>
        public void MovimentoAutorizado()
        {
            this.player.Movimenta();
            VerificaColisaoPlayer();
        }


        /// <summary>
        /// Desenha o tiro na tela e começa a movimentação desse tiro na tela.
        /// </summary>
        public void TiroAutorizado()
        {
            lock (_lock)
            {
                tiros.Add(this.newTiro);
            }
            this.newTiro.PodeIr();
        }


        /// <summary>
        /// Define a nova posição do player remoto com base nos dados recebidos.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="direcao"></param>
        public void DefinePosicaoPlayerRemoto(float x, float y, char direcao)
        {
            this.playerEnemy.SetPosicao(x, y);
            lock (_lock)
            {
                this.playerEnemy.Draw(this.g);
            }
        }


        /// <summary>
        /// Retorna um vetor indicando a posição do player em coordenadas normalizadas.
        /// </summary>
        /// <returns></returns>
        public float[] GetPosPlayerUni()
        {
            float[] pos = new float[2];
            pos[0] = this.player.xAtual;
            pos[1] = this.player.yAtual;
            return pos;
        }





        /// <summary>
        /// Desenha o tiro disparado pelo outro jogador.
        /// </summary>
        /// <param name="dados">Dados referentes a posição do tiro do outro jogador.</param>
        public void TiroDisparadoOutroJogador(string[] dados)
        {
            //vetor de dados sempre tem que ser um vetor com 4 posição.

            Tiro newTiro = new Tiro(dados, this.player.tamX, this.pb.Size.Width);
            newTiro.PodeIr();
            this.tirosInimigos.Add(newTiro);
        }


        //Remove o bloco e o tiro que colidiu.
        public void RemoveBlocos(string[] dados)
        {
            int idTiro = int.Parse(dados[2]);
            int col = int.Parse(dados[1].Substring(2, 1));
            int fil = int.Parse(dados[1].Substring(3, 1));
            int bl = int.Parse(dados[1].Substring(4, 2));
            Obstaculo ob = ProcuraOObs(col, fil, bl);
            ob.mostrarAtual = false;
        }


        private void RemoveTiro(int id)
        {
            Tiro tir = null;

            tir = ProcuraOTiro(id, this.tiros);
            if (tir != null)
                tir.colidiu = true;

            tir = ProcuraOTiro(id, this.tirosInimigos);
            if (tir != null)
                tir.colidiu = true;

        }

        #endregion

        public void JogadorAtingido(string[] dados)
        {
            this.player.ApplyDamange();
            this.tirosInimigos.Remove(BuscaTiro(int.Parse(dados[2])));

            //Verifica se o player NÃO tem vida, se não tiver, reinicia os valores abaixo.
            //if (this.player.TemVida())
            //{
            //    this.player.Reestart();
            //    congelado = true;
            //    tm_Congelamento.Start();
            //    if (this.player.PegouBand)
            //        VoltaBandeira();
            //    if (this.player.PegouPowerUp)
            //        VoltaPowerUp();
            //}
        }

        private void AcerteiOutroJogador()
        {
            vidasOutroPlayer -= 1;
            if (vidasOutroPlayer == 0)
                EnviaMsgCongelaJogador();
            
        }


        public void ColisaoAutorisada(string[] dados)
        {

            #region COLISAO_TIRO
            if (dados[0] == "T")
            {                
                //testo se a colisão foi com a fileira de blocos.
                string col = dados[1].Substring(0, 2);
                if (col == "BL")
                {
                    this.RemoveBlocos(dados);
                }
                else if (col == "J1" && this.qualPlayer == 0)
                {
                    //quer dizer que o tiro foi com o meu player.
                    this.JogadorAtingido(dados);
                }
                else if (col == "J2" && this.qualPlayer == 1)
                {
                    
                    this.JogadorAtingido(dados);
                }
                else
                {
                    //Quando chegar aqui, quer dizer q acertei o jogador adiversario.
                    AcerteiOutroJogador();
                }
                this.RemoveTiro(int.Parse(dados[2]));
            }
            else if (dados[0] == "CE")
            {
                //colisão com outra coisa.
            }
            #endregion


            #region COL_PLA_LOCAL
            else if (dados[0].Equals("J1"))//representa o player local. Indiferente se e o servidor ou não.
            {
                //bandeira 2
                if (String.Compare(dados[1], "B1") == 0 || String.Compare(dados[1], "B2") == 0)
                {
                    //Console.WriteLine("Alguem colidiu com uma bandeira. Quem foi: " + dados[0]);
                    VerificaQuemColidiuBandeira(dados);
                }
                else if (String.Compare(dados[1], "PU") == 0)
                {
                    //Console.WriteLine("Alquem colidiu com o Power Up.");
                    VerificaQuemColidiuPowerUp(dados);
                }
                else if (String.Compare(dados[1], "J2") == 0)
                {
                    //Não sei o que fazer. E agora??
                    Console.WriteLine("Colisão entre jogadores.");
                }
            }
            #endregion


            #region COL_PLA_REMOTO
            else if (dados[0] == "J2") //Representa o player remoto. Indiferente se for o servidor ou não.
            {
                //Colisão com bandeira. Verifica quem colidiu com a bandeira e marca quem colidiu e a bandeira que foi colidida.
                if (String.Compare(dados[1], "B1") == 0 || String.Compare(dados[1], "B2") == 0)
                {
                    
                    VerificaQuemColidiuBandeira(dados);
                }
                //Colisão com Power Up. Verifica quem colidiu com o Power Up e marca o Power Up como inativo.
                else if (String.Compare(dados[1], "PU") == 0)
                {
                    VerificaQuemColidiuPowerUp(dados);
                }
                else if (String.Compare(dados[1], "J2") == 0)
                {
                    //Não sei o que fazer. E agora??
                    Console.WriteLine("Colisão entre jogadores.");
                }
            }
            #endregion
        }


        //---------------- Tratamento das MSG recebidas -- PRIVATES ---------------------------

        /// <summary>
        /// Envia MSG que foi disparada um tiro.
        /// </summary>
        /// <param name="t"></param>
        private void EnviaMsgTiro(Tiro t)
        {
            string aux = string.Format("{0}|{1}|{2}|{3}", t.GetX, t.GetY, t.GetDirecao, t.GetId);
            int qtd = aux.Length + 5;
            string msg = string.Format("13{0}{1}", qtd.ToString("000"), aux);
            this.frm_Inicio.EnviaMsgTcp(msg);
        }       


        private void tm_Congelamento_Tick(object sender, EventArgs e)
        {
            
            EnviaMsgDescongelaJogador();
            tm_Congelamento.Stop();
        }


        //---------------- MENSSAGENS DE COLISÕES DO PLAYER-----------
        private void ColisaoPlayerBandeira(Bandeira band)
        {
            string auxMsg = string.Format("J1|" + band.GetBandeira);
            if (this.qualPlayer == 0)//se o jogador local for o jogador 1 (servidor)
                auxMsg = string.Format("J1|" + band.GetBandeira);

            else if (this.qualPlayer == 1)//se o jogador local for o jogador 2 (cliente)
                auxMsg = string.Format("J2|" + band.GetBandeira);


            int tam = auxMsg.Length + 5;
            string msg = string.Format("15{0}{1}", tam.ToString("000"), auxMsg);
            this.frm_Inicio.EnviaMsgTcp(msg);
        }


        private void ColisaoPlayerPowerUp()
        {
            string aux = "";
            if (this.qualPlayer == 1)//representa o player que e o cliente.
                aux = string.Format("J1|PU");
            else if (this.qualPlayer == 0)//representa o player que e o servidor.
                aux = string.Format("J2|PU");

            int tam = aux.Length + 5;
            string msg = string.Format("15{0}{1}", tam.ToString("000"), aux);
            this.frm_Inicio.EnviaMsgTcp(msg);
        }


        private void ColisaoEntreJogadores()
        {
            string aux = "";
            if (this.qualPlayer == 1)//representa o player que e o cliente.
                aux = string.Format("J1|J2");
            else if (this.qualPlayer == 0)//representa o player que e o servidor.
                aux = string.Format("J2|J1");

            int tam = aux.Length + 5;
            string msg = string.Format("15{0}{1}", tam.ToString("000"), aux);
            this.frm_Inicio.EnviaMsgTcp(msg);

            Console.WriteLine("Colisão entre players. MGS Send: " + msg);
        }


        private void TiroColidiuPowerUp(Tiro t)
        {
            string aux = string.Format("T|PU|{0}", t.GetId);
            int tam = aux.Length + 5;
            string msg = string.Format("15{0}{1}", tam.ToString("000"), aux);
            this.frm_Inicio.EnviaMsgTcp(msg);
        }


        private void TiroColidiuBandeira(Bandeira band, Tiro t)
        {
            string aux = string.Format("T|{0}|{1}", band.GetBandeira, t.GetId);
            int tam = aux.Length + 5;
            string msg = string.Format("15{0}{1}", tam.ToString("000"), aux);
            this.frm_Inicio.EnviaMsgTcp(msg);
        }



        //---------------- MENSSAGENS DE COLISÕES DOS TIROS -----------


        /// <summary>
        /// Ouve a colisão do tiro com um obstaculo.
        /// </summary>
        /// <param name="obs"></param>
        /// <param name="t"></param>
        private void EnviaMsgColisaoObstaculo(Obstaculo obs, Tiro t)
        {
            string aux = string.Format("T|BL{0}{1}{2}|{3}", obs.GetColuna, obs.GetFileira, obs.GetBloco.ToString("00"), t.GetId);
            int qtd = aux.Length + 5;
            string msg = string.Format("15{0}{1}", qtd.ToString("000"), aux);
            this.frm_Inicio.EnviaMsgTcp(msg);
        }


        /// <summary>
        /// Ouve a colisão do tiro com o player remoto.
        /// </summary>
        /// <param name="t"></param>
        private void TiroColidiuPlayerMsg(Tiro t)
        {
            string aux = "";

            if (this.qualPlayer == 0)
                aux = string.Format("T|J2|{0}", t.GetId);
            else if (this.qualPlayer == 1)
                aux = string.Format("T|J1|{0}", t.GetId);

            int qtd = aux.Length + 5;
            string msg = string.Format("15{0}{1}", qtd.ToString("000"), aux);
            this.frm_Inicio.EnviaMsgTcp(msg);
        }


    
        /*
         * Envia essa MSG dizendo que o player nao tem mais vidas. Portanto deve ser congelado.
         * ESSA E A MSG 17/0 -> MSG DE CONGELAR O OUTRO JOGADOR.
        */
        private void EnviaMsgCongelaJogador()
        {
            this.frm_Inicio.EnviaMsgTcp("170060");
        }

        /*
         * MSG 17/1 -> MSG QUE DIS QUE O OUTRO JOGADOR FOI DESCONGELADO.
        */
        private void EnviaMsgDescongelaJogador()
        {
            this.frm_Inicio.EnviaMsgTcp("170061");
        }


        /// <summary>
        /// Congela o jogador local.
        /// </summary>
        public void CongelaJogadorLocal()
        {
            this.player.Reestart();
            this.player.Draw(this.g);
            congelado = true;
            tm_Congelamento.Start();
            VoltaElementosJogo();
            Console.WriteLine("Congelado.");
        }


        private void VoltaElementosJogo()
        {
            if (this.player.PegouBand)
                VoltaBandeira();
            if (this.player.PegouPowerUp)
                VoltaPowerUp();
        }


        /*
         * FUNÇÃO EXECUTADA QUANDO RECEBE A MSG 17/1 -> DIZ QUE O OUTRO JOGADOR FOI DESCONGELADO.
         * RETORNA A VIDA DELE PARA 3, MUDA PARA QUE ELE POSSA LEVAR TIRO NOVAMENTE.
         */
        public void DescongelaJogadorLocal()
        {
            outroJogadorCongelado = false;
            vidasOutroPlayer = 3;
        }


        /*
         * Executada para confirma a ação de congelar o outro jogador.
         * Executa quando recebe a MSG 18 de confirmação de congelamento.
         * O PLAYER FOI CONGELADO.
         */
        public void ConfirmaCongelamento()
        {
            outroJogadorCongelado = true;
            this.powerUp.mostrarAtual = true;
        }

        /*
         * Executada quando recebe a confirmação que o tempo e congelamento do jogador remoto passou.
         * Executa quando a o recebimento da MSG 18.
        */
        public void ConfirmaDescongelamento()
        {
            this.congelado = false;
        }
    }
}