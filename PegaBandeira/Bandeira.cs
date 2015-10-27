using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PegaBandeira
{
    class Bandeira : ElementoJogo
    {
        private bool myBand;


        public bool GetMyBand { get { return this.myBand; } }
        public bool Pegada { get { return this.mostrarAtual; } set { this.mostrarAtual = value; } }
        public float GetPosX { get { return this.xAtual; } }
        public float GetPosY { get { return this.yAtual; } }
        public float GetTam { get { return this.tamX; } }



        public Bandeira(AreaPlayers area, int qualBand)
        {
            //defino o tamanho da bandeira.
            this.tamX = AreaPlayers.CalcPercet(5f, area.GetAltPb);
            this.mostrarInicial = true;
            this.mostrarAtual = this.mostrarInicial;

            //e a bandeira do player local.
            if (qualBand == 0)
            {
                //defini a posição da bandeira do player local.
                this.xInicial = (area.GetAreaPlayerLocal.largura / 2) - (this.tamX / 2);
                this.yInicial = area.GetAreaPlayerLocal.altura - (AreaPlayers.CalcPercet(2, area.GetAreaPlayerLocal.altura)) + this.tamX;
                this.xAtual = this.xInicial;
                this.yAtual = this.yInicial;
                this.myBand = false;
            }//nao e a bandeira do player local. e a bandeira do player remoto.
            else if(qualBand == 1)
            {
                this.xInicial = area.GetAreaPlayerRemoto.x + (area.GetAreaPlayerRemoto.largura / 2) - (this.tamX / 2);
                this.yInicial = AreaPlayers.CalcPercet(12, area.GetAreaPlayerLocal.altura);
                this.xAtual = this.xInicial;
                this.yAtual = this.yInicial;
                this.myBand = true;
            }
        }

        public void Draw(Graphics g)
        {
            if (this.mostrarAtual)
            {
                g.FillRectangle(new SolidBrush(Color.Blue), this.xAtual, this.yAtual, this.tamX, this.tamX);
            }
        }

    }
}