using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;


namespace PegaBandeira
{
    public class Conecta
    {


        public const int PORTA_CONEXAO = 2015;
        protected MenuInicial frm_Inicial;
        protected Socket socket = null;
        protected Thread escuta = null;
        protected Thread checaConexao = null;
        protected Conexoes conexao = null;
        private bool localPlayer;
        private bool remotePlayer;
        private bool msg19;
        

        //public bool FoiConvidado { get { return this.fuiConvidado; } set { this.fuiConvidado = value; } }

        //public bool Msg19 { get { return this.msg19; } set { this.msg19 = value; } }
        public bool RemotePlayer { get { return this.remotePlayer; } set { this.remotePlayer = value; } }
        public bool LocalPlayer { get { return this.localPlayer; } set { this.localPlayer = value; } }


        //ip e porta local

        private int PortToConnect;


        //public bool EstaConectado()
        //{
        //    if (this.socket != null)
        //        return this.socket.Connected;
        //    else
        //        return false;
        //}


        //armazenão a porta e o ip para conectar com cliente (caso o jogador escolher jogar como cliente)
        //ou amazenarão o ip e porta local caso o jogador eja o servidor.
        private int localPort;         
        private string ipLocal;

        /// <summary>
        /// Retorna a porta de conexão TCP.
        /// </summary>
        protected int PortaTcp { get { return this.localPort; } set { this.localPort = value; } }
        protected string Ip { get { return this.ipLocal; } set { this.ipLocal = value; } }
        

        public virtual void IniciaServer() { }

        public virtual void EnviaMsg(string msg) { }

        protected virtual void IniciaEscuta(int porta) { }

        protected virtual void IniciaEscuta() { }

        protected virtual void VerificaDadosRecebidos(string msg) { }

        protected virtual void StartCheckConnection() { }


        //********

        protected void Verifica()
        {
            bool conectado = true;
            while (conectado)
            {
                if (VerificaConexaoTcp())
                {
                    conectado = false;
                    //EncerraConexaoTcp(sockTcp);
                    //ParaEscutaServTcp();
                    MessageBox.Show("A conexão com o outro jogador foi encerrada. Clique em OK para voltar ao loby.");
                    //como o formulario inicial estará escondido, mostra ele novamente e reinicia a thread de escuta UDP.
                    //this.frm_Inicial.Show();

                }
                Thread.Sleep(15);
            }
        }

        public bool VerificaConexaoTcp()
        {
            bool hasProblem = false;
            bool part1, part2;

            part1 = this.conexao.socket.Poll(10, SelectMode.SelectRead);
            part2 = this.conexao.socket.Available == 0;

            if (part1 && part2)
            {
                //encerro a coneção
                EncerraConexaoTcp();
                this.conexao = null;
                hasProblem = true;
            }
            return hasProblem;
        }

        public void EncerraConexaoTcp()
        {
            
            //try
            //{
                //Console.WriteLine("Estado conexao: " + VerificaConexaoTcp());
                //if (!VerificaConexaoTcp())
                //{
                    //this.socket.Shutdown(SocketShutdown.Both);
                    //socket.Disconnect(true);
                    this.socket.Close();
                    this.socket = null;
                    Console.WriteLine("Encerrou a conexao.");
                //}
                
            //}
            //catch (Exception e)
            //{
            //    MessageBox.Show("Erro ao encerrar conexão. ERRO: " + e.ToString());
            //    Console.WriteLine(e.ToString());
            //}
        }

        protected void AddCliConected(Socket handler)
        {
            try
            {
                string ip = IPAddress.Parse(((IPEndPoint)handler.RemoteEndPoint).Address.ToString()).ToString();
                int porta = ((IPEndPoint)handler.RemoteEndPoint).Port;
                this.conexao = new Conexoes();
                this.conexao.socket = handler;
                this.conexao.ip = ip;
                this.conexao.porta = porta;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao adionar cliente a conexões.");
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
