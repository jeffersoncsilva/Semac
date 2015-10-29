using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;


namespace PegaBandeira
{
    class ClientTcp : Conecta
    {
        private static ManualResetEvent connectDone = new ManualResetEvent(false);
        private static ManualResetEvent sendDone = new ManualResetEvent(false);
        private static ManualResetEvent receiveDone = new ManualResetEvent(false);
        //para saber se os dois jogadores ja estão prontos para começar a partida.

        public ClientTcp(MenuInicial m) { this.frm_Inicial = m; }

        public bool Conecta(string ip, int porta)
        {
            try
            {
                IPAddress ipa = IPAddress.Parse(ip);
                IPEndPoint remoteEp = new IPEndPoint(ipa, porta);
                this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                connectDone.Reset();
                this.socket.BeginConnect(remoteEp, new AsyncCallback(ConnectCallback), this.socket);
                connectDone.WaitOne();
                IniciaEscuta();
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("Erro na conexão. ERRO: " + e.ToString());
                return false;
            }
        }



        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;
                client.EndConnect(ar);
                connectDone.Set();
                AddCliConected(client);
            }
            catch (Exception e)
            {
                MessageBox.Show("Erro na conexão. ERRO: " + e.ToString());
            }
        }



        protected override void IniciaEscuta()
        {
            this.escuta = new Thread(() => ClientReciveTcp());
            this.escuta.Name = "EscutaTcpClient";
            this.escuta.Start();
            //verificar se ja existe uma conexão com o outro jogador.
        }

       
        //fução responsavel por receber os dados.
        private void ClientReciveTcp()
        {
            while (true)
            {
                if (this.socket == null)
                    return;
                try
                {
                    receiveDone.Reset();
                    StateObject state = new StateObject();
                    state.workSocket = this.socket;
                    this.socket.BeginReceive(state.buffer, 0, StateObject.BufferSize, SocketFlags.None, new AsyncCallback(RecevCliTcp), state);
                    receiveDone.WaitOne();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    return;
                }
            }
        }

        private void RecevCliTcp(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;
            try
            {
                int bytesRead = handler.EndReceive(ar);
                if (bytesRead > 0)
                {
                    string dadosReciv = Encoding.ASCII.GetString(state.buffer, 0, bytesRead);
                    //trata os dados recebidos.
                    VerificaDadosRecebidos(dadosReciv);
                }
                receiveDone.Set();
            }
            catch (Exception e)
            {
               Console.WriteLine("Conexão encerrada. ERRO: " + e.ToString());
               this.frm_Inicial.Invoke((MethodInvoker)delegate() { this.frm_Inicial.ConexaoEncerrada(); });
            }
        }

        

        public override void EnviaMsg(string msg)
        {
            try
            {
                byte[] byteData = Encoding.ASCII.GetBytes(msg);
                this.socket.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(EnviaCallback), this.socket);
            }
            catch (Exception e)
            {
                MessageBox.Show("Erro no envido de dados. ERRO: " + e.ToString());
            }
        }

        private void EnviaCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;
                int bytesSent = client.EndSend(ar);
                //dados foram enviados para o servidor.
            }
            catch (Exception e)
            {
                MessageBox.Show("Erro no envido de dados. ERRO: " + e.ToString());
            }
        }
        

        protected override void VerificaDadosRecebidos(string msg)
        {
            string tipo = msg.Substring(0, 2);
            int tam = int.Parse(msg.Substring(2, 3));
            string str = msg.Remove(0, 5);
            string[] dados = str.Split('|');

            switch (int.Parse(tipo))
            {
                case 10:
                    this.RemotePlayer = true;
                    if (this.RemotePlayer && this.LocalPlayer)
                    {   //player no lado direito.
                        this.frm_Inicial.Invoke((MethodInvoker)delegate() { this.frm_Inicial.CarregaCampoBatalha(1); });
                    }
                    break;


                case 11:
                    //Console.WriteLine("msg 11 recebida.");
                    this.frm_Inicial.Invoke((MethodInvoker)delegate() { this.frm_Inicial.TrataMsgOnze(dados); });

                    break;
                case 12:                    
                    //Console.WriteLine("Recebi msg 12. MSG: {0}", dados.ToString());
                    this.frm_Inicial.Invoke((MethodInvoker)delegate() { this.frm_Inicial.TrataMsgDoze(); });

                    break;
                case 13:
                    break;
                case 14:
                    break;
                case 15:
                    break;
                case 16:
                    break;
                case 17:
                    break;
                case 18:
                    break;
                case 19:
                    //MessageBox.Show("O outro jogador desistiu do jogo.");
                    this.EncerraConexaoTcp();
                    this.Msg19 = true;
                    this.frm_Inicial.Invoke((MethodInvoker)delegate() { this.frm_Inicial.ConexaoEncerrada(); });
                    break;
            }
        }
    }
}