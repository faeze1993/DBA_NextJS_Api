using back_Models.Common;
using back_Models.Slider;
using back_Services.Slider;
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
    public class SliderController : ApiController
    {
        //[UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        //[JwtAuthorize]
        [Route("Slider/GetAllSliders")]
        public JsonResult<List<SliderViewModel>> GetAllSliders([FromBody] SearchViewModel searchViewModel)
        {
            var q = SliderService.GetAllSliders(searchViewModel?.SearchText,EnvironmentVariable.UserId);
            return Json(q);
        }

        [UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [JwtAuthorize]
        [Route("Slider/GetDetailSlider")]
        public JsonResult<SliderViewModel> GetDetailSlider([FromUri] SliderViewModel SliderViewModel)
        {
            var q = SliderService.GetDetailSlider(SliderViewModel.Id, EnvironmentVariable.UserId);
            return Json(q);
        }

        [UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [JwtAuthorize]
        [Route("Slider/InsertSlider")]
        public JsonResult<MessageClass> InsertSlider()
        {
            var q = SliderService.InsertSlider(EnvironmentVariable.UserId);
            return Json(q);
        }

        [UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [JwtAuthorize]
        [Route("Slider/EditSlider")]
        public JsonResult<MessageClass> EditSlider()
        {
            var q = SliderService.EditSlider(EnvironmentVariable.UserId);
            return Json(q);
        }

        [UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [JwtAuthorize]
        [Route("Slider/DeleteSlider")]
        public JsonResult<MessageClass> DeleteSlider([FromUri] SliderViewModel SliderViewModel)
        {
            var q = SliderService.DeleteSlider(SliderViewModel.Id, EnvironmentVariable.UserId);
            return Json(q);
        }
    }
}
