using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;

namespace MigDashboard.Hubs
{
    public class TaskHub : Hub
    {
        public string? PythonScript { get;  }  // python脚本
        public TaskHub(IConfiguration configuration) : base()
        {
            PythonScript = configuration.GetSection("DBScript").Value;
        }
        public async Task StartTask(int taskId)
        {
            //await Clients.All.SendAsync("TaskStart", taskId);
            Process.Start("python", $"{PythonScript} {taskId}");
            /*var message = "2022-11-21 06:18:33,253 - [line:17] - INFO: 开始生成sdlc.tf_so_order_zy的查询数据。。。。";
            message = $"{PythonScript} {taskId}";
            await Clients.All.SendAsync("TaskMessage", taskId, message);*/
            //await Clients.All.SendAsync("TaskEnd", taskId);
        }
    }
}
