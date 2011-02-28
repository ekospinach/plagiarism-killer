using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

using Winnowing.Models;

namespace Winnowing.Controllers
{
    public class ResultController : Controller
    {
        //
        // GET: /Result/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Show(List<resultInfo> result)
        {
            return View(result);

        }

    }
}
