namespace Artix.API.Endpoints.Controllers.Shared;

using Artix.API.Core.Contract.Primitives.Infra.File;
using Artix.API.Core.Contract.Primitives.Authorization;
using Common;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

public class UploadController : SharedController
{
    private readonly IUploadService _service;
    private readonly IFileStorage _storage;


    public UploadController(IMediator mediator, IUploadService service, IFileStorage storage) : base(mediator)
    {
        this._service = service;
        this._storage = storage;
    }

    [HttpPost("initiate")]
    [Authorize(Policy = AdminPermission.DigitalAssetManage)]
    public async Task<IActionResult> Initiate([FromForm] string fileName, [FromForm] long totalSize,
        [FromForm] int chunkSize = 2_000_000)
    {
        var s = await this._service.InitiateAsync(fileName, totalSize, chunkSize, CurrentUserId(), CancellationToken.None);
        return this.Ok(new { uploadId = s.Id, chunkSize = s.ChunkSize, totalChunks = s.TotalChunks });
    }

    [HttpPut("{uploadId}/chunk/{index}")]
    [Authorize(Policy = AdminPermission.DigitalAssetManage)]
    [RequestSizeLimit(8_000_000)]
    public async Task<IActionResult> UploadChunk([FromRoute] Guid uploadId, [FromRoute] int index)
    {
        await this._storage.SaveChunkAsync(uploadId, index, this.Request.Body, this.HttpContext.RequestAborted);
        await this._service.MarkChunkReceivedAsync(uploadId, index, CurrentUserId(), CancellationToken.None);
        return this.Ok();
    }

    [HttpPost("{uploadId}/complete")]
    [Authorize(Policy = AdminPermission.DigitalAssetManage)]
    public async Task<IActionResult> Complete([FromRoute] Guid uploadId)
    {
        await this._service.MergeChunksAsync(uploadId, CurrentUserId(), CancellationToken.None);
        return this.Ok();
    }

    [HttpGet("{uploadId}/status")]
    [Authorize(Policy = AdminPermission.DigitalAssetRead)]
    public async Task<IActionResult> Status([FromRoute] Guid uploadId)
    {
        var s = await this._service.GetStatusAsync(uploadId, CurrentUserId(), CancellationToken.None);
        if (s == null) return this.NotFound();
        var received = s.ReceivedChunks.Keys.OrderBy(k => k).ToArray();
        return this.Ok(new
        {
            s.Id,
            s.FileName,
            s.TotalSize,
            s.ChunkSize,
            s.TotalChunks,
            received,
            s.Completed
        });
    }

    private long CurrentUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return long.TryParse(value, out var userId)
            ? userId
            : throw new UnauthorizedAccessException("User identity is missing.");
    }
}
