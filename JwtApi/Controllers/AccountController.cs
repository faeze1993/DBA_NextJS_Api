using API.JwtApi.Jwt;
using Models.Account;
using Models.MessageClass;
using Models.User;
using JwtApi.Jwt;
using JwtApi.Redis;
using Services.Library;
using Services.User;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Results;

namespace API.JwtApi.Controllers
{
   
    public class AccountController : ApiController
    {
        
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [JwtAuthorize]
        [Route("Account/LogOut")]
        public JsonResult<MessageClass> LogOut()
        {
            var mc = new MessageClass();

            var accessToken = HttpContext.Current.Request.Headers["Authorization"];
            if (accessToken != null)
            {
                accessToken = accessToken.Substring(7);
                var deleted = UserDataRedisCacheService.RemoveTokenKeyFromRedis(accessToken);
                if (!deleted)
                {
                    mc.Message = "خطایی رخ داده!" ;
                    mc.Status = "error";
                    return Json(mc);
                }

                //var addTokenToBlackList = TokenService.SetTokenToBalckList(accessToken);
                //if (!addTokenToBlackList)
                //{
                //    mc.AddMessage("LogOut is Fail", Enumeration.ShowMessageState.Error);
                //    return Json(mc);
                //}
                mc.Message ="LogOut is Successful";
                mc.Status = "success";
            }
            else
            {
                mc.Message = "خطایی رخ داده!";
                mc.Status = "error";
            }
              
            return Json(mc);
        }

        /// <summary>
        ///  آیا login هست
        /// </summary>
        /// <returns></returns>
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [UpdateTokenExpireTime]
        [HttpGet]
        [JwtAuthorize]
        [Route("Account/IsLogin")]
        public JsonResult<MessageClass> IsLogin()
        {
            var mc = new MessageClass();

            if (User.Identity.IsAuthenticated)
            {
                mc.Message = "Logged In";
                mc.Status = "success";
            }
            else
            {
                mc.Message = "Not Logged In";
                mc.Status = "error";
            }

            return Json(mc);
        }


        //[EnableCors(origins: "*", headers: "*", methods: "*")]
        //[HttpPost]
        //[JwtAuthorize]
        //[Route("api/WebApi/AccountApiService/ResetPassword")]
        //public JsonResult<MessageClass> ResetPassword(ResetPasswordViewModel model)
        //{
        //    var mc = LoginService.UpdateUserPassword(model, model.Id, EnvironmentVariable.PlaceID);
        //    return Json(mc);
        //}


        /// <summary>
        ///   
        /// </summary>
        /// <returns></returns>
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [AllowAnonymous]
        [Route("Account/Register")]
        public JsonResult<MessageClass> RegisterUser(RegisterUserApiViewModel registerUserApiViewModel)
        {
            var mc = new MessageClass();

            var q = UserService.RegisterUser(registerUserApiViewModel);
            mc.Data = new { /*userId = EnvironmentVariable.UserID, placeId = EnvironmentVariable.PlaceID, periodId = EnvironmentVariable.PeriodID, logo = logo */};
            return Json(q);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [UpdateTokenExpireTime]
        [HttpGet]
        [JwtAuthorize]
        [Route("Account/GetUserData")]
        public JsonResult<UserProfileDataViewModel> GetUserData()
        {
            var q = UserService.GetUserData(EnvironmentVariable.UserId);
            return Json(q);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("Account/CheckVerifyCode")]
        public JsonResult<MessageClass> CheckVerifyCode([FromUri] CheckVerifyCodeViewModel checkVerifyCodeViewModel)
        {
            var q = UserService.CheckVerifyCode(checkVerifyCodeViewModel.Code, checkVerifyCodeViewModel.Username);
            return Json(q);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("Account/GetVerifyCode")]
        public JsonResult<MessageClass> GetVerifyCode([FromUri] CheckVerifyCodeViewModel checkVerifyCodeViewModel)
        {
            var q = UserService.GetVerifyCode(checkVerifyCodeViewModel.Username);
            return Json(q);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("Account/CheckMobileNumber")]
        public JsonResult<MessageClass> CheckMobileNumber([FromUri] CheckMobileNumberViewModel checkMobileNumberViewModel)
        {
            var q = UserService.CheckMobileNumber(checkMobileNumberViewModel.Mobile);
            return Json(q);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("Account/SaveProfileImage")]
        public JsonResult<MessageClass> SaveProfileImage()
        {
            var q = UserService.SaveProfileImage(EnvironmentVariable.UserId);
            return Json(q);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("Account/EditUserAccount")]
        public JsonResult<MessageClass> EditUserAccount([FromBody] RegisterUserApiViewModel registerUserApiViewModel)
        {
            var q = UserService.EditUserAccount(registerUserApiViewModel ,EnvironmentVariable.UserId);
            return Json(q);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("Account/ChangePassOnLoginMode")]
        public JsonResult<MessageClass> ChangePassOnLoginMode([FromBody] RegisterUserApiViewModel registerUserApiViewModel)
        {
            var q = UserService.ChangePassOnLoginMode(registerUserApiViewModel, EnvironmentVariable.UserId);
            return Json(q);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("Account/ChangePassword")]
        public JsonResult<MessageClass> ChangePassword([FromBody] RegisterUserApiViewModel registerUserApiViewModel)
        {
            var q = UserService.ChangePassword(registerUserApiViewModel, EnvironmentVariable.UserId);
            return Json(q);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("Account/VerifyCaptchaResponse")]
        public JsonResult<RecaptchaResponseViewModel> VerifyCaptchaResponse([FromBody] RecaptchaViewModel recaptchaViewModel)
        {
            var q = UserService.VerifyCaptchaResponse(recaptchaViewModel);
            return Json(q);
        }
    }
}
