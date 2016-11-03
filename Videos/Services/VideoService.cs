using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Videos.Models;

namespace Videos.Services {
    public class VideoService {
        List<Video> listaArquivos;
        string caminho;
        string artista;
        string tipo;

        #region getters and setters
        public string Caminho {
            get {
                return caminho;
            }
            set {
                caminho = value;
            }
        }

        public string Tipo {
            get {
                return tipoCaminho();
            }
            set {
                tipo = value;
            }
        }

        public string Artista {
            get {
                return artistaCaminho();
            }
            set {
                artista = value;
            }
        }
        
        public List<Video> ListaArquivos {
            get {
                return listarArquivos();
            }
            set {
                listaArquivos = value;
            }
        }
        #endregion

        private List<Video> listarArquivos() {
            listaArquivos = new List<Video>();
            try {
                string[] arquivos = Directory.GetFiles(caminho);
                foreach (string arquivo in arquivos) {
                    FileInfo info = new FileInfo(arquivo);

                    if (!info.Extension.Equals(".nfo") && !info.Extension.Equals(".srt")) {
                        Video video = new Video();

                        TipoVideo tipo = new TipoVideo();
                        tipo.Tipo = tipoCaminho();

                        Artista artista = new Artista(artistaCaminho());

                        video.Caminho = info.FullName;
                        video.Titulo = Path.GetFileNameWithoutExtension(info.Name);
                        video.Data = info.LastWriteTime;
                        video.Tipo = tipo;
                        video.Artista = artista;
                        video.Extensao = info.Extension;

                        listaArquivos.Add(video);
                    }
                }
                return listaArquivos;
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public List<VideoEntity> listarVideos() {
            VideosEntities db = new VideosEntities();
            List<VideoEntity> listaVideos;

            listaVideos = (from video in db.Video
                           join a in db.Artista on video.artista equals a.id
                           join t in db.Tipo on video.tipo equals t.id
                           //where video.artista.(artista)
                           select video).OrderByDescending(video=>video.data).ToList();
                                
            return listaVideos;
        }

        private string artistaCaminho() {
            string[] split = caminho.Split('\\');
            return split[4];
        }

        private string tipoCaminho() {
            string[] split = caminho.Split('\\');
            string[] tipos = {"Lives","MVs"};
            if (split.Length > 5) {
                if (tipos.Contains(split[5])) {
                    return split[5];
                }
            }
            return "misc";
        }

    }
}