from pyspark.sql import SparkSession
from pyspark import SparkConf, SparkContext
from pyspark.sql.functions import window, column, desc, col, lit
from pyspark.sql.types import StructField, StructType, StringType, LongType
import collections


spark = SparkSession.builder \
    .appName('PySpark Demo') \
    .master('spark://spark:7077') \
    .getOrCreate()

flightData2015 = spark \
    .read \
    .format("csv") \
    .option("inferSchema", "true") \
    .option("header", "true") \
    .load("/data/flight-data/csv/2015-summary.csv")
spark.conf.set("spark.sql.shuffle.partitions", "5")
divisBy2 = flightData2015.where("number % 2 = 0")
flightData2015.withColumn("numberOne", lit(1))
flightData2015.withColumnRenamed("numberOne", "numberOneone")
flightData2015.sort("count").explain()
flightData2015.take(3)
flightData2015.createOrReplaceTempView("flight_data_2015")
staticSchema = flightData2015.schema
flightData2015.printSchema()
sqlQuery = spark.sql("""
    SELECT DEST_COUNTRY_NAME, count(1)
    FROM flight_data_2015
    GROUP BY DEST_COUNTRY_NAME
    ORDER BY count(1) DESC
    LIMIT 5
    """)
sqlQuery.show(2)

myManualSchema = StructType([
    StructField("DEST_COUNTRY_NAME", StringType(), True), # name , type, nullable
    StructField("ORIGIN_COUNTRY_NAME", StringType(), True),
    StructField("count", LongType(), False, metadata={"hello":"world"})
    ])
df = spark.read.format("json").schema(myManualSchema) \
.load("/data/flight-data/json/2015-summary.json")

conf = SparkConf().setMaster("spark-master").setAppName("demo test")
sc = SparkContext(conf=conf)
lines = sc.textFile("file:///SparkCourse/ml-100k/u.data")
ratings = lines.map(lambda x: x.split()[2])
result = ratings.countByValue()
sortedResult = collections.OrderedDict(sorted(result.items()))
for key, value in sortedResult.iteritems():
    print(key, " ", value)







def square(x):
    return x*x

spark.udf.register("square", square, Integertype())
df = spark.sql("select square(2) as a")


df.select(coalesce(col("Description"), col("CustomerId"))).show()6
df.repartition(5, col("DEST_COUNTRY_NAME")).coalesce(2)

################ Streaming

streamingDataFrame = spark.readStream \
    .schema(staticSchema) \
    .format("csv") \
    .option("header", "true") \
    .load("/data/retail-data/by-day/*.csv")

purchaseByCustomerPerHour = streamingDataFrame \
    .selectExpr(
    "CustomerId",
    "(UnitPrice * Quantity) as total_cost",
    "InvoiceDate") \
    .groupBy(
    col("CustomerId"), window(col("InvoiceDate"), "1 day")) \
    .sum("total_cost")

purchaseByCustomerPerHour.writeStream \
    .format("memory") \
    .queryName("customer_purchases") \
    .outputMode("complete") \
    .start()
# format: memory, console

spark.sql("""
    SELECT *
    FROM customer_purchases
    ORDER BY `sum(total_cost)` DESC
    """) \
    .show(5)


################ MLib
from pyspark.ml.feature import StringIndexer, OneHotEncoder, VectorAssembler
from pyspark.ml import Pipeline
from pyspark.ml.clustering import KMeans


trainDataFrame = spark.sql("""
    SELECT UnitPrice, Quantity, day_of_week
    FROM orders
    """)


indexer = StringIndexer() \
    .setInputCol("day_of_week") \
    .setOutputCol("day_of_week_index")

encoder = OneHotEncoder()\
    .setInputCol("day_of_week_index")\
    .setOutputCol("day_of_week_encoded")

vectorAssembler = VectorAssembler() \
    .setInputCols(["UnitPrice", "Quantity", "day_of_week_encoded"]) \
    .setOutputCol("features")

transformationPipeline = Pipeline() \
    .setStages([indexer, encoder, vectorAssembler])

fittedPipeline = transformationPipeline.fit(trainDataFrame)
transformedTraining = fittedPipeline.transform(trainDataFrame)
transformedTraining.cache()


kmeans = KMeans() \
    .setK(20) \
    .setSeed(1)
kmModel = kmeans.fit(transformedTraining)
kmModel.computeCost(transformedTraining)
testDataFrame = spark.sql("""
    SELECT UnitPrice, Quantity, day_of_week
    FROM orders_2
    """)
transformedTest = fittedPipeline.transform(testDataFrame)
kmModel.computeCost(transformedTest)



##########
from pyspark.sql import Row


spark.sparkContext.parallelize([Row(1), Row(2), Row(3)]).toDF()