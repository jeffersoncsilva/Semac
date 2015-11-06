using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PegaBandeira
{
    class AreaPlayers
    {
        private int larg;
        private int alt;
        private PosRect playerLocal;
        private PosRect playerRemoto;


        public AreaPlayers(int x, int y)
        {
            this.larg = x;
            this.alt = y;
            SetaAreas();
        }
        /*
         * TRETA:  a area do player local e a area do player remoto e a area do player remoto e a area do player local
         * TA TROCADA AS AREAS. SÃO AO CONTRARIO. A DO LOCAL E O REMOTO E A DO REMOTO E DO LOCAL.
         * E MAIS FACIL MUDA ISSO AQUI DO QUE EM OUTRO LUGAR. AI SO GERA DUVIDA AQUI E NAO EM OUTROS LOCAIS.
         * 
        */
        public PosRect GetAreaPlayerLocal { get { return this.playerLocal; } }
        public PosRect GetAreaPlayerRemoto { get { return this.playerRemoto; } }

        //retorna a largura e altura do Picturebox.
        public int GetLargPb { get { return this.larg; } }
        public int GetAltPb { get { return this.alt; } }


        private void SetaAreas()
        {
            //crio os objetos que irao guardar as areas dos players.
            playerLocal = new PosRect();
            playerRemoto = new PosRect();

            //refere-se a area do primeiro player.
            playerLocal.altura = CalcPercet(90, this.alt);
            playerLocal.largura = CalcPercet(10, this.larg);
            playerLocal.x = 0;
            playerLocal.y = CalcPercet(10, this.alt);

            //refere-se a area do segundo player.
            playerRemoto.altura = CalcPercet(90, this.alt);
            playerRemoto.largura = CalcPercet(10, this.larg);
            playerRemoto.y = CalcPercet(10, this.alt);
            playerRemoto.x = CalcPercet(90, this.larg);
        }

        public static float CalcPercet(float porcento, float valor)
        {
            return valor * (porcento / 100);
        }


        public void Draw(Graphics g)
        {
            g.FillRectangle(new SolidBrush(Color.Aqua), playerLocal.x, playerLocal.y, playerLocal.largura, playerLocal.altura);
            g.FillRectangle(new SolidBrush(Color.AliceBlue), playerRemoto.x, playerRemoto.y, playerRemoto.largura, playerRemoto.altura);
        }

    }
}
