using System;
using System.Collections.Generic;
using AutoMapper.Configuration.Annotations;
using Newtonsoft.Json;
using ProjectManagerAPI.Core.Models;

namespace ProjectManagerAPI.Core.Resources
{
    public class TaskViewResource
    {
        public TaskViewResource()
        {
            ChildTasks = new List<TaskViewResource>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Remark { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public float Percent { get; set; }
        public string PhaseName { get; set; }
        public string UserName { get; set; }
        public int Duration { get; set; }
        public Guid ParentNId { get; set; }
        [JsonIgnore]
        public Task ParentN { get; set; }
        public ICollection<TaskViewResource> ChildTasks { get; set; }
    }
}
