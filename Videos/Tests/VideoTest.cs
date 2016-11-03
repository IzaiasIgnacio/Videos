using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Videos.Services;
using Videos.Models;
using System.IO;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Text;

namespace Videos.Tests {

    [TestClass]
    public class VideoServiceTest {

        [TestMethod]
        public void testeListaArquivos() {
            VideoService video = new VideoService();
            List<Video> listaArquivos = new List<Video>();
            string caminho = @"K:\ICI\Vídeos\kpop";
            string[] pastas = Directory.GetDirectories(caminho, "*", System.IO.SearchOption.AllDirectories);
            foreach (string pasta in pastas) {
                video.Caminho = pasta;
                listaArquivos.AddRange(video.ListaArquivos);
            }
            Assert.IsNotNull(listaArquivos);
        }

        [TestMethod]
        public void testeListaPastasArtistas() {
            List<String> listaPastas = new List<string>();
            string caminho = @"K:\ICI\Vídeos\kpop";
            string[] pastas = Directory.GetDirectories(caminho);
            foreach (var pasta in pastas) {
                DirectoryInfo info = new DirectoryInfo(pasta);
                listaPastas.Add(info.Name);
            }
            Assert.IsNotNull(listaPastas);
        }

        [TestMethod]
        public void testeAtualizarVideos() {
            VideoService video = new VideoService();
            List<Video> listaArquivos = new List<Video>();
            string caminho = @"K:\ICI\Vídeos\kpop";
            string[] pastas = Directory.GetDirectories(caminho, "*", System.IO.SearchOption.AllDirectories);
            foreach (string pasta in pastas) {
                video.Caminho = pasta;
                listaArquivos.AddRange(video.ListaArquivos);
            }
            VideoEntity videoEntity;
            ArtistaEntity artistaEntity;
            TipoEntity tipoEntity;

            var db = new VideosEntities();
            foreach (var arquivo in listaArquivos) {
                video.Caminho = arquivo.Caminho;
                string nomeArtista = video.Artista;
                string tipo = video.Tipo;

                videoEntity = db.Video.FirstOrDefault(a => a.caminho == arquivo.Caminho);
                if (videoEntity == null) {
                    videoEntity = new VideoEntity();
                    artistaEntity = new ArtistaEntity();
                    tipoEntity = new TipoEntity();
                    db.Video.Add(videoEntity);

                    artistaEntity = db.Artista.First(a => a.artista == nomeArtista);
                    tipoEntity = db.Tipo.First(a => a.tipo == tipo);

                    videoEntity.titulo = arquivo.Titulo;
                    videoEntity.caminho = video.Caminho;
                    videoEntity.artista = artistaEntity.id;
                    videoEntity.tipo = tipoEntity.id;
                    videoEntity.extensao = arquivo.Extensao;
                    videoEntity.data = arquivo.Data;
                    try {
                        db.SaveChanges();
                    }
                    catch(DbEntityValidationException ex) {
                        foreach (var validationErrors in ex.EntityValidationErrors) {
                            foreach (var validationError in validationErrors.ValidationErrors) {
                                string a = validationError.PropertyName;
                                string b = validationError.ErrorMessage;
                            }
                        }
                    }
                    catch (Exception ex) {
                        //TODO
                    }
                }
            }
        }

        [TestMethod]
        public void testeArtistaCaminho() {
            VideoService video = new VideoService();
            video.Caminho = @"K:\ICI\Vídeos\kpop\red velvet\MVs";
            string artista = video.Artista;
            Assert.IsInstanceOfType(artista,typeof(string));
        }

        [TestMethod]
        public void testeTipoCaminho() {
            VideoService video = new VideoService();
            video.Caminho = @"K:\ICI\Vídeos\kpop\red velvet\MVs";
            string tipo = video.Tipo;
            Assert.IsInstanceOfType(tipo, typeof(string));
        }

        [TestMethod]
        public void testeInserirArtista() {
            VideoService video = new VideoService();
            video.Caminho = @"K:\ICI\Vídeos\kpop\red velvet\MVs";
            string nomeArtista = video.Artista;
            ArtistaEntity artistaEntity;

            var db = new VideosEntities();
            artistaEntity = db.Artista.FirstOrDefault(b => b.artista == nomeArtista);
            if (artistaEntity == null) {
                artistaEntity = new ArtistaEntity();
                db.Artista.Add(artistaEntity);
                artistaEntity.artista = nomeArtista;
                db.SaveChanges();
            }
        }

        [TestMethod]
        public void testeGerarPlaylist() {
            VideoService videoService = new VideoService();
            List<VideoEntity> listaVideos = videoService.listarVideos();

            using (FileStream file = new FileStream("playlist.m3u", FileMode.Create)) {
                byte[] linha = Encoding.UTF8.GetBytes(Environment.NewLine);
                foreach (VideoEntity video in listaVideos) {
                    byte[] bytes = Encoding.UTF8.GetBytes(video.caminho);
                    file.Write(bytes, 0, bytes.Length);
                    file.Write(linha,0,linha.Length);
                }
            }
        }
        
    }
}