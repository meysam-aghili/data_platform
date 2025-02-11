from minio import Minio
import json

minio_host = "172.20.65.42:10000"
minio_username = "minioadmin"
minio_password = "minioadmin"
bucket_name = "clickhouse"
client = Minio(endpoint=minio_host, access_key=minio_username, secret_key=minio_password, secure=False)

if not client.bucket_exists(bucket_name):
	client.make_bucket(bucket_name)

policy = {
    "Version": "2012-10-17",
    "Statement": [
        {
            "Effect": "Allow",
            "Principal": {"AWS": "*"},
            "Action": ["s3:GetBucketLocation",],
            "Resource": f"arn:aws:s3:::{bucket_name}"
        },
        {
            "Effect": "Allow",
            "Principal": {"AWS": "*"},
            "Action":"s3:GetObject",
            "Resource": f"arn:aws:s3:::{bucket_name}/*"
        }
    ]
}
client.set_bucket_policy(bucket_name, json.dumps(policy))

print(client.list_buckets())