import time
import requests

HOST = "http://10.74.178.2:5000"
# 部署时去掉以下语句的注释-------------
#HOST = "http://134.48.152.8:3040"


def send_task_message(id, message):
    """将任务执行信息发送给SignalR Hub"""
    format_message = f"{time.strftime('%Y-%m-%d %H:%M:%S', time.localtime())} - [ID:{id}] - INFO: {message}"
    requests.get(f"{HOST}/api/hubs/taskmessage/{id}/{format_message}")


def send_task_startmessage(id, message):
    """发送任务启动消息, 主要用于前端作一些预处理"""
    requests.get(f"{HOST}/api/hubs/taskstart/{id}")
    send_task_message(id, message)


def send_task_endmessage(id, message):
    """发送任务结束消息, 主要用于前端作一些清理"""
    requests.get(f"{HOST}/api/hubs/taskend/{id}")
    send_task_message(id, message)


