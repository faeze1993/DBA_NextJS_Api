using back_Models.Comment;
using back_Models.Common;
using back_Services.Comment;
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
    public class CommentController : ApiController
    {
        [UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [JwtAuthorize]
        [Route("Comment/SaveComment")]
        public JsonResult<MessageClass> SaveComment([FromBody]CommentViewModel CommentViewModel)
        {
            var q = CommentService.InsertOrUpdate(CommentViewModel,EnvironmentVariable.UserId);
            return Json(q);
        }

        [UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [JwtAuthorize]
        [Route("Comment/DeleteComment")]
        public JsonResult<MessageClass> DeleteComment([FromUri] CommentViewModel CommentViewModel)
        {
            var q = CommentService.Delete((int)CommentViewModel.Id, EnvironmentVariable.UserId);
            return Json(q);
        }

        [UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [JwtAuthorize]
        [Route("Comment/GetCommentsGridData")]
        public JsonResult<PaginationViewModel<CommentViewModel>> GetCommentsGridData([FromBody] PaginateViewModel paginateViewModel)
        {
            var q = CommentService.GetCommentsGridData(EnvironmentVariable.UserId);
            var comments = new PaginationViewModel<CommentViewModel>
            {
                TotalItems = q.Count(),
                ItemList = q.Skip(paginateViewModel.currntpage == 1 ? 0 : paginateViewModel.perpage * (paginateViewModel.currntpage-1)).Take(paginateViewModel.perpage).ToList(),   
            };
            return Json(comments);
        }

        [UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("Comment/GetArticleComment")]
        public JsonResult<List<CommentViewModel>> GetArticleComment([FromBody] CommentRequest CommentRequest)
        {
            var q = CommentService.GetComments(CommentRequest.ArticleId, CommentRequest.perSection, CommentRequest.currentSection, EnvironmentVariable.UserId);
            return Json(q);
        }

        [UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [JwtAuthorize]
        [Route("Comment/ToggleConfirm")]
        public JsonResult<MessageClass> ToggleConfirm([FromUri] CommentViewModel CommentViewModel)
        {
            var q = CommentService.ToggleConfirm((int)CommentViewModel.Id, EnvironmentVariable.UserId);
            return Json(q);
        }
    }
}
