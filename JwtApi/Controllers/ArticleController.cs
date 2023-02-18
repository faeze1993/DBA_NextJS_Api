using back_Models.Article;
using back_Models.Common;
using back_Services.Article;
using back_Services.PanelCustomValue;
using JwtApi.Jwt;
using Models.MessageClass;
using Services.Library;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Results;

namespace JwtApi.Controllers
{

    public class ArticleController : ApiController
    {
        [UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost, System.Web.Mvc.ValidateInput(false)]
        [JwtAuthorize]
        [Route("Article/Save")]
        public JsonResult<MessageClass> Save()
        {
            var q = ArticleService.Save(EnvironmentVariable.UserId);
            return Json(q);
        }

        [UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost, System.Web.Mvc.ValidateInput(false)]
        [JwtAuthorize]
        [Route("Article/ToggleEnable")]
        public JsonResult<MessageClass> ToggleEnable([FromUri] ArticleViewModel ArticleViewModel)
        {
            var q = ArticleService.ToggleEnable((int)ArticleViewModel.Id, EnvironmentVariable.UserId);
            return Json(q);
        }

        [UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [JwtAuthorize]
        [Route("Article/GetAllArticlesForDashboard")]
        public JsonResult<PaginationViewModel<ArticleViewModel>> GetAllArticlesForDashboard([FromBody] ArticleViewModel ArticleViewModel)
        {
            var q = ArticleService.GetAllArticlesForDashboard(ArticleViewModel.Id, EnvironmentVariable.UserId);
            var result = new PaginationViewModel<ArticleViewModel>
            {
                ItemList = q.Skip(ArticleViewModel.paginetedata.currntpage == 1 ? 0 : ArticleViewModel.paginetedata.perpage * (ArticleViewModel.paginetedata.currntpage - 1)).Take(ArticleViewModel.paginetedata.perpage).ToList(),
                TotalItems = q.ToList().Count()
            };
            return Json(result);
        }


        [UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [JwtAuthorize]
        [Route("Article/GetAllArticlesForDropdown")]
        public JsonResult<List<ValueLabelViewModel>> GetAllArticlesForDropdown([FromBody] ArticleViewModel ArticleViewModel)
        {
            var q = ArticleService.GetAllArticlesForDropdown(ArticleViewModel.Id).ToList();
            return Json(q);
        }


        [UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [JwtAuthorize]
        [Route("Article/GetArticlesForEdit")]
        public JsonResult<ArticleViewModel> GetArticlesForEdit([FromUri] ArticleViewModel ArticleViewModel)
        {
            var q = ArticleService.GetArticlesForEdit((int)ArticleViewModel.Id, EnvironmentVariable.UserId);
            return Json(q);
        }

        [UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [JwtAuthorize]
        [Route("Article/GetAllArticlesForDashboardUsingSearch")]
        public JsonResult<PaginationViewModel<ArticleViewModel>> GetAllArticlesForDashboardUsingSearch([FromBody] ArticleSearchViewModel ArticleSearchViewModel)
        {
            var q = ArticleService.GetAllArticlesForDashboardUsingSearch(ArticleSearchViewModel.SerachValue, EnvironmentVariable.UserId);
            var result = new PaginationViewModel<ArticleViewModel>
            {
                ItemList = q.Skip(ArticleSearchViewModel.paginetedata.currntpage == 1 ? 0 : ArticleSearchViewModel.paginetedata.perpage * (ArticleSearchViewModel.paginetedata.currntpage - 1)).Take(ArticleSearchViewModel.paginetedata.perpage).ToList(),
                TotalItems = q.ToList().Count()
            };
            return Json(result);
        }

   

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("Article/GetAllArticlesOnAuthrId")]
        public JsonResult<List<ArticleViewModel>> GetAllArticlesOnAuthrId([FromBody] ArticleViewModel ArticleViewModel)
        {
            var q = ArticleService.GetAllArticlesOnAuthrId((int) ArticleViewModel.AuthorId);
            return Json(q);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("Article/GetAllArticlesForMainPage")]
        public JsonResult<ArticleForMainPageViewModel> GetAllArticlesForMainPage()
        {
            var q = ArticleService.GetAllArticlesForMainPage(EnvironmentVariable.UserId);
            return Json(q);
        }

        //[UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        //[JwtAuthorize]
        [Route("Article/GetArticleDetail")]
        public JsonResult<SingleArticleViewModel> GetArticleDetail([FromUri] ArticleViewModel ArticleViewModel)
        {
            var q = ArticleService.GetArticleDetail((int)ArticleViewModel.Id, EnvironmentVariable.UserId);
            var result = new SingleArticleViewModel
            {
                Article = q,
                testfromserver = true,
            };
            return Json(result);
        }

        [UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [JwtAuthorize]
        [Route("Article/DeleteArticle")]
        public JsonResult<MessageClass> DeleteArticle([FromUri]ArticleViewModel ArticleViewModel)
        {
            var q = ArticleService.DeleteArticle((int)ArticleViewModel.Id, EnvironmentVariable.UserId);
            return Json(q);
        }

        [UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [JwtAuthorize]
        [Route("Article/DeleteArticleClone")]
        public JsonResult<MessageClass> DeleteArticleClone([FromUri] ArticleViewModel ArticleViewModel)
        {
            var q = ArticleService.DeleteArticle((int)ArticleViewModel.Id, EnvironmentVariable.UserId);
            return Json(q);
        }

        [UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [JwtAuthorize]
        [Route("Article/GetArticleTypes")]
        public JsonResult<List<ArticleTypeViewModel>> GetArticleTypes()
        {
            var q = ArticleService.GetArticleTypes(EnvironmentVariable.UserId);
            return Json(q);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("Article/GetAllArticlesForArchiveWithPaginate")]
        public JsonResult<PaginationViewModel<ArticleViewModel>> GetAllArticlesForArchiveWithPaginate([FromBody] ArticleViewModel ArticleViewModel)
        {
            var q = ArticleService.GetAllArticles( ArticleViewModel.paginetedata.perpage, ArticleViewModel.paginetedata.currntpage, ArticleViewModel.Id, ArticleViewModel.AuthorId, EnvironmentVariable.UserId);
            return Json(q);
        }
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [Route("Article/GetAllArticles2")]
        public JsonResult<List<ArticleViewModel>> GetAllArticles2([FromUri] int? articleId)
        {
            var q = ArticleService.GetAllArticles2(articleId);
            return Json(q);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("Article/GetAllArticleTrees")]
        public JsonResult<string> GetAllArticleTrees()
        {
            var q = ArticleService.GetAllArticleTrees(EnvironmentVariable.UserId);
            return Json(q);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("Article/GetAllArticleforArticlePath")]
        public JsonResult<List<ArticleViewModel>> GetAllArticleforArticlePath()
        {
            var q = ArticleService.GetAllArticleforArticlePath(EnvironmentVariable.UserId);
            return Json(q);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("Article/GetAllArticleMenu")]
        public JsonResult<string> GetAllArticleMenu()
        {
            var q = ArticleService.GetAllArticleMenu(EnvironmentVariable.UserId);
            return Json(q);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("Article/GetArticleUsingSearch")]
        public JsonResult<List<ArticleViewModel>> GetArticleUsingSearch([FromUri] ArticleSearchViewModel ArticleSearchViewModel)
        {
            var q = ArticleService.GetArticleUsingSearch(ArticleSearchViewModel.SerachValue, EnvironmentVariable.UserId);
            return Json(q);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("Article/GetAllArticlesKeyWord")]
        public JsonResult<List<string>> GetAllArticlesKeyWord()
        {
            var q = ArticleService.GetAllArticlesKeyWord(EnvironmentVariable.UserId);
            return Json(q);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("Article/GetBreadCrumbListOnArticleId")]
        public JsonResult<List<BreadCrumbViewModel>> GetBreadCrumbListOnArticleId([FromUri] BreadCrumbViewModel BreadCrumbViewModel)
        {
            var q = ArticleService.GetBreadCrumbListOnArticleId(BreadCrumbViewModel.Id, EnvironmentVariable.UserId);
            return Json(q);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("Article/SaveArticleVisit")]
        public void SaveArticleVisit([FromUri] ArticleViewModel ArticleViewModel)
        {
            ArticleService.SaveArticleVisit((int)ArticleViewModel.Id, EnvironmentVariable.UserId);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("Article/GetArticleMetaData")]
        public JsonResult<MetaDataViewModel> GetArticleMetaData([FromUri] ArticleViewModel ArticleViewModel)
        {
            var q =ArticleService.GetArticleMetaData((int)ArticleViewModel.Id);
            return Json(q);
        }
    }
}