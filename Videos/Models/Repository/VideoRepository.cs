using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Videos.Models.Entity;

namespace Videos.Models.Repository {
    public class VideoRepository : BaseRepository {

        public video getVideoById(int id) {
            return db.video.Where(v => v.id == id).FirstOrDefault();
        }
        
        /*private List<video> listarArquivos(string caminho) {
            List<video> listaArquivos = new List<video>();
            try {
                string[] arquivos = Directory.GetFiles(caminho);
                foreach (string arquivo in arquivos) {
                    FileInfo info = new FileInfo(arquivo);

                    if (!info.Extension.Equals(".nfo") && !info.Extension.Equals(".srt")) {
                        video video = new video();

                        tipo tipo = new tipo();
                        tipo.descricao = tipoCaminho();

                        artista artista = new artista(artistaCaminho());

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
        }*/

        public List<video> listarVideos() {
            List<video> listaVideos;

            listaVideos = (from video in db.video
                           join video_artista in db.video_artista on video.id equals video_artista.id_video
                           join artista in db.artista on video_artista.id_artista equals artista.id
                           join tipo in db.tipo on video.id_tipo equals tipo.id
                           where video_artista.principal == true
                           select video).OrderByDescending(video=>video.data).ToList();
                                
            return listaVideos;
        }

        public List<video> VideosPlaylist(int[] artistas, int[] tipos, int[] tags = null) {
            List<video> listaVideos;

            listaVideos = (from video in db.video
                           join video_artista in db.video_artista on video.id equals video_artista.id_video
                           join video_tag in db.video_tag on video.id equals video_tag.id into tag
                           from t in tag.DefaultIfEmpty()
                           where artistas.Contains(video_artista.id_artista)
                           where tipos.Contains(video.id_tipo)
                           select video).ToList();
            
            if (tags != null) {
                List<video> remover = new List<video>();
                foreach (video video in listaVideos) {
                    if (video.video_tag.Count == 0) {
                        remover.Add(video);
                    }
                    else {
                        bool remove = true;
                        foreach (video_tag tag in video.video_tag) {
                            if (tags.Contains(tag.id_tag)) {
                                remove = false;
                            }
                        }
                        if (remove) {
                            remover.Add(video);
                        }
                    }        
                }
                listaVideos.RemoveAll(v => remover.ToArray().Contains(v));
            }
            
            listaVideos = listaVideos.OrderByDescending(video => video.data).ToList();

            return listaVideos;
        }

        public void salvar(string arquivo) {
            FileInfo dados = new FileInfo(arquivo);
            video video = new video();

            video.titulo = Path.GetFileNameWithoutExtension(dados.Name);
            video.caminho = dados.FullName;
            video.data = dados.LastWriteTime;
            video.extensao = dados.Extension;
            video.tipo = getTipoByCaminho(arquivo);
            video.video_artista = new List<video_artista>();
            video.video_artista.Add(new video_artista { artista = getArtistaByCaminho(arquivo), video = video, principal = true });

            db.video.Add(video);
            db.SaveChanges();
        }

        private tipo getTipoByCaminho(string caminho) {
            string[] split = caminho.Split('\\');
            string[] tipos = { "Lives", "MVs" };
            if (split.Length > 5) {
                if (tipos.Contains(split[5])) {
                    string descricao = split[5];
                    return db.tipo.Where(d => d.descricao == descricao).FirstOrDefault();
                }
            }
            return db.tipo.Where(d => d.descricao == "misc").FirstOrDefault();
        }

        private artista getArtistaByCaminho(string caminho) {
            string split = caminho.Split('\\')[4];
            return db.artista.Where(n => n.nome == split).FirstOrDefault();
        }

        public bool findByCaminho(string caminho) {
            return db.video.Where(v => v.caminho == caminho).Any();
        }
        
    }
}