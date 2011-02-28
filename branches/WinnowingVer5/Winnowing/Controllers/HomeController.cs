using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using Winnowing.Models;

namespace Winnowing.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(FormCollection formValues, HttpPostedFileBase up1)
        {
             //save the uploaded file
            //double result = Judge("d:\\hello.txt");
            //Response.Write(result);
             if (Request.Files.Count > 0)
            {
                var c = Request.Files[0];

                string fileName = "d:\\test.txt";
                c.SaveAs(fileName);
            }
            List<resultInfo> result = winnowing.Process("d:\\test.txt");
           // IQueryable<resultInfo> result1 = (IQueryable<resultInfo>)winnowing.Process("d:\\test.txt");
            return View("Result", result);
          //  return Content((string)result); 
        }


        public ActionResult About()
        {
            return View();
        }

    }
}
