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
    }
}