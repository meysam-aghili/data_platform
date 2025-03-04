from pyspark.sql import SparkSession


spark = SparkSession.builder \
    .appName('PySpark Demo') \
    .master('spark://spark:7077') \
    .getOrCreate()

flightData2015 = spark \
    .read \
    .option("inferSchema", "true") \
    .option("header", "true") \
    .csv("/data/flight-data/csv/2015-summary.csv")

spark.conf.set("spark.sql.shuffle.partitions", "5")

divisBy2 = flightData2015.where("number % 2 = 0")
flightData2015.sort("count").explain()
flightData2015.take(3)
flightData2015.createOrReplaceTempView("flight_data_2015")
sqlQuery = spark.sql("""
    SELECT DEST_COUNTRY_NAME, count(1)
    FROM flight_data_2015
    GROUP BY DEST_COUNTRY_NAME
    ORDER BY count(1) DESC
    LIMIT 5
    """)
sqlQuery.show()
