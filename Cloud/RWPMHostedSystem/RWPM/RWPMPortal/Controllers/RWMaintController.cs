using InfloCommon.Repositories;
using RWPMPortal.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RWPMPortal.Controllers
{
     [Authorize(Roles = (UIConstants.ROLE_BATTELLE_STR + "," + UIConstants.ROLE_RWMAINT_ADMIN_STR + "," + UIConstants.ROLE_RWMAINT_ELEVATED_STR + "," + UIConstants.ROLE_RWMAINT_READONLY_STR))]
    public class RWMaintController : Controller
    {
        IUnitOfWork _uow;
        public RWMaintController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        // GET: RWMaint
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = (UIConstants.ROLE_BATTELLE_STR + "," + UIConstants.ROLE_RWMAINT_ADMIN_STR))]
        public ActionResult Admin()
        {
            return View();
        }
    }
}