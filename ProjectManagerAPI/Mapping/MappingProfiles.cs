using AutoMapper;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.Models.Resources;
using System.Linq;

namespace ProjectManagerAPI.Mapping
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
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

            CreateMap<UserType, CreatedUserType>()
                .ForMember(u => u.ParentNID, opt => opt.MapFrom(k => k.ParentN.ID));
            CreateMap<UserType, UserTypeViewResource>()
                .ForMember(u => u.url, opt => opt.MapFrom(u => "api/UserType/" + u.ID));
            CreateMap<UserType, UserTypeResource>()
                .ForMember(u => u.ParentN, opt => opt.Ignore())
                .AfterMap((c, u) =>
                {
                    MapParentUserType(c, u);
                });
            CreateMap<Avatar, AvatarResource>();
            CreateMap<User, UserResource>()
                .ForMember(u => u.AvatarUrl, opt => opt.MapFrom(p => p.Avatars.FirstOrDefault(a => a.IsMain) != null ? p.Avatars.FirstOrDefault(a => a.IsMain).Path : null))
                .ForMember(u => u.EmailConfirmed, opt => opt.MapFrom(a => a.IsActived))
                .ForMember(u => u.GroupID, opt => opt.MapFrom(g => g.GroupRef))
                .ForMember(u => u.UserTypeID, opt => opt.MapFrom(r => r.UserType.ID));
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
        private void MapParentUserType(UserType domain, UserTypeResource resource)
        {
            //basic mapping
            resource.ID = domain.ID;
            resource.Name = domain.Name;
            resource.Remark = domain.Remark;
            resource.UserCreated = domain.UserCreated;
            resource.DateCreated = domain.DateCreated;
            resource.DateModified = domain.DateModified;

            if (domain.ParentN == null)
                return;

            var parent_n = new UserTypeResource();
            MapParentUserType(domain.ParentN, parent_n);

            resource.ParentN = parent_n;
        }
    }
}
