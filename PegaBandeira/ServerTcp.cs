﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;


namespace PegaBandeira
{
    /*
 * Trabalho desenvolvido na integração das disciplinas
 * Programação Concorrente 
 * Oficina de Jogos Multiplayer 
 * Redes de Computadores
 * 
 * Alunos:
 * Jefferson C. Silva
 * Vinicios Coelho
 */
    public class ServerTcp : Conecta
    {
        const int MAX_CONNECTION = 1;
        private bool isListeningServerTcp;
        private static ManualResetEvent conectado = new ManualResetEvent(false);

        public ServerTcp(int porta, MenuInicial m)
        {
            this.frm_Inicial = m;
            this.PortaTcp = porta;
            isListeningServerTcp = false;
        }

        /// <summary>
        /// Inicia o servidor para estar apito a receber conexões.
        /// </summary>
        public override void IniciaServer()
        {
            this.escuta = new Thread(() => IniciaEscuta(this.PortaTcp));
            this.escuta.Name = "TrheadTcpEsc";
            this.escuta.Start();
        }


        /// <summary>
        /// Inicia a escuta de forma assincrona.
        /// </summary>
        /// <param name="porta"></param>
        protected override void IniciaEscuta(int porta)
        {

            isListeningServerTcp = false;
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, this.PortaTcp);
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                this.socket.Bind(localEndPoint);
                this.socket.Listen(MAX_CONNECTION);
                isListeningServerTcp = true;
                while (true)
                {
                    conectado.Reset();
                    this.socket.BeginAccept(new AsyncCallback(AceitaConexaoCallback), this.socket);
                    conectado.WaitOne();
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Erro na hora de iniciar a escuta. ERRO: " + ex.ToString());
                Console.WriteLine(ex.ToString());
            }

        }


        private void AceitaConexaoCallback(IAsyncResult ar)
        {
            conectado.Set();
            try
            {
                Socket sTcp = (Socket)ar.AsyncState;
                Socket handler = this.socket.EndAccept(ar);
                AddCliConected(handler);
                StateObject estado = new StateObject();
                estado.workSocket = handler;
                handler.BeginReceive(estado.buffer, 0, StateObject.BufferSize, SocketFlags.None, new AsyncCallback(ServTcpRecivCallback), estado);
                this.frm_Inicial.Invoke((MethodInvoker)delegate() { this.frm_Inicial.ConexoAceita(); });
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Erro ao aceitar conexão. ERRO: " + ex.ToString());
            }
        }


        private void ServTcpRecivCallback(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;
            try
            {
                int bytesRead = handler.EndReceive(ar);
                if (bytesRead > 0)
                {
                    string dadosReciv = Encoding.ASCII.GetString(state.buffer, 0, bytesRead);
                    //hora de checar que tipo de dado foi recebido e o que fazer com eles.
                    VerificaDadosRecebidos(dadosReciv);
                    //recomeça o recebimento de dados de forma assincrona
                    StateObject newState = new StateObject();
                    newState.workSocket = handler;
                    handler.BeginReceive(newState.buffer, 0, StateObject.BufferSize, SocketFlags.None, new AsyncCallback(ServTcpRecivCallback), newState);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Conexão com o outro jogador perdida. ERRO: " + e.ToString());
                this.frm_Inicial.Invoke((MethodInvoker)delegate() { this.frm_Inicial.EmPartida = false; });
                this.frm_Inicial.Invoke((MethodInvoker)delegate() { this.frm_Inicial.TrataMsgDezenove(); });
            }
        }


        /// <summary>
        /// Realiza o manuzeio dos dados recebidos.
        /// </summary>
        /// <param name="msg">Dados recebidos.</param>
        protected override void VerificaDadosRecebidos(string msg)
        {
            while (!String.IsNullOrEmpty(msg))
            {
                int tipo = int.Parse(msg.Substring(0, 2));

                int tam = int.Parse(msg.Substring(2, 3));

                string dado = msg.Substring(0, tam);
                dado = dado.Remove(0, 5);

                msg = msg.Remove(0, tam);
                
                string[] dados = dado.Split('|');

                switch (tipo)
                {
                    case 10:
                        this.RemotePlayer = true;
                        if (this.RemotePlayer && this.LocalPlayer)
                        {   //player no lado esquerdo.
                            //this.frm_Inicial.Invoke((MethodInvoker)delegate() { this.frm_Inicial.EmPartida = true; });
                            this.frm_Inicial.Invoke((MethodInvoker)delegate() { this.frm_Inicial.CarregaCampoBatalha(0); });
                        }
                        break;

                    case 11:
                        this.frm_Inicial.Invoke((MethodInvoker)delegate() { this.frm_Inicial.TrataMsgOnze(dados); });
                        break;

                    case 12:
                        this.frm_Inicial.Invoke((MethodInvoker)delegate() { this.frm_Inicial.TrataMsgDoze(); });
                        break;

                    case 13:
                        this.frm_Inicial.Invoke((MethodInvoker)delegate() { this.frm_Inicial.TrataMsgTreze(dados); });
                        break;

                    case 14:
                        this.frm_Inicial.Invoke((MethodInvoker)delegate() { this.frm_Inicial.TrataMsgCatorze(); });
                        break;

                    case 15:
                        this.frm_Inicial.Invoke((MethodInvoker)delegate() { this.frm_Inicial.TrataMsgQuinze(dados); });
                        break;

                    case 16:
                        this.frm_Inicial.Invoke((MethodInvoker)delegate() { this.frm_Inicial.TrataMsgDezeceis(dados); });
                        break;

                    case 17:
                        this.frm_Inicial.Invoke((MethodInvoker)delegate() { this.frm_Inicial.TrataMsgDezecete(dados); });
                        break;

                    case 18:
                        this.frm_Inicial.Invoke((MethodInvoker)delegate() { this.frm_Inicial.TrataMsgDezoito(dados); });
                        break;

                    case 19:

                        //this.EncerraConexaoTcp();
                        //this.Msg19 = true;
                        this.frm_Inicial.Invoke((MethodInvoker)delegate() { this.frm_Inicial.TrataMsgDezenove(); });


                        break;
                }
            }
        }



        /// <summary>
        /// Somente envia a mensagem para quem estiver conectado.
        /// </summary>
        /// <param name="msg"></param>
        public override void EnviaMsg(string msg)
        {
            try
            {
                byte[] bytes = Encoding.ASCII.GetBytes(msg);
                Socket handler = this.conexao.socket;
                handler.BeginSend(bytes, 0, bytes.Length, 0, new AsyncCallback(ServTcpSendCallback), handler);
            }
            catch (Exception e)
            {
                //MessageBox.Show("Erro ao enviar dados. ERRO:" + e.ToString());
                Console.WriteLine(e.ToString());
            }
        }



        private void ServTcpSendCallback(IAsyncResult ar)
        {
            try
            {
                Socket handler = (Socket)ar.AsyncState;
                int nBytes = handler.EndSend(ar);
            }
            catch (Exception e)
            {
                //MessageBox.Show("Erro na função ServTcpSendCallback. Erro ao enviar msg. ERRO: " + e.ToString());
                Console.WriteLine(e.ToString());
            }
        }




    }
}