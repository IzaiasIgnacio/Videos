using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using Videos.Models.Entity;
using Videos.Models.ViewModel;

namespace Videos.Models.Repository {
    public class VideoRepository : BaseRepository {

        public video getVideoById(int id) {
            return db.video.Where(v => v.id == id).FirstOrDefault();
        }
        
        public List<video> listarVideos() {
            List<video> listaVideos;

            listaVideos = (from video in db.video
                           join video_artista in db.video_artista on video.id equals video_artista.id_video
                           join artista in db.artista on video_artista.id_artista equals artista.id
                           join tipo in db.tipo on video.id_tipo equals tipo.id
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
        
        public void salvarVideo(VideoDataView dados) {
            video video = db.video.Find(dados.Id);

            if (video.titulo != dados.Titulo) {
                FileInfo file = new FileInfo(video.caminho);
                File.Move(video.caminho, file.DirectoryName +"\\"+ dados.Titulo+video.extensao);
                video.titulo = dados.Titulo;
                video.caminho = file.DirectoryName + "\\" + dados.Titulo + video.extensao;
            }
            video.duracao = dados.Duracao;
            video.resolucao = dados.Resolucao;
            video.formato_video = dados.FormatoVideo;
            video.fps = dados.Fps;
            video.canais_audio = dados.CanaisAudio;
            video.formato_audio = dados.FormatoAudio;

            db.Entry(video).State = EntityState.Modified;

            foreach (artista artista in dados.Artistas) {
                db.video_artista.Add(new video_artista { id_artista = artista.id, id_video = video.id, principal = false });
            }

            int[] artistas = dados.Artistas.Select(v => v.id).ToArray();
            List<video_artista> excluirArtistas = db.video_artista.
                Where(v => !artistas.Contains(v.id)).
                Where(v => v.id_video == video.id).
                Where(v => v.principal == false).
                ToList();

            foreach (video_artista va in excluirArtistas) {
                db.video_artista.Remove(va);
            }

            MusicaRepository musicaRepository = new MusicaRepository();
            foreach (musica musica in dados.Musicas) {
                db.video_musica.Add(new video_musica {
                    id_musica = musica.id,
                    id_video = video.id
                });
            }

            int[] musicas = dados.Musicas.Select(m => m.id).ToArray();
            List<video_musica> excluirMusicas = db.video_musica.
                Where(v => !musicas.Contains(v.id)).
                Where(v => v.id_video == video.id).
                ToList();

            foreach (video_musica vm in excluirMusicas) {
                db.video_musica.Remove(vm);
            }

            TagRepository tagRepository = new TagRepository();
            foreach (tag tag in dados.Tags) {
                db.video_tag.Add(new video_tag {
                    id_tag = tag.id,
                    id_video = video.id
                });
            }

            int[] tags = dados.Tags.Select(t => t.id).ToArray();
            List<video_tag> excluirTags = db.video_tag.
                Where(v => !tags.Contains(v.id)).
                Where(v => v.id_video == video.id).
                ToList();

            foreach (video_tag vt in excluirTags) {
                db.video_tag.Remove(vt);
            }

            db.SaveChanges();
        }

        public video salvar(string arquivo) {
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

            return video;
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

        public video findByCaminho(string caminho) {
            return db.video.Where(v => v.caminho == caminho).FirstOrDefault();
        }

        public void adicionarVideoMusica(int id_video, int id_musica) {
            if (!db.video_musica.Any(v => v.id_musica == id_musica && v.id_video == id_video)) {
                video_musica vm = new video_musica { id_video = id_video, id_musica = id_musica };
                db.video_musica.Add(vm);
                db.SaveChanges();
            }
        }
        
    }
}