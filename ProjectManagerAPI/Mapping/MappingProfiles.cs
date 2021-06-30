using AutoMapper;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.Resources;
using System.Linq;

namespace ProjectManagerAPI.Mapping
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            // Group Type
            CreateMap<GroupType, GroupTypeResource>()
                .ForMember(u => u.ParentN, opt => opt.Ignore())
                .AfterMap((c, u) =>
                {
                    MapParentGroup(c, u);
                });
            CreateMap<GroupType, GroupTypeViewResource>()
                .ForMember(u => u.url, opt => opt.MapFrom(u => "api/GroupType/" + u.ID));
            CreateMap<GroupType, CreatedGroupType>()
                .ForMember(u => u.ParentNID, opt => opt.MapFrom(k => k.ParentN.ID));
           //User
            CreateMap<Avatar, AvatarResource>();
            CreateMap<User, UserResource>()
                .ForMember(u => u.AvatarUrl, opt => opt.MapFrom(p => p.Avatars.FirstOrDefault(a => a.IsMain) != null ? p.Avatars.FirstOrDefault(a => a.IsMain).Path : null))
                .ForMember(u => u.EmailConfirmed, opt => opt.MapFrom(a => a.IsActived))
                .ForMember(u => u.GroupID, opt => opt.MapFrom(g => g.GroupRef));
            CreateMap<User, SearchUserResource>()
                .ForMember(u => u.AvatarUrl, opt => opt.MapFrom(p => p.Avatars.FirstOrDefault(a => a.IsMain) != null ? p.Avatars.FirstOrDefault(a => a.IsMain).Path : null));

            //Group
            CreateMap<Group, CreatedGroup>();
            CreateMap<Group, GroupViewResource>()
                .ForMember(u => u.Url, opt => opt.MapFrom(u => "api/Group/" + u.ID))
                .ForMember(u => u.Users, opt => opt.MapFrom(g => g.Users.Count));
            CreateMap<Group, GroupResource>()
                .ForMember(u => u.Url, opt => opt.MapFrom(u => "api/Group/" + u.ID));
            //Project
            CreateMap<Project, CreateProject>();
            CreateMap<Project, ProjectViewResource>().ForMember(u => u.url, opt => opt.MapFrom(u => "api/Project/" + u.ID));
            CreateMap<Project, ProjectResource>().ForMember(u => u.url, opt => opt.MapFrom(u => "api/Project/" + u.ID));
            //Report
            CreateMap<Report, CreatedReport>();
        }

        private void MapParentGroup(GroupType domain, GroupTypeResource resource)
        {
            //basic mapping
            resource.ID = domain.ID;
            resource.Name = domain.Name;
            resource.Group = domain.Group;
            resource.Remark = domain.Remark;
            resource.UserCreated = domain.UserCreated;
            resource.DateCreated = domain.DateCreated;
            resource.DateModified = domain.DateModified;
            resource.IsActived = domain.IsActived;
            resource.IsDeleted = domain.IsDeleted;

            if (domain.ParentN == null)
                return;

            var parent_n = new GroupTypeResource();
            MapParentGroup(domain.ParentN, parent_n);

            resource.ParentN = parent_n;
        }
        
    }
}
