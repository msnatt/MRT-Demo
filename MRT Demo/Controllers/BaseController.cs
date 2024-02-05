using MRT_Demo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MRT_Demo.Controllers
{
    public class BaseController : Controller
    {
        public MRTEntities db = new MRTEntities();
    }
}