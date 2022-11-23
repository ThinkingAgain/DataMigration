import sys
import query_mysql_to_oracle as q


def run():
    task_id = sys.argv[1]
    q.execute_task_by_id(task_id)

if __name__ == "__main__":
    run()
