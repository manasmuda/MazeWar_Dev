using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NTest : MonoBehaviour
{
    [SerializeField]
    private InputField playerId;
    [SerializeField]
    private Button connectButton;
    [SerializeField]
    private InputField msg;
    [SerializeField]
    private Button msgButton;
    [SerializeField]
    private Button msgText;

    private Client client;
    // Start is called before the first frame update
    void Start()
    {
        connectButton.onClick.AddListener(Connect);
        msgButton.onClick.AddListener(SendMsg);
        client = GameObject.Find("Client").GetComponent<Client>();
    }

    void Connect()
    {
        client.ConnectWithPlayerId(playerId.text);
    }

    void SendMsg()
    {
        UdpMsgPacket msgPacket = new UdpMsgPacket(PacketType.Spawn, msg.text,"","");
        client.networkClient.SendPacket(msgPacket);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
