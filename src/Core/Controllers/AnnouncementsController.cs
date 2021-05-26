//using Microsoft.AspNetCore.Mvc;
//using System.Collections.Generic;
//using Pb.Api.Entities;
//using Pb.Api.Models.Announcements;
//using AutoMapper;
//using Pb.Api.Interfaces;

//namespace Pb.Api.Controllers
//{
//    [ApiController]
//    [Route("[controller]")]
//    public class AnnouncementsController : BaseController
//    {
//        private readonly IAnnouncementService _announcementService;
//        private readonly IMapper _mapper;

//        public AnnouncementsController(
//            IAnnouncementService announcementService,
//            IMapper mapper)
//        {
//            _announcementService = announcementService;
//            _mapper = mapper;
//        }

//        [HttpPost("create")]
//        public IActionResult Create(CreateRequest model)
//        {
//            var registred = _announcementService.Create(model, Request.Headers["host"]);
//            return Ok(new { message = "Inscription réussie. Merci de vérifier votre compte en suivant le lien reçu par email.", status = registred ? "Added" : "AlreadyRegistred" });
//        }

//        [Authorize(Role.Admin)]
//        [HttpGet]
//        public ActionResult<IEnumerable<AnnouncementResponse>> GetAll()
//        {
//            var accounts = _announcementService.GetAll();
//            return Ok(accounts);
//        }

//        [Authorize]
//        [HttpGet("{id:int}")]
//        public ActionResult<AnnouncementResponse> GetById(int id)
//        {
//            // Users can get their own account and admins can get any account
//            if (id != Account.Id && Account.RoleId != Role.Admin)
//                return Unauthorized(new { message = "Unauthorized" });

//            var account = _announcementService.GetById(id);
//            return Ok(account);
//        }

//        [Authorize]
//        [HttpPut("{id:int}")]
//        public ActionResult<AnnouncementResponse> Update(int id, UpdateRequest model)
//        {
//            var announcement = GetById(id);
//            // Users can update their announcements and admins can update any announcement
//            if (announcement.Value.Account.Id != Account.Id && Account.RoleId != Role.Admin)
//                return Unauthorized(new { message = "Unauthorized" });

//            announcement = _announcementService.Update(id, model);
//            return Ok(announcement);
//        }

//        [Authorize]
//        [HttpDelete("{id:int}")]
//        public IActionResult Delete(int id)
//        {
//            var announcement = GetById(id);
//            // Users can delete their announcements and admins can delete any announcement
//            if (announcement.Value.Id != Account.Id && Account.RoleId != Role.Admin)
//                return Unauthorized(new { message = "Unauthorized" });

//            _announcementService.Delete(id);
//            return Ok(new { message = "Announcement deleted successfully" });
//        }
//    }
//}
