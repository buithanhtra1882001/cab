using AutoMapper;
using CabMediaService.Grpc.Protos.MediaClient;
using CabPostService.Grpc.Protos.UserClient;
using CabPostService.Models.Dtos;
using CabPostService.Models.Dtos.PostVideo;
using CabPostService.Models.Entities;
using Grpc.Core;
using Grpc.Net.Client;

namespace CabPostService.Grpc.Procedures;

public class MediaService : IMediaService
{
    private readonly ILogger<MediaService> _logger;
    private readonly IMapper _mapper;
    private readonly MediaProtoService.MediaProtoServiceClient _client;

    public MediaService(ILogger<MediaService> logger, IMapper mapper, IConfiguration configuration)
    {
        _logger = logger;
        _mapper = mapper;
        var channel = GrpcChannel.ForAddress(configuration.GetValue<string>("MediaService:BaseAddress"));
        _client = new MediaProtoService.MediaProtoServiceClient(channel);
    }
    public async Task<UploadPostVideoResponse> UploadFileAsync(UploadPostVideoRequest request)
    {
        var fileRequest = new FileRequest
        {
            UserId = request.UserId,
            Content = null,
            Type = "video"
        };
        try
        {
            var result = await _client.UploadFileAsync(fileRequest);
            var response = _mapper.Map<UploadPostVideoResponse>(result);
            return response;
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex.Message);
            throw new Exception(ex.Message);
        }
    }
}