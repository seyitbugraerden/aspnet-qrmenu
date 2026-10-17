using Microsoft.AspNetCore.Mvc;

namespace {{file.namespace}};

public class {{type}}Controller : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
