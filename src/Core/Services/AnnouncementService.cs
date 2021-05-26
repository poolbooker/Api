//using Microsoft.Extensions.Options;
//using System;
//using System.Collections.Generic;
//using Pb.Api.Entities;
//using Pb.Api.Helpers;
//using AutoMapper;
//using System.IO;
//using Pb.Api.Interfaces;
//using Pb.Api.Models.Announcements;

//namespace Pb.Api.Services
//{
//    public class AnnouncementService : IAnnouncementService
//    {
//        private readonly PoolBookerDbContext _context;
//        private readonly IMapper _mapper;
//        private readonly AppSettings _appSettings;
//        private readonly IEmailService _emailService;

//        public AnnouncementService(
//            PoolBookerDbContext context,
//            IMapper mapper,
//            IOptions<AppSettings> appSettings,
//            IEmailService emailService)
//        {
//            _context = context;
//            _mapper = mapper;
//            _appSettings = appSettings.Value;
//            _emailService = emailService;
//        }

//        public bool Create(CreateRequest model, string target)
//        {
//            var announcement = _mapper.Map<Announcement>(model);

//            announcement.Created = DateTime.UtcNow;

//            // Save announcement
//            _context.Announcements.Add(announcement);
//            _context.SaveChanges();

//            // Send confirmation email
//            SendConfirmationEmail(announcement, $"https://{target}");
//            return true;
//        }

//        public IEnumerable<AnnouncementResponse> GetAll()
//        {
//            var accounts = _context.Announcements;
//            return _mapper.Map<IList<AnnouncementResponse>>(accounts);
//        }

//        public AnnouncementResponse GetById(int id)
//        {
//            var announcement = GetAnnouncement(id);
//            return _mapper.Map<AnnouncementResponse>(announcement);
//        }

//        public AnnouncementResponse Create(CreateRequest model)
//        {
//            var announcement = _mapper.Map<Announcement>(model);
//            announcement.Created = DateTime.UtcNow;

//            // Save announcement
//            _context.Announcements.Add(announcement);
//            _context.SaveChanges();

//            return _mapper.Map<AnnouncementResponse>(announcement);
//        }

//        public AnnouncementResponse Update(int id, UpdateRequest model)
//        {
//            var announcement = GetAnnouncement(id);

//            // Copy model to announcement and save
//            _mapper.Map(model, announcement);
//            announcement.Updated = DateTime.UtcNow;
//            _context.Announcements.Update(announcement);
//            _context.SaveChanges();

//            return _mapper.Map<AnnouncementResponse>(announcement);
//        }

//        public void Delete(int id)
//        {
//            var announcement = GetAnnouncement(id);
//            _context.Announcements.Remove(announcement);
//            _context.SaveChanges();
//        }

//        private Announcement GetAnnouncement(int id)
//        {
//            var announcement = _context.Announcements.Find(id);
//            if (announcement == null)
//                throw new KeyNotFoundException("Announcement not found");
//            return announcement;
//        }

//        private void SendConfirmationEmail(Announcement announcement, string target)
//        {
//            string message;
//            if (!string.IsNullOrEmpty(target))
//            {
//                using (var reader = new StreamReader(Path.Combine(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "Templates"), "VerificationEmail.html")))
//                {
//                    message = reader.ReadToEnd()
//                        .Replace("{User}", announcement.Account.FirstName)
//                        .Replace("{BannerImageSrc}", $"{target}/app-images/LogoBanner.png")
//                        .Replace("{SecurityUrl}", "https://www.poolbooker.com/securite");
//                }
//                            _emailService.Send(
//                to: announcement.Account.Email,
//                subject: "Publication de votre annonce",
//                html: $@"{message}");
//            }

//        }
//    }
//}
