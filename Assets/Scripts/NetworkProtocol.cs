using System;
using System.IO;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

public class NetworkProtocol
{
    public static SimpleMessage[] Receive(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        var messages = new List<SimpleMessage>();
        while (stream.DataAvailable)
        {
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                SimpleMessage message = formatter.Deserialize(stream) as SimpleMessage;
                messages.Add(message);
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Error receiving a message: " + e.Message);
                System.Console.WriteLine("Aborting the rest of the messages");
                break;
            }
        }

        return messages.ToArray();
    }

    public static void Send(TcpClient client, SimpleMessage message)
    {
        if (client == null) return;
        NetworkStream stream = client.GetStream();
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, message);
    }

    public static byte[] getPacketBytes(UdpMsgPacket packet)
    {
        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        bf.Serialize(ms, packet);

        return ms.ToArray();
    }

    public static UdpMsgPacket getPacketfromBytes(byte[] bytes)
    {
        MemoryStream ms = new MemoryStream();
        BinaryFormatter bf = new BinaryFormatter();
        ms.Write(bytes, 0, bytes.Length);
        ms.Seek(0, SeekOrigin.Begin);
        UdpMsgPacket msgPacket = bf.Deserialize(ms) as UdpMsgPacket;
        return msgPacket;
    }
}