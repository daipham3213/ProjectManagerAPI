using AutoMapper;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectManagerAPI.Core;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.Resources;
using ProjectManagerAPI.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AvatarController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public AvatarController(IUnitOfWork unitOfWork, IMapper mapper, IPhotoService photoService)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            this._photoService = photoService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string userName)
        {
            
            var user = await this._unitOfWork.Users.GetUser(userName);
            if (user == null)
                return NotFound();

            var avatars = await this._unitOfWork.Avatars.GetAvatars(userName);

            await this._unitOfWork.Complete();

            var result = this._mapper.Map<List<Avatar>, List<AvatarResource>>(avatars);

            return Ok(result);
        }

        [HttpGet("main")]
        public async Task<IActionResult> GetMain(string userName)
        {       
            var user = await this._unitOfWork.Users.GetUser(userName);
            if (user == null)
                return NotFound();

            var avatar = await this._unitOfWork.Avatars.SingleOrDefault(a => a.UserID == user.Id && a.IsMain == true);
            if (avatar == null)
                return NotFound();

            var result = this._mapper.Map<Avatar, AvatarResource>(avatar);
            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UploadAvatar(string userName, IFormFile file)
        {

            if (file == null)
                return BadRequest("no file");

            var user = await this._unitOfWork.Users.GetUser(userName);
            if (user == null)
                return NotFound();

            //Get main avatar of user
            var oldAvatar = await this._unitOfWork.Avatars.SingleOrDefault(a => a.UserID == user.Id && a.IsMain == true);
            if (oldAvatar != null)
                oldAvatar.IsMain = false;

            //Upload new photo to clound
            var cloudPhoto = await this._photoService.AddPhoto(file);

            //Create a new photo in db
            var newAvatar = new Avatar()
            {
                Id = cloudPhoto.Id,
                Path = cloudPhoto.Url,
                UploadTime = DateTime.Now,
                IsMain = true,
                PublicID = cloudPhoto.publicid,
                User = user,
                UserID = user.Id,
            };
            try
            {
                await this._unitOfWork.Avatars.Add(newAvatar);
                await this._unitOfWork.Complete();
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message, statusCode: 500, title: "Upload Photo");
            }

            return Ok(this._mapper.Map<Avatar, AvatarResource>(newAvatar));
        }

        [HttpDelete("{photoID}")]
        [Authorize]
        public async Task<IActionResult> DeleteAvatar(string userName, Guid photoId)
        {
            var user = await this._unitOfWork.Users.GetUser(userName);
            if (user == null)
                return NotFound();

            //Load all avatars of user
            await this._unitOfWork.Avatars.Load(a => a.UserID == user.Id);

            var avatar = user.Avatars.FirstOrDefault(a => a.Id == photoId);
            if (avatar == null)
                return NotFound();

            if (avatar.IsMain)
                return BadRequest("Can not delete main avatar");

            // Delete photo from cloud service
            var photo = await this._unitOfWork.Avatars.Get(photoId);
            var result = await this._photoService.DeletePhoto(photo.PublicID);

            if (result == null)
                return Problem();

            // Delete photo from database
            user.Avatars.Remove(avatar);
            this._unitOfWork.Avatars.Remove(avatar);

            await this._unitOfWork.Complete();

            return Ok();
        }
    }
}
