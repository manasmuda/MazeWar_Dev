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

    //Cognito credentials for sending signed requests to the API
    //public static Amazon.Runtime.ImmutableCredentials cognitoCredentials = null;


    public static Client clientInstance = null;

    private AWSClient awsClient;    public string roomId;

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
        playerSessionObj.IpAddress = "127.0.0.1";
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

    public void GameReady(int timeLeft)
    {
        StartCoroutine(StartLobby(timeLeft));
       
    }

    IEnumerator StartLobby(int timeLeft)
    {
        Debug.Log("Lobby Started");
        yield return new WaitForSeconds(timeLeft / 1000);

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
}