using back_Models.PanelCustomValue;
using back_Services.PanelCustomValue;
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
    public class PanelCustomValueController : ApiController
    {
       
        [UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [JwtAuthorize]
        [Route("PanelCustomValue/Save")]
        public JsonResult<MessageClass> Save()
        {
            var q = PanelCustomValueService.Save(EnvironmentVariable.UserId);
            return Json(q);
        }

        [UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [JwtAuthorize]
        [Route("PanelCustomValue/GetPanelCustomeValue")]
        public JsonResult<PanelCustomValueEditViewModel> GetPanelCustomeValue()
        {
            var q = PanelCustomValueService.GetPanelCustomeValue(EnvironmentVariable.UserId);
            return Json(q);
        }

        //[UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("PanelCustomValue/GetMainPageAboutUs")]
        public JsonResult<PanelCustomValueEditViewModel> GetMainPageAboutUs()
        {
            var q = PanelCustomValueService.GetMainPageAboutUs(EnvironmentVariable.UserId);
            return Json(q);
        }

        [UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("PanelCustomValue/GetAboutPageAboutUs")]
        public JsonResult<PanelCustomValueEditViewModel> GetAboutPageAboutUs()
        {
            var q = PanelCustomValueService.GetAboutPageAboutUs(EnvironmentVariable.UserId);
            return Json(q);
        }

        [UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("PanelCustomValue/GetWebSiteTitle")]
        public JsonResult<string> GetWebSiteTitle()
        {
            var q = PanelCustomValueService.GetValue("WebSiteTitle", EnvironmentVariable.UserId).Value;
            return Json(q);
        }


        [UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("PanelCustomValue/GetFooterContent")]
        public JsonResult<PanelCustomValueEditViewModel> GetFooterContent()
        {
            var q = PanelCustomValueService.GetFooterContent(EnvironmentVariable.UserId);
            return Json(q);
        }


        [UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("PanelCustomValue/GetMainPageKeyWord")]
        public JsonResult<string[]> GetMainPageKeyWord()
        {
            var q = PanelCustomValueService.GetMainPageKeyWord(EnvironmentVariable.UserId);
            return Json(q);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("PanelCustomValue/GetLogImage")]
        public JsonResult<string> GetLogImage()
        {
            var q = PanelCustomValueService.GetValue("WebsiteLogo", EnvironmentVariable.UserId).ImageUrl;
            return Json(q);
        }
    }
}
