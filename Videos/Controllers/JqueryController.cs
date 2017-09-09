using System.Web.Mvc;
using Videos.Models.Repository;
using Videos.Models.ViewModel;
using Videos.Models.Entity;
using Videos.Models.Services;

namespace Videos.Controllers {
    public class JqueryController : Controller {

        public ActionResult ExibirFormVideo(int id) {
            VideoRepository videoRepository = new VideoRepository();
            VideoDataService videoDataService = new VideoDataService();

            VideoDataView videoDataView = videoDataService.getMetadata(id);

            return PartialView("FormVideoView", videoDataView);
        }

    }   
}
 