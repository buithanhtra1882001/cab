using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using CabMediaService.Controllers.Base;
using CabMediaService.Models.Dtos;
using CabMediaService.Services.Interfaces;

namespace CabMediaService.Controllers
{
    public class VideoController : ApiController<VideoController>
    {
        private readonly IAWSMediaService _imageService;
        private readonly IDropBoxMediaService _dropBoxMediaService;
        private readonly string VIDEO = "PostVideos";
        public VideoController(ILogger<VideoController> logger
            , IAWSMediaService imageService, IDropBoxMediaService dropBoxMediaService)
            : base(logger)
        {
            _imageService = imageService;
            _dropBoxMediaService = dropBoxMediaService;
        }
        
        /// <summary>
        /// Upload single or multiple videos.
        /// </summary>
        /// <response code="200">Uploaded images successfully</response>
        /// <response code="401">Authentication is failed</response>
        /// <response code="403">Authorization is failed</response>
        [HttpPost("upload")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<UploadVideoResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UploadVideo([Required] Guid auid)
        {
            var formCollection = await Request.ReadFormAsync();
            var files = formCollection.Files;
            var type = VIDEO;
            var userId = GetUserIdFromToken();
            //var result = await _imageService.UploadVideosAsync(auid, type, files, BearerToken);
            var result = await _dropBoxMediaService.UploadVideoAsync(userId, files, BearerToken);
            
            return Ok(result);
        }
    }
}