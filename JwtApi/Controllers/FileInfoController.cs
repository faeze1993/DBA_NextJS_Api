using back_Models.Article;
using back_Models.Common;
using back_Models.File;
using back_Services.File;
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
    public class FileInfoController : ApiController
    {
        [UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [JwtAuthorize]
        [Route("FileInfo/SaveFiles")]
        public JsonResult<MessageClass> SaveFiles()
        {
            var q = FileInfoService.SaveFiles(EnvironmentVariable.UserId);
            return Json(q);
        }

        [UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [JwtAuthorize]
        [Route("FileInfo/SaveSingleFile")]
        public JsonResult<MessageClass> SaveSingleFile()
        {
            var q = FileInfoService.SaveSingleFile(EnvironmentVariable.UserId);
            return Json(q);
        } 
        [UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [JwtAuthorize]
        [Route("FileInfo/GetRelativeUrl")]
        public JsonResult<MessageClass> GetRelativeUrl([FromBody] FileInfoViewModel fileInfoViewModel)
        {
            var q = FileInfoService.GetRelativeUrl(fileInfoViewModel.stream_id);
            return Json(q);
        }

        [UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [JwtAuthorize]
        [Route("FileInfo/SaveFileOrFolderDirectly")]
        public JsonResult<MessageClass> SaveFileOrFolderDirectly()
        {
            var q = FileInfoService.SaveFileOrFolderDirectly(EnvironmentVariable.UserId);
            return Json(q);
        }

        [UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [JwtAuthorize]
        [Route("FileInfo/GetAllChildNodeOfDirectory")]
        public JsonResult<PaginationViewModel<FileManagementViewModel>> GetAllChildNodeOfDirectory([FromBody] FileManagementViewModel fileManagementViewModel)
        {
            var q = FileInfoService.GetAllChildNodeOfDirectory(fileManagementViewModel.Stream_id, EnvironmentVariable.UserId);
            var result = new PaginationViewModel<FileManagementViewModel>
            {
                ItemList = q.Skip(fileManagementViewModel.paginetedata.currntpage == 1 ? 0 : fileManagementViewModel.paginetedata.perpage * (fileManagementViewModel.paginetedata.currntpage - 1)).Take(fileManagementViewModel.paginetedata.perpage).ToList(),
                TotalItems = q.Count()
            };
            return Json(result);
        }

        [UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [JwtAuthorize]
        [Route("FileInfo/GetFilesBySerach")]
        public JsonResult<PaginationViewModel<FileManagementViewModel>> GetFilesBySerach([FromUri] FileSearchViewModel fileSearchViewModel)
        {
            var q = FileInfoService.GetFilesBySerach(fileSearchViewModel.SerachValue, EnvironmentVariable.UserId);
            var result = new PaginationViewModel<FileManagementViewModel>
            {
                ItemList = q.Skip(0).Take(10).ToList(),
                TotalItems = q.Count()
            };
            return Json(result);
        }

        [UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [JwtAuthorize]
        [Route("FileInfo/GetBreadCrumbListOnStreamId")]
        public JsonResult<List<FileBreadCrumbViewModel>> GetBreadCrumbListOnStreamId([FromUri] FileBreadCrumbViewModel fileBreadCrumbViewModel)
        {
            var q = FileInfoService.GetBreadCrumbListOnStreamId(fileBreadCrumbViewModel.StreamId, EnvironmentVariable.UserId);
            return Json(q);
        }

        [UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [JwtAuthorize]
        [Route("FileInfo/DeleteFileOrFolder")]
        public JsonResult<MessageClass> DeleteFileOrFolder([FromUri] FileManagementViewModel fileManagementViewModel)
        {
            var q = FileInfoService.DeleteFileOrFolder(fileManagementViewModel.Stream_id, EnvironmentVariable.UserId);
            return Json(q);
        }

        [UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [JwtAuthorize]
        [Route("FileInfo/SaveUserFile")]
        public JsonResult<MessageClass> SaveUserFile()
        {
            var q = FileInfoService.SaveUserFile(EnvironmentVariable.UserId);
            return Json(q);
        }

        [UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [JwtAuthorize]
        [Route("FileInfo/GetUserfileList")]
        public JsonResult<List<UserFilesViewModel>> GetUserfileList()
        {
            var q = FileInfoService.GetUserfileList(EnvironmentVariable.UserId);
            return Json(q);
        }
    }
}
