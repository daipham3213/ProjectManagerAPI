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
                .ForMember(u => u.Url, opt => opt.MapFrom(u => "api/GroupType/" + u.Id));
            CreateMap<GroupType, CreatedGroupType>()
                .ForMember(u => u.ParentNid, opt => opt.MapFrom(k => k.ParentN.Id));
            //User
            CreateMap<Avatar, AvatarResource>();
            CreateMap<User, UserResource>()
                .ForMember(u => u.AvatarUrl, opt => opt.MapFrom(p => p.Avatars.FirstOrDefault(a => a.IsMain) != null ? p.Avatars.FirstOrDefault(a => a.IsMain).Path : null))
                .ForMember(u => u.EmailConfirmed, opt => opt.MapFrom(a => a.IsActived))
                .ForMember(u => u.GroupId, opt => opt.MapFrom(g => g.GroupRef));
            CreateMap<User, SearchUserResource>()
                .ForMember(u => u.AvatarUrl, opt => opt.MapFrom(p => p.Avatars.FirstOrDefault(a => a.IsMain) != null ? p.Avatars.FirstOrDefault(a => a.IsMain).Path : null));

            //Group
            CreateMap<Group, CreatedGroup>();
            CreateMap<Group, GroupViewResource>()
                .ForMember(u => u.Url, opt => opt.MapFrom(u => "api/Group/" + u.Id))
                .ForMember(u => u.Users, opt => opt.MapFrom(g => g.Users.Count));
            CreateMap<Group, GroupResource>()
                .ForMember(u => u.Url, opt => opt.MapFrom(u => "api/Group/" + u.Id));
            //Project
            CreateMap<Project, CreateProject>();
            CreateMap<Project, ProjectViewResource>().ForMember(u => u.url, opt => opt.MapFrom(u => "api/Project/" + u.Id));
            CreateMap<Project, ProjectResource>().ForMember(u => u.url, opt => opt.MapFrom(u => "api/Project/" + u.Id));
            //Report
            CreateMap<Report, CreatedReport>();
            CreateMap<Report, ReportViewResource>()
                .ForMember(u => u.Url, opt => opt.MapFrom(u => "api/Group/" + u.Id))
                .ForMember(u => u.GroupName, opt => opt.MapFrom(g => g.Group.Name))
                .ForMember(u => u.ProjectName, opt => opt.MapFrom(g => g.Project.Name));
            CreateMap<Report, ReportResource>()
                .ForMember(u => u.Url, opt => opt.MapFrom(u => "api/Group/" + u.Id))
                .ForMember(u => u.GroupName, opt => opt.MapFrom(g => g.Group.Name))
                .ForMember(u => u.ProjectName, opt => opt.MapFrom(g => g.Project.Name))
                .ForMember(u => u.GroupUrl, opt => opt.MapFrom(u => "api/Group/" + u.GroupId))
                .ForMember(u => u.ProjectUrl, opt => opt.MapFrom(u => "api/Project/" + u.ProjectId))
                .ForMember(u => u.UserCreated, opt => opt.MapFrom(u => "api/User/" + u.ProjectId));

        }

        private void MapParentGroup(GroupType domain, GroupTypeResource resource)
        {
            //basic mapping
            resource.Id = domain.Id;
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

            var parentN = new GroupTypeResource();
            MapParentGroup(domain.ParentN, parentN);

            resource.ParentN = parentN;
        }


    }
}
