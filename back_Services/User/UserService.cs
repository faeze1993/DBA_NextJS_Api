
using back_Models.Common;
using back_Models.User;
using back_Services.File;
using back_Services.Utility;
using CORE;
using Models.Account;
using Models.MessageClass;
using Models.User;
using Services.Utility;
using System;
using System.Collections.Generic;
using System.Data.Linq.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Services.User
{
    public class UserService
    {

        public static MessageClass RegisterUser(RegisterUserApiViewModel model)
        {
            var mc = new MessageClass();
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, 0);
            //اگر شماره موبایل وارد شده قبلا در سیستم ثبت شده باشد و مقدار verify ، true باشد پیام خطا نمایش داده شود
            var hasVerifiedPeople = db.Peoples.Any(el => el.Mobile == model.Mobile && el.User.HasVerify == true);
            var hasVerifiedUser = db.Users.Any(el => el.Username == model.Mobile.ToString() && el.HasVerify == true);

            if (hasVerifiedPeople || hasVerifiedUser)
            {
                mc.Message = "شماره موبایل وارد شده تکراری می باشد";
                mc.Status = "error";
                return mc;
            }

            // اگر شماره موبایل وارد شده قبلا در سیستم ثبت شده باشد و مقدار verify ، false باشد ، کاربر.به مرحله احراز هویت هدایت می شود
            var hasunVerifyPeople = db.Peoples.Any(el => el.Mobile == model.Mobile && el.User.HasVerify == false);
            var hasunVerifyUser = db.Users.Any(el => el.Username == model.Mobile.ToString() && el.HasVerify == false);

            if (hasunVerifyPeople || hasunVerifyUser)
            {
                mc = UpdateUserAndPeople(model);
                return mc;
            }

            var lastLoginDate = DateTime.Now;
            var passwordExpirationDate = (DateTime.Now).AddDays(30);
            var lastPasswordChangedDate = DateTime.Now;
            var creationDate = DateTime.Now;
            var lastEnabledDate = DateTime.Now;
            var finishLockDatetime = DateTime.Now;
            Random rnd = new Random();
            var verificationCode = rnd.Next(10000, 99999);
            var expireTimeVerificationCode = DateTime.Now.AddMinutes(3);
            var ipAddress = GetIPAddress();
            var passwordHash = SecurityService.GetSha256Hash(model.PassWord);


            var result = db.ExecuteQuery<int>("SELECT NEXT VALUE FOR [dbo].[NewIntegerID] AS NexIntId");
            var newId = result.SingleOrDefault();

            var people = new CORE.People()
            {
                ID = newId,
                BirthDate = DateTime.Now/*model.BirthDate*/,
                E_mail = model.Email,
                FirstName = model.FName,
                GenderID = 0/*model.GenderId*/,
                LastName = string.Empty,
                Mobile = long.Parse(model.Mobile.ToString().TrimStart('0'))
            };

            var user = new CORE.User()
            {
                ID = newId,
                Username = model.Mobile.ToString(),
                Password = passwordHash,
                IPAddress = ipAddress,
                CreationDate = creationDate,
                IsEnabled = true,
                IsOnLine = false,
                PasswordExpirationDate = passwordExpirationDate,
                ChangedPermission = false,
                VerificationCode = verificationCode,
                ExpireTimeVerificationCode = expireTimeVerificationCode,
                FailedPasswordAttemptCount = 0,
                FinishLockDatetime = finishLockDatetime,
                HasVerify = false,
                LastEnabledDate = lastEnabledDate,
                LastLoginDate = lastLoginDate,
                LastPasswordChangedDate = lastPasswordChangedDate
            };

            var useRole = new CORE.UserRole()
            {
                UserID = newId,
                RoleID = (int)CORE.Enums.Role.User
            };

            try
            {
                db.Peoples.InsertOnSubmit(people);
                db.Users.InsertOnSubmit(user);
                db.UserRoles.InsertOnSubmit(useRole);
            }
            catch (Exception)
            {

                throw;
            }


            try
            {
                db.SubmitChanges();
                mc.Message = "عملیات با موفقیت انجام شد";
                mc.Status = "success";
            }
            catch (Exception e)
            {
                mc.Message = "خطایی رخ داده!";
                mc.Status = "error";
                //نمونه کار احمقانه
                //if (e.Message.ToString().IndexOf("UQ_User_Username") > 0 || e.Message.ToString().IndexOf("UQ_People_Mobile") > 0)
                //{
                //    var hasPeople = db.Peoples.Any(el => el.Mobile == model.Mobile && el.User.HasVerify == true);
                //    var hasUser = db.Users.Any(el => el.Username == model.Mobile.ToString() && el.HasVerify == true);
                //    if(hasPeople || hasUser)
                //    {
                //        mc.Message = "شماره موبایل تکراری می باشد";
                //        mc.Status = "error";
                //    }
                //    else
                //    {
                //        mc.Message = "کاربر با این مشخصات ثبت شده است";
                //        mc.Status = "duplicate";
                //    }
                //}
                return mc;
            }

            SendVerificationCodeSms(model.FName, model.LName, model.Mobile.ToString(), verificationCode.ToString());
            return mc;
        }
        public static MessageClass UpdateUserAndPeople(RegisterUserApiViewModel model)
        {
            var db = new CoreDataContext();
            var mc = new MessageClass();

            var lastLoginDate = DateTime.Now;
            var passwordExpirationDate = (DateTime.Now).AddDays(30);
            var lastPasswordChangedDate = DateTime.Now;
            var creationDate = DateTime.Now;
            var lastEnabledDate = DateTime.Now;
            var finishLockDatetime = DateTime.Now;
            Random rnd = new Random();
            var verificationCode = rnd.Next(10000, 99999);
            var expireTimeVerificationCode = DateTime.Now.AddMinutes(3);
            var ipAddress = GetIPAddress();
            var passwordHash = SecurityService.GetSha256Hash(model.PassWord);

            var dbPeople = db.Peoples.SingleOrDefault(el => el.Mobile == model.Mobile && el.User.HasVerify == false);

            dbPeople.FirstName = model.FName;
            dbPeople.LastName = string.Empty;
            dbPeople.Mobile = long.Parse(model.Mobile.ToString().TrimStart('0'));

            var dbUser = db.Users.SingleOrDefault(el => el.ID == dbPeople.ID && el.HasVerify == false);

            dbUser.Username = model.Mobile.ToString();
            dbUser.Password = passwordHash;
            dbUser.IPAddress = ipAddress;
            dbUser.CreationDate = creationDate;
            dbUser.IsEnabled = true;
            dbUser.IsOnLine = false;
            dbUser.PasswordExpirationDate = passwordExpirationDate;
            dbUser.ChangedPermission = false;
            dbUser.VerificationCode = verificationCode;
            dbUser.ExpireTimeVerificationCode = expireTimeVerificationCode;
            dbUser.FailedPasswordAttemptCount = 0;
            dbUser.FinishLockDatetime = finishLockDatetime;
            dbUser.HasVerify = false;
            dbUser.LastEnabledDate = lastEnabledDate;
            dbUser.LastLoginDate = lastLoginDate;
            dbUser.LastPasswordChangedDate = lastPasswordChangedDate;

            try
            {
                db.SubmitChanges();
                mc.Message = "عملیات با موفقیت انجام شد";
                mc.Status = "success";
            }
            catch (Exception e)
            {
                mc.Message = "خطایی رخ داده!";
                mc.Status = "error";
                return mc;
            }

            SendVerificationCodeSms(model.FName, model.LName, model.Mobile.ToString(), verificationCode.ToString());
            return mc;
        }
        private static void SendVerificationCodeSms(string firstName, string lastName, string mobile, string verificationCode)
        {
            var name = firstName + " " + lastName;
            var mc = PayamSmsService.Send(name, verificationCode, mobile);
        }
        public static CORE.User FindUser(string userName, string password, string ip, out string message)
        {
            message = null;
            var db = new CoreDataContext();
            var passwordHash = SecurityService.GetSha256Hash(password);
            var userInDb = db.Users.FirstOrDefault(x => x.Username == userName.TrimStart('0') && x.Password == passwordHash && x.HasVerify);
            if (userInDb == null)
                message = "کاربر مورد نظر یافت نشد";
            return userInDb;
        }
        public static UserProfileDataViewModel GetUserData(int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var q = (from user in db.Users
                     join people in db.Peoples on user.ID equals people.ID
                     join gender in db.Genders on people.GenderID equals gender.ID
                     join fileView in db.FilesViews on people.Files_ImageID equals fileView.stream_id into lFileView
                     from fileView in lFileView.DefaultIfEmpty()
                     join userRole in db.UserRoles on user.ID equals userRole.UserID
                     join role in db.Roles on userRole.RoleID equals role.ID
                     where user.ID == userId
                     group new { roleId = role.ID }
                     by new
                     {
                         user.ID,
                         people.FirstName,
                         people.LastName,
                         people.E_mail,
                         fileView.directory,
                         people.GenderID,
                         gender.LatinName,
                         user.Username,
                         user.CreationDate
                     }
                     into grp
                     select new UserProfileDataViewModel
                     {
                         ID = grp.Key.ID,
                         FullName = grp.Key.FirstName + ' ' + grp.Key.LastName,
                         Email = grp.Key.E_mail,
                         ImageUrl = grp.Key.directory.Replace(@"\", "/"),
                         GenderId = grp.Key.GenderID,
                         Gender = grp.Key.LatinName,
                         Username = grp.Key.Username,
                         CreationDate = CalendarService.ConvertToPersian(grp.Key.CreationDate).ToString("HH:mm yyyy/MM/dd"),
                         IsAdmin = grp.Sum(el => el.roleId == (int)CORE.Enums.Role.User ? 0 : 1) == 1 ? true : false
                     }).SingleOrDefault();
            return q;
        }
        private static string GetIPAddress()
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return context.Request.ServerVariables["REMOTE_ADDR"];
        }
        public static MessageClass CheckVerifyCode(int code, string userName)
        {
            var mc = new MessageClass();
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, 0);
            var dbUser = db.Users.SingleOrDefault(el => el.Username == userName);

            if (dbUser.VerificationCode == code && dbUser.ExpireTimeVerificationCode > DateTime.Now)
            {
                dbUser.HasVerify = true;
                db.SubmitChanges();
                mc.Message = "عملیات با موفقیت انجام شد";
                mc.Status = "success";
            }
            else
            {
                mc.Message = "کد وارد شده صحیح نمی باشد";
                mc.Status = "error";
            }

            return mc;
        }
        public static MessageClass GetVerifyCode(string userName)
        {
            var mc = new MessageClass();
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, 0);
            var dbUser = db.Users.SingleOrDefault(el => el.Username == userName);
            if (dbUser == null)
            {
                mc.Message = "کاربر مورد نظر یافت نشد";
                mc.Status = "error";
                return mc;
            }

            var dbPeople = db.Peoples.SingleOrDefault(el => el.ID == dbUser.ID);

            Random rnd = new Random();
            var verificationCode = rnd.Next(10000, 99999);
            var expireTimeVerificationCode = DateTime.Now.AddMinutes(3);

            dbUser.VerificationCode = verificationCode;
            dbUser.ExpireTimeVerificationCode = expireTimeVerificationCode;
            try
            {
                db.SubmitChanges();
                SendVerificationCodeSms(dbPeople.FirstName, dbPeople.LastName, dbPeople.Mobile.ToString(), verificationCode.ToString());
            }
            catch (Exception)
            {

                throw;
            }


            return mc;
        }
        public static MessageClass CheckMobileNumber(long mobile)
        {
            var mc = new MessageClass();
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, 0);
            var mbl = long.Parse(mobile.ToString().TrimStart('0'));
            var dbPeople = db.Peoples.SingleOrDefault(el => el.Mobile == mbl);
            if (dbPeople == null)
            {
                mc.Message = "کاربر مورد نظر یافت نشد";
                mc.Status = "error";
                return mc;
            }

            var dbUser = db.Users.SingleOrDefault(el => el.ID == dbPeople.ID);

            Random rnd = new Random();
            var verificationCode = rnd.Next(10000, 99999);
            var expireTimeVerificationCode = DateTime.Now.AddMinutes(3);

            dbUser.VerificationCode = verificationCode;
            dbUser.ExpireTimeVerificationCode = expireTimeVerificationCode;
            try
            {
                db.SubmitChanges();
                SendVerificationCodeSms(dbPeople.FirstName, dbPeople.LastName, dbPeople.Mobile.ToString(), verificationCode.ToString());
            }
            catch (Exception)
            {

                throw;
            }

            mc.Status = "success";
            mc.Data = dbUser.Username;
            return mc;
        }
        public static List<UserViewModel> GetAllUsersList(string searchValue, int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);

            var qq = (from user in db.Users
                      join people in db.Peoples on user.ID equals people.ID
                      select new { user, people });

            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                var strSearchValue = string.Format("%{0}%", searchValue).Replace(" ", "%");

                qq = (from data in qq
                      where (SqlMethods.Like(data.user.Username, strSearchValue) ||
                            SqlMethods.Like(data.people.FirstName, strSearchValue) ||
                            SqlMethods.Like(data.people.LastName, strSearchValue) ||
                            SqlMethods.Like(data.people.Mobile.ToString(), strSearchValue) ||
                            SqlMethods.Like(data.people.E_mail, strSearchValue))
                      select new { data.user, data.people });
            }

            var q = (from data in qq
                     join userRole in db.UserRoles on data.user.ID equals userRole.UserID into lUserRole
                     from userRole in lUserRole.DefaultIfEmpty()
                     join role in db.Roles on userRole.RoleID equals role.ID
                     group new
                     {
                         data.user.ID,
                         data.user.Username,
                         FullName = data.people.FirstName + ' ' + data.people.LastName,
                         RoleId = role.ID,
                         role.Name,
                         data.user.LastLoginDate,
                         data.user.HasVerify,
                         data.people.Mobile,
                         data.people.E_mail
                     }
                     by new
                     {
                         data.user.ID,
                         data.user.Username,
                         FullName = data.people.FirstName + ' ' + data.people.LastName,
                         data.user.LastLoginDate,
                         data.user.HasVerify,
                         data.people.Mobile,
                         data.people.E_mail
                     }
                     into grp
                     select new UserViewModel
                     {
                         ID = grp.Key.ID,
                         Username = grp.Key.Username,
                         FullName = grp.Key.FullName,
                         LastLoginDate = grp.Key.LastLoginDate != null ? CalendarService.ConvertToPersian((DateTime)grp.Key.LastLoginDate).ToString("HH:mm yyyy/MM/dd") : null,
                         HasVerify = grp.Key.HasVerify,
                         Mobile = grp.Key.Mobile,
                         Email = grp.Key.E_mail,
                         UserRoles = grp.Select(el => new ValueLabelViewModel { value = el.RoleId, label = el.Name }).ToList()
                     }).ToList();
            return q;
        }
        public static List<UserViewModel> GetAllAuthorList(int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);

            var q = (from user in db.Users
                     join people in db.Peoples on user.ID equals people.ID
                     join fileView in db.FilesViews on people.Files_ImageID equals fileView.stream_id into lFileView
                     from fileView in lFileView.DefaultIfEmpty()
                     join userRole in db.UserRoles on user.ID equals userRole.UserID
                     join role in db.Roles on userRole.RoleID equals role.ID
                     where role.ID == (int)CORE.Enums.Role.Publisher || role.ID == (int)CORE.Enums.Role.Admin
                     select new UserViewModel
                     {
                         ID = user.ID,
                         FullName = people.FirstName + " " + people.LastName,
                         ImageUrl = fileView.directory.Replace(@"\", "/"),
                         Description = people.Description
                     }).Distinct().ToList();
            return q;
        }
        public static UserViewModel GetPublisherProfileData(int id, int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var q = (from user in db.Users
                     join people in db.Peoples on user.ID equals people.ID
                     join fileView in db.FilesViews on people.Files_ImageID equals fileView.stream_id into lFileView
                     from fileView in lFileView.DefaultIfEmpty()
                     where user.ID == id
                     select new UserViewModel
                     {
                         ID = user.ID,
                         Email = people.E_mail,
                         Description = people.Description,
                         FullName = people.FirstName + " " + people.LastName,
                         ImageUrl = fileView.directory.Replace(@"\", "/")
                     }).SingleOrDefault();
            return q;
        }
        public static List<ValueLabelViewModel> GetRoles(int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var q = db.Roles.Select(el => new ValueLabelViewModel
            {
                value = el.ID,
                label = el.Name
            }).ToList();
            return q;
        }
        public static MessageClass DeleteUser(int id, int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var mc = new MessageClass();
            var userInDb = db.Users.SingleOrDefault(el => el.ID == id);

            if (userInDb == null)
            {
                mc.Message = "کاربر مورد نظر یافت نشد";
                mc.Status = "error";
                return mc;
            }
            var hasArticle = db.Articles.Any(el => el.User_AuthorID == id);

            if (hasArticle)
            {
                mc.Message = "کاربر مورد نظر دارای مقاله می باشد";
                mc.Status = "error";
                return mc;
            }

            db.Users.DeleteOnSubmit(userInDb);

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
        public static MessageClass EditUserByAdmin(EditUserByAdminViewModel model, int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var mc = new MessageClass();

            var userInDb = db.Users.SingleOrDefault(el => el.ID == model.Id);
            if (userInDb == null)
            {
                mc.Message = "کاربر مورد نظر یافت نشد";
                mc.Status = "error";
                return mc;
            }

            userInDb.HasVerify = model.HasVerify;

            var hasUserRole = model.UserRoles.Any(el => el.value == 3);
            if (model.UserRoles.Count == 0 || (model.UserRoles.Count > 0 && !hasUserRole))
            {
                mc.Message = "امکان حذف نقش 'کاربر' وجود ندارد";
                mc.Status = "error";
                return mc;
            }

            //حذف تمامی نقش ها به غیر از کاربر 
            var oldUserRoles = db.UserRoles.Where(el => el.UserID == model.Id && el.RoleID != 3).Select(el => el);
            if (oldUserRoles.Any())
            {
                try
                {
                    db.UserRoles.DeleteAllOnSubmit(oldUserRoles);
                }
                catch (Exception e)
                {
                    mc.Message = "خطایی رخ داده!" + e.Message;
                    mc.Status = "error";
                }
            };

            var newUserRoles = model.UserRoles.Where(el => el.value != 3).Select(el => new UserRole
            {
                UserID = model.Id,
                RoleID = (byte)el.value
            }).ToList();

            if (newUserRoles.Any())
            {
                try
                {
                    db.UserRoles.InsertAllOnSubmit(newUserRoles);
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
        public static MessageClass SaveProfileImage(int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var mc = new MessageClass();

            var file = HttpContext.Current.Request.Files;
            var directoryName = HttpContext.Current.Request.Params["directoryName"];


            var newId = Guid.NewGuid();

            var byteArray = FileInfoService.StreamToByteArray(file[0].InputStream);
            var fileName = string.Format("{0}_{1}", newId, file[0].FileName);
            db.SaveFiles(newId, fileName, byteArray, directoryName);

            var people = db.Peoples.SingleOrDefault(el => el.ID == userId);
            if (people != null)
                people.Files_ImageID = newId;
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
        public static MessageClass EditUserAccount(RegisterUserApiViewModel registerUserApiViewModel, int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var mc = new MessageClass();
            var people = db.Peoples.SingleOrDefault(el => el.ID == userId);
            if (people != null)
            {
                people.FirstName = registerUserApiViewModel.FName != null && registerUserApiViewModel.FName != string.Empty ? registerUserApiViewModel.FName : people.FirstName;
                people.LastName = registerUserApiViewModel.LName != null && registerUserApiViewModel.LName != string.Empty ? registerUserApiViewModel.LName : people.LastName;
                people.Description = registerUserApiViewModel.Description != null && registerUserApiViewModel.Description != string.Empty ? registerUserApiViewModel.Description : people.Description;
                people.E_mail = registerUserApiViewModel.Email != null && registerUserApiViewModel.Email != string.Empty ? registerUserApiViewModel.Email : people.E_mail;
                people.Mobile = registerUserApiViewModel.Mobile != 0 ? long.Parse(registerUserApiViewModel.Mobile.ToString().TrimStart('0')) : people.Mobile;
                people.GenderID = registerUserApiViewModel.GenderId != 0 ? registerUserApiViewModel.GenderId : people.GenderID;
            }

            var user = db.Users.SingleOrDefault(el => el.ID == userId);
            if (user != null)
            {
                user.Username = registerUserApiViewModel.Username != null && registerUserApiViewModel.Username != string.Empty ? registerUserApiViewModel.Username : user.Username;
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
        public static MessageClass ChangePassOnLoginMode(RegisterUserApiViewModel registerUserApiViewModel, int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var mc = new MessageClass();
            var user = db.Users.SingleOrDefault(el => el.ID == userId);
            if (user != null)
            {
                var passwordHash = SecurityService.GetSha256Hash(registerUserApiViewModel.PassWord);
                user.Password = passwordHash;
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
        public static MessageClass ChangePassword(RegisterUserApiViewModel registerUserApiViewModel, int userId)
        {
            var db = new CoreDataContext();
            CoreDataContext.SetDbSessionInfo(db, userId);
            var mc = new MessageClass();
            var user = db.Users.SingleOrDefault(el => el.Username == registerUserApiViewModel.Username);
            if (user != null)
            {
                var passwordHash = SecurityService.GetSha256Hash(registerUserApiViewModel.PassWord);
                user.Password = passwordHash;
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
        public static RecaptchaResponseViewModel VerifyCaptchaResponse(RecaptchaViewModel recaptchaViewModel)
        {
            var responseString = RecaptchaVerify(recaptchaViewModel.CaptchaToken);
            var response = Newtonsoft.Json.JsonConvert.DeserializeObject<RecaptchaResponseViewModel>(responseString);
            return response;
        }
        public static string RecaptchaVerify(string captchaToken)
        {
            string apiAddress = "https://www.google.com/recaptcha/api/siteverify";
            string recaptchaSecret = "6LepF-MeAAAAAEGIwuqrv-SIqxB46R_1V5D1o25U";
            string url = $"{apiAddress}?secret={recaptchaSecret}&response={captchaToken}";

            try
            {
                HttpClient httpClient = new HttpClient();
                var responseString = httpClient.GetStringAsync(url).Result;
                return responseString;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
