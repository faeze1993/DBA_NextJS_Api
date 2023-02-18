using back_Models.Common;
using back_Models.PanelCustomValue;
using back_Models.User;
using back_Services.File;
using CORE;
using Models.MessageClass;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Linq.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace back_Services.PanelMenu
{
    public class PanelMenuService
    {
        public static List<PanelMenuViewModel> GetPanelMenuOnUserRole(int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var menus = (from panelMenu in db.PanelMenus
                         join panelMenuRole in db.PanelMenuRoles on panelMenu.ID equals panelMenuRole.PanelMenuID
                         join userRole in db.UserRoles on panelMenuRole.RoleID equals userRole.RoleID
                         join fileView in db.FilesViews on panelMenu.Files_IconID equals fileView.stream_id into lFileView
                         from fileView in lFileView.DefaultIfEmpty()
                         where userRole.UserID == userId
                         group new {
                             userRole.UserID,
                             panelMenu.ID,
                             panelMenu.Name,
                             panelMenu.NavigateUrl,
                             fileView.directory
                         }
                         by new{
                             userRole.UserID,
                             panelMenu.ID,
                             panelMenu.Name,
                             panelMenu.NavigateUrl,
                             fileView.directory
                         }
                         into grp
                         select new PanelMenuViewModel
                         {
                             Id = grp.Key.ID,
                             Name = grp.Key.Name,
                             NavigateUrl = grp.Key.NavigateUrl,
                             IconDirectoty = grp.Key.directory.Replace(@"\", "/")
                         }).ToList();
            return menus;
        }

        public static List<PanelMenuViewModel> GetPanelMenuList(string searchValue,int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);


            var panelMenus = db.PanelMenus.Select(el => el);
            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                var strSearchingValue = string.Format("%{0}%", searchValue.Trim().Replace(' ', '%'));
                panelMenus = panelMenus.Where(el => SqlMethods.Like(el.Name, strSearchingValue));
            }

            var menus = (from menu in panelMenus
                         join fileView in db.FilesViews on menu.Files_IconID equals fileView.stream_id into lFileView
                         from fileView in lFileView.DefaultIfEmpty()
                         join panelMenuRole in db.PanelMenuRoles on menu.ID equals panelMenuRole.PanelMenuID 
                         join role in db.Roles on panelMenuRole.RoleID equals role.ID 
                         group new
                         {
                             RoleId = role.ID,
                             roleName = role.Name,
                             menu.ID,
                             menu.Name,
                             menu.NavigateUrl,
                             fileView.directory,
                             menu.Files_IconID
                         }
                         by new
                         {
                             menu.ID,
                             menu.Name,
                             menu.NavigateUrl,
                             fileView.directory,
                             menu.Files_IconID
                         }
                     into grp
                         select new PanelMenuViewModel
                         {
                             Id = grp.Key.ID,
                             Name = grp.Key.Name,
                             NavigateUrl = grp.Key.NavigateUrl,
                             IconDirectoty = grp.Key.directory.Replace(@"\", "/"),
                             IconId = grp.Key.Files_IconID,
                             PanelMenuRoles = grp.Select(el => new ValueLabelViewModel { value = el.RoleId, label = el.roleName }).ToList()
                         }).ToList();
            return menus;
        }

        public static MessageClass UpdatePanelMenu(int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var mc = new MessageClass();

            int.TryParse(HttpContext.Current.Request.Params["Id"], out var id);
            var name = HttpContext.Current.Request.Params["Name"];
            var panelMenuRoles = HttpContext.Current.Request.Params["PanelMenuRoles"];
            var IconFile = HttpContext.Current.Request.Files["IconFile"];
            Guid.TryParse(HttpContext.Current.Request.Params["IconId"], out var iconId);

            var panelMenuRolesList = JsonConvert.DeserializeObject<List<ValueLabelViewModel>>(panelMenuRoles);

            var dbPanelMenu = db.PanelMenus.SingleOrDefault(el => el.ID == id);


            if (IconFile != null && IconFile.ContentLength > 0)
            {
                var newIconId = FileInfoService.SaveImage(IconFile, "panelmenu", userId,ref db);
                dbPanelMenu.Files_IconID = newIconId;

                var oldIcon = db.FilesViews.Where(el => el.stream_id == iconId).Select(el => el).SingleOrDefault();
                if (oldIcon  != null)
                    db.FilesViews.DeleteOnSubmit(oldIcon);
            }

            dbPanelMenu.Name = name;

            var oldPanelMenuRoles = db.PanelMenuRoles.Where(el => el.PanelMenuID == id).Select(el => el);
            if (oldPanelMenuRoles.Any())
            {
                try
                {
                    db.PanelMenuRoles.DeleteAllOnSubmit(oldPanelMenuRoles);

                    var newPanelMenuRoles = panelMenuRolesList.Select(el => new PanelMenuRole
                    {
                        PanelMenuID = dbPanelMenu.ID,
                        RoleID = (byte)el.value
                    }).ToList();

                    db.PanelMenuRoles.InsertAllOnSubmit(newPanelMenuRoles);
                }
                catch (Exception e)
                {

                    mc.Message = "خطایی رخ داده!" + e.Message;
                    mc.Status = "error";
                }

            }


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
    }
}
