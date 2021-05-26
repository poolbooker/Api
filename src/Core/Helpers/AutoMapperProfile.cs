using AutoMapper;
using Pb.Api.Entities;

namespace Pb.Api.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Account, Models.Accounts.AccountResponse>();

            CreateMap<Account, Models.Accounts.AuthenticateResponse>();

            CreateMap<Models.Accounts.RegisterRequest, Account>();

            CreateMap<Models.Accounts.CreateRequest, Account>();

            CreateMap<Models.Accounts.UpdateRequest, Account>()
                .ForAllMembers(x => x.Condition(
                    (src, dest, prop) =>
                    {
                        // ignore null & empty string properties
                        if (prop == null) return false;
                        if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) return false;

                        // ignore null role
                        //if (x.DestinationMember.Name == "Role" && src.Role == null) return false;

                        return true;
                    }
                ));

            //CreateMap<Announcement, Models.Announcements.AnnouncementResponse>();

            //CreateMap<Models.Announcements.CreateRequest, Announcement>();

            //CreateMap<Models.Announcements.UpdateRequest, Announcement>()
            //    .ForAllMembers(x => x.Condition(
            //        (src, dest, prop) =>
            //        {
            //            // ignore null & empty string properties
            //            if (prop == null) return false;
            //            if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) return false;

            //            return true;
            //        }
            //    ));
        }
    }
}