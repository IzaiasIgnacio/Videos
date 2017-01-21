using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Videos.Models {
    public class Artista {
        string nome;
        public string Nome {
            get {
                return nome;
            }
            set {
                nome = value;
            }
        }

        public Artista(string nomeArtista) {
            nome = nomeArtista;
        }
    }
}