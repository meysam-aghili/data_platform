exec master..xp_cmdSHELL 'mkdir F:\SQLData\*|db|*'
exec master..xp_cmdSHELL 'mkdir F:\SQLIndex\*|db|*'
exec master..xp_cmdSHELL 'mkdir F:\SQLLog\*|db|*'

CREATE DATABASE [*|db|*] IF NOT EXISTS ON PRIMARY 
( NAME = N'*|db|*', FILENAME = N'F:\SQLData\*|db|*\*|db|*.mdf' , SIZE = 29724672KB , FILEGROWTH = 65536KB ), FILEGROUP [FG_Index] 
( NAME = N'*|db|*_Index', FILENAME = N'F:\SQLIndex\*|db|*\*|db|*_Index.ndf' , SIZE = 5120000KB , FILEGROWTH = 65536KB ), FILEGROUP [FG_Regular] 
( NAME = N'*|db|*_Regular', FILENAME = N'F:\SQLData\*|db|*\*|db|*_Regular.ndf' , SIZE = 10240000KB , FILEGROWTH = 65536KB) LOG ON 
( NAME = N'*|db|*_log', FILENAME = N'E:\SQLLog\*|db|*\*|db|*_log.ldf' , SIZE = 1024000KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB ) 
