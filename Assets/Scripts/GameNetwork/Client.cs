using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.CognitoIdentity.Model;
using Amazon.Lambda;
using Amazon.Lambda.Model;
using UnityEngine.SceneManagement;

using Facebook.Unity;

// *** MAIN CLIENT CLASS FOR MANAGING CLIENT CONNECTIONS AND MESSAGES ***

public class Client : MonoBehaviour
{

    // Local player
    public NetworkClient networkClient;

    private bool loading;
    private bool gameStarted=false;

    private PlayerSessionObject playerSessionObj=new PlayerSessionObject();

    private bool connectionSuccess = false;

    //We get events back from the NetworkServer through this static list
    public static List<SimpleMessage> messagesToProcess = new List<SimpleMessage>();


    public int tick = 0;
    private float tickRate = 0.2f;
    public float tickCounter = 0f;

    private bool tickMode = false;

    private int prevTick = -1;

    //Cognito credentials for sending signed requests to the API
    //public static Amazon.Runtime.ImmutableCredentials cognitoCredentials = null;


    public static Client clientInstance = null;

    private AWSClient awsClient;    
    public string roomId;

    [SerializeField]
    private MazeController mazeController;

    [SerializeField]
    private GameObject character;


    public GameObject coinPrefab;

    [SerializeField]
    private GameObject mainCharPrefab;

    [SerializeField]
    private GameObject blueCharPrefab;
    [SerializeField]
    private GameObject redCharPrefab;

    [SerializeField]
    private GameObject playerPointer;

    private GameObject miniMapCamera;

    void Awake()
    {
        UnityInitializer.AttachToGameObject(this.gameObject);
        DontDestroyOnLoad(this.gameObject);
        if (clientInstance == null)
        {
            clientInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Called by Unity when the Gameobject is created
    void Start()
    {
        
        //awsClient = GameObject.Find("AWSClient").GetComponent<AWSClient>();
        // Set up Mobile SDK
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log(scene.buildIndex);
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void FetchGameAndPlayerSession(Dictionary<string,string> payLoad)
    {
        
        //StartCoroutine(ConnectToServer());
        AWSConfigs.AWSRegion = "ap-south-1"; // Your region here
        AWSConfigs.HttpClient = AWSConfigs.HttpClientOption.UnityWebRequest;
        // paste this in from the Amazon Cognito Identity Pool console
        CognitoAWSCredentials credentials = new CognitoAWSCredentials(
            "ap-south-1:633d62b0-bdef-4b2b-8b33-1e091db82f24", // Identity pool ID
            RegionEndpoint.APSouth1 // Region
        );

        string payLoadString = MyDictionaryToJson(payLoad);
        AmazonLambdaClient client = new AmazonLambdaClient(awsClient.GetCredentials(), RegionEndpoint.APSouth1);
        InvokeRequest request = new InvokeRequest
        {
            FunctionName = "TacticalCombatGetGS",
            InvocationType = InvocationType.RequestResponse,
            Payload=  payLoadString
        };

        loading = true;
        client.InvokeAsync(request,
            (response) =>
            {
                if (response.Exception == null)
                {
                    if (response.Response.StatusCode == 200)
                    {
                        var payload = Encoding.ASCII.GetString(response.Response.Payload.ToArray()) + "\n";
                        playerSessionObj = JsonUtility.FromJson<PlayerSessionObject>(payload);
                        Debug.Log(playerSessionObj.PlayerSessionId);
                        Debug.Log(playerSessionObj.IpAddress);
                        Debug.Log(playerSessionObj.Port);
                        Debug.Log(playerSessionObj);

                        if (playerSessionObj.PlayerSessionId == null)
                        {
                            Debug.Log($"Error in Lambda: {payload}");
                            loading=false;
                        }
                        else
                        {
                            roomId = playerSessionObj.RoomId;
                            StartCoroutine(ConnectToServer());
                            //QForMainThread(ActionConnectToServer, playerSessionObj.IpAddress, Int32.Parse(playerSessionObj.Port), playerSessionObj.PlayerSessionId);
                        }
                    }
                    else
                    {
                        loading = false;
                    }
                }
                else
                {
                    loading = false;
                    Debug.LogError(response.Exception);
                }
            });
    }

    public string MyDictionaryToJson(Dictionary<string, string> dict)
    {
        List<string> entries=new List<string> { };
        foreach(KeyValuePair<string,string> item in dict)
        {
            entries.Add(string.Format("\"{0}\":\"{1}\"", item.Key, item.Value));
        }
        return "{" + string.Join(",", entries) + "}";
    }

#if UNITY_ANDROID
	public void UsedOnlyForAOTCodeGeneration() {
		//Bug reported on github https://github.com/aws/aws-sdk-net/issues/477
		//IL2CPP restrictions: https://docs.unity3d.com/Manual/ScriptingRestrictions.html
		//Inspired workaround: https://docs.unity3d.com/ScriptReference/AndroidJavaObject.Get.html

		AndroidJavaObject jo = new AndroidJavaObject("android.os.Message");
		int valueString = jo.Get<int>("what");
        string stringValue = jo.Get<string>("what");
	}
#endif


    public void connectionCB(bool x)
    {
        if (!x)
        {
            connectionSuccess = false;
        }
    }

    public void ConnectWithPlayerId(string playerIdx)
    {
        playerSessionObj = new PlayerSessionObject();
        //#if UNITY_ANDROID
        //playerSessionObj.IpAddress = "10.0.2.2";
        //#endif
        //#if UNITY_PLAYER
        playerSessionObj.IpAddress = "192.168.43.205";// ip address if using lan
        //playerSessionObj.IpAddress = "127.0.0.1";
        //#endif
        playerSessionObj.Port = 1935;
        playerSessionObj.GameSessionId = "gsess-abc";
        playerSessionObj.PlayerSessionId = playerIdx;
        StartCoroutine(ConnectToServer());
        //SceneManager.LoadScene("Game");
    }


    // Update is called once per frame
    void Update()
    {
        //this.ProcessMessages();
        if (connectionSuccess)
        {
            // Only send updates 5 times per second to avoid flooding server with messages
            networkClient.RecieveUdp();
            this.tickCounter += Time.deltaTime;
            if (this.tickCounter < 0.2f)
            {
                return;
            }
            this.tickCounter = 0.0f;
            tick++;
            this.networkClient.Update();
            if (gameStarted)
            {
                ProcessMessages();
                CreateClientState();
            }
        }

    }

    IEnumerator ConnectToServer()
    {

        yield return null;

        this.networkClient = new NetworkClient(this);


        yield return StartCoroutine(this.networkClient.DoMatchMakingAndConnect(playerSessionObj));

        if (this.networkClient.ConnectionSucceeded())
        {
            this.connectionSuccess = true;
            //SimpleMessage message = new SimpleMessage(MessageType.Success, "Player Successfully connected");
            //this.networkClient.SendMessage(message);
        }

        yield return null;
    }

    // Process messages received from server
   void ProcessMessages()
    {
        // Go through any messages to process
        foreach (SimpleMessage msg in messagesToProcess)
        {
             
        }
        messagesToProcess.Clear();
    }

    public void CreateClientState()
    {
        ClientState clientState = new ClientState();
        clientState.tick = tick;
        clientState.playerId = MyData.playerId;
        clientState.team = MyData.team;
        clientState.position = new float[3]{character.transform.position.x, character.transform.position.y, character.transform.position.z};
        clientState.angle = new float[3] { character.transform.rotation.eulerAngles.x, character.transform.rotation.eulerAngles.y, character.transform.rotation.eulerAngles.z };
        clientState.health = Convert.ToInt32(character.GetComponent<CharacterData>().CurrentHealth);
        clientState.movementPressed = character.GetComponent<NewPlayer>().currentSpeed > 100f;
        UdpMsgPacket packet = new UdpMsgPacket(PacketType.ClientState, "", MyData.playerId, MyData.team);
        packet.clientState = clientState;
        networkClient.SendPacket(packet);
        Debug.Log("Client State Sent");
    }

    public void GameReady(int timeLeft)
    {
        StartCoroutine(StartLobby(timeLeft));
    }

    IEnumerator StartLobby(int timeLeft)
    {
        Debug.Log("Lobby Started");
        yield return new WaitForSeconds(10- timeLeft / 1000);

        tick = 0;
        gameStarted = true;
        Debug.Log("Lobby Ended");


    }

    public void HandleOpponentLeft()
    {
        Debug.Log("Opponent Left");
    }

    public void ResetClient()
    {
        connectionSuccess = false;
        networkClient.Disconnect();
        networkClient = null;
        playerSessionObj = new PlayerSessionObject();
        messagesToProcess.Clear();
        
        gameStarted = false;
        roomId = null;
    }

    public void HandleGameState(GameState state)
    {
        // Debug.Log("GameState handling started");
        if (state.tick > prevTick)
        {
            prevTick = state.tick;
            float tempDist = (tick - state.tick - 2) * 40 * 0.2f;
            try
            {
                if (MyTeamData.teamName == "blue")
                {
                    for (int i = 0; i < state.blueTeamState.Count; i++)
                    {
                        string id = state.blueTeamState[i].playerId;
                        if (MyTeamData.playerData.ContainsKey(id))
                        {
                            if (state.blueTeamState[i].movementPressed && tempDist > 4)
                            {
                                float ay = state.blueTeamState[i].angle[1];
                                state.blueTeamState[i].position[0] = state.blueTeamState[i].position[0] + tempDist * Mathf.Cos(ay);
                                state.blueTeamState[i].position[2] = state.blueTeamState[i].position[2] + tempDist * Mathf.Sin(ay);
                            }
                            GameObject playerObject = MyTeamData.playerData[id];
                            playerObject.GetComponent<CharacterSyncScript>().NewPlayerState(state.blueTeamState[i]);
                            playerObject.GetComponent<CharacterData>().NewPlayerState(state.blueTeamState[i]);
                        }
                        else
                        {

                        }
                    }
                    for (int i = 0; i < state.redTeamState.Count; i++)
                    {
                        string id = state.redTeamState[i].playerId;
                        if (state.redTeamState[i].movementPressed && tempDist > 4)
                        {
                            float ay = state.redTeamState[i].angle[1];
                            state.redTeamState[i].position[0] = state.redTeamState[i].position[0] + tempDist * Mathf.Cos(ay);
                            state.redTeamState[i].position[2] = state.redTeamState[i].position[2] + tempDist * Mathf.Sin(ay);
                        }
                        GameObject playerObject = OppTeamData.playerData[id];
                        playerObject.GetComponent<CharacterSyncScript>().NewPlayerState(state.redTeamState[i]);
                        playerObject.GetComponent<CharacterData>().NewPlayerState(state.redTeamState[i]);
                    }
                }
                else
                {
                    for (int i = 0; i < state.redTeamState.Count; i++)
                    {
                        string id = state.redTeamState[i].playerId;
                        if (MyTeamData.playerData.ContainsKey(id))
                        {
                            if (state.redTeamState[i].movementPressed && tempDist > 0)
                            {
                                float ay = state.redTeamState[i].angle[1];
                                state.redTeamState[i].position[0] = state.redTeamState[i].position[0] + tempDist * Mathf.Cos(ay);
                                state.redTeamState[i].position[2] = state.redTeamState[i].position[2] + tempDist * Mathf.Sin(ay);
                            }
                            GameObject playerObject = MyTeamData.playerData[id];
                            playerObject.GetComponent<CharacterSyncScript>().NewPlayerState(state.redTeamState[i]);
                            playerObject.GetComponent<CharacterData>().NewPlayerState(state.redTeamState[i]);
                        }
                        else
                        {

                        }
                    }
                    for (int i = 0; i < state.blueTeamState.Count; i++)
                    {
                        string id = state.blueTeamState[i].playerId;
                        if (state.blueTeamState[i].movementPressed && tempDist > 0)
                        {
                            float ay = state.blueTeamState[i].angle[1];
                            state.blueTeamState[i].position[0] = state.blueTeamState[i].position[0] + tempDist * Mathf.Cos(ay);
                            state.blueTeamState[i].position[2] = state.blueTeamState[i].position[2] + tempDist * Mathf.Sin(ay);
                        }
                        GameObject playerObject = OppTeamData.playerData[id];
                        playerObject.GetComponent<CharacterSyncScript>().NewPlayerState(state.blueTeamState[i]);
                        playerObject.GetComponent<CharacterData>().NewPlayerState(state.blueTeamState[i]);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
            Debug.Log("Gamestate handling ended");
        }
    }

    public void ShootAction(ClientState state)
    {
        UdpMsgPacket packet = new UdpMsgPacket(PacketType.Shoot,"",MyData.playerId,MyData.team);
        packet.clientState = state;
        networkClient.SendPacket(packet);
    }

    //Remove Coroutines for functions below this after detailed matchmaking
    public void SetUpMaze(MazeCell[,] maze)
    {
        StartCoroutine(SetMaze(maze));
    }

    IEnumerator SetMaze(MazeCell[,] maze)
    {
        yield return new WaitForSeconds(0.2f);

        miniMapCamera = GameObject.Find("MiniMapCamera");
        GameObject MCO = GameObject.Find("Maze");
        Debug.Log(MCO);
        mazeController = MCO.GetComponent<MazeController>();
        Debug.Log(mazeController);
        mazeController.InstantiateMaze(maze);
        MCO.GetComponent<MeshCombiner>().enabled = true;
    }

    public void SetUpCoins(Vector3[] _coinPos)
    {
        StartCoroutine(SetCoins(_coinPos));
    }

    IEnumerator SetCoins(Vector3[] _coinPos)
    {
        yield return new WaitForSeconds(0.5f);

        GameObject coinParent = new GameObject("CoinsParent");

        for(int i =0; i< _coinPos.Length;i++)
        {
            Instantiate(coinPrefab, _coinPos[i], Quaternion.identity, coinParent.transform);
        }

    }


    public void HandlePlayerData()
    {
        MyTeamData.teamName = MyData.team;
        if (MyData.team == "red")
        {
            MyTeamData.charPrefab = redCharPrefab;
            OppTeamData.charPrefab = blueCharPrefab;
            Quaternion q1 = Quaternion.identity;
            q1.eulerAngles = new Vector3(0, 180f, 0);
            MyTeamData.spwanDirection = q1;
            Quaternion q2 = Quaternion.identity;
            q2.eulerAngles = new Vector3(0, 0, 0);
            OppTeamData.spwanDirection = q2;
        }
        else
        {
            MyTeamData.charPrefab = blueCharPrefab;
            OppTeamData.charPrefab = redCharPrefab;
            Quaternion q1 = Quaternion.identity;
            q1.eulerAngles = new Vector3(0, 0, 0);
            MyTeamData.spwanDirection = q1;
            Quaternion q2 = Quaternion.identity;
            q2.eulerAngles = new Vector3(0, 180f, 0);
            OppTeamData.spwanDirection = q2;
        }
    }

    public void MyCharacterSpwan(Vector3 pos)
    {
        StartCoroutine(MyCharSpawn(pos));
    }

    IEnumerator MyCharSpawn(Vector3 pos)
    {
        yield return new WaitForSeconds(0.2f);

        character = Instantiate(mainCharPrefab, pos, MyTeamData.spwanDirection);
        playerPointer = Instantiate(playerPointer, new Vector3(pos.x, 10f, pos.z),MyTeamData.spwanDirection);
        MiniMapCamera mmc = miniMapCamera.GetComponent<MiniMapCamera>();
        mmc.pointer = playerPointer;
        mmc.player = character.transform;
        playerPointer.GetComponent<PointerScript>().player = character.transform;
        playerPointer.GetComponent<PointerScript>().minimapCamera = miniMapCamera.transform;
        miniMapCamera.GetComponent<MinimapFollowPlayer>().player = character.transform;
        GameObject.Find("TempCamera").SetActive(false);
    }

    public void OtherCharSpawn(string id,string team,Vector3 pos)
    {
         StartCoroutine(CharSpawn(id,team,pos));
    }

    IEnumerator CharSpawn(string id,string team,Vector3 pos)
    {
        yield return new WaitForSeconds(0.2f);
        if (team == MyTeamData.teamName)
        {
            GameObject tempObject = Instantiate(MyTeamData.charPrefab, pos, MyTeamData.spwanDirection);
            tempObject.GetComponent<CharacterData>().id = id;
            MyTeamData.playerData.Add(id, tempObject);
            miniMapCamera.GetComponent<MiniMapCamera>().AddPlayer(tempObject);
        }
        else
        {
            GameObject tempObject = Instantiate(OppTeamData.charPrefab, pos, OppTeamData.spwanDirection);
            tempObject.GetComponent<CharacterData>().id = id;
            OppTeamData.playerData.Add(id, tempObject);
        }
    }
    // End Function changes
}