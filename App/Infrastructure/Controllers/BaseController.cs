using System.Web.Mvc;

namespace App.Infrastructure.Controllers
{
    public class BaseController : Controller
    {
        public const int PAGE_SIZE = 10;

        public JsonResult Success<T>(T data)
        {
            return Json(new { Status = true, StatusCode = 200, Message = "OK", Data = data });
            }

        public JsonResult BadRequest(string message = "BAD_REQUEST")
        {
            return Json(new { Status = false, StatusCode = 400, Message = message });
        }

        public JsonResult NotFound(string message = "NOT_FOUND")
        {
            return Json(new { Status = false, StatusCode = 404, Message = message });
        }

        public JsonResult InternalError(string message = "INTERNAL_ERROR")
        {
            return Json(new { Status = false, StatusCode = 500, Message = message });
        }
    }
}