using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using System.Windows.Forms;

namespace PegaBandeira
{
    public class ServerUdp
    {
        ManualResetEvent reset = new ManualResetEvent(false);
        MenuInicial frm_Inicial;
        Thread escuta = null;
        Thread broadcast = null;
        Socket socket;

        const string BROADCAST = "255.255.255.255";
        const int BYTES = 512;
        byte[] bytesEnviados = new byte[BYTES];

        EndPoint jogQueConvidou;    //jogador que convidou para jogar. Servira para saber com quem ira jogar.
        EndPoint localEndPoint;

        List<QuemTaOn> userOnline;
        List<QuemTaOn> quemRespondeu;

        QuemTaOn cidadaoOn;
        static int id = 0;
        string nomePlayerLocal;
        string apelidoPlayerLocal;
        bool comecouJogo;

        public bool naoAceito;


        //Método construtor da classe
        public ServerUdp(MenuInicial frm)
        {
            this.frm_Inicial = frm;
            userOnline = new List<QuemTaOn>();
            quemRespondeu = new List<QuemTaOn>();
        }


        /// <summary>
        /// Iniciar a escutar os dados vindos de broadcast UDP.
        /// </summary>
        /// <param name="nL"></param>
        /// <param name="aL"></param>
        /// <param name="localIp"></param>
        public void IniciaEscuta(string nL, string aL)
        {
            this.nomePlayerLocal = nL;
            this.apelidoPlayerLocal = aL;
            this.escuta = new Thread(() => Escuta());
            this.escuta.Name = "EscutaBroadcast";
            this.escuta.Priority = ThreadPriority.Highest;
            this.escuta.IsBackground = true;
            this.escuta.Start();
        }


        /// <summary>
        /// Envia o broadcast na rede para ver quem ta online.
        /// </summary>
        public void EnviaBroadcastRede()
        {

                IPAddress broadCast = IPAddress.Parse(BROADCAST);
                IPEndPoint ipEnd = new IPEndPoint(broadCast, 20152);
                EndPoint end = (EndPoint)ipEnd;
                broadcast = new Thread(() => SendBroadCast(end));
                broadcast.Name = "Broadcast";
                broadcast.Start();
            
           
        }


        private void SendBroadCast(EndPoint brod)
        {
            while (MenuInicial.executando)
            {
                reset.Reset();
                Console.WriteLine("Voltou.");
                while (!comecouJogo)
                {
                    //Console.WriteLine("Broadcast enviado.");
                    string nome = string.Format(apelidoPlayerLocal + "|" + nomePlayerLocal);
                    int aux = nome.Length + 5;
                    string msg = string.Format("01{0}{1}", aux.ToString("000"), nome);
                    SendMsgUdp(msg, brod);
                    Thread.Sleep(1000);
                }
                reset.WaitOne();
                comecouJogo = false;
                Console.WriteLine("Volta o broadcast.");
            }
        }


        private void Escuta()
        {
            try
            {
                localEndPoint = new IPEndPoint(IPAddress.Any, 20152);
                this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                this.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
                this.socket.Bind(localEndPoint);

                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
    
                EndPoint epSender = (EndPoint)localEndPoint;
                this.socket.BeginReceiveFrom(bytesEnviados, 0, bytesEnviados.Length, SocketFlags.None, ref epSender, new AsyncCallback(EscutaCallback), epSender);
            }
            catch (Exception e)
            {
                MessageBox.Show("Erro no inicio da escuta. ERRO: " + e.ToString());
                Console.WriteLine(e.ToString());
            }
        }


        private void EscutaCallback(IAsyncResult iResult)
        {
            try
            {
                EndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
                int dadosLengh = 0;
                byte[] dados = null;
                try
                {
                    dadosLengh = this.socket.EndReceiveFrom(iResult, ref endPoint);
                    dados = new byte[dadosLengh];
                    Array.Copy(bytesEnviados, dados, dadosLengh);
                    VerificaMsgUdp(dados, endPoint);
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("Erro na hora de recomeçar a escuta Callcabck.");
                    Console.WriteLine(ex.ToString());
                }
                finally
                {
                    EndPoint newEnd = new IPEndPoint(IPAddress.Any, 0);
                    this.socket.BeginReceiveFrom(bytesEnviados, 0, bytesEnviados.Length, SocketFlags.None, ref newEnd, new AsyncCallback(EscutaCallback), newEnd);
                }
            }
            catch (Exception e)
            {
                //MessageBox.Show("Erro na escuta Callcabck.");
                Console.WriteLine(e.ToString());
            }
        }


        public void SendMsgUdp(string msg, EndPoint destino)
        {
            try
            {
                byte[] dadosToSend = System.Text.Encoding.ASCII.GetBytes(msg);
                this.socket.BeginSendTo(dadosToSend, 0, dadosToSend.Length, SocketFlags.None, destino, new AsyncCallback(SendCallback), destino);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao enviar mensagem. ERRO" + ex.ToString());
                return;
            }
        }


        private void SendCallback(IAsyncResult iResult)
        {
            try
            {
                this.socket.EndSendTo(iResult);
            }
            catch
            {
                MessageBox.Show("Erro ao enviar mensagem.");
            }
        }


        //funções auxiliares
        private bool TemEsseJogador(EndPoint jo)
        {
            foreach (var jog in userOnline)
            {
                if (jog.cidEndPoint.ToString() == jo.ToString())
                    return true;


                //Console.WriteLine("N: " + jog.nome);
                //Console.WriteLine("A: " + jog.apelido);
                //Console.WriteLine("EP: " + jog.cidEndPoint.ToString());
                //Console.WriteLine("ID: " + jog.id);


            }

            return false;
        }


        //MSG 03
        /// <summary>
        /// Procura o cidadao escolhido na lista e envia um convite para iniciar a jogar com esse cidadao.
        /// </summary>
        /// <param name="index"></param>
        public void EnviaConviteJogo(int index)
        {
            foreach (var cidadao in userOnline)
            {
                if (cidadao.id == index)
                {
                    int aux = apelidoPlayerLocal.Count() + 5;
                    string msg = string.Format("03" + aux.ToString("000") + apelidoPlayerLocal);
                    SendMsgUdp(msg, cidadao.cidEndPoint);
                    this.jogQueConvidou = cidadao.cidEndPoint;
                    frm_Inicial.Invoke((MethodInvoker)delegate() { frm_Inicial.AddNomeEnemy(cidadao.apelido); });
                }
            }
        }


        /// <summary>
        /// Envia essa msg se for aceito o convite.
        /// </summary>
        public void AceitoConvite()
        {
            string aux = string.Format(apelidoPlayerLocal + "|" + Conecta.PORTA_CONEXAO);
            int comp = aux.Length + 5;
            string msg = string.Format("04" + comp.ToString("000") + aux);
            SendMsgUdp(msg, this.jogQueConvidou);
        }


        /// <summary>
        /// Envia essa msg se nao quiser esse convite.
        /// </summary>
        public void RecusaConviteJogo()
        {
            string aux = string.Format(apelidoPlayerLocal + "|" + 0);
            int comp = aux.Length + 5;
            string msg = string.Format("04" + comp.ToString("000") + aux);
            SendMsgUdp(msg, this.jogQueConvidou);
        }


        private void AddListOnline(string msg, EndPoint cidadao)
        {
            if (!TemEsseJogador(cidadao))
            {
                string newMsg = msg.Remove(0, 5);
                string[] noAp = newMsg.Split('|');
                cidadaoOn = new QuemTaOn();
                this.cidadaoOn.cidEndPoint = cidadao;
                this.cidadaoOn.nome = noAp[1];
                this.cidadaoOn.apelido = noAp[0];
                this.cidadaoOn.id = id;
                this.userOnline.Add(cidadaoOn);
                id += 1;
                frm_Inicial.Invoke((MethodInvoker)delegate() { frm_Inicial.AddJogador(this.cidadaoOn.nome, this.cidadaoOn.apelido); });
                frm_Inicial.Invoke((MethodInvoker)delegate() { frm_Inicial.AtivaConvite(); });
            }
        }


        private void VerificaMsgUdp(byte[] dados, EndPoint remetente)
        {
            //Console.WriteLine("Recebendo MSG.");
            string msgRecebida = System.Text.Encoding.ASCII.GetString(dados);
           
            string tipo = msgRecebida.Substring(0, 2);
            int tam = int.Parse(msgRecebida.Substring(2, 3));
            string str = msgRecebida.Remove(0, 5);
            string[] nomeApelido = str.Split('|');
            
            if (nomeApelido[0] == this.apelidoPlayerLocal)
                return;

            switch (tipo)
            {
                case "01":
                    string msgResp = string.Format(apelidoPlayerLocal + "|" + nomePlayerLocal);
                    int tamResp = msgResp.Count() + 5;
                    SendMsgUdp(string.Format("02" + tamResp.ToString("000") + msgResp), remetente);
                    AddListOnline(msgRecebida, remetente);
                    break;

                case "02":
                    AddListOnline(msgRecebida, remetente);
                    break;

                case "03":
                    this.jogQueConvidou = remetente;
                    this.frm_Inicial.Invoke((MethodInvoker)delegate() { this.frm_Inicial.ConviteReciv(nomeApelido[0]); });
                    break;

                case "04":
                    if (nomeApelido[1].Count() > 3)
                    {
                        if (jogQueConvidou.AddressFamily == remetente.AddressFamily && naoAceito)
                            return;

                        int p = int.Parse(nomeApelido[1]);
                        this.frm_Inicial.Invoke((MethodInvoker)delegate() { this.frm_Inicial.ConnectToServerTcp(GetIp(remetente), p); });
                    }
                    else
                    {
                        this.frm_Inicial.Invoke((MethodInvoker)delegate() { this.frm_Inicial.ConexaoNegada(); });
                    }
                    break;
            }
        }


        private string GetIp(EndPoint ed)
        {
            string s = ed.ToString();
            string[] i = s.Split(':');
            return i[0];
        }


        public void ParaUdp()
        {
            this.comecouJogo = true;
            //Console.WriteLine("Parou o UDP.");
        }


        public void VoltaUdpBroadcast()
        {
            reset.Set();
        }


    }

    public class QuemTaOn
    {
        public int id;
        public string nome;
        public string apelido;
        public EndPoint cidEndPoint;
        public bool perguntei;
    }

    public class Conexoes
    {
        public Socket socket;
        public String ip;
        public int porta;
    }

    class StateObject
    {
        // Criação de objeto para leitura assíncrona de dado
        public Socket workSocket = null;                // socket do cliente
        public const int BufferSize = 1024;             // tamanho do buffer de leitura (recebimento)
        public byte[] buffer = new byte[BufferSize];    // buffer de leitura (recebimento)
    }

}