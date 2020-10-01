using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeManager : MonoBehaviour
{

    [SerializeField]
    private Button logoutButton;
    // Start is called before the first frame update
    void Start()
    {
        logoutButton.onClick.AddListener(Logout);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Logout()
    {
        GameObject.Find("AWSClient").GetComponent<AWSClient>().Logout();
    }
}
