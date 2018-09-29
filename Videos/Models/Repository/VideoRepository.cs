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

        public int getTotalVideos() {
            return db.video.Count();
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
            video.tamanho = dados.Tamanho;
            video.formato_video = dados.FormatoVideo;
            video.fps = dados.Fps;
            video.canais_audio = dados.CanaisAudio;
            video.formato_audio = dados.FormatoAudio;

            db.Entry(video).State = EntityState.Modified;

            foreach (artista artista in dados.Artistas) {
                if (!db.video_artista.Any(va => va.id_artista == artista.id && va.id_video == video.id)) {
                    db.video_artista.Add(new video_artista { id_artista = artista.id, id_video = video.id, principal = false });
                }
            }

            int[] artistas = dados.Artistas.Select(v => v.id).ToArray();
            List<video_artista> excluirArtistas = db.video_artista.
                Where(va => !artistas.Contains(va.id_artista)).
                Where(va => va.id_video == video.id).
                Where(va => va.principal == false).
                ToList();

            foreach (video_artista va in excluirArtistas) {
                db.video_artista.Remove(va);
            }

            MusicaRepository musicaRepository = new MusicaRepository();
            foreach (musica musica in dados.Musicas) {
                if (!db.video_musica.Any(vm => vm.id_musica == musica.id && vm.id_video == video.id)) {
                    db.video_musica.Add(new video_musica {
                        id_musica = musica.id,
                        id_video = video.id
                    });
                }
            }

            int[] musicas = dados.Musicas.Select(m => m.id).ToArray();
            List<video_musica> excluirMusicas = db.video_musica.
                Where(vm => !musicas.Contains(vm.id_musica)).
                Where(vm => vm.id_video == video.id).
                ToList();

            foreach (video_musica vm in excluirMusicas) {
                db.video_musica.Remove(vm);
            }

            TagRepository tagRepository = new TagRepository();
            foreach (tag tag in dados.Tags) {
                if (!db.video_tag.Any(vt => vt.id_tag == tag.id && vt.id_video == video.id)) {
                    db.video_tag.Add(new video_tag {
                        id_tag = tag.id,
                        id_video = video.id
                    });
                }
            }

            int[] tags = dados.Tags.Select(t => t.id).ToArray();
            List<video_tag> excluirTags = db.video_tag.
                Where(vt => !tags.Contains(vt.id_tag)).
                Where(vt => vt.id_video == video.id).
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
            video.categoria = getCategoriaByCaminho(arquivo);
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
                    return db.tipo.Where(d => d.pasta == descricao).FirstOrDefault();
                }
            }
            return db.tipo.Where(d => d.descricao == "misc").FirstOrDefault();
        }

        private categoria getCategoriaByCaminho(string caminho) {
            string[] split = caminho.Split('\\');
            string[] categorias = { "kpop", "jpop", "western" };
            if (split.Length > 3) {
                if (categorias.Contains(split[3])) {
                    string descricao = split[3];
                    return db.categoria.Where(c => c.pasta == descricao).FirstOrDefault();
                }
            }
            return null;
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

        public void excluir(video video) {
            BaseVideoView view = new BaseVideoView();
            List<string> arquivos = Directory.GetFiles(view.pastaCapturas, video.id + "_*").ToList();
            foreach (string arquivo in arquivos) {
                File.Delete(arquivo);
            }
            db.video.Remove(video);
            db.SaveChanges();
        }

        public void excluirArquivo(video video) {
            File.Delete(video.caminho);
        }

        public void setFavorito(int id, bool favorito) {
            video video = db.video.Find(id);
            video.favorito = favorito;
            db.SaveChanges();
        }
    }
}