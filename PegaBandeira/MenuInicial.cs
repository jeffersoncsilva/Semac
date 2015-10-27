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
    /*
    * VERSÃO: 0.2.6
    */
    public partial class MenuInicial : Form
    {
        ServerUdp serverUdp;
        ServerTcp serverTcp = null;
        ClientTcp clientTcp = null;
        CampoBatalha cBat = null;

        private bool connect;
        private bool euDesisto;
        private bool foiConvidado;


        public MenuInicial()
        {
            InitializeComponent();
            serverUdp = new ServerUdp(this);
            HabilitaBotoes();
            this.connect = false;
            this.foiConvidado = false;
        }

        private void btn_VeriJog_Click(object sender, EventArgs e)
        {
            //Dispara o boradcast na rede para encontrar os jogadores que estiverem online
            gb_Config.Enabled = false;
            serverUdp.IniciaEscuta(txb_Nome.Text, txb_Apelido.Text);
            serverUdp.EnviaBroadcastRede();
            HabilitaBotoes();
        }

        private void btn_IniPartida_Click(object sender, EventArgs e)
        {
            string msg = "10005";
            if (serverTcp != null)
            {
                serverTcp.LocalPlayer = true;
                if (serverTcp.RemotePlayer && serverTcp.LocalPlayer)
                {
                    serverTcp.EnviaMsg(msg);
                    CarregaCampoBatalha();
                }
                else
                {
                    serverTcp.EnviaMsg(msg);
                }
            }
            else if (clientTcp != null)
            {
                clientTcp.LocalPlayer = true;
                if (clientTcp.RemotePlayer && clientTcp.LocalPlayer)
                {
                    clientTcp.EnviaMsg(msg);
                    CarregaCampoBatalha();
                }
                else
                {
                    clientTcp.EnviaMsg(msg);
                }
            }
            btn_IniPartida.Visible = false;
            lbl_Espera.Visible = true;
        }

        private void btn_Convida_Click(object sender, EventArgs e)
        {
            if (ltb_JogOn.Items.Count > 0)
            {
                serverUdp.EnviaConviteJogo(ltb_JogOn.SelectedIndex);
                btn_Convida.Enabled = false;
                tm_verJogOn.Start();
                //começo a contar o tempo de resposta
            }
        }

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
            }
        }

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




        //função chamada sempre que o nome e apelido e mudado para liberar o botao de verificar
        //jogadores online.
        private void AlteraNomeTxb(object sender, EventArgs e)
        {
            HabilitaBotoes();
        }


        //habilita o botao de verificar jogadores online (broadcast)
        private void HabilitaBotoes()
        {
            if (txb_Apelido.Text != "" && txb_Nome.Text != "")
            {
                btn_VeriJog.Enabled = true;
            }
        }

        //----------------------DELEGATES METHODS------------------

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
        /// <param name="jogadorNome"></param>
        /// <param name="jogadorApelido"></param>
        public void AddJogador(string jogadorNome, string jogad)
        {
            ltb_JogOn.Items.Add(string.Format(jogadorNome + "; " + jogad));
        }

        //mostra o nome do jogador adversario que convidou para jogo.
        public void AddNomeEnemy(string nome) 
        { 
            lbl_Convidou.Text = string.Format("Convidou: {0} para jogar. Esperando pela resposta dojogador.", nome);  
        }






        //mostra uma message box quado foi convidado para jogo, para escoher se deseja ou nao jogar.
        public void ConviteReciv(string apelido)
        {
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




        #region

        //Método que conecta como "cliente" ao outro jogador.
        public void ConnectToServerTcp(string ip, int port)
        {
            this.clientTcp = new ClientTcp(this);
            if (clientTcp.Conecta(ip, port))
            {
                ConexoAceita();
            }
        }

        //quando o jogador recusa o convite para jogo.
        public void ConexaoNegada()
        {
            lbl_Convidou.Text = "O jogador convidado recusou o convite.";
            btn_Convida.Enabled = true;
            btn_IniPartida.Visible = false;
        }

        //quando o jogador aceita o convite para jogo, e ja esta conectado com o adversario.
        public void ConexoAceita()
        {
            lbl_Convidou.Text = "Conectado com o outro jogador.";
            btn_IniPartida.Visible = true;
            this.connect = true;
            tm_verJogOn.Stop();
        }

        public void CarregaCampoBatalha()
        {
            //cBat = new CampoBatalha(this);
            cBat.Show();
            this.Hide();
        }

        //quando o jogador desiste de jogar. Quando ele desiste, envio uma msg de desistencia e fecha a conexao.
        public void Desisto()
        {
            this.euDesisto = true;

            string msg = "19005";
            if (this.serverTcp != null && !this.serverTcp.Msg19)
            {


                this.serverTcp.EnviaMsg(msg);
                this.serverTcp.EncerraConexaoTcp();
                this.serverTcp = null;


            }
            else if (this.clientTcp != null && !this.clientTcp.Msg19)
            {


                this.clientTcp.EnviaMsg(msg);
                this.clientTcp.EncerraConexaoTcp();
                this.clientTcp = null;

            }
           
            Padrao();
        }

        //quando o outro jogador encerra a conexão
        public void ConexaoEncerrada()
        {

            if (this.euDesisto)
                return;

            if (this.cBat != null)
            {
                this.cBat.Close();
                this.cBat = null;
            }

            MessageBox.Show("O outro jogador desistiu ou a conexão foi perdida. Clique em OK para voltar ao loby.");
            if (serverTcp != null)
            {
                this.serverTcp.EncerraConexaoTcp();
                this.serverTcp = null;
            }
            else if (clientTcp != null)
            {
                this.clientTcp.EncerraConexaoTcp();
                this.clientTcp = null;
            }
            
            Padrao();
        }        

        //restaura os dados importantes para os valores iniciais para poder iniciar outra partida.
        public void Padrao()
        {


            btn_IniPartida.Visible = false;
            gb_Convite.Enabled = true;
            gb_ConviteRecv.Visible = false;
            lbl_Espera.Visible = false;            
            this.connect = false;
            this.euDesisto = false;
            this.foiConvidado = false;
            //recomeça a thread e broadcast.


        }
        #endregion

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
    }
}