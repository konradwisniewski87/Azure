using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BlobStorage.Controllers
{
	[ApiController]
	[Microsoft.AspNetCore.Components.Route("[controller]")]
	public class BlobStorageController : ControllerBase
	{
		private readonly IConfiguration _config;

		public BlobStorageController(IConfiguration config)
        {
			_config = config;
		}

        [HttpPost("upload")]
		public async Task<IActionResult> Upload(IFormFile file)
		{
			BlobServiceClient blobServiceClient = new BlobServiceClient(_config.GetValue<string>("ConnectionString"));

			var containerName = "documents";
			BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

			await containerClient.CreateIfNotExistsAsync();

			BlobClient blobClient = containerClient.GetBlobClient(file.FileName);

			var blobHttpHeaders = new BlobHttpHeaders();
			blobHttpHeaders.ContentType = file.ContentType;

			//await blobClient.UploadAsync(file.OpenReadStream()); //return error when file exist
			//await blobClient.UploadAsync(file.OpenReadStream(), overwrite: true);
			await blobClient.UploadAsync(file.OpenReadStream(), blobHttpHeaders);//overwrite as default

			return Ok();
		}

		[HttpGet("download")]
		public async Task<IActionResult> Download([FromQuery]string blobName)
		{
			var connectionString = "DefaultEndpointsProtocol=https;AccountName=storage1sane;AccountKey=jzmbliQaH8pHjCoiKb5PFKCJld2+vSc8HyqL34dGsXy2XOj8Nmrx3bbQcS2uGyrPfhP5YEgt9dCr+AStPiTX4w==;EndpointSuffix=core.windows.net";
			BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

			var containerName = "documents";
			BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

			await containerClient.CreateIfNotExistsAsync();

			BlobClient blobClient = containerClient.GetBlobClient(blobName);

			var downloadResponse = await blobClient.DownloadContentAsync();
			var content = downloadResponse.Value.Content.ToStream();
			var contentType = blobClient.GetProperties().Value.ContentType;

			return File(content, contentType, fileDownloadName: blobName);
		}
    }

	
}
