
using back_Models.Common;
using back_Models.ConectUs;
using back_Services.ConectUs;
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
    public class ContentUsController : ApiController
    {
       
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("ContentUs/Save")]
        public JsonResult<MessageClass> Save([FromBody] ConectUsViewModel ConectUsViewModel)
        {
            var q = ConectUsService.Save(ConectUsViewModel, EnvironmentVariable.UserId);
            return Json(q);
        }

        [UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [JwtAuthorize]
        [Route("ContentUs/Delete")]
        public JsonResult<MessageClass> Delete([FromUri] ConectUsViewModel ConectUsViewModel)
        {
            var q = ConectUsService.Delete((int)ConectUsViewModel.Id, EnvironmentVariable.UserId);
            return Json(q);
        }

        [UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [JwtAuthorize]
        [Route("ContentUs/GetConectUsGridData")]
        public JsonResult<PaginationViewModel<ConectUsViewModel>> GetConectUsGridData([FromBody] PaginateViewModel paginateViewModel)
        {
            var q = ConectUsService.GetConectUsGridData(EnvironmentVariable.UserId);
            var result = new PaginationViewModel<ConectUsViewModel>
            {
                ItemList = q.Skip(paginateViewModel.currntpage == 1 ? 0 : paginateViewModel.perpage * (paginateViewModel.currntpage - 1)).Take(paginateViewModel.perpage).ToList(),
                TotalItems = q.ToList().Count()
            };
            return Json(result);
        }
       
        [UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [JwtAuthorize]
        [Route("ContentUs/Update")]
        public JsonResult<MessageClass> Update([FromUri] ConectUsViewModel ConectUsViewModel)
        {
            var q = ConectUsService.Update((int)ConectUsViewModel.Id, EnvironmentVariable.UserId);
            return Json(q);
        }
    }
}
