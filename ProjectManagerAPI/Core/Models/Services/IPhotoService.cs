using Microsoft.AspNetCore.Http;
using ProjectManagerAPI.Core.Models.ServiceResource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Core.Models.Services
{
    public interface IPhotoService
    {
        Task<PhotoResource> AddPhoto(IFormFile file);
        Task<IList<PhotoResource>> AddPhotos(IFormCollection files);
        Task<string> DeletePhoto(string publicid);
        Task<string> DeletePhotos(IList<string> ids);
    }
}
