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

        private string bandeira;

        public bool GetMyBand { get { return this.myBand; } }
        public bool Pegada { get { return this.mostrarAtual; } set { this.mostrarAtual = value; } }
        public float GetPosX { get { return this.xAtual; } }
        public float GetPosY { get { return this.yAtual; } }
        public float GetTam { get { return this.tamX; } }
        public string GetBandeira { get { return this.bandeira; } }


        public Bandeira(AreaPlayers area, int qualBand)
        {
            //defino o tamanho da bandeira.
            this.tamX = AreaPlayers.CalcPercet(5f, area.GetAltPb);
            this.mostrarInicial = true;
            this.mostrarAtual = this.mostrarInicial;

            //e a bandeira do player local. ----> BANDEIRA DO O LADO DIREITO.
            if (qualBand == 1)
            {
                this.xInicial = AreaPlayers.CalcPercet(5, area.GetLargPb) - (this.tamX / 2);
                this.yInicial = AreaPlayers.CalcPercet(90, area.GetAltPb) - (this.tamY / 2);

                this.xAtual = this.xInicial;
                this.yAtual = this.yInicial;
                this.myBand = false;
                this.bandeira = "B1";
            }//nao e a bandeira do player local. e a bandeira do player remoto. 
                //------> BANDEIRA DO LADO ESQUERDO.
            else if(qualBand == 0)
            {
                

                this.xInicial = AreaPlayers.CalcPercet(95, area.GetLargPb) - (this.tamX / 2);
                this.yInicial = AreaPlayers.CalcPercet(10, area.GetAltPb);

                this.xAtual = this.xInicial;
                this.yAtual = this.yInicial;
                this.myBand = true;
                this.bandeira = "B2";
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