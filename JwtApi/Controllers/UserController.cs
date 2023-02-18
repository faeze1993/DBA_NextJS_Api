using back_Models.Common;
using back_Models.User;
using JwtApi.Jwt;
using Models.MessageClass;
using Models.User;
using Services.Library;
using Services.User;
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
    public class UserController : ApiController
    {
        [UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [JwtAuthorize]
        [Route("User/GetAllUsersList")]
        public JsonResult<PaginationViewModel<UserViewModel>> GetAllUsersList([FromBody] PaginateViewModel paginateViewModel)
        {
            var q = UserService.GetAllUsersList(paginateViewModel.searchValue, EnvironmentVariable.UserId);
            var result = new PaginationViewModel<UserViewModel>
            {
                ItemList = q.Skip(paginateViewModel.currntpage == 1 ? 0 : paginateViewModel.perpage * (paginateViewModel.currntpage - 1)).Take(paginateViewModel.perpage).ToList(),
                TotalItems = q.ToList().Count()
            };
            return Json(result);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("User/GetAllAuthorList")]
        public JsonResult<List<UserViewModel>> GetAllAuthorList()
        {
            var q = UserService.GetAllAuthorList(EnvironmentVariable.UserId);
            return Json(q);
        }
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("User/GetPublisherProfileData")]
        public JsonResult<UserViewModel> GetPublisherProfileData([FromUri] UserViewModel userViewModel)
        {
            var q = UserService.GetPublisherProfileData(userViewModel.ID, EnvironmentVariable.UserId);
            return Json(q);
        }

        [UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [JwtAuthorize]
        [Route("User/GetRoles")]
        public JsonResult<List<ValueLabelViewModel>> GetRoles()
        {
            var q = UserService.GetRoles(EnvironmentVariable.UserId);
            return Json(q);
        }

        [UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [JwtAuthorize]
        [Route("User/DeleteUser")]
        public JsonResult<MessageClass> DeleteUser([FromUri] UserViewModel UserViewModel)
        {
            var q = UserService.DeleteUser(UserViewModel.ID, EnvironmentVariable.UserId);
            return Json(q);
        }


        [UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [JwtAuthorize]
        [Route("User/EditUserByAdmin")]
        public JsonResult<MessageClass> EditUserByAdmin([FromBody] EditUserByAdminViewModel EditUserByAdminViewModel)
        {
            var q = UserService.EditUserByAdmin(EditUserByAdminViewModel, EnvironmentVariable.UserId);
            return Json(q);
        }
    }
}
