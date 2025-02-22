import psycopg2


pg_host = "172.20.65.42"
pg_username = "postgres"
pg_password = "postgres"

def pg_exec_query(database: str, query: str, params = None, autocommit = True, ignore_errors = False):
    conn = psycopg2.connect(host=pg_host, database=database, user=pg_username, password=pg_password)
    conn.autocommit = autocommit
    cur = conn.cursor()
    try:
        cur.execute(query, params)
        if not autocommit:
            conn.commit()
    except (Exception, psycopg2.Error) as error:
        if not autocommit:
            conn.rollback()
        if not ignore_errors:
            raise error
        print(error)
    finally:
        cur.close()
        conn.close()

def create_pg_db():
    pg_exec_query(database="postgres", query="CREATE DATABASE metabase;", ignore_errors=True)

def main():
    create_pg_db()
        
if __name__=="__main__":
    main()
    