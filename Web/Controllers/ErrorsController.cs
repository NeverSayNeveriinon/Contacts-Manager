using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;


namespace Web.Controllers;

public class ErrorsController : Controller
{
    [Route("Error")]
    public IActionResult Error()
    {
        IExceptionHandlerPathFeature ? exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
        if (exceptionHandlerPathFeature != null)
        {
            ViewBag.ErrorMessage = exceptionHandlerPathFeature.Error.Message;
        }
        return View(); 
    }
}