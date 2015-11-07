using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PegaBandeira
{
    class PowerUp : ElementoJogo
    {
        Image powerUp;
        public PowerUp(float larg, float alt)
        {
            

            DefineTamanho(larg, alt);
            DefinePosicao(larg, alt);
            this.mostrarInicial = true;
            this.mostrarAtual = this.mostrarInicial;
            powerUp = ResizeImage.ScaleImage(Properties.Resources.pUp, this.tamX, this.tamX);
        }

        private void DefineTamanho(float larg, float alt)
        {
            this.tamX = AreaPlayers.CalcPercet(3f, larg);
            this.tamY = AreaPlayers.CalcPercet(3f, alt);
        }

        private void DefinePosicao(float larg, float alt)
        {
            this.xInicial = AreaPlayers.CalcPercet(50, larg) - (this.tamX / 2);
            this.yInicial = AreaPlayers.CalcPercet(50, alt) - (this.tamY / 2);
            this.xAtual = this.xInicial;
            this.yAtual = this.yInicial;
        }

        public void Draw(Graphics g)
        {
            if (this.mostrarAtual)
            {
                g.DrawImage(powerUp, xAtual, yAtual);
                //g.FillRectangle(new SolidBrush(Color.BlueViolet), this.xAtual, this.yAtual, this.tamX, this.tamX);
            }
        }


    }
}
