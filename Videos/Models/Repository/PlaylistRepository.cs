using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Videos.Models.Entity;

namespace Videos.Models.Repository {
    public class PlaylistRepository : BaseRepository {

        public playlist getPlaylistById(int id) {
            return db.playlist.Where(p => p.id == id).FirstOrDefault();
        }

        public void AdicionarVideoPlaylist(int id_playlist, int id_video) {
            if (!checkPlaylistHasVideo(id_playlist, id_video)) {
                playlist_filtros pf = new playlist_filtros();
                pf.id_playlist = id_playlist;
                pf.valor = id_video.ToString();
                pf.tipo = "video";
                db.playlist_filtros.Add(pf);
                db.SaveChanges();
            }
        }

        public bool checkPlaylistHasVideo(int id_playlist, int id_video) {
            return db.playlist_filtros.Where(p => p.id_playlist == id_playlist && p.valor == id_video.ToString()).Any();
        }

    }
}