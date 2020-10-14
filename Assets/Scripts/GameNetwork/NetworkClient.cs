 using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

// *** NETWORK CLIENT FOR TCP CONNECTIONS WITH THE SERVER ***

public class NetworkClient
{

	private TcpClient client = null;

	//private MatchStatusInfo matchStatusInfo = null;
	private PlayerSessionObject playerSessionObject;

	private bool connectionSucceeded = false;

	public bool ConnectionSucceeded() { return connectionSucceeded; }

	private AWSClient awsClient;

	private Socket udpClient;
	private IPEndPoint endPoint;

	private Client clientScript;

	private byte[] buffer=new byte[12400];

	public NetworkClient(Client client)
    {
		//awsClient = GameObject.Find("AWSClient").GetComponent<AWSClient>();
		this.clientScript = client;
	}

	// Calls the matchmaking client to do matchmaking against the backend and then connects to the game server with TCP
	public IEnumerator DoMatchMakingAndConnect(PlayerSessionObject x)
	{
		playerSessionObject = x;
		Debug.Log("Connecting to Server..");
        yield return null;

		Connect();

		yield return null;
	}

    // Called by the client to receive new messages
	public void Update()
	{
		if (client == null) return;
		var messages = NetworkProtocol.Receive(client);
        
		foreach (SimpleMessage msg in messages)
		{
			HandleMessage(msg);
		}
	}

	public void RecieveUdp()
    {
		if (udpClient != null && udpClient.Available != 0)
		{
			//Debug.Log("Message Recieve Started");
			byte[] buffer = new byte[12400];
			udpClient.Receive(buffer);

			//string data = Encoding.Default.GetString(buffer);
			UdpMsgPacket msgPacket = NetworkProtocol.getPacketfromBytes(buffer);
			//Debug.Log("Received: " + msgPacket.message);
			HandleUdpMessage(msgPacket);
		}
	}

	private void UDPRecieveCallBack(IAsyncResult result)
	{
		try
		{
			IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
			byte[] data =new byte[12400]; 
			int length=udpClient.EndReceive(result);
			

			Array.Copy(buffer, 0, data, 0, length);
			buffer = new byte[12400];
			udpClient.BeginReceive(buffer,0,12400,0,UDPRecieveCallBack, null);
			UdpMsgPacket msgPacket = NetworkProtocol.getPacketfromBytes(data);
			//string msg=Encoding.Default.GetString(data);
			Debug.Log("Recieved UDP msg:" + msgPacket.type);
			HandleUdpMessage(msgPacket);

			Debug.Log(msgPacket.message);

			//UdpMsgPacket msgPacket = new UdpMsgPacket(PacketType.Spawn, "Welcome to UDP");
			//SendPacket(msgPacket, clientEndPoint);

		}
		catch (Exception e)
		{
			Debug.Log(e);
		}
	}

	private bool TryConnect()
	{
		try
		{
			//Connect with matchmaking info
			Debug.Log("Connect..");
			client = new TcpClient(this.playerSessionObject.IpAddress, this.playerSessionObject.Port);
            client.NoDelay = true; // Use No Delay to send small messages immediately. UDP should be used for even faster messaging
			Debug.Log("Done");

			endPoint = new IPEndPoint(IPAddress.Parse(this.playerSessionObject.IpAddress), this.playerSessionObject.Port+20);
			udpClient = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			udpClient.Blocking = false;
			//udpClient.BeginReceive(buffer, 0, 12400, 0, UDPRecieveCallBack, null);
			Debug.Log("UdpClient Created");
			// Send the player session ID to server so it can validate the player
			SimpleMessage connectMessage = new SimpleMessage(MessageType.Connect, this.playerSessionObject.PlayerSessionId);
			this.SendMessage(connectMessage);

			return true;
		}
		catch (ArgumentNullException e)
		{
			Debug.Log(e.Message);
			Debug.Log(e.InnerException);
			Debug.Log(e.StackTrace);
			Debug.Log(e.ToString());
			Debug.Log(e);
			client = null;
			return false;
		}
		catch (SocketException e) // server not available
		{
			Debug.Log(e.Message);
			Debug.Log(e.InnerException);
			Debug.Log(e.StackTrace);
			Debug.Log(e.ToString());
			Debug.Log(e);
			client = null;
			return false;
		}
	}

	private void Connect()
	{
		// try to connect to a local server
		if (TryConnect() == false)
		{
			Debug.Log("Failed to connect to server");
			//GameObject.FindObjectOfType<UIManager>().SetTextBox("Connection to server failed.");
		}
		else
		{
			Debug.Log("Successfully connected to Server");
			//We're ready to play, let the server know
			this.Ready();
			//GameObject.FindObjectOfType<UIManager>().SetTextBox("Connected to server");
		}
	}

	// Send ready to play message to server
	public void Ready()
	{
		if (client == null) return;
		this.connectionSucceeded = true;

        // Send READY message to let server know we are ready
        SimpleMessage message = new SimpleMessage(MessageType.Ready);
		try
		{
			NetworkProtocol.Send(client, message);
		}
		catch (SocketException e)
		{
			HandleDisconnect();
		}
	}

    // Send serialized binary message to server
    public void SendMessage(SimpleMessage message)
    {
        if (client == null) return;
        try
        {
            NetworkProtocol.Send(client, message);
        }
        catch (SocketException e)
        {
			Debug.Log(e);
            HandleDisconnect();
        }
    }

	public void SendPacket(UdpMsgPacket msgPacket)
	{
		try
		{

			byte[] arr = NetworkProtocol.getPacketBytes(msgPacket);
			udpClient.SendTo(arr, endPoint);
			//Debug.Log("Packet Sent:" + msgPacket.type);
		}
		catch(Exception e)
        {
			Debug.Log(e);
        }
	}

	// Send disconnect message to server
	public void Disconnect()
	{
		if (client == null) return;
        SimpleMessage message = new SimpleMessage(MessageType.Disconnect);
		try
		{
			NetworkProtocol.Send(client, message);
		}

		finally
		{
			HandleDisconnect();
		}
	}

	// Handle a message received from the server
	private void HandleMessage(SimpleMessage msg)
	{
		// parse message and pass json string to relevant handler for deserialization
		Debug.Log("Message received:" + msg.messageType + ":" + msg.message);

		if (msg.messageType == MessageType.Reject)
			HandleReject();
		else if (msg.messageType == MessageType.Disconnect)
			HandleDisconnect();
		else if (msg.messageType == MessageType.PlayerAccepted)
			HandlePlayerAccepted(msg);
		else if (msg.messageType == MessageType.PlayerLeft)
			HandleOtherPlayerLeft(msg);
		else if (msg.messageType == MessageType.GameReady)
			HandleGameReady(msg);
		else if (msg.messageType == MessageType.GameStarted)
			HandleGameStarted(msg);
		else if (msg.messageType == MessageType.PlayerData)
			HandlePlayerData(msg);
		else if (msg.messageType == MessageType.PlayerGameData)
			HandlePlayerGameData(msg);
		else if (msg.messageType == MessageType.ServerTick)
			HandleServerTick(msg);
		else
		{
			Client.messagesToProcess.Add(msg);
		}
	}

	public void HandleUdpMessage(UdpMsgPacket packet)
    {
		//Debug.Log("Packet received:" + packet.type);
		if (packet.type == PacketType.GameState)
        {
			clientScript.HandleGameState(packet.gameState); 
        }
		else if (packet.type == PacketType.Shoot)
        {
			clientScript.HandleOtherShoot(packet);
        }
    }

	private void HandleReject()
	{
		NetworkStream stream = client.GetStream();
		stream.Close();
		client.Close();
		client = null;
	}

	private void HandleDisconnect()
	{
		Debug.Log("Got disconnected by server");
		NetworkStream stream = client.GetStream();
		stream.Close();
		client.Close();
		client = null;
	}

	private void HandlePlayerData(SimpleMessage msg)
	{
		Debug.Log("Recieved Player Data");
		UdpMsgPacket packet = new UdpMsgPacket(PacketType.UDPConnect, "", msg.playerId, msg.team);
		Debug.Log("Team: " + packet.team);
		MyData.playerId = msg.playerId;
		MyData.team = msg.team;
		string hg1 = msg.stringArrData[0];
		string hg2 = msg.stringArrData[1];
		string lg1 = msg.stringArrData[2];
		string lg2 = msg.stringArrData[3];
		Gadget lgadget1=null;
		Gadget lgadget2=null;
		Gadget hgadget1=null;
		Gadget hgadget2=null;
		Debug.Log(lg1);
		try
		{
			GameObject lg1object = Resources.Load<GameObject>("GadgetControllers/" + lg1 + "GadgetController");
			lg1object = GameObject.Instantiate(lg1object, Vector3.zero, Quaternion.identity);
			lgadget1 = lg1object.GetComponent<Gadget>();
			GameObject lg2object = Resources.Load<GameObject>("GadgetControllers/" + lg2 + "GadgetController");
			lg2object = GameObject.Instantiate(lg2object, Vector3.zero, Quaternion.identity);
			lgadget2 = lg2object.GetComponent<Gadget>();
			GameObject hg1object = Resources.Load<GameObject>("GadgetControllers/" + hg1 + "GadgetController");
			hg1object = GameObject.Instantiate(hg1object, Vector3.zero, Quaternion.identity);
			hgadget1 = hg1object.GetComponent<Gadget>();
			GameObject hg2object = Resources.Load<GameObject>("GadgetControllers/" + hg2 + "GadgetController");
			hg2object = GameObject.Instantiate(hg2object, Vector3.zero, Quaternion.identity);
			hgadget2 = hg2object.GetComponent<Gadget>();
		}
		catch (Exception e)
		{
			Debug.LogError(e);
			Debug.LogError(e.Message);
			Debug.LogError(e.InnerException);
		}
		MyData.LGadget1 = lgadget1;
		MyData.HGadget1 = hgadget1;
		MyData.LGadget2 = lgadget2;
		MyData.HGadget2 = hgadget2;
		MyData.gadgets = new Gadget[4]{hgadget1, hgadget2, lgadget1, lgadget2};
		clientScript.HandlePlayerData();
		SendPacket(packet);
    }

	private void HandlePlayerAccepted(SimpleMessage msg)
    {
		Debug.Log("Player Accepted");
		SceneManager.LoadScene("GameScene");// Remove 
		clientScript.pauseUpdate = true;
    }

	private void HandleOtherPlayerLeft(SimpleMessage message)
	{
		Debug.Log("Opponent player left, you won the game");
		if (client!=null && client.Connected)
		{
			NetworkStream stream = client.GetStream();
			stream.Close();
			client.Close();
		}
		client = null;
	}

	private void HandleGameReady(SimpleMessage msg)
    {
		Debug.Log("Game Ready");
		int ms = DateTime.UtcNow.Millisecond;
		int dif = ms - msg.time;
		int tr = dif / 1000;
		Debug.Log("Time Remaining " + tr);
		clientScript.GameReady(dif);
		MazeCell[,] maze=MazeConvertor.ToMazeArray(msg.listData);
		clientScript.SetUpMaze(maze);
		//instantiate coins
		clientScript.SetUpCoins(Converter.ToArray(msg.list1));
	}

	private void HandleGameStarted(SimpleMessage msg)
    {
		Debug.Log("Game Started");
		int ms = DateTime.UtcNow.Millisecond;
		int dif = ms - msg.time;
		int tt = dif / 200;
		int ttc = dif % 200;
		float ttcf = ((float)ttc) / 1000f;
		clientScript.tick = tt;
		clientScript.tickCounter = ttcf;
    }

	private void HandlePlayerGameData(SimpleMessage msg)
    {
		Debug.Log("Player Game Data Recieved");
        foreach(KeyValuePair<string,float[]> spawnData in msg.redSpanData)
        {
            if (MyData.playerId == spawnData.Key)
            {
				Vector3 pos = new Vector3(spawnData.Value[0], spawnData.Value[1], spawnData.Value[2]);
				clientScript.MyCharacterSpwan(pos);
			}
            else
            {
				Vector3 pos = new Vector3(spawnData.Value[0], spawnData.Value[1], spawnData.Value[2]);
				clientScript.OtherCharSpawn(spawnData.Key, "red", pos);
				//clientScript.CharSpawn();
			}
        }
		foreach (KeyValuePair<string, float[]> spawnData in msg.blueSpanData)
		{
			if (MyData.playerId == spawnData.Key)
			{
				Vector3 pos = new Vector3(spawnData.Value[0], spawnData.Value[1], spawnData.Value[2]);
				clientScript.MyCharacterSpwan(pos);
			}
            else
            {
				Vector3 pos = new Vector3(spawnData.Value[0], spawnData.Value[1], spawnData.Value[2]);
				clientScript.OtherCharSpawn(spawnData.Key, "blue", pos);
				//clientScript.CharSpawn();
			}
		}
    }

	private void HandleServerTick(SimpleMessage msg)
	{
		//Debug.Log("Handle Server Tick");
		int ms = DateTime.UtcNow.Millisecond;

		int dif = ms - msg.time;
		//Debug.Log(dif);
		if (dif > 0) { 
			int tt = dif / 200;
			int ttc = dif % 200;
			float ttcf = ((float)ttc) / 1000f;
			clientScript.tick = msg.intData + tt;
			clientScript.tickCounter = ttcf;
		}
        else
        {
			clientScript.tick = msg.intData;
		}
	}
}
