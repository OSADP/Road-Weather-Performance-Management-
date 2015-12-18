using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RWPMPortal.Common;
using InfloCommon.Repositories;
using InfloCommon.Models;
using RWPMPortal.Models;

namespace RWPMPortal.Controllers
{
    [Authorize(Roles = (UIConstants.ROLE_BATTELLE_STR + "," + UIConstants.ROLE_TRAFCONT_ADMIN_STR + "," + UIConstants.ROLE_TRAFCONT_ELEVATED_STR + "," + UIConstants.ROLE_TRAFCONT_READONLY_STR))]
    public class TrafContController : Controller
    {
        IUnitOfWork _uow;
        public TrafContController(IUnitOfWork uow)
        {
            _uow = uow;
        }
        /*need pop out nav                  @if (User.IsInRole(UIConstants.ROLE_BATTELLE_STR) || User.IsInRole(UIConstants.ROLE_TRAFCONT_ADMIN_STR))
                            {
                                <ul>
                                    <li>@Html.ActionLink("Administration", "Admin", "TrafCont")</li>
                                </ul>
                            }
         * */


        // GET: TrafCont
        public ActionResult Index()
        {
            
            TrafContViewModel m = new TrafContViewModel();
            //Center map on Minneapolis
            m.MapLatitude = 44.9778;// 39.9883468;
            m.MapLongitude = -93.2650;// -83.0210581;
            m.MapZoom = 14;
            m.QWarnAlerts = new List<MotoristAlertModel.QWarnAlert>();
            m.SpeedHarmAlerts = new List<MotoristAlertModel.SpdHarmAlert>();

            return View(m);
        }

        [Authorize(Roles = (UIConstants.ROLE_BATTELLE_STR + "," + UIConstants.ROLE_TRAFCONT_ADMIN_STR))]
        public ActionResult Admin()
        {
         
            return View();
        }

       [Authorize(Roles = (UIConstants.ROLE_BATTELLE_STR + "," + UIConstants.ROLE_TRAFCONT_ADMIN_STR))]
        public JsonResult PostSpdHarm(string roadwayId, double beginMM, double endMM)
        {
            ViewBag.Message = string.Format("Unspecified error.");
            var succ = new object();

            try
            {
                var invalidate = _uow.TMEOutput_SPDHARMMessage_CVs
                    .Where(s => s.ValidityDuration == InfloCommon.Models.Common.INFLO_VALIDITY_DURATION_ACTIVE 
                        && s.RoadwayId == roadwayId 
                        && s.BeginMM == beginMM 
                        && s.EndMM == endMM);//.ToList();
                foreach (var s in invalidate)
                {
                    s.ValidityDuration = InfloCommon.Models.Common.INFLO_VALIDITY_DURATION_MANUAL_INACTIVE;//Make invalid
                }
                _uow.Commit();
                succ = new { Success = true, Value = true};
            }
            catch (Exception)
            {
                succ = new { Success = false, Value = false };
            }

            return Json(succ);

           
        }
       
        public JsonResult PostQWarn(string roadwayId, double endMM)
        {
            ViewBag.Message = string.Format("Unspecified error.");
            var succ = new object();

            try
            {
                var invalidate = _uow.TMEOutput_QWARNMessage_CVs
  .Where(d => d.ValidityDuration == InfloCommon.Models.Common.INFLO_VALIDITY_DURATION_ACTIVE 
      && d.RoadwayID == roadwayId 
      && d.FOQMMLocation == endMM);
                foreach (var s in invalidate)
                {
                    s.ValidityDuration = InfloCommon.Models.Common.INFLO_VALIDITY_DURATION_MANUAL_INACTIVE;//Make invalid
                }
                _uow.Commit();
                succ = new { Success = true, Value = true };
            }
            catch (Exception)
            {
                succ = new { Success = false, Value = false };
            }

            return Json(succ);


        }
    }
}