using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RWPMPortal.Common;
using InfloCommon.Repositories;

namespace RWPMPortal.Controllers
{
    [Authorize(Roles = (UIConstants.ROLE_BATTELLE_STR + "," + UIConstants.ROLE_MOTORADV_ADMIN_STR + "," + UIConstants.ROLE_MOTORADV_ELEVATED_STR + "," + UIConstants.ROLE_MOTORADV_READONLY_STR))]
    public class MotorAdvController : Controller
    {

        IUnitOfWork _uow;
        public MotorAdvController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        // GET: MotorAdv
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = (UIConstants.ROLE_BATTELLE_STR + "," + UIConstants.ROLE_MOTORADV_ADMIN_STR))]
        public ActionResult Admin()
        {
            return View();
        }
    }
}