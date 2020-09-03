using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System;
using System.Threading;
using Amazon;
using Amazon.Runtime;
using Amazon.CognitoIdentity;
using Amazon.CognitoIdentity.Model;
using Amazon.CognitoSync;
using Amazon.CognitoSync.SyncManager;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using UnityEngine.SceneManagement;
using Facebook.Unity;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.Multiplayer;
using UnityEngine.SocialPlatforms;

public class AWSClient : MonoBehaviour
{
    private Dataset playerInfo;
    private CognitoSyncManager syncManager;
    private CognitoAWSCredentials awsCredentials;
    private CognitoAWSCredentials dbUsersCredentials;
    private DynamoDBContext dbContext;
    private AmazonDynamoDBClient dbClient;

    private bool sync = false;
    private bool loginPage = true;

    public bool loading = true;

    private int fbandcsInitiated = 0;

    private static AWSClient awsClientInstance = null;

    private int currentTempStrategy = -1;
    private Dictionary<string, int> currentStrategy;

    public delegate void PictureCallBack(Texture2D texture);
    public PictureCallBack pcb;

    public Texture2D fbTexture;
    public Texture2D googleTexture;

    public bool fbauth = false;
    public bool gauth = false;

    public bool firstTime = false;

    void Awake()
    {
        loading = true;
        sync = false;
        loginPage = true;
        fbandcsInitiated = 0;
        UnityInitializer.AttachToGameObject(this.gameObject);
        DontDestroyOnLoad(this.gameObject);

        if (awsClientInstance == null)
        {
            awsClientInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (!FB.IsInitialized)
        {
            FB.Init(FbInitCallBack);
        }
        else
        {
            fbandcsInitiated = fbandcsInitiated + 1;
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Login Client Started");

        AWSConfigs.HttpClient = AWSConfigs.HttpClientOption.UnityWebRequest;
        awsCredentials = new CognitoAWSCredentials(
             "ap-south-1:bbe820ac-fd38-4a12-9fd4-349c1425a57e", // Identity pool ID
              RegionEndpoint.APSouth1 // Region
        );
        syncManager = new CognitoSyncManager(awsCredentials, RegionEndpoint.APSouth1);
        dbClient = new AmazonDynamoDBClient(awsCredentials, RegionEndpoint.APSouth1);
        dbContext = new DynamoDBContext(dbClient);
        playerInfo = syncManager.OpenOrCreateDataset("playerInfo");
        playerInfo.OnSyncSuccess += SyncSuccessCallBack;
        playerInfo.OnSyncFailure += HandleSyncFailure;
        fbandcsInitiated = fbandcsInitiated + 1;
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().RequestEmail().RequestIdToken().RequestServerAuthCode(false).Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
        //Social.localUser.Authenticate(GoogleLoginCallback);
    }

    public CognitoAWSCredentials GetCredentials()
    {
        return awsCredentials;
    }

    void FbInitCallBack()
    {
        FB.ActivateApp();
        Debug.Log("FB has been initialized");
        fbandcsInitiated = fbandcsInitiated + 1;
    }

    public void FaceBookLogin()
    {
        if (!FB.IsLoggedIn)
        {
            FB.LogInWithReadPermissions(new List<string> { "public_profile", "email" }, FbLoginCallBack);
        }
    }

    void FbLoginCallBack(ILoginResult result)
    {
        if (result.Error == null)
        {
            Debug.Log("Facebook login successfull");
            fbauth = true;
            gauth = false;
            string uid = AccessToken.CurrentAccessToken.UserId;
            Debug.Log("uid is null");
            if (playerInfo != null && !string.IsNullOrEmpty(playerInfo.Get("uid")) && !uid.Equals(playerInfo.Get("uid")))
            {
                awsCredentials.Clear();
                playerInfo.Delete();
            }
            Debug.Log("aws credentials is null");
            awsCredentials.AddLogin("graph.facebook.com", AccessToken.CurrentAccessToken.TokenString);
            Debug.Log("playerInfo is null");
            playerInfo.SynchronizeOnConnectivity();
            loading = true;
        }
        else
        {
            Debug.Log("Facebook Login unsuccessfully:" + result.Error);
        }
    }

    public void GoogleLogin()
    {
        Social.localUser.Authenticate((bool success) =>
        {
            if (success)
            {
                Debug.Log("Google login successfull");
                ((GooglePlayGames.PlayGamesPlatform)Social.Active).SetGravityForPopups(Gravity.BOTTOM);
                fbauth = false;
                gauth = true;
                string uid = Social.localUser.id;
                Debug.Log("uid is null");
                Debug.Log("mPlatform.GetIdToken " + ((PlayGamesLocalUser)Social.localUser).mPlatform.GetIdToken());
                Debug.Log("Social.localUser...GetIdToken " + ((PlayGamesLocalUser)Social.localUser).GetIdToken());
                Debug.Log("PlayGamesPlatform...GetServerAuthCode " + PlayGamesPlatform.Instance.GetServerAuthCode());
                Debug.Log("PlayGamesPlatform...GetIdToken " + PlayGamesPlatform.Instance.GetIdToken());
                if (playerInfo != null && !string.IsNullOrEmpty(playerInfo.Get("uid")) && !uid.Equals(playerInfo.Get("uid")))
                {
                    awsCredentials.Clear();
                    playerInfo.Delete();
                }
                Debug.Log("aws credentials is null");
                string token = PlayGamesPlatform.Instance.GetIdToken();
                awsCredentials.AddLogin("accounts.google.com", token);
                Debug.Log(token);
                Debug.Log("playerInfo is null");
                playerInfo.SynchronizeOnConnectivity();
                loading = true;
            }
            else
            {
                Debug.Log("Google Login unsuccessfully");
            }
        });
    }

    private void HandleSyncFailure(object sender, SyncFailureEventArgs e)
    {
        Dataset dataset = sender as Dataset;
        if (dataset.Metadata != null)
        {
            Debug.Log("Sync failed for dataset : " + dataset.Metadata.DatasetName);
        }
        else
        {
            Debug.Log("Sync failed");
        }
        // Handle the error
        Debug.Log(e.Exception);
        Debug.Log(e.Exception.Message);
        Debug.Log(e.Exception.InnerException);
    }

    void SyncSuccessCallBack(object sender, SyncSuccessEventArgs e)
    {
        Debug.Log("Synchronize Successfull");
        List<Record> newRecords = e.UpdatedRecords;
        for (int k = 0; k < newRecords.Count; k++)
        {
            Debug.Log(newRecords[k].Key + " was updated: " + newRecords[k].Value);
        }
        if (string.IsNullOrEmpty(playerInfo.Get("uid")))
        {
            firstTime = true;
            Debug.Log("Player Data Not Updated");
            if (fbauth)
            {
                playerInfo.Put("uid", AccessToken.CurrentAccessToken.UserId);
                fetchFBName();
            }
            else if (gauth)
            {
                playerInfo.Put("uid", Social.localUser.id);
                playerInfo.Put("name", Social.localUser.userName);
                UploadUserData("G");
            }
        }
        else
        {
            UserData.name = playerInfo.Get("name");
            UserData.uid = playerInfo.Get("uid");
            if (fbauth)
                UserData.provider = "FB";
            else if (gauth)
                UserData.provider = "G";
            Debug.Log("Player Data Synchronized");
            sync = true;
            loginPage = true;
            loading = false;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log(scene.buildIndex);
    }

    public void Logout()
    {
        if (fbauth)
        {
            FB.LogOut();
        }
        else if (gauth)
        {
            PlayGamesPlatform.Instance.SignOut();
        }
        awsCredentials.ClearCredentials();
        UserData.name = null;
        UserData.provider = null;
        UserData.uid = null;
        SceneManager.LoadScene("LoginScene");
    }

    public void fetchFBName()
    {
        FB.API("me?fields=first_name", HttpMethod.GET, NameCallBack);
    }

    public void getFBProfileImage(PictureCallBack pcb)
    {
        this.pcb = pcb;
        if (fbTexture != null)
        {
            this.pcb(fbTexture);
        }
        else
        {
            FB.API("me/picture?width=100&height=100", HttpMethod.GET, FBPictureCallBack);
        }
    }

    void NameCallBack(IGraphResult result)
    {
        Debug.Log("Retrieved name from fb");
        IDictionary<string, object> profil = result.ResultDictionary;
        playerInfo.Put("name", profil["first_name"].ToString());
        UploadUserData("FB");
        //playerInfo.SynchronizeOnConnectivity();
    }

    void FBPictureCallBack(IGraphResult result)
    {
        Debug.Log("FB picture cb");
        fbTexture = result.Texture;
        Debug.Log("Fb texture loaded");
        this.pcb(result.Texture);
    }

    public void getGoogleProfileImage(PictureCallBack pcb)
    {
        Debug.Log("Goole picture request");
        this.pcb = pcb;
        if (googleTexture != null)
        {
            this.pcb(googleTexture);
        }
        else
        {
            //Participant p = PlayGamesPlatform.Instance.RealTime.GetSelf();
            //Debug.Log(p.Player.AvatarURL);
            Debug.Log(Social.localUser.image);
            //Debug.Log(((PlayGamesLocalUser)Social.localUser).mPlatform.image);
            Debug.Log(PlayGamesPlatform.Instance.localUser.image);
            Debug.Log(((PlayGamesLocalUser)Social.localUser).image);
            StartCoroutine(LoadGoogleImage());
        }
    }

    IEnumerator LoadGoogleImage()
    {
        while (PlayGamesPlatform.Instance.localUser.image == null)
        {
            Debug.Log("IMAGE NOT FOUND");
            yield return null;
        }
        Debug.Log("Image Found");
        googleTexture = PlayGamesPlatform.Instance.localUser.image;
        this.pcb(PlayGamesPlatform.Instance.localUser.image);
        /*using (WWW www = new WWW(url))
        {
            yield return www;
            www.LoadImageIntoTexture(googleTexture);
            //ImgMine.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0f, 0f));
            this.pcb(googleTexture);
        }*/
    }


    void UpdateUI()
    {
        sync = false;
        loginPage = false;
        Debug.Log(playerInfo.Get("name"));
        Debug.Log(playerInfo.Get("uid"));
        if (!firstTime)
        {
            SceneManager.LoadScene("Home");
        }
        else
        {
            firstTime = false;
            SceneManager.LoadScene("Home");
        }

    }

    private void UploadUserData(string provider)
    {

        Debug.Log("Uploading user data to DynamoDB");
        playerInfo.SynchronizeOnConnectivity();
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

    void Update()
    {
        if (fbandcsInitiated == 2)
        {
            Debug.Log("FB and AWS initiated");
            fbandcsInitiated = 0;
            loading = false;
            if (FB.IsLoggedIn)
            {
                fbauth = true;
                gauth = false;
                loading = true;
                awsCredentials.AddLogin("graph.facebook.com", AccessToken.CurrentAccessToken.TokenString);
                Debug.Log("Already LoggedIn FB");
                playerInfo.SynchronizeOnConnectivity();
                fbandcsInitiated = 0;
            }
            else// if (PlayGamesPlatform.Instance.IsAuthenticated() || Social.localUser.authenticated)
            {
                loading = true;
                PlayGamesPlatform.Instance.Authenticate(SignInInteractivity.NoPrompt, (result) => {
                    // handle results
                    Debug.Log(result);
                    if (result == GooglePlayGames.BasicApi.SignInStatus.Success)
                    {

                        Debug.Log("Success-1");
                        Social.localUser.Authenticate((success) =>
                        {
                            ((GooglePlayGames.PlayGamesPlatform)Social.Active).SetGravityForPopups(Gravity.BOTTOM);
                            Debug.Log("uid:" + Social.localUser.id);
                            Debug.Log("Success-2");
                            if (success)
                            {
                                Debug.Log("Success-3");
                            }
                            gauth = true;
                            fbauth = false;
                            loading = true;
                            awsCredentials.AddLogin("accounts.google.com", PlayGamesPlatform.Instance.GetIdToken());
                            Debug.Log("Already LoggedIn G");
                            playerInfo.SynchronizeOnConnectivity();
                        });
                    }
                    else
                    {
                        fbauth = false;
                        gauth = false;
                        loading = false;
                    }
                });
            }
            /*else
            {
                loading = false;
            }*/
        }
        if ((FB.IsLoggedIn || Social.localUser.authenticated) && sync && loginPage)
        {
            UpdateUI();
        }

    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public string MyDictionaryToJson(Dictionary<string, string> dict)
    {
        List<string> entries = new List<string> { };
        foreach (KeyValuePair<string, string> item in dict)
        {
            entries.Add(string.Format("\"{0}\":\"{1}\"", item.Key, item.Value));
        }
        return "{" + string.Join(",", entries) + "}";
    }

}


