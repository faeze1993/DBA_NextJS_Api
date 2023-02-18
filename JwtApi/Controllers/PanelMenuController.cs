using back_Models.Common;
using back_Models.PanelCustomValue;
using back_Services.PanelCustomValue;
using back_Services.PanelMenu;
using JwtApi.Jwt;
using Models.MessageClass;
using Services.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Results;

namespace JwtApi.Controllers
{
    public class PanelMenuController : ApiController
    {
        [UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [JwtAuthorize]
        [Route("PanelMenu/GetPanelMenuOnUserRole")]
        public JsonResult<List<PanelMenuViewModel>> GetPanelMenuOnUserRole()
        {
            var q = PanelMenuService.GetPanelMenuOnUserRole(EnvironmentVariable.UserId);
            return Json(q);
        }

        [UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [JwtAuthorize]
        [Route("PanelMenu/GetPanelMenuList")]
        public JsonResult<List<PanelMenuViewModel>> GetPanelMenuList([FromBody] SearchViewModel searchViewModel)
        {
            var q = PanelMenuService.GetPanelMenuList(searchViewModel?.SearchText,EnvironmentVariable.UserId);
            return Json(q);
        }

        [UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [JwtAuthorize]
        [Route("PanelMenu/UpdatePanelMenu")]
        public JsonResult<MessageClass> UpdatePanelMenu()
        {
            var q = PanelMenuService.UpdatePanelMenu(EnvironmentVariable.UserId);
            return Json(q);
        }
    }
}
