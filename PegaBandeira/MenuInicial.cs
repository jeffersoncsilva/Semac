using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Timers;


namespace PegaBandeira
{
    public partial class MenuInicial : Form
    {
        private int player; //0: player 1 servidor || 1: player 2 cliente.

        public static Boolean executando = true;

        ServerUdp serverUdp;
        ServerTcp serverTcp = null;
        ClientTcp clientTcp = null;
        CampoBatalha cBat = null;

        private bool connect;
        //private bool euDesisto;
        private bool foiConvidado;

        private bool emPartida;
        public bool EmPartida { get { return this.emPartida; } set { this.emPartida = value; } }

        public MenuInicial()
        {
            InitializeComponent();
            serverUdp = new ServerUdp(this);
            HabilitaBotoes();
            this.connect = false;
            this.foiConvidado = false;
        }


        /// <summary>
        /// Inicia o envio de broadcas na rede para verificar os jogadores que estão online. Ativa a escuta de braodcast da rede.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_VeriJog_Click(object sender, EventArgs e)
        {
            //Dispara o boradcast na rede para encontrar os jogadores que estiverem online
            IniciaUdp();
        }



        public void IniciaUdp()
        {
            serverUdp = new ServerUdp(this);
            gb_Config.Enabled = false;
            serverUdp.IniciaEscuta(txb_Nome.Text, txb_Apelido.Text);
            serverUdp.EnviaBroadcastRede();
            HabilitaBotoes();
            ltb_JogOn.Items.Clear();
            //ativa a porra do botao para convidar os jogadores.
            gb_Convite.Enabled = true;
            gb_Convite.Visible = true;
            btn_Convida.Enabled = true;
        }


        /// <summary>
        /// Envia msg para dizer ao outro jogador que pode iniciar a partida. Caso este ja tenha enviado essa msg, inicia a partida.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_IniPartida_Click(object sender, EventArgs e)
        {
            PlayerLocalPronto();
            btn_IniPartida.Visible = false;
            lbl_Espera.Visible = true;
            //emPartida = true;
        }


        /// <summary>
        /// Convida o jogador selecionado para jogar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Convida_Click(object sender, EventArgs e)
        {
            try
            {
                if (ltb_JogOn.Items.Count > 0)
                {
                    serverUdp.EnviaConviteJogo(ltb_JogOn.SelectedIndex);
                    btn_Convida.Enabled = false;
                    //começo a contar o tempo de resposta
                    tm_verJogOn.Start();
                }
            }
            catch
            {
                MessageBox.Show("Selceciona alguem.");
            }
        }


        /// <summary>
        /// Aceita o convite de jogo realizado pelo outro jogaor.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_AceitoConv_Click(object sender, EventArgs e)
        {
            if (this.foiConvidado)
            {
                serverTcp = new ServerTcp(Conecta.PORTA_CONEXAO, this);
                serverTcp.IniciaServer();
                serverUdp.AceitoConvite();
                gb_Convite.Enabled = false;
                gb_ConviteRecv.Visible = false;
                tm_verJogOn.Stop();
                //paro a thread de escuta e envio de broadcast. e atribuo um valor null para poder recomeçar depois a 
                //perquisa por jogadores.
                this.serverUdp.ParaUdp();
                //this.serverUdp = null;
            }
        }


        /// <summary>
        /// Recusa o convite de jogo do outro jogador.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_RecusConv_Click(object sender, EventArgs e)
        {
            if (this.foiConvidado)
            {
                tm_verJogOn.Stop();
                serverUdp.RecusaConviteJogo();
                gb_ConviteRecv.Visible = false;
                this.foiConvidado = false;
                gb_Convite.Enabled = true;
            }
        }


        /// <summary>
        /// Verifica se o nome e apelido atende os requisitos para poder habilitar o botão de inicio de partida.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AlteraNomeTxb(object sender, EventArgs e)
        {
            HabilitaBotoes();
        }


        /// <summary>
        /// Para verificar se o jogador pode iniciar a escuta e envio de broadcast.
        /// </summary>
        private void HabilitaBotoes()
        {
            //habilita o botao de verificar jogadores online (broadcast)
            if (txb_Apelido.Text != "" && txb_Nome.Text != "")
            {
                btn_VeriJog.Enabled = true;
            }
        }


        /// <summary>
        /// Ativa a parte para poder fazer um convite a um jogador.
        /// </summary>
        public void AtivaConvite()
        {
            gb_Convite.Visible = true;
            gb_Convite.Enabled = true;
        }


        /// <summary>
        /// Adiciona um jogador a lista de jogadores online.
        /// </summary>
        /// <param name="jogadorNome">Nome do jogador.</param>
        /// <param name="jogadorApelido">Apelido do jogador.</param>
        public void AddJogador(string jogadorNome, string jogad)
        {
            ltb_JogOn.Items.Add(string.Format(jogadorNome + "; " + jogad));
        }


        /// <summary>
        /// Para mostrar o nome do jogador que o vonvidou para jogo.
        /// </summary>
        /// <param name="nome">nome jogador.</param>
        public void AddNomeEnemy(string nome)
        {  //mostra o nome do jogador adversario que convidou para jogo.
            lbl_Convidou.Text = string.Format("Convidou: {0} para jogar. Esperando pela resposta dojogador.", nome);
        }


        /// <summary>
        /// Mostra as opções de aceitar ou negar o convite de jogo recebido por um jogador.
        /// </summary>
        /// <param name="apelido"></param>
        public void ConviteReciv(string apelido)
        {//Ativar os botoes de aceitar e negar o convite de jogo.
            if (!this.connect && !this.foiConvidado)
            {
                this.foiConvidado = true;
                gb_ConviteRecv.Visible = true;
                tm_verJogOn.Start();
                gb_Convite.Enabled = false;
                lbl_NomJogConv.Text = string.Format("O jogador {0} te convidou para jogar. Deseja aceitar o convite?", apelido);
            }
            else
            {
                serverUdp.RecusaConviteJogo();
            }
        }


        /// <summary>
        /// Conecta ao outro jogador como cliente.
        /// </summary>
        /// <param name="ip">Ip do outro jogador.</param>
        /// <param name="port">Porta do outro jogador.</param>
        public void ConnectToServerTcp(string ip, int port)
        {//Método que conecta como "cliente" ao outro jogador.
            this.clientTcp = new ClientTcp(this);
            if (clientTcp.Conecta(ip, port))
            {
                ConexoAceita();
                this.serverUdp.ParaUdp();
                //this.serverUdp = null;
            }
        }


        /// <summary>
        /// Recusa o convite de jogar do jogador.
        /// </summary>
        public void ConexaoNegada()
        {//quando o jogador recusa o convite para jogo.
            lbl_Convidou.Text = "O jogador convidado recusou o convite.";
            btn_Convida.Enabled = true;
            btn_IniPartida.Visible = false;
        }


        /// <summary>
        /// Aceita o convite do outro jogador.
        /// </summary>
        public void ConexoAceita()
        {//quando o jogador aceita o convite para jogo, e ja esta conectado com o adversario.
            lbl_Convidou.Text = "Conectado com o outro jogador.";
            btn_IniPartida.Visible = true;
            this.connect = true;
            tm_verJogOn.Stop();
        }


        /// <summary>
        /// Carrega o campo de batalha do jogo.
        /// </summary>
        /// <param name="tipo">Se o jogador e um cliente ou o servidor.</param>
        public void CarregaCampoBatalha(int tipo)
        {
            if (!emPartida)
            {
                cBat = new CampoBatalha(this, tipo);
                cBat.Show();
                cBat.DefineNomeJogador(txb_Nome.Text);//ganbiarra pra mostrar o nome do jogador na tela.
                this.Hide();
                emPartida = true;
            }
            else
            {
                cBat.ComecaRodada();
                //cBat.Show();
                Console.WriteLine("Carrega campo batalha.");
            }
        }


        /// <summary>
        /// Quando a a desistencia do jogador da partida.
        /// </summary>
        public void FimDeJogo()
        {//quando o jogador desiste de jogar. Quando ele desiste, envio uma msg de desistencia e fecha a conexao.
            //this.euDesisto = true;

            string msg = "19005";
            if (this.serverTcp != null)
            {
                this.serverTcp.EnviaMsg(msg);
                this.serverTcp.EncerraConexaoTcp();
                this.serverTcp = null;
                //Console.WriteLine("Fechou conexao server.");

            }
            else if (this.clientTcp != null)
            {
                this.clientTcp.EnviaMsg(msg);
                this.clientTcp.EncerraConexaoTcp();
                this.clientTcp = null;
                //Console.WriteLine("Fechou conexao cliente.");
            }
        }

        /// <summary>
        /// Avisa ao outro jogador que a rodada acabou.
        /// </summary>
        public void FinDeRodada()
        {
            string msg = "19005";
            if (this.serverTcp != null)
            {
                this.serverTcp.EnviaMsg(msg);
                //Console.WriteLine("Fechou conexao server.");
            }
            else if (this.clientTcp != null)
            {
                this.clientTcp.EnviaMsg(msg);
                //Console.WriteLine("Fechou conexao cliente.");
            }
        }


        /// <summary>
        /// Restaura os valores para poder começar novamente a escutar e enviar o broadcast.
        /// </summary>
        public void Padrao()
        {
            //restaura os dados importantes para os valores iniciais para poder iniciar outra partida.
            btn_IniPartida.Visible = false;
            gb_Convite.Enabled = true;
            gb_ConviteRecv.Visible = false;
            lbl_Espera.Visible = false;
            ltb_JogOn.Items.Clear();
            this.connect = false;
            //this.euDesisto = false;
            this.foiConvidado = false;
            //recomeça a thread e broadcast.


        }


        //a cada 5 segundos, ele verifica quem ta online.
        private void verJogOn_Tick(object sender, EventArgs e)
        {
            if (this.foiConvidado)
            {
                Console.WriteLine("Tempo limite exedido. O convite foi recusado automaticamente.");

                this.serverUdp.RecusaConviteJogo();
                gb_ConviteRecv.Visible = false;
                gb_Convite.Enabled = true;
                this.foiConvidado = false;
                tm_verJogOn.Stop();
            }
            else if (!this.connect)
            {
                ConexaoNegada();
                tm_verJogOn.Stop();
                this.serverUdp.naoAceito = true;
            }
        }


        public void EnviaMsgTcp(string msg)
        {
            if (this.serverTcp != null)
            {
                this.serverTcp.EnviaMsg(msg);
            }
            else if (this.clientTcp != null)
            {
                this.clientTcp.EnviaMsg(msg);
            }
        }


        public void TrataMsgOnze(string[] dados)
        {
            //divido os dados nas posições recebidas. (player remoto).
            float xU = float.Parse(dados[0]);
            float yU = float.Parse(dados[1]);
            char dir = char.Parse(dados[2]);

            this.cBat.DefinePosicaoPlayerRemoto(xU, yU, dir);//seto as novas posições.
            //monta a msg 12

            float[] pos = this.cBat.GetPosPlayerUni();//pego a posição do player local para montar a resposta do jogador.
            string aux = string.Format("{0}|{1}", pos[0], pos[1]);
            int count = aux.Length + 5;
            string msg = string.Format("12{0}{1}", count.ToString("000"), aux);
            if (serverTcp != null)
                serverTcp.EnviaMsg(msg);
            else if (clientTcp != null)
                clientTcp.EnviaMsg(msg);
        }


        /// <summary>
        /// Autoriza a movimentação do player.
        /// </summary>
        public void TrataMsgDoze()
        {
            this.cBat.MovimentoAutorizado();
        }


        public void TrataMsgTreze(string[] dados)
        {
            //trato os dados, ou seja, crio o tiro e inicio a movimentação desse tiro.
            this.cBat.TiroDisparadoOutroJogador(dados);
            //respondo a msg 13 com a msg 14.
            string aux = string.Format("{0}|{1}|{2}|{3}", dados[0], dados[1], dados[2], dados[3]);
            int tam = aux.Length + 5;
            string msgRet = string.Format("14{0}{1}", tam.ToString("000"), aux);
            EnviaMsgTcp(msgRet);
        }


        /// <summary>
        /// Autoriza o disparo do tiro.
        /// </summary>
        public void TrataMsgCatorze()
        {
            this.cBat.TiroAutorizado();
        }


        public void TrataMsgQuinze(string[] dados)
        {
            string aux = "";
            //responde com a msg 16.
            if (dados.Length == 3)
                aux = string.Format("{0}|{1}|{2}", dados[0], dados[1], dados[2]);
            else if (dados.Length == 2)
                aux = string.Format("{0}|{1}", dados[0], dados[1]);

            int qtd = aux.Length + 5;
            string msg = string.Format("16{0}{1}", qtd.ToString("000"), aux);
            this.EnviaMsgTcp(msg);

            //Trato os dados recebidos.
            this.cBat.ColisaoAutorisada(dados);
        }


        public void TrataMsgDezeceis(string[] dados)
        {
            this.cBat.ColisaoAutorisada(dados);
        }


        public void TrataMsgDezecete(string[] dados)
        {
            //Console.WriteLine("Msg 17 recebida.");
            if (int.Parse(dados[0]) == 0)
            {
                //congelo o jogador. E responde com a MSG 18.
                this.cBat.CongelaJogadorLocal();
                //Envia a MSG 18 confirmando o congelamento do jogador.
                EnviaMsgTcp("180060");
            }
            else if (int.Parse(dados[0]) == 1)
            {
                //descongelo o jogador.
                this.cBat.DescongelaJogadorLocal();
                //Envia a MSG 18 confirmando o descongelamento do jogador.
                EnviaMsgTcp("180061");
            }
        }


        public void TrataMsgDezoito(string[] dados)
        {
            //Console.WriteLine("Msg 18 recebida.");
            if (int.Parse(dados[0]) == 0)
            {
                //confirmação do congelo do jogador remoto.
                this.cBat.ConfirmaCongelamento();
            }
            else if (int.Parse(dados[0]) == 1)
            {
                //confirmação do descongelo do jogador remoto.
                this.cBat.ConfirmaDescongelamento();
            }
        }

        //retorna true se nao tiver conexao.
        //retorna false se tiver tudo OK. Se tiver CONEXÃO.
        public bool TemConexao()
        {
            try
            {
                bool conection = false;
                if (this.serverTcp != null)
                {
                    conection = this.clientTcp.VerificaConexaoTcp();
                    //Console.WriteLine("Testou conexao." + conection);
                }
                else if (this.clientTcp != null)
                {
                    conection = this.clientTcp.VerificaConexaoTcp();
                    //Console.WriteLine("Testou conexao." + conection);
                }
                //se de erro muda para true e testa de novo.
                return conection;
            }
            catch(Exception e)
            {
                //MessageBox.Show("Catch. " + e.ToString());
                return false;
            }
        }


        public void VoltaLocalPlayer()
        {
            if (this.clientTcp != null)
            {
                this.clientTcp.LocalPlayer = false;
                this.clientTcp.RemotePlayer = false;
            }
            else if (this.serverTcp != null)
            {
                this.serverTcp.LocalPlayer = false;
                this.serverTcp.RemotePlayer = false;
                Console.WriteLine("Sou o servidor e voltel o local player para false.");
            }
        }


        public void PlayerLocalPronto()
        {
            string msg = "10005";
            //+--------+  SERVIDOR  +--------+
            if (serverTcp != null)
            {
                serverTcp.LocalPlayer = true;
                if (serverTcp.RemotePlayer && serverTcp.LocalPlayer)
                {
                    serverTcp.EnviaMsg(msg);
                    this.player = 0;
                    CarregaCampoBatalha(player);

                }
                else
                    serverTcp.EnviaMsg(msg);
                Console.WriteLine("R: " + serverTcp.RemotePlayer);
                Console.WriteLine("L: " + serverTcp.LocalPlayer);
            }
            //+--------+  CLIENTE  +--------+
            else if (clientTcp != null)
            {
                clientTcp.LocalPlayer = true;
                if (clientTcp.RemotePlayer && clientTcp.LocalPlayer)
                {
                    clientTcp.EnviaMsg(msg);
                    this.player = 1;
                    CarregaCampoBatalha(player);
                }
                else
                    clientTcp.EnviaMsg(msg);
            }
        }


        public void TrataMsgDezenove()
        {
            try
            {
                if (TemConexao())
                {
                    //mostra msg de conexao perdida.
                    MessageBox.Show("Conexão perdia. Provavelmente o jogador desistiu. Continue para ver resultados.");
                }
                this.cBat.VerificaVencedor();
                VoltaLocalPlayer();
                this.cBat.EndPart = true;
            }
            catch (Exception e)
            {
                //so pra nao dar erro. 
                MessageBox.Show("ERRO: " + e.ToString());
            }
            //}
        }


        public void VoltaUdp()
        {
            this.serverUdp.VoltaUdpBroadcast();
            //EncerraConexaoTcp();
            Padrao();
            btn_Convida.Enabled = true;
            emPartida = false;
        }


        private void EncerraConexaoTcp()
        {
            if (this.serverTcp != null)
            {
                serverTcp.EncerraConexaoTcp();
                serverTcp = null;
            }
            else if (this.clientTcp != null)
            {
                clientTcp.EncerraConexaoTcp();
                clientTcp = null;
            }
        }


        private void MenuInicial_FormClosed(object sender, FormClosedEventArgs e)
        {
            executando = false;
        }
    }
}