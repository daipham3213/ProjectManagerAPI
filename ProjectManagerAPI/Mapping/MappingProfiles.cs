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
            CreateMap<Group, CreatedDepartment>();
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
                .ForMember(u => u.UserCreated, opt => opt.MapFrom(u => "api/user/profile?key=" + u.UserCreated));

            //phase
            CreateMap<Phase, CreatedPhase>();
            CreateMap<Phase, PhaseViewResource>().ForMember(u => u.Url, opt => opt.MapFrom(u => "api/Phase/" + u.Id));
            CreateMap<Phase, PhaseResource>().ForMember(u => u.Url, opt => opt.MapFrom(u => "api/Phase/" + u.Id));

            //Request
            CreateMap<Request,CreatedRequest>();
            CreateMap<Request, RequestResource>()
                .ForMember(u => u.url_true,
                    opt => opt.MapFrom(u => "api/Request/activegroup?requestId=" + u.Id + "&isActive=true"))
                .ForMember(u => u.url_false,
                    opt => opt.MapFrom(u => "api/Request/activegroup?requestId=" + u.Id + "&isActive=false"));

            //Task
            CreateMap<Task, CreatedTask>();
            CreateMap<Task, TaskViewResource>()
                .ForMember(u => u.Url, opt => opt.MapFrom(u => "api/Task/" + u.Id))
                .ForMember(u => u.ChildTasks, opt => opt.Ignore())
                .AfterMap((p, u) =>
                {
                    MapChildrenTask(p,u);
                });
            CreateMap<Task, TaskResources>()
                .ForMember(u => u.Url, opt => opt.MapFrom(u => "api/Task/" + u.Id))
                .ForMember(u => u.PhaseName, opt => opt.MapFrom(g => g.Phase.Name))
                .ForMember(u => u.UserName, opt => opt.MapFrom(g => g.User.Name))
                .ForMember(u => u.PhaseUrl, opt => opt.MapFrom(u => "api/Task/" + u.PhaseId))
                .ForMember(u => u.UserUrl, opt => opt.MapFrom(u => "api/Task/" + u.UserId));
                //.ForMember(u => u.UserCreated, opt => opt.MapFrom(u => "api/user/profile?key=" + u.UserCreated));
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

        private void MapChildrenTask(Task parent, TaskViewResource mapped)
        {
            //basic mapping
            mapped.Id = parent.Id;
            mapped.UserName = parent.User.Name;
            mapped.StartDate = parent.StartDate.Value;
            mapped.DueDate = parent.DueDate.Value;
            mapped.PhaseName = parent.Phase.Name;
            mapped.Percent = parent.Percent;
            mapped.Name = parent.Name;
            mapped.Remark = parent.Remark;

            //Child mapping
            if (parent.ChildTasks.Count == 0)
                return;

            foreach (var child in parent.ChildTasks)
            {
                var mappedResource = new TaskViewResource();
                MapChildrenTask(child, mappedResource);

                mappedResource.ChildTasks.Add(mappedResource);
            }

        }
    }
}
