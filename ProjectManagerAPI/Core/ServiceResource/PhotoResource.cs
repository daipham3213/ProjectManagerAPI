using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Core.ServiceResource
{
    public class PhotoResource
    {
        public Guid Id { get; set; }
        public string publicid { get; set; }
        public string Url { get; set; }
    }
}
