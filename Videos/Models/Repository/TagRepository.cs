using System.Linq;
using Videos.Models.Entity;

namespace Videos.Models.Repository {
    public class TagRepository : BaseRepository {
        public tag GetTagByNome(string nome) {
            tag tag = db.tag.Where(t => t.nome == nome).FirstOrDefault();
            if (tag == null) {
                tag = AdicionarTag(nome);
            }
            return tag;
        }

        private tag AdicionarTag(string nome) {
            tag tag = new tag { nome = nome };
            db.tag.Add(tag);
            db.SaveChanges();

            return tag;
        }
    }
}