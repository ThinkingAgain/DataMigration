# 集成定单查询
import pymysql
import cx_Oracle
import datetime
import logging
from hubs import *

def init():
    logging.basicConfig(level=logging.DEBUG,  # 控制台打印的日志级别
                        filename='query1.log',
                        filemode='a',  ##模式，有w和a，w就是写模式，每次都会重新写日志，覆盖之前的日志
                        # a是追加模式，默认如果不写的话，就是追加模式
                        format='%(asctime)s - [line:%(lineno)d] - %(levelname)s: %(message)s')

def print_log(message):
    print(message)
    logging.info(message)

def load_config() -> list:
    config = []
    # 连接字符串格式：[用户名]/[密码]@//[主机名]:[端口]/[DB 服务名]
    #conn = cx_Oracle.connect("dev/asdfwj@//localhost:1521/XEPDB1")
    conn = pymysql.connect(host='localhost', user='root', passwd='asdfwj', db='query', port=3306,
                           charset='utf8')
    sql = "select * from query_config"
    cur = conn.cursor()
    cur.execute(sql)
    res = cur.fetchall()
    for x in res:
        config.append(
            dict(
                zip(
                    [k[0] for k in cur.description],
                    [v for v in x]
                )
            )
        )
    return config

def get_from_mysql(dbname: str, sql: str) -> list:
    data = []
    try:
        db = pymysql.connect(host='134.32.161.3', user='shenhj15', passwd='sd@weihy7', db=dbname, port=8072,
                             charset='utf8')
        # 检验数据库是否连接成功
        cursor = db.cursor()
        records = cursor.execute(sql)
        if not records:
            print_log("Not get records!")
            return []
        #
        print_log(f"共查询到 {records} 条记录")
        all = cursor.fetchall()
        for x in all:
           data.append(
               dict(
                   zip(
                       [k[0] for k in cursor.description],
                       [v for v in x]
                   )
               )
           )
           #print(x)

    except pymysql.Error as e:
        print_log(e)
        print_log('操作数据库失败')
        return []
    finally:
        # 如果连接成功就要关闭数据库
        if db:
            db.close()
    return data

def trans_value(v):
    if v == None:
        return 'null'
    if isinstance(v, datetime.datetime):
        d = v.strftime("%Y-%m-%d %H:%M:%S")
        return f"to_date('{d}', 'YYYY-MM-DD HH24:MI:SS')"
    if "'" in v:
        v = v.replace("'", "")
    return f"'{v}'"

def gen_sqls(data: list, table: str)->list:
    print_log("正在生成sql语句。。。")
    sqls = []
    for x in data:
        ks = []
        vs = []
        for k, v in x.items():
            ks.append(k)
            vs.append(trans_value(v))
        fields = ",".join(ks)
        values = ",".join(vs)
        sqls.append(f"INSERT INTO {table}({fields}) VALUES({values})")
    return sqls

def exec_oraclesqls(sqls: list):
    try:
        tns = cx_Oracle.makedsn('134.32.44.7', 1635, 'sdlc')
        db = cx_Oracle.connect('wujun33', 'wujun_123', tns)
        cursor = db.cursor()
        for sql in sqls:
            #print(sql)
            cursor.execute(sql)
        cursor.close()
        db.commit()
    except cx_Oracle.Error as e:
        print_log(e)
    finally:
        db.close()


# 以下是为执行单个任务增加的方法=================================================================================
def get_config_by_id(task_id) -> dict:
    """获取指定 id 的任务数据"""
    conn = pymysql.connect(host='localhost', user='root', passwd='asdfwj', db='query', port=3306,
                           charset='utf8')
    sql = f"select * from query_config where id={task_id}"
    cur = conn.cursor()
    cur.execute(sql)
    res = cur.fetchall()
    for x in res:
        task_data = dict(
                zip(
                    [k[0] for k in cur.description],
                    [v for v in x]
                )
            )

    return task_data


def execute_task_by_id(task_id):
    q = get_config_by_id(task_id)
    if not q:
        send_task_message(task_id, f"ID为{task_id}的任务数据加载失败!")
        return
    send_task_startmessage(task_id, "任务数据载入成功, 开始执行。。。")
    mdb = q["mysql_db"]
    msql = q["MYSQL_SQL"]
    otable = q["oracle_table"]
    send_task_message(task_id, f"删除{otable}的历史数据。。。")


    # 部署时去掉以下语句的注释-------------------------------------------------
    if not exec_oraclesqls_of_hub(task_id, [f"truncate table {otable}"]):
        send_task_endmessage(task_id, "出现异常, 终止任务!")
        return

    send_task_message(task_id, f"开始生成{otable}的查询数据。。。。")

    # 部署时去掉以下语句的注释-------------------------------------------------
    if not exec_oraclesqls_of_hub(task_id, gen_sqls(get_from_mysql_of_hub(task_id, mdb, msql), otable)):
        send_task_endmessage(task_id, "出现异常, 终止任务!")
        return

    send_task_message(task_id, f"{otable}更新完成。。。")

    send_task_endmessage(task_id, f"任务执行完成========")


def exec_oraclesqls_of_hub(id, sqls: list):
    """Hub启动的数据库操作, 主要是异常处理推送消息至Hub"""
    if not sqls: # sqls中没有语句, 视为异常
        return False
    try:
        tns = cx_Oracle.makedsn('134.32.44.7', 1635, 'sdlc')
        db = cx_Oracle.connect('wujun33', 'wujun_123', tns)
        cursor = db.cursor()
        for sql in sqls:
            #print(sql)
            cursor.execute(sql)
        cursor.close()
        db.commit()
    except cx_Oracle.Error as e:
        send_task_message(id, e)
        return False
    finally:
        db.close()
    return True


def get_from_mysql_of_hub(id, dbname: str, sql: str) -> list:
    """Hub启动的数据库操作, 主要是异常处理推送消息至Hub"""
    data = []
    db = None
    try:
        db = pymysql.connect(host='134.32.161.3', user='shenhj15', passwd='sd@weihy7', db=dbname, port=8072,
                             charset='utf8')
        # 检验数据库是否连接成功
        cursor = db.cursor()
        records = cursor.execute(sql)
        if not records:
            print_log("Not get records!")
            return []
        #
        print_log(f"共查询到 {records} 条记录")
        all = cursor.fetchall()
        for x in all:
           data.append(
               dict(
                   zip(
                       [k[0] for k in cursor.description],
                       [v for v in x]
                   )
               )
           )
           #print(x)

    except pymysql.Error as e:
        send_task_message(id, e)
        return []
    finally:
        # 如果连接成功就要关闭数据库
        if db:
            db.close()
    return data




if __name__ == "__main__":
    init()
    print_log("////////////////////////////////////////////////////////////")    
    config = load_config()
    if (not config):
        logging.info("加载配置数据失败!")


    for q in config:
        mdb = q["mysql_db"]
        msql = q["MYSQL_SQL"]
        otable = q["oracle_table"]
        print_log(f"删除{otable}的历史数据。。。")
        exec_oraclesqls([f"truncate table {otable}"])
        print_log(f"开始生成{otable}的查询数据。。。。")
      #  print(gen_sqls(get_from_mysql(mdb, msql), otable))
        exec_oraclesqls(gen_sqls(get_from_mysql(mdb, msql), otable))
        print_log(f"{otable}更新完成========")
    print_log("***********************************************")



