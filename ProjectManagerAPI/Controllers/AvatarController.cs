using AutoMapper;
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
using System.Threading.Tasks;
using ProjectManagerAPI.Core.Policy;

namespace ProjectManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AvatarController : Controller
    {
        public AvatarController(IUnitOfWork unitOfWork, IMapper mapper, IPhotoService photoService,
            IAuthorizationService authorization)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _photoService = photoService;
            _authorization = authorization;
        }

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;
        private readonly IAuthorizationService _authorization;


        [HttpGet]
        public async Task<IActionResult> Get(string userName)
        {
            var user = await _unitOfWork.Users.GetUser(userName);
            if (user == null)
                throw new Exception("Username can not be found.");

            var avatars = await _unitOfWork.Avatars.GetAvatars(userName);

            await _unitOfWork.Complete();

            var result = _mapper.Map<List<Avatar>, List<AvatarResource>>(avatars);

            return Ok(result);
        }

        [HttpGet("main")]
        public async Task<IActionResult> GetMain(string userName)
        {
            var user = await _unitOfWork.Users.GetUser(userName);
            if (user == null)
                throw new Exception("User can not be found.");

            var avatar = await _unitOfWork.Avatars.SingleOrDefault(a => a.UserId == user.Id && a.IsMain);
            if (avatar == null)
                throw new Exception("Main Avatar can not be found.");

            var result = _mapper.Map<Avatar, AvatarResource>(avatar);
            return Ok(result);
        }

        [HttpPost("main")]
        public async Task<IActionResult> SwitchMain(string userName, Guid id)
        {
            var user = await _unitOfWork.Users.GetUser(userName);
            if (user == null)
                throw new Exception("User can not be found.");

            var main = await _unitOfWork.Avatars.SingleOrDefault(a => a.UserId == user.Id && a.IsMain);
            main.IsMain = false;

            var avatar = await _unitOfWork.Avatars.SingleOrDefault(a => a.UserId == user.Id && a.Id == id);
            if (avatar == null)
                throw new Exception("Main Avatar can not be found.");

            avatar.IsMain = true;
            await this._unitOfWork.Complete();
            var result = _mapper.Map<Avatar, AvatarResource>(avatar);
            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UploadAvatar(string userName, IFormFile file)
        {
            if (file == null)
                throw new Exception("no file");
            var user = await _unitOfWork.Users.GetUser(userName);
            if (user == null)
                throw new Exception("User can not be found");

            //Get main avatar of user
            var oldAvatar = await _unitOfWork.Avatars.SingleOrDefault(a => a.UserId == user.Id && a.IsMain);
            if (oldAvatar != null)
                oldAvatar.IsMain = false;

            //Upload new photo to clound
            var cloudPhoto = await _photoService.AddPhoto(file);

            //Create a new photo in db
            var newAvatar = new Avatar
            {
                Id = cloudPhoto.Id,
                Path = cloudPhoto.Url,
                UploadTime = DateTime.Now,
                IsMain = true,
                PublicId = cloudPhoto.Publicid,
                User = user,
                UserId = user.Id,
            };
            try
            {
                await _unitOfWork.Avatars.Add(newAvatar);
                await _unitOfWork.Complete();
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message, statusCode: 500, title: "Upload Photo");
            }

            return Ok(_mapper.Map<Avatar, AvatarResource>(newAvatar));
        }

        [HttpDelete("{photoID}")]
        [Authorize]
        public async Task<IActionResult> DeleteAvatar(string userName, Guid photoId)
        {
            var user = await _unitOfWork.Users.GetUser(userName);
            if (user == null)
                return NotFound("User can not be found");

            //Load all avatars of user
            await _unitOfWork.Avatars.Load(a => a.UserId == user.Id);

            var avatar = user.Avatars.FirstOrDefault(a => a.Id == photoId);
            if (avatar == null)
                throw new Exception("Avatar can not be found");

            if (avatar.IsMain)
                throw new Exception("Can not delete main avatar");

            await this._authorization.AuthorizeAsync(User, avatar, Operations.AvatarDelete);

            // Delete photo from cloud service
            var photo = await _unitOfWork.Avatars.Get(photoId);
            var result = await _photoService.DeletePhoto(photo.PublicId);

            if (result == null)
                throw new Exception("Error while deleting photo.");

            // Delete photo from database
            user.Avatars.Remove(avatar);
            _unitOfWork.Avatars.Remove(avatar);

            await _unitOfWork.Complete();

            return Ok(new {message = "Deleted successfully."});
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetList(string username)
        {
            var result = await this._unitOfWork.Avatars.GetAvatars(username);
            return Ok(_mapper.Map<IEnumerable<AvatarResource>>(result));
        }
    }
}