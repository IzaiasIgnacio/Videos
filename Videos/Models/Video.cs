using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Videos.Models {
    public class Video {
        string caminho;
        string titulo;
        string extensao;
        DateTime data;
        TipoVideo tipo;
        Artista artista;

        public string Caminho { get; set; }
        public string Titulo { get; set; }
        public string Extensao { get; set; }
        public DateTime Data { get; set; }
        public TipoVideo Tipo { get; set; }
        public Artista Artista { get; set; }
    }
}