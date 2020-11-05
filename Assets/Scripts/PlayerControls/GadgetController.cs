using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GadgetController : MonoBehaviour
{
    private CanvasGroup GadgetPanel;

    private GameObject lg1Obj;
    private GameObject lg2Obj;
    private GameObject hg1Obj;
    private GameObject hg2Obj;

    private GameObject[] gadgetObjects;

    public static GadgetController instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GadgetPanel = GameObject.Find("Canvas/GamePanel/GadgetPanel").GetComponent<CanvasGroup>();

        lg1Obj = GameObject.Find("Canvas/GamePanel/GadgetPanel/LGadget1");
        lg2Obj = GameObject.Find("Canvas/GamePanel/GadgetPanel/LGadget2");
        hg1Obj= GameObject.Find("Canvas/GamePanel/GadgetPanel/HGadget1");
        hg2Obj= GameObject.Find("Canvas/GamePanel/GadgetPanel/HGadget2");

        gadgetObjects = new GameObject[4] { hg1Obj, hg2Obj, lg1Obj, lg2Obj };

        gadgetObjects[0].GetComponent<Button>().onClick.AddListener(delegate { GadgetRequest(0); });
        gadgetObjects[1].GetComponent<Button>().onClick.AddListener(delegate { GadgetRequest(1); });
        gadgetObjects[2].GetComponent<Button>().onClick.AddListener(delegate { GadgetRequest(2); });
        gadgetObjects[3].GetComponent<Button>().onClick.AddListener(delegate { GadgetRequest(3); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GadgetRequest(int x)
    {
        SimpleMessage message = new SimpleMessage(MessageType.GadgetCallAction);
        message.intData = x;
        message.stringArrData = new string[2] { MyData.playerId, MyData.team };
        Client.clientInstance.networkClient.SendMessage(message);
    }

    public void GadgetSelect(int x,string id)
    {
        MyData.gadgets[x].CallAction(id);
        StartCoroutine(StartGadgetTimer(gadgetObjects[x], MyData.gadgets[x]));
    }

    IEnumerator StartGadgetTimer(GameObject obj,Gadget gadget)
    {
        float time = gadget.reloadTime;
        obj.transform.GetChild(0).GetComponent<CanvasGroup>().alpha = 1;
        obj.GetComponent<Button>().interactable = false;
        Text textview= obj.transform.GetChild(0).GetChild(0).GetComponent<Text>();
        textview.text = Convert.ToString(Convert.ToInt32(time));
        while (time > 0f)
        {
            time = time - 1f;
            textview.text = Convert.ToString(Convert.ToInt32(time));
            yield return new WaitForSeconds(1f);
        }
        obj.transform.GetChild(0).GetComponent<CanvasGroup>().alpha = 0;
        yield return new WaitForSeconds(0.05f);
        if (gadget.useLimit > 0)
        {
            obj.GetComponent<Button>().interactable = false;
        }
    }

    void LockGadgetControl()
    {
        GadgetPanel.interactable = false;
    }

    void UnlockGadgetControl()
    {
        GadgetPanel.interactable = true;
    }
}
