using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Videos.Models.Entity;
using Videos.Models.Repository;

namespace Videos.Models.ViewModel {
    public class PlaylistsView {
        public int id_video { get; set; }
        public List<playlist> ListaPlaylists { get; set; }
        public string PlaylistHasVideo(int id_playlist, int id_video) {
            PlaylistRepository playlistrepository = new PlaylistRepository();
            if (playlistrepository.checkPlaylistHasVideo(id_playlist, id_video)) {
                return "*";
            }
            return null;
        }
    }
}