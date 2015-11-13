using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    class ElementoJogo
    {
        public float xInicial;
        public float yInicial;
        public bool mostrarInicial;

        public float xAtual;
        public float yAtual;
        public bool mostrarAtual;


        public float tamX;
        public float tamY;

        public virtual void Reestart()
        {
            xAtual = xInicial;
            yAtual = yInicial;
            mostrarAtual = mostrarInicial;
        }
    }
}