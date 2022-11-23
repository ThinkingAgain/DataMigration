using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MigDashboard.Hubs;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MigDashboard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HubsController : ControllerBase
    {
        private readonly IHubContext<TaskHub> _hubContext;
        public HubsController(IHubContext<TaskHub> taskHub)
        {
            _hubContext = taskHub;
        }
        
        
        // 调用客户端方法推送任务消息
        // id: 任务id
        [HttpGet("taskmessage/{id}/{message}")]
        public async Task<String> PushMessage(int id, string message)
        {
            await _hubContext.Clients.All.SendAsync("TaskMessage", id, message);
            return $"{id} - {message}";
        }

        // 调用客户端方法启动任务
        // id: 任务id
        [HttpGet("taskstart/{id}")]
        public async Task<int> TaskStart(int id)
        {
            await _hubContext.Clients.All.SendAsync("TaskStart", id);
            return id;
        }

        // 调用客户端方法停止任务
        // id: 任务id
        [HttpGet("taskend/{id}")]
        public async Task<int> TaskEnd(int id)
        {
            await _hubContext.Clients.All.SendAsync("TaskEnd", id);
            return id;
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
