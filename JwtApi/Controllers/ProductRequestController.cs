using back_Models.Common;
using back_Models.ProductRequest;
using back_Services.ProductRequest;
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
    public class ProductRequestController : ApiController
    {

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("ProductRequest/Save")]
        public JsonResult<MessageClass> Save([FromBody] ProductRequestViewModel ProductRequestViewModel)
        {
            var q = ProductRequestService.Save(ProductRequestViewModel, EnvironmentVariable.UserId);
            return Json(q);
        }

        [UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [JwtAuthorize]
        [Route("ProductRequest/Delete")]
        public JsonResult<MessageClass> Delete([FromUri] ProductRequestViewModel ProductRequestViewModel)
        {
            var q = ProductRequestService.Delete((int)ProductRequestViewModel.Id, EnvironmentVariable.UserId);
            return Json(q);
        }

        [UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [JwtAuthorize]
        [Route("ProductRequest/GetProductRequestGridData")]
        public JsonResult<PaginationViewModel<ProductRequestViewModel>> GetProductRequestGridData([FromBody] PaginateViewModel paginateViewModel)
        {
            var q = ProductRequestService.GetProductRequestGridData(paginateViewModel.searchValue, EnvironmentVariable.UserId);
            var result = new PaginationViewModel<ProductRequestViewModel>
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
        [Route("ProductRequest/Update")]
        public JsonResult<MessageClass> Update([FromUri] ProductRequestViewModel ProductRequestViewModel)
        {
            var q = ProductRequestService.Update((int)ProductRequestViewModel.Id, EnvironmentVariable.UserId);
            return Json(q);
        }
    }
}
