using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Videos.Models.Entity;
using Videos.Models.ViewModel;
using Videos.Models.Repository;
using System.Linq;
using System.Text;
using MediaToolkit.Model;
using MediaToolkit;
using MediaToolkit.Options;
using Videos.Models.Services;

namespace Videos.Tests {

    [TestClass]
    public class VideoTest {

        /*[TestMethod]
        public void testeListaArquivos() {
            VideosView videoView = new VideosView();
            List<video> listaArquivos = new List<video>();
            string caminho = @"K:\ICI\Vídeos\kpop";
            string[] pastas = Directory.GetDirectories(caminho, "*", System.IO.SearchOption.AllDirectories);
            foreach (string pasta in pastas) {
                string[] arquivos = Directory.GetFiles(pasta);
                foreach (string arquivo in arquivos) {
                    FileInfo dados = new FileInfo(arquivo);
                    listaArquivos.Add(new video {
                        titulo = dados.Name,
                        caminho = dados.FullName,
                        data = dados.LastWriteTime,
                        extensao = dados.Extension,
                        tipo = videoView.getTipoByCaminho(arquivo)
                    });
                }
            }
            Assert.IsNotNull(listaArquivos);
        }*/

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
            List<string> listaArquivos = new List<string>();
            string caminho = @"K:\ICI\Vídeos\kpop";
            string[] pastas = Directory.GetDirectories(caminho, "*", System.IO.SearchOption.AllDirectories);
            foreach (string pasta in pastas) {
                string[] arquivos = Directory.GetFiles(pasta);
                foreach (string arquivo in arquivos) {
                    FileInfo dados = new FileInfo(arquivo);
                    string[] extensoes = { ".mp4", ".mkv", ".ts", ".tp", ".avi"  };
                    if (extensoes.Contains(dados.Extension)) {
                        listaArquivos.Add(arquivo);
                    }
                }
            }

            VideoRepository videoRepository = new VideoRepository();
            foreach (var arquivo in listaArquivos) {
                video video = videoRepository.findByCaminho(arquivo);
                if (video == null) {
                    videoRepository.salvar(arquivo);
                }
            }
        }

        /*[TestMethod]
        public void testeMusicas() {
            VideosView videoView = new VideosView();
            List<string> listaArquivos = new List<string>();
            string caminho = @"K:\ICI\Vídeos\kpop";
            string[] pastas = Directory.GetDirectories(caminho, "*", System.IO.SearchOption.AllDirectories);
            foreach (string pasta in pastas) {
                DirectoryInfo info = new DirectoryInfo(pasta);
                if (info.Name == "MVs") {
                    string[] arquivos = Directory.GetFiles(pasta);
                    foreach (string arquivo in arquivos) {
                        FileInfo dados = new FileInfo(arquivo);
                        string[] extensoes = { ".mp4", ".mkv", ".ts", ".tp", ".avi" };
                        if (extensoes.Contains(dados.Extension)) {
                            listaArquivos.Add(arquivo);
                        }
                    }
                }
            }

            using (FileStream file = new FileStream("musicas.txt", FileMode.Create)) {
                byte[] linha = Encoding.UTF8.GetBytes(Environment.NewLine);
                foreach (string video in listaArquivos) {
                    byte[] bytes = Encoding.UTF8.GetBytes(Path.GetFileNameWithoutExtension(video));
                    file.Write(bytes, 0, bytes.Length);
                    file.Write(linha, 0, linha.Length);
                }
            }
        }*/

        /*[TestMethod]
        public void testeArtistaCaminho() {
            VideoRepository video = new VideoRepository();
            video.Caminho = @"K:\ICI\Vídeos\kpop\red velvet\MVs";
            string artista = video.Artista;
            Assert.IsInstanceOfType(artista,typeof(string));
        }

        [TestMethod]
        public void testeTipoCaminho() {
            VideoRepository video = new VideoRepository();
            video.Caminho = @"K:\ICI\Vídeos\kpop\red velvet\MVs";
            string tipo = video.Tipo;
            Assert.IsInstanceOfType(tipo, typeof(string));
        }

        [TestMethod]
        public void testeInserirArtista() {
            VideoRepository video = new VideoRepository();
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
        }*/

        [TestMethod]
        public void testeGerarPlaylist() {
            VideoRepository videoRepository = new VideoRepository();
            List<video> listaVideos = videoRepository.VideosPlaylist(new int[] { 2013, 2009 }, new int[] { 1 }, new int[] { 1 });

            using (FileStream file = new FileStream("playlist.m3u", FileMode.Create)) {
                byte[] linha = Encoding.UTF8.GetBytes(Environment.NewLine);
                foreach (video video in listaVideos) {
                    byte[] bytes = Encoding.UTF8.GetBytes(video.caminho);
                    file.Write(bytes, 0, bytes.Length);
                    file.Write(linha,0,linha.Length);
                }
            }
        }

        [TestMethod]
        public void testeMediaInfo() {
            VideoRepository videoRepository = new VideoRepository();
            VideosView view = new VideosView();

            List<video> lista = videoRepository.listarVideos();
            foreach (video video in lista) {
                var inputFile = new MediaFile { Filename = video.caminho };

                using (var engine = new Engine()) {
                    engine.GetMetadata(inputFile);
                    video.duracao = inputFile.Metadata.Duration.ToString().Substring(0, 8);
                    video.formato_audio = inputFile.Metadata.AudioData.Format;
                    video.resolucao = inputFile.Metadata.VideoData.FrameSize;

                    double seconds = TimeSpan.Parse(video.duracao).TotalSeconds;
                    seconds = seconds * 90 / 100;
                    for (int i = 1; i <= 6; i++) {
                        var outputFile = new MediaFile { Filename = view.pastaCapturas + video.id + @"_captura_" + seconds / i + ".jpg" };
                        var options = new ConversionOptions { Seek = TimeSpan.FromSeconds(seconds / i) };
                        engine.GetThumbnail(inputFile, outputFile, options);
                    }
                }
            }
        }

        [TestMethod]
        public void testeMusicas() {
            VideoRepository videoRepository = new VideoRepository();
            MusicaRepository musicaRepository = new MusicaRepository();
            VideosView view = new VideosView();

            List<musica> musicas = musicaRepository.Listar<musica>().Where(m => m.titulo.Length > 2).ToList();
            List<video> lista = videoRepository.listarVideos();
            foreach (video video in lista.Distinct().ToList()) {
                List<musica> resultado = musicas.Where(m => video.titulo.ToLower().Contains(m.titulo.ToLower())).ToList();
                if (resultado != null) {
                    foreach (musica m in resultado) {
                        videoRepository.adicionarVideoMusica(video.id, m.id);
                        System.Diagnostics.Debug.WriteLine(video.caminho + " " + m.titulo);
                    }
                }
            }
        }

    }
}