using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using CabMediaService.Controllers.Base;
using CabMediaService.Models.Dtos;
using CabMediaService.Services.Interfaces;

namespace CabMediaService.Controllers
{
    public class ImagesController : ApiController<ImagesController>
    {
        private readonly IAWSMediaService _imageService;
        private readonly IDropBoxMediaService _dropBoxMediaService;
        private readonly string VIDEO = "PostVideos";
        public ImagesController(ILogger<ImagesController> logger, IAWSMediaService imageService, IDropBoxMediaService dropBoxMediaService) : base(logger)
        {
            _imageService = imageService;
            _dropBoxMediaService = dropBoxMediaService;
        }

        /// <summary>
        /// Get single image.
        /// </summary>
        /// <response code="200">Got image successfully</response>
        /// <response code="401">Authentication is failed</response>
        /// <response code="403">Authorization is failed</response>
        [HttpGet("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(MediaImageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(Guid id)
        {
            var image = await _imageService.GetAsync(id);
            return Ok(image);
        }

        /// <summary>
        /// Get multiple images.
        /// </summary>
        /// <response code="200">Got a list of images successfully</response>
        /// <response code="401">Authentication is failed</response>
        /// <response code="403">Authorization is failed</response>
        [HttpPost("list")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<MediaImageResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetList([Required][FromBody] IEnumerable<Guid> ids)
        {
            var images = await _imageService.GetListAsync(ids);
            return Ok(images);
        }

        /// <summary>
        /// Upload single or multiple images.
        /// </summary>
        /// <response code="200">Uploaded images successfully</response>
        /// <response code="401">Authentication is failed</response>
        /// <response code="403">Authorization is failed</response>
        [HttpPost("upload")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<UploadImageResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Upload([Required] Guid auid)
        {
            var userId = GetUserIdFromToken();
            var formCollection = await Request.ReadFormAsync();
            var files = formCollection.Files;
            var type = formCollection["type"];
            //var result = await _imageService.UploadAsync(auid, type, files); //AWS
            var result = await _dropBoxMediaService.UploadImageAsync(files, userId); //Dropbox
            return Ok(result);
        }


        /// <summary>
        /// Update single or multiple images.
        /// </summary>
        /// <response code="200">Uploaded images successfully</response>
        /// <response code="401">Authentication is failed</response>
        /// <response code="403">Authorization is failed</response>
        [HttpPut("update")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update([Required] Guid auid, [Required][FromBody] UpdateMediaImageRequest request)
        {
            await _imageService.UpdateLastUsedAsync(request.Guids, request.LastUsedAt);
            return Ok(new HttpMessageResponse
            {
                Message = "Images are updated successfully"
            });
        }

        /// <summary>
        /// Delete an image.
        /// </summary>
        /// <response code="200">Deleted an image successfully</response>
        /// <response code="401">Authentication is failed</response>
        /// <response code="403">Authorization is failed</response>
        [HttpDelete("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(HttpMessageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete([Required] Guid auid, Guid id)
        {
            var httpMessageResponse = new HttpMessageResponse();
            await _imageService.DeleteAsync(auid, id);
            httpMessageResponse.Message = "Image is deleted successfully";
            return Ok(httpMessageResponse);
        }

        /// <summary>
        /// Delete multiple images.
        /// </summary>
        /// <response code="200">Deleted images successfully</response>
        /// <response code="401">Authentication is failed</response>
        /// <response code="403">Authorization is failed</response>
        [HttpDelete("")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(HttpMessageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteMany([Required] Guid auid, [FromBody] IEnumerable<Guid> ids)
        {
            var httpMessageResponse = new HttpMessageResponse();
            await _imageService.DeleteManyAsync(auid, ids);
            httpMessageResponse.Message = "Images are deleted successfully";
            return Ok(httpMessageResponse);
        }
    }
}