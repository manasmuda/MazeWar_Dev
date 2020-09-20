﻿using System;
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

		if (udpClient!=null && udpClient.Available != 0)
		{
			Debug.Log("Message Recieve Started");
			byte[] buffer = new byte[12800];
			udpClient.Receive(buffer);

			//string data = Encoding.Default.GetString(buffer);
			UdpMsgPacket msgPacket = NetworkProtocol.getPacketfromBytes(buffer);
			Debug.Log("Received: " + msgPacket.message);
			HandleUdpMessage(msgPacket);
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
			Debug.Log("Packet Sent:" + msgPacket.type);
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
		Debug.Log("Packet received:" + packet.type);
		if (packet.type == PacketType.GameState)
        {
			clientScript.HandleGameState(packet.gameState); 
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
		UdpMsgPacket packet = new UdpMsgPacket(PacketType.UDPConnect,"",msg.playerId,msg.team);
		Debug.Log("Team: " + packet.team);
		MyData.playerId = msg.playerId;
		MyData.team = msg.team;
		clientScript.HandlePlayerData();
		SendPacket(packet);
    }

	private void HandlePlayerAccepted(SimpleMessage msg)
    {
		Debug.Log("Player Accepted");
		SceneManager.LoadScene("GameScene");// Remove 
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
		//
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
		Debug.Log("Handle Server Tick");
		int ms = DateTime.UtcNow.Millisecond;
		int dif = ms - msg.time;
		int tt = dif / 200;
		int ttc = dif % 200;
		float ttcf = ((float)ttc) / 1000f;
		clientScript.tick = msg.intData+tt;
		clientScript.tickCounter = ttcf;
	}
}
