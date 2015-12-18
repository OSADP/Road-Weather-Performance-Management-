using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using RWPMPortal.Common;
using RWPMPortal.Models;
using System.Threading.Tasks;
using System;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;

namespace RWPMPortal.Controllers
{
    [Authorize(Roles = (UIConstants.ROLE_BATTELLE_STR))]
    public class BattelleAccountController : Controller
    {

        private ApplicationUserManager _userManager;

        public BattelleAccountController()
        {
        }

        public BattelleAccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }



        // GET: BattelleAccount
        public ActionResult CreateUsers()
        {
            NewUserViewModel model = new NewUserViewModel();
            model.RoleList = GetPermissionChoices();
            return View(model);
        }

        public SelectList GetPermissionChoices()
        {
            List<SelectListItem> choices = new List<SelectListItem>();
            foreach (RoleTypes enumValue in Enum.GetValues(typeof(RoleTypes)))
            {
                //Display text is our enum names.  Value is the role strings in the db.
                choices.Add(new SelectListItem { Text = enumValue.ToString(), Value = _GetRoleStr(enumValue) });

            }
            SelectList objselectlist = new SelectList(choices, "Value", "Text");
            return objselectlist;
        }

        // GET: BattelleAccount
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateUsers(NewUserViewModel model)
        {
            ViewBag.Message = string.Format("Unspecified error in form.");
            try
            {
                if (ModelState.IsValid)
                {
                    //You can add additional fields to the Application User in the Identitymodels class in the models folder
                    var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                    //If you have the role id's in a constant or have the role manager setup you can add the role here like this
                    //user.Roles.Add(new IdentityUserRole()
                    //{
                    //RoleId = role id from constant or rolmanager
                    //});
                    var result = await UserManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        //or you can add the role after the user is created. Note when the user is created the user entity 
                        //should be filled with the new GUID
                        //RoleTypes selectedRole = (RoleTypes)Enum.Parse(typeof(RoleTypes), model.SelectedRoles[0]);
                        //result = await UserManager.AddToRoleAsync(user.Id, _GetRoleStr(model.RoleType));
                        result = await UserManager.AddToRolesAsync(user.Id, model.SelectedRoles);
                        if (result.Succeeded)
                        {
                            ModelState.Clear();
                            ViewBag.Message = string.Format("New user {0} created.", user.Email);
                            model = null;
                            model = new NewUserViewModel();
                            model.RoleList = GetPermissionChoices();
                            return View(model);
                        }
                        else
                        {
                            ViewBag.Message = string.Format("AddToRolesAsync for user {0} failed.", user.Email);
                        }

                    }
                    else
                    {
                        ViewBag.Message = string.Format("UserManager.CreateAsync for user {0} failed.", user.Email);
                    }
                }
            }
            catch (Exception e)
            {
                ViewBag.Message = e.Message;
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        private string _GetRoleStr(RoleTypes roleType)
        {
            switch (roleType)
            {
                case RoleTypes.Battelle:
                    return UIConstants.ROLE_BATTELLE_STR;
                case RoleTypes.RWMaint_Admin:
                    return UIConstants.ROLE_RWMAINT_ADMIN_STR;
                case RoleTypes.RWMaint_Elevated:
                    return UIConstants.ROLE_RWMAINT_ELEVATED_STR;
                case RoleTypes.RWMaint_ReadOnly:
                    return UIConstants.ROLE_RWMAINT_READONLY_STR;
                case RoleTypes.MotorAdv_Admin:
                    return UIConstants.ROLE_MOTORADV_ADMIN_STR;
                case RoleTypes.MotorAdv_Elevated:
                    return UIConstants.ROLE_MOTORADV_ELEVATED_STR;
                case RoleTypes.MotorAdv_ReadOnly:
                    return UIConstants.ROLE_MOTORADV_READONLY_STR;
                case RoleTypes.TrafCont_Admin:
                    return UIConstants.ROLE_TRAFCONT_ADMIN_STR;
                case RoleTypes.TrafCont_Elevated:
                    return UIConstants.ROLE_TRAFCONT_ELEVATED_STR;
                case RoleTypes.TrafCont_ReadOnly:
                    return UIConstants.ROLE_TRAFCONT_READONLY_STR;

            }
            return null;
        }
    }
}