using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    [SerializeField]
    private Button facebookLoginButton;
    [SerializeField]
    private Button googleLoginButton;

    [SerializeField]
    private CanvasGroup loadingPanel;
    [SerializeField]
    private CanvasGroup loginPanel;

    private AWSClient awsClient;

    [SerializeField]
    private Button goHome;

    // Start is called before the first frame update
    void Start()
    {
        facebookLoginButton.onClick.AddListener(FacebookLogin);
        googleLoginButton.onClick.AddListener(GoogleLogin);
        //goHome.onClick.AddListener(GoHome);
        awsClient = GameObject.Find("AWSClient").GetComponent<AWSClient>();
    }

    public void FacebookLogin()
    {
        awsClient.FaceBookLogin();
    }

    public void GoogleLogin()
    {
        awsClient.GoogleLogin();
    }

    public void GoHome()
    {
        SceneManager.LoadScene("Home");
    }

    // Update is called once per frame
    void Update()
    {
        if (awsClient.loading)
        {
            showLoading();
        }
        else
        {
            hideLoading();
        }
    }

    public void showLoading()
    {
        loginPanel.alpha = 0f;
        loginPanel.blocksRaycasts = false;
        loadingPanel.alpha = 1f;
        loadingPanel.blocksRaycasts = true;
    }

    public void hideLoading()
    {
        loginPanel.alpha = 1f;
        loginPanel.blocksRaycasts = true;
        loadingPanel.alpha = 0f;
        loadingPanel.blocksRaycasts = false;
    }
}
