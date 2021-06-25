using AutoMapper;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.Models.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Mapping
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<GroupType, GroupTypeResource>()
                .ForMember(u => u.ParentN, opt => opt.Ignore())
                .AfterMap((c, u) => {
                    MapParent(c, u);
                });
            CreateMap<GroupType, GroupTypeViewResource>()
                .ForMember(u => u.url, opt => opt.MapFrom(u =>"api/grouptype/" + u.ID));
        }

        private void MapParent(GroupType domain, GroupTypeResource resource)
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
            MapParent(domain.ParentN, parent_n);

            resource.ParentN = parent_n;
        }
    }
}
