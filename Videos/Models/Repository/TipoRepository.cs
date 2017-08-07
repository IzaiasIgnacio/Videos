using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Videos.Models.Entity;

namespace Videos.Models.Repository {
    public class TipoRepository : BaseRepository {
        public tipo GetTipoByDescricao(string descricao) {
            tipo tipo = db.tipo.Where(d => d.descricao == descricao).FirstOrDefault();

            return tipo;
        }
    }
}