using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Videos.Models.Entity;
using Videos.Models.Repository;
using Videos.Models.ViewModel;

namespace Videos.Models.Services {
    public class VideoDataService {
        VideoDataView view;
        List<string> thumbs;

        public VideoDataService() {
            view = VideoDataView.GetVideoDataView();
        }

        public VideoDataView getVideoMetaData(int id) {
            VideoRepository videoRepository = new VideoRepository();
            video video = videoRepository.getVideoById(id);

            if (File.Exists(video.caminho)) {
                var inputFile = new MediaFile { Filename = video.caminho };

                using (var engine = new Engine()) {
                    engine.GetMetadata(inputFile);
                    view.Id = id;
                    view.ArtistaPrincipal = video.video_artista.Where(a => a.principal = true).FirstOrDefault().artista.nome;
                    view.Duracao = inputFile.Metadata.Duration.ToString().Substring(0, 8);
                    view.Resolucao = inputFile.Metadata.VideoData.FrameSize;
                    view.FormatoVideo = inputFile.Metadata.VideoData.Format;
                    view.Fps = inputFile.Metadata.VideoData.Fps.ToString();
                    view.CanaisAudio = inputFile.Metadata.AudioData.ChannelOutput;
                    view.FormatoAudio = inputFile.Metadata.AudioData.Format;
                }
                
                FileInfo fi = new FileInfo(video.caminho);
                view.Tamanho = (Convert.ToInt64(fi.Length) / 1024 / 1024);
            }

            return view;
        }

        public List<string> getThumbs(int id) {
            thumbs = new List<string>();
            string[] arquivos = Directory.GetFiles(view.pastaCapturas,id+"_*");

            if (arquivos.Count() < 6) {
                VideoRepository videoRepository = new VideoRepository();
                video video = videoRepository.getVideoById(id);
                GerarThumbs(video);
            }
            else {
                foreach (string arquivo in arquivos) {
                    FileInfo dados = new FileInfo(arquivo);
                    thumbs.Add(dados.Name);
                }
            }

            return thumbs;
        }

        private void GerarThumbs(video video) {
            if (File.Exists(video.caminho)) {
                var inputFile = new MediaFile { Filename = video.caminho };

                using (var engine = new Engine()) {
                    engine.GetMetadata(inputFile);
                    string duracao = inputFile.Metadata.Duration.ToString().Substring(0, 8);

                    double seconds = TimeSpan.Parse(duracao).TotalSeconds;
                    seconds = seconds * 90 / 100;
                    for (int i = 1; i <= 6; i++) {
                        var outputFile = new MediaFile { Filename = view.pastaCapturas + video.id + @"_captura_" + seconds / i + ".jpg" };
                        var options = new ConversionOptions { Seek = TimeSpan.FromSeconds(seconds / i) };
                        engine.GetThumbnail(inputFile, outputFile, options);
                        thumbs.Add(video.id + @"_captura_" + seconds / i + ".jpg");
                    }
                }
            }
        }

    }
}