namespace Artix.API.Endpoints.Controllers.Shared;

using Artix.API.Core.Contract.Primitives.Infra.File;
using Common;
using MediatR;
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
    public async Task<IActionResult> Initiate([FromForm] string fileName, [FromForm] long totalSize,
        [FromForm] int chunkSize = 2_000_000)
    {
        var s = await this._service.InitiateAsync(fileName, totalSize, chunkSize, CancellationToken.None);
        return this.Ok(new { uploadId = s.Id, chunkSize = s.ChunkSize, totalChunks = s.TotalChunks });
    }

    [HttpPut("{uploadId}/chunk/{index}")]
    public async Task<IActionResult> UploadChunk([FromRoute] Guid uploadId, [FromRoute] int index)
    {
        await this._storage.SaveChunkAsync(uploadId, index, this.Request.Body, this.HttpContext.RequestAborted);
        await this._service.MarkChunkReceivedAsync(uploadId, index, CancellationToken.None);
        return this.Ok();
    }

    [HttpPost("{uploadId}/complete")]
    public async Task<IActionResult> Complete([FromRoute] Guid uploadId)
    {
        await this._service.MergeChunksAsync(uploadId, CancellationToken.None);
        return this.Ok();
    }

    [HttpGet("{uploadId}/status")]
    public async Task<IActionResult> Status([FromRoute] Guid uploadId)
    {
        var s = await this._service.GetStatusAsync(uploadId, CancellationToken.None);
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
}
