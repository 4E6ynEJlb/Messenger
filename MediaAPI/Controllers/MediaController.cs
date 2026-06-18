using Application.Models.Output;
using Dapper;
using Domain.Models.Documents;
using Domain.Models.Types;
using Domain.Stores.MongoDB;
using Infrastructure.Database;
using Infrastructure.Storage;
using Microsoft.AspNetCore.Mvc;

namespace MaintenanceAPI.Controllers
{
    [ApiController]
    public class MediaController : ControllerBase
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IObjectStorage _objectStorage;
        private readonly INewMediaStore _newMediaStore;
        public MediaController(IDbConnectionFactory dbConnectionFactory, IObjectStorage objectStorage, INewMediaStore newMediaStore)
        {
            _connectionFactory = dbConnectionFactory;
            _objectStorage = objectStorage;
            _newMediaStore = newMediaStore;
        }

        [ProducesResponseType(typeof(FileResult), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken, bool download = false)
        {
            await using var conn = await _connectionFactory.CreateConnectionAsync();
            const string sql = 
                """
                SELECT (media_id, file_name, content_type) :: media_file
                FROM private.media
                WHERE media_id = @media_id
                LIMIT 1
                """;
            MediaFile? file = await conn.ExecuteScalarAsync<MediaFile>(Cmd(sql,
                new { media_id = id }, cancellationToken));
            if (file is null)
            {
                NewMedia? newFile = await _newMediaStore.GetOneByIdAsync(id, cancellationToken);
                if (newFile is null)
                    return NotFound();
                file = new MediaFile
                {
                    MediaId = newFile.MediaId,
                    FileName = newFile.FileName,
                    ContentType = newFile.ContentType
                };
            }
            var stream = await _objectStorage.GetAsync(id, cancellationToken);

            return download?new FileStreamResult(stream, file.ContentType)
            {
                FileDownloadName = file.FileName
            } : 
            new FileStreamResult(stream, file.ContentType);
        }

        [ProducesResponseType(typeof(MediaInfo), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> Info(Guid id, CancellationToken cancellationToken)
        {
            await using var conn = await _connectionFactory.CreateConnectionAsync();
            const string sql =
                """
                SELECT (media_id, file_name, content_type) :: media_file
                FROM private.media
                WHERE media_id = @media_id
                LIMIT 1
                """;
            var file = await conn.ExecuteScalarAsync<MediaFile>(Cmd(sql,
                new { media_id = id }, cancellationToken));
            if (file is null)
            {
                NewMedia? newFile = await _newMediaStore.GetOneByIdAsync(id, cancellationToken);
                if (newFile is null)
                    return NotFound();
                file = new MediaFile
                {
                    MediaId = newFile.MediaId,
                    FileName = newFile.FileName,
                    ContentType = newFile.ContentType
                };
            }
            MediaInfo mediaInfo = new()
            {
                FileName = file.FileName,
                ContentType = file.ContentType
            };
            return Ok(mediaInfo);
        }

        private static CommandDefinition Cmd(string sql, object? param, CancellationToken cancellationToken) =>
            new(sql, param, cancellationToken: cancellationToken);
    }
}
