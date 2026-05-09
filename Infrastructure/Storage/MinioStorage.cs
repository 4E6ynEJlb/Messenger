using Infrastructure.Exceptions;
using Infrastructure.Models;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;

namespace Infrastructure.Storage
{
    public class MinioStorage : IObjectStorage
    {
        private readonly IMinioClient _client;
        private readonly string _bucketName;
        public MinioStorage(IMinioClient client, IOptions<MinioOptions> options)
        {
            _client = client;
            _bucketName = options.Value.BucketName;
            CreateBucketAsync();
        }

        public async Task<MemoryStream> GetAsync(Guid name, CancellationToken cancellationToken)
        {
            StatObjectArgs statObjectArgs = new StatObjectArgs().WithBucket(_bucketName).WithObject(name.ToString());
            var statArgs = await _client.StatObjectAsync(statObjectArgs);
            MemoryStream stream = new MemoryStream(Convert.ToInt32(statArgs.Size));
            GetObjectArgs args = new GetObjectArgs().WithBucket(_bucketName).WithObject(name.ToString()).WithCallbackStream(async str =>
            {
                await str.CopyToAsync(stream);
                stream.Seek(0, SeekOrigin.Begin);
            });
            try
            {
                await _client.GetObjectAsync(args, cancellationToken);
            }
            catch (ObjectNotFoundException e)
            {
                throw new FileDoesNotExistException(e);
            }
            return stream;
        }

        public async Task SaveAsync(Stream stream, Guid name, CancellationToken cancellationToken)
        {
            PutObjectArgs args = new PutObjectArgs().WithBucket(_bucketName).WithObject(name.ToString()).WithStreamData(stream).WithObjectSize(stream.Length).WithContentType("application/octet-stream");
            await _client.PutObjectAsync(args, cancellationToken);
        }

        public async Task DeleteAsync(Guid name, CancellationToken cancellationToken)
        {
            RemoveObjectArgs args = new RemoveObjectArgs().WithBucket(_bucketName).WithObject(name.ToString());
            await _client.RemoveObjectAsync(args, cancellationToken);
        }

        private async void CreateBucketAsync()
        {
            BucketExistsArgs bucketExistsArgs = new BucketExistsArgs().WithBucket(_bucketName);
            if (!await _client.BucketExistsAsync(bucketExistsArgs))
            {
                MakeBucketArgs makeBucketArgs = new MakeBucketArgs().WithBucket(_bucketName);
                await _client.MakeBucketAsync(makeBucketArgs);
            }
        }
    }
}
