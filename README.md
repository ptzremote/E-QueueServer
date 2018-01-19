# E-Queue Server
This is the electronic queue server implementation.
E-QueueServer is .NET Core application that handles clients, such as a terminal, operator app or board for showing the queue state.

# Dependency
- **CoreNetLib**

The server communicates with clients over TCP using my CoreNetLib wrapper for TcpListener and TcpClient.
[GitHub](https://github.com/ptzremoute/CoreNetLib)
[NuGet](https://www.nuget.org/packages/CoreNetLib/)

- **Netonsoft.Json**

Serializer for CoreNetLib. Use on both server and clients. But BinaryFormatter using by default in CoreNetLib.

- **EntityFramework.Core**

SQLite using by default. But you can implement IDb interface and use MS SQL for example.

- **NLog.Extensions.Logging**

CoreNetLogging class fron CoreNetLib is a wrapper for Microsoft.Extensions.Logging.
Nlog using in this implementation but you can use that you want.
