using back_Models.Common;
using back_Models.Product;
using back_Services;
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
    public class ProductController : ApiController
    {
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("Product/GetAllProducts")]
        public JsonResult<List<ProductViewModel>> GetAllProducts()
        {
            var q = ProductService.GetAllProducts(EnvironmentVariable.UserId);
            return Json(q);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("Product/GetProductList")]
        public JsonResult<List<ValueLabelViewModel>> GetProductList()
        {
            var q = ProductService.GetProductList();
            return Json(q);
        }


        [UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [JwtAuthorize]
        [Route("Product/GetAllProductsForPanel")]
        public JsonResult<List<ProductViewModel>> GetAllProductsForPanel([FromBody] SearchViewModel searchViewModel)
        {
            var q = ProductService.GetAllProductsForPanel(searchViewModel?.SearchText,EnvironmentVariable.UserId);
            return Json(q);
        }

        [UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [JwtAuthorize]
        [Route("Product/GetDetailProduct")]
        public JsonResult<ProductViewModel> GetDetailProduct([FromUri] ProductViewModel ProductViewModel)
        {
            var q = ProductService.GetDetail(ProductViewModel.Id, EnvironmentVariable.UserId);
            return Json(q);
        }

        [UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [JwtAuthorize]
        [Route("Product/InsertProduct")]
        public JsonResult<MessageClass> InsertProduct()
        {
            var q = ProductService.Insert(EnvironmentVariable.UserId);
            return Json(q);
        }

        [UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [JwtAuthorize]
        [Route("Product/EditProduct")]
        public JsonResult<MessageClass> EditProduct()
        {
            var q = ProductService.Edit(EnvironmentVariable.UserId);
            return Json(q);
        }

        [UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [JwtAuthorize]
        [Route("Product/DeleteProduct")]
        public JsonResult<MessageClass> DeleteProduct([FromUri] ProductViewModel ProductViewModel)
        {
            var q = ProductService.Delete(ProductViewModel.Id, EnvironmentVariable.UserId);
            return Json(q);
        }
    }
}
