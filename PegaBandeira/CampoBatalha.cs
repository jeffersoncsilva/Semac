using System;
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
        public const int TIMEOUT = 120;
        public const string TIMEOUTSTRING = "Tempo resante da partida: \n";
        public const string TIMEENDSTRING = "Acabou a partida. A proxima partida inicia em: ";
        private int tempoRestante;
        private int proxPartida = 2;
        private bool endPartida;
        private bool podeAtirar;
        private static readonly object _lock = new object();
        
        //necessario para fazer o desenho na tela.
        PictureBox pb;
        Bitmap surface;
        Graphics g;

        //constantes para definir o tamanho da tela
        private const int LARGURA = 1024;
        private const int ALTURA = 800;
        private MenuInicial frm_Inicio;

        //essa trhead fica redesenhando os obj na tela.
        private Thread desenha;

        //Referente aos players do jogo.
        private Player player;
        private AreaPlayers areaPlay;
        private Interface hud;
        private JogadorInimigo playerEnemy;

        private Bandeira[] bands;
        private static List<ElementoJogo> elementosJogo;

        //private Tiro bullet = null;
        private List<Tiro> tiros;

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



        private void InicializaVariaveis(int tipo)
        {
            elementosJogo = new List<ElementoJogo>();   //cria a lista com todos os elementos do jogo.
            this.podeAtirar = true;                     //define que o jogador pode atirar.
            this.areaPlay = new AreaPlayers(this.pb.Size.Width, this.pb.Size.Height);   //cria a rea de jogo do player.
            //refere-se a HUD do jogo.
            this.hud = new Interface(this.pb.Size.Width, this.pb.Size.Height);      //cria a hud do jogo.
            Label[] lab = new Label[] { this.lbl_NomeJog, this.lbl_Placar, this.lbl_TempoRestante, this.lbl_Inativo };//pega as labels da hud do jogo que ta no form.
            this.hud.ConfgLabels(lab);  //define as posições e tamanhos das labels que tem no jogo.


            this.player = new Player(this.pb.Size.Width, this.pb.Size.Height, tipo, this.frm_Inicio);
            playerEnemy = new JogadorInimigo(this.player.tamX, this.player.tamY, this.pb.Size.Width, this.pb.Size.Height, tipo);

            //thread de desenho.
            this.desenha = new Thread(() => Draw());
            this.desenha.Name = "DesenhaTela";
            this.desenha.Start();

            //timer que controle o tempo de jogo.
            this.tempoRestante = TIMEOUT;
            this.tm_UpdtTempoPartida.Start();

            //cria as bandeiras que ha no jogo.
            this.bands = new Bandeira[2];
            this.bands[0] = new Bandeira(this.areaPlay, 0);
            this.bands[1] = new Bandeira(this.areaPlay, 1);

            //adicionas as bandeiras a lista de lemento de jogo e eo player.
            elementosJogo.Add(bands[0]);
            elementosJogo.Add(bands[1]);

            //cra a lista de tiros.
            this.tiros = new List<Tiro>();

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

            Console.WriteLine("Elementos no jogo: " + elementosJogo.Count);
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

        #region

        private void CampoBatalha_Resize(object sender, EventArgs e)
        {
            /*
             * Essa seria uma solução para fazer com que o fomr redesenha-se de forma
             * que fique da mesma perspectiva quando redimencionar a tela do formulario.
             * Defido a desempenho, quando se redimensiona o formulario, esta ocorrendo um grande salto no consumo
             * de memoria, entao foi desativado a opção de redimencionar o formulario para evitar
             * maiores problemas de falta de memoria.
             */
            //ConfGraphics();
            //hud.Altura = this.pb.Size.Height;
            //hud.Largra = this.pb.Size.Width;
            //hud.DefineRetangulos();
        }


        private void CampoBatalha_FormClosing(object sender, FormClosingEventArgs e)
        {
            desenha.Abort();        //paro a thread de desenho.
            //frm_Inicio.Desisto();   //envio a msg de desistencia para o jogador.
            //frm_Inicio.Show();      // mostro o formulario inicial.
        }


        private void CampoBatalha_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!endPartida)
            {
                this.player.Input(e, this.listaObs);
                VerificaColisaoPlayer();
            }
        }


        private void CampoBatalha_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Application.Exit();

            if (e.KeyCode == Keys.Space && !endPartida && podeAtirar && !PossoAtirar())
            {
                Tiro t = new Tiro(this.player.xAtual, this.player.yAtual, this.player.tamX, LARGURA, this.player.direcaoJogador);
                lock (_lock)
                {
                    tiros.Add(t);
                }
                podeAtirar = false;
                tm_Bala.Start();
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


        private void VerificaColisaoPlayer()
        {
            #region BANDEIRAS
            //para verificar se houve colisão com alguma bandeira. Somente ira verificar se o player local nao tiver pegado 
            //nenhuma bandeira.
            if (!this.player.PegouBand)
            {
                for (int i = 0; i < this.bands.Length && !this.player.PegouBand; i++)
                {
                    if (player.Colisao((int)bands[i].GetPosX, (int)bands[i].GetPosY, (int)bands[i].GetTam, (int)bands[i].GetTam) && bands[i].GetMyBand)
                    {
                        this.player.PegouBand = true;
                        bands[i].mostrarAtual = false;
                    }
                }

            }
            #endregion

            #region PowerUp


            if (this.player.Colisao(this.powerUp.xAtual, this.powerUp.yAtual, this.powerUp.tamX, this.powerUp.tamY))
            {
                this.player.PegouPowerUp = true;
                this.powerUp.mostrarAtual = false;
                this.player.MudaVelPwUp();
            }


            #endregion

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
                        if (t.SaiuTela(LARGURA, ALTURA))
                        {
                            t.colidiu = true;
                        }

                        foreach (var o in listaObs)
                        {
                            rect = new Rectangle((int)o.xAtual, (int)o.yAtual, (int)o.tamX, (int)o.tamY);
                            if (t.Colisao(rect) && o.mostrarAtual)
                            {
                                t.colidiu = true;
                                o.mostrarAtual = false;
                                break;
                            }
                        }
                    }
                }
            }
        }


        private void Draw()
        {
            while (true)
            {
                try
                {
                    this.Invoke((MethodInvoker)delegate() { ResetaGraphics(); });
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
                }
                catch (Exception e)
                {
                    //Console.WriteLine("ERRO: " + e.ToString());
                }
                Thread.Sleep(16);
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
            Cria(x, y, tX, tY);


            //define a nova posição em x do 2ª bloco.
            x = AreaPlayers.CalcPercet(30, largAreaJogo) + AreaPlayers.CalcPercet(10, this.pb.Size.Width); 
            Cria(x, y, tX, tY);


            //define a nova posição em x do 3ª bloco.
            x = AreaPlayers.CalcPercet(70, largAreaJogo) + AreaPlayers.CalcPercet(10, this.pb.Size.Width); 
            Cria(x, y, tX, tY);


            //define a nova posição em x do 4ª bloco.
            x = AreaPlayers.CalcPercet(85, largAreaJogo) + AreaPlayers.CalcPercet(10, this.pb.Size.Width); 
            Cria(x, y, tX, tY);

        }


        private void Cria(float xIni, float yIni, float tX, float tY)
        {
            float x = xIni;
            float y = yIni;
            Obstaculo obs;
            for (int j = 0; j < 2; j++)
            {
                for (int i = 0; i < 10; i++)
                {
                    obs = new Obstaculo(x, y, tX, tY);
                    listaObs.Add(obs);
                    elementosJogo.Add(obs);
                    y += tY;
                }
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

        #endregion


        //---------------- Tratamento das MSG recebidas. ---------------------------

        /// <summary>
        /// Define a nova posição do player remoto com base nos dados recebidos.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="direcao"></param>
        public void DefinePosicaoPlayerRemoto(float x, float y, char direcao)
        {
            this.playerEnemy.SetPosicao(x, y);
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
       
    }
}