using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Videos.Models.Entity;

namespace Videos.Models.Repository {
    public class BaseRepository {
        protected VideosEntities db;

        public BaseRepository() {
            db = new VideosEntities();
        }

        public List<T> Listar<T>() where T : class {
            return db.Set<T>().ToList();
        }

    }
}