using back_Models.Common;
using back_Models.Ticket;
using back_Services.Ticket;
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
    public class TicketController : ApiController
    {
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("Ticket/Save")]
        public JsonResult<MessageClass> Save([FromBody] TicketViewModel TicketViewModel)
        {
            var q = TicketService.Save(TicketViewModel, EnvironmentVariable.UserId);
            return Json(q);
        }

        [UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [JwtAuthorize]
        [Route("Ticket/Delete")]
        public JsonResult<MessageClass> Delete([FromUri] TicketViewModel TicketViewModel)
        {
            var q = TicketService.Delete((int)TicketViewModel.Id, EnvironmentVariable.UserId);
            return Json(q);
        }

        [UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [JwtAuthorize]
        [Route("Ticket/GetTicketGridData")]
        public JsonResult<List<TicketViewModel>> GetTicketGridData()
        {
            var q = TicketService.GetTicketGridData(EnvironmentVariable.UserId);
            return Json(q);
        }

        [UpdateTokenExpireTime]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [JwtAuthorize]
        [Route("Ticket/GetTicketGridDataForPanel")]
        public JsonResult<PaginationViewModel<TicketViewModel>> GetTicketGridDataForPanel([FromBody] PaginateViewModel paginateViewModel)
        {
            var q = TicketService.GetTicketGridDataForPanel(EnvironmentVariable.UserId);
            var result = new PaginationViewModel<TicketViewModel>
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
        [Route("Ticket/ToggleIsReplied")]
        public JsonResult<MessageClass> ToggleIsReplied([FromUri] TicketViewModel TicketViewModel)
        {
            var q = TicketService.ToggleIsReplied((int)TicketViewModel.Id);
            return Json(q);
        }
    }
}
