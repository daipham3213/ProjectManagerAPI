using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using ProjectManagerAPI.Core.ServiceResource;
using ProjectManagerAPI.Core.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Persistence.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;

        public PhotoService(IConfiguration config)
        {
            var account = new Account(
                config["Cloudinary:CloudName"],
                config["Cloudinary:ApiKey"],
                config["Cloudinary:ApiSecret"]
                );
            _cloudinary = new Cloudinary(account);
        }

        public async Task<PhotoResource> AddPhoto(IFormFile file)
        {
            if (file.Length > 0)
            {
                await using var stream = file.OpenReadStream();

                try
                {
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.FileName, stream),
                        Transformation = new Transformation().Height(500).Width(500).Crop("fill")
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                    if (uploadResult.Error != null)
                        throw new Exception(uploadResult.Error.Message);

                    return new PhotoResource
                    {
                        Id = Guid.NewGuid(),
                        Publicid = uploadResult.PublicId,
                        Url = uploadResult.SecureUrl.ToString()
                    };
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine(e);
                }

            }

            return null;
        }

        public async Task<IList<PhotoResource>> AddPhotos(IFormCollection files)
        {
            var photos = new List<PhotoResource>();

            foreach (var file in files.Files)
            {
                var photo = await AddPhoto(file);
                photos.Add(photo);
            }

            return photos;
        }

        public async Task<string> DeletePhoto(string publicid)
        {
            var deleteParams = new DeletionParams(publicid);

            var deleteResult = await _cloudinary.DestroyAsync(deleteParams);

            return deleteResult.Result == "ok" ? deleteResult.Result : null;
        }

        public async Task<string> DeletePhotos(IList<string> ids)
        {
            foreach (var id in ids)
            {
                var deleteResult = await DeletePhoto(id);

                if (deleteResult == null)
                    throw new Exception("An error occur when deleting a photo from coundinary");

            }

            return "ok";
        }
    }
}
