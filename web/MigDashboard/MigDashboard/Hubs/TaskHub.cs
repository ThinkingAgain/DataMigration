using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;

namespace MigDashboard.Hubs
{
    public class TaskHub : Hub
    {
        public string? PythonScript { get;  }  // python脚本
        public string? PonScript { get; }  // Pon文件处理脚本
        private readonly IConfiguration _config;
        public TaskHub(IConfiguration configuration) : base()
        {
            PythonScript = configuration.GetSection("DBScript").Value;
            PonScript = configuration["CMDC:PonProcessor:Script"];
            _config = configuration;
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

        /// <summary>
        /// 执行Pon文件处理
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public async Task PonProcessTask(string taskId)            
        {
            Process.Start("python", $"{PonScript} {taskId}");            
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="confPath">配置文件中的字串, 指示是哪个目录</param>
        /// <returns></returns>
        public async Task DeleteFile(string fileName, string confPath)
        {
            try
            {
                File.Delete(Path.Combine(_config[confPath], fileName));
            }
            catch (Exception)
            {
               
            }            
            await Clients.Caller.SendAsync("RefreshPage");
        }


    }
}
