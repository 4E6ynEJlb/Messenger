using Application.Models.Output;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MaintenanceAPI.Controllers
{
    [ApiController]
    public class MediaController : ControllerBase
    {
        private readonly IMediaService _mediaService;
        public MediaController(IMediaService mediaService)
        {
            _mediaService = mediaService;
        }

        [ProducesResponseType(typeof(FileResult), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken, bool download = false)
        {
            MediaStream file = await _mediaService.GetFile(id, cancellationToken);

            return download 
                ? new FileStreamResult(file.Content, file.ContentType) {FileDownloadName = file.FileName} 
                : new FileStreamResult(file.Content, file.ContentType);
        }

        [ProducesResponseType(typeof(MediaInfo), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> Info(Guid id, CancellationToken cancellationToken)
        {            
            return Ok(await _mediaService.GetFileInfo(id, cancellationToken));
        }
    }
}
