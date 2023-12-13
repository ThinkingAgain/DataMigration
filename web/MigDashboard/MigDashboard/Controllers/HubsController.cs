using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MigDashboard.Hubs;
using System.Drawing;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MigDashboard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HubsController : ControllerBase
    {
        private readonly IHubContext<TaskHub> _hubContext;
        private readonly IConfiguration _config;
        public HubsController(IHubContext<TaskHub> taskHub, IConfiguration configuration)
        {
            _hubContext = taskHub;
            _config = configuration;
        }
        
        
        // 调用客户端方法推送任务消息
        // id: 任务id
        [HttpGet("taskmessage/{id}/{message}")]
        public async Task<String> PushMessage(string id, string message)
        {
            await _hubContext.Clients.All.SendAsync("TaskMessage", id, message);
            return $"{id} - {message}";
        }       


        // 调用客户端方法启动任务
        // id: 任务id
        [HttpGet("taskstart/{id}")]
        public async Task<string> TaskStart(string id)
        {
            await _hubContext.Clients.All.SendAsync("TaskStart", id);
            return id;
        }

        // 调用客户端方法停止任务
        // id: 任务id
        [HttpGet("taskend/{id}")]
        public async Task<string> TaskEnd(string id)
        {
            await _hubContext.Clients.All.SendAsync("TaskEnd", id);
            return id;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        [HttpPost("files/upload")]
        public async Task<IActionResult> UploadFileAsync(IFormFile formFile)
        {
            // 扩展名验证
            string[] permittedExtensions = { ".xlsx", ".xls" };

            var ext = Path.GetExtension(formFile.FileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
            {
                return BadRequest(new { message = "文件类型错误" });
            }

            // 文件大小验证
            if (formFile.Length > 0)
            {
                var filePath = Path.Combine(_config["CMDC:PonProcessor:WaitingFolder"], formFile.FileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    await formFile.CopyToAsync(stream);
                }
            }


            // Process uploaded files
            // Don't rely on or trust the FileName property without validation.

            return Ok(new { message = "文件上传成功" });
        }

        // GET: api/<HubsController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<HubsController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<HubsController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<HubsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<HubsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
