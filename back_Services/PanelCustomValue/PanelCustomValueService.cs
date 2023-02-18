using back_Models.PanelCustomValue;
using back_Services.File;
using CORE;
using Models.MessageClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace back_Services.PanelCustomValue
{
    public class PanelCustomValueService
    {
      
        public static MessageClass Save(int userId)
        {

            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var mc = new MessageClass();

            var MainPageAboutUsTitle = HttpContext.Current.Request.Params["MainPageAboutUsTitle"];
            var MainPageAboutUsText = HttpContext.Current.Request.Params["MainPageAboutUsText"];
            var mainPageAboutUsImageUrl = HttpContext.Current.Request.Files["MainPageAboutUsImageUrl"];
            var FooterAboutUsText = HttpContext.Current.Request.Params["FooterAboutUsText"];
            var Tel = HttpContext.Current.Request.Params["Tel"];
            var Fax = HttpContext.Current.Request.Params["Fax"];
            var EnamadCode = HttpContext.Current.Request.Params["EnamadCode"];
            var AboutUsTitle = HttpContext.Current.Request.Params["AboutUsTitle"];
            var AboutUsText = HttpContext.Current.Request.Params["AboutUsText"];
            var aboutUsImageUrl = HttpContext.Current.Request.Files["AboutUsImageUrl"];
            var websiteLogo = HttpContext.Current.Request.Files["WebsiteLogo"];
            var WebSiteTitle = HttpContext.Current.Request.Params["WebSiteTitle"];
            var MainPageKeyWord = HttpContext.Current.Request.Params["MainPageKeyWord"];

            if (mainPageAboutUsImageUrl != null)
            {
                var dbMainPageAboutUsImageId = FileInfoService.SaveImage(mainPageAboutUsImageUrl, "panel",userId, ref db);
                var dbMainPageAboutUsImageUrl = db.PanelCustomValues.SingleOrDefault(el => el.LatinName == "MainPageAboutUsImageUrl");
                dbMainPageAboutUsImageUrl.Files_ImageID = dbMainPageAboutUsImageId;
            }

            if (websiteLogo != null)
            {
                var websiteLogoImageId = FileInfoService.SaveImage(websiteLogo, "panel",userId, ref db);
                var dbWebsiteLogoImageId = db.PanelCustomValues.SingleOrDefault(el => el.LatinName == "WebsiteLogo");
                dbWebsiteLogoImageId.Files_ImageID = websiteLogoImageId;
            }

            //var AboutUsImageUrlImageId = FileInfoService.SaveImage(aboutUsImageUrl, "panel");
            //var dbAboutUsImageUrl = db.PanelCustomValues.SingleOrDefault(el => el.LatinName == "AboutUsImageUrl");
            //dbAboutUsImageUrl.Files_ImageID = AboutUsImageUrlImageId;


            var dbMainPageAboutUsTitle = db.PanelCustomValues.SingleOrDefault(el => el.LatinName == "MainPageAboutUsTitle");
            dbMainPageAboutUsTitle.Value = MainPageAboutUsTitle;

            var dbMainPageAboutUsText = db.PanelCustomValues.SingleOrDefault(el => el.LatinName == "MainPageAboutUsText");
            dbMainPageAboutUsText.Value = MainPageAboutUsText;

            var dbFooterAboutUsText = db.PanelCustomValues.SingleOrDefault(el => el.LatinName == "FooterAboutUsText");
            dbFooterAboutUsText.Value = FooterAboutUsText;

            var dbTel = db.PanelCustomValues.SingleOrDefault(el => el.LatinName == "Tel");
            dbTel.Value = Tel;

            var dbFax = db.PanelCustomValues.SingleOrDefault(el => el.LatinName == "Fax");
            dbFax.Value = Fax;

            var dbEnamadCode = db.PanelCustomValues.SingleOrDefault(el => el.LatinName == "EnamadCode");
            dbEnamadCode.Value = EnamadCode;

            var dbAboutUsTitle = db.PanelCustomValues.SingleOrDefault(el => el.LatinName == "AboutUsTitle");
            dbAboutUsTitle.Value = AboutUsTitle;

            var dbAboutUsText = db.PanelCustomValues.SingleOrDefault(el => el.LatinName == "AboutUsText");
            dbAboutUsText.Value = AboutUsText;

            var dbWebSiteTitle = db.PanelCustomValues.SingleOrDefault(el => el.LatinName == "WebSiteTitle");
            dbWebSiteTitle.Value = WebSiteTitle;

            var dbMainPageKeyWord = db.PanelCustomValues.SingleOrDefault(el => el.LatinName == "MainPageKeyWord");
            dbMainPageKeyWord.Value = MainPageKeyWord;



            try
            {
                db.SubmitChanges();
                mc.Message = "عملیات با موفقیت انجام شد";
                mc.Status = "success";

            }
            catch (Exception e)
            {
                mc.Message = "خطایی رخ داده!" + e.Message;
                mc.Status = "error";
            }

            return mc;
        }

        public static PanelCustomValueEditViewModel GetPanelCustomeValue(int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var model = new PanelCustomValueEditViewModel
            {
                MainPageAboutUsTitle = db.PanelCustomValues.SingleOrDefault(el => el.LatinName == "MainPageAboutUsTitle").Value,
                MainPageAboutUsText = db.PanelCustomValues.SingleOrDefault(el => el.LatinName == "MainPageAboutUsText").Value,
                MainPageAboutUsImageUrl = (from panelCustomValue in db.PanelCustomValues
                                           join filesView in db.FilesViews on panelCustomValue.Files_ImageID equals filesView.stream_id
                                           where panelCustomValue.LatinName == "MainPageAboutUsImageUrl"
                                           select filesView.directory.Replace(@"\", "/")).SingleOrDefault(),/*"http://localhost/pic/" + filesView.directory.Replace("\\DBA_DIRECTORY\\", "").Replace(@"\", "/")).SingleOrDefault(),*/
                FooterAboutUsText = db.PanelCustomValues.SingleOrDefault(el => el.LatinName == "FooterAboutUsText").Value,
                Tel = db.PanelCustomValues.SingleOrDefault(el => el.LatinName == "Tel").Value,
                Fax = db.PanelCustomValues.SingleOrDefault(el => el.LatinName == "Fax").Value,
                EnamadCode = db.PanelCustomValues.SingleOrDefault(el => el.LatinName == "EnamadCode").Value,
                AboutUsTitle = db.PanelCustomValues.SingleOrDefault(el => el.LatinName == "AboutUsTitle").Value,
                AboutUsText = db.PanelCustomValues.SingleOrDefault(el => el.LatinName == "AboutUsText").Value,
                AboutUsImageUrl = (from panelCustomValue in db.PanelCustomValues
                                   join filesView in db.FilesViews on panelCustomValue.Files_ImageID equals filesView.stream_id
                                   where panelCustomValue.LatinName == "AboutUsImageUrl"
                                   select filesView.directory.Replace(@"\", "/")).SingleOrDefault(),/*"http://localhost/pic/" + filesView.directory.Replace("\\DBA_DIRECTORY\\", "").Replace(@"\", "/")).SingleOrDefault()*/
                WebSiteTitle = db.PanelCustomValues.SingleOrDefault(el => el.LatinName == "WebSiteTitle").Value,
                MainPageKeyWord = db.PanelCustomValues.SingleOrDefault(el => el.LatinName == "MainPageKeyWord").Value,
                WebsiteLogoUrl = (from panelCustomValue in db.PanelCustomValues
                                  join filesView in db.FilesViews on panelCustomValue.Files_ImageID equals filesView.stream_id
                                  where panelCustomValue.LatinName == "WebsiteLogo"
                                  select filesView.directory.Replace(@"\", "/")).SingleOrDefault()/*"http://localhost/pic/" + filesView.directory.Replace("\\DBA_DIRECTORY\\", "").Replace(@"\", "/")).SingleOrDefault()*/
            };

            return model;
        }

        public static PanelCustomValueViewModel GetValue(string latinName, int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var q = (from panelCustomValue in db.PanelCustomValues
                     join fileView in db.FilesViews on panelCustomValue.Files_ImageID equals fileView.stream_id into lFileView
                     from fileView in lFileView.DefaultIfEmpty()
                     where panelCustomValue.LatinName == latinName
                     select new PanelCustomValueViewModel()
                     {
                         Name = panelCustomValue.Name,
                         LatinName = panelCustomValue.LatinName,
                         Value = panelCustomValue.Value,
                         FileImageId = panelCustomValue.Files_ImageID,
                         ImageUrl = fileView.directory.Replace(@"\", "/")
                     }).SingleOrDefault();

            return q;
        }

        public static PanelCustomValueEditViewModel GetMainPageAboutUs(int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var model = new PanelCustomValueEditViewModel
            {
                MainPageAboutUsTitle = db.PanelCustomValues.SingleOrDefault(el => el.LatinName == "MainPageAboutUsTitle").Value,
                MainPageAboutUsText = db.PanelCustomValues.SingleOrDefault(el => el.LatinName == "MainPageAboutUsText").Value,
                MainPageAboutUsImageUrl = (from panelCustomValue in db.PanelCustomValues
                                           join fileView in db.FilesViews on panelCustomValue.Files_ImageID equals fileView.stream_id
                                           where panelCustomValue.LatinName == "MainPageAboutUsImageUrl"
                                           select fileView.directory.Replace(@"\", "/")).SingleOrDefault()
            };
            return model;
        }

        public static PanelCustomValueEditViewModel GetAboutPageAboutUs(int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var model = new PanelCustomValueEditViewModel
            {
                AboutUsTitle = db.PanelCustomValues.SingleOrDefault(el => el.LatinName == "AboutUsTitle").Value,
                AboutUsText = db.PanelCustomValues.SingleOrDefault(el => el.LatinName == "AboutUsText").Value,
                AboutUsImageUrl = db.PanelCustomValues.SingleOrDefault(el => el.LatinName == "AboutUsImageUrl").Value
            };


            return model;
        }

        public static PanelCustomValueEditViewModel GetFooterContent(int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var model = new PanelCustomValueEditViewModel
            {
                FooterAboutUsText = db.PanelCustomValues.SingleOrDefault(el => el.LatinName == "FooterAboutUsText").Value,
                Tel = db.PanelCustomValues.SingleOrDefault(el => el.LatinName == "Tel").Value,
                Fax = db.PanelCustomValues.SingleOrDefault(el => el.LatinName == "Fax").Value
            };


            return model;
        }

        public static string[] GetMainPageKeyWord(int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var keyWords = db.PanelCustomValues.SingleOrDefault(el => el.LatinName == "MainPageKeyWord")?.Value?.Split(',');
            return keyWords;
        }
    }
}
