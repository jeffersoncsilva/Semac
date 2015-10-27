using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PegaBandeira
{
    class Obstaculo : ElementoJogo
    {

        public Obstaculo(float x, float y, float tX, float tY)
        {
            this.xInicial = x;
            this.yInicial= y;
            this.xAtual = this.xInicial;
            this.yAtual = this.yInicial;
            this.tamX = tX;
            this.tamY = tY;
            this.mostrarInicial = true;
            this.mostrarAtual = this.mostrarInicial;
        }

        public void Draw(Graphics g)
        {
            if(this.mostrarAtual)
                g.FillRectangle(new SolidBrush(Color.Red), this.xAtual, this.yAtual, this.tamX, this.tamY);
        }
    }
}