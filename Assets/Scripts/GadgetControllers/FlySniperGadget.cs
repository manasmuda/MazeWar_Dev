using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlySniperGadget : Gadget
{
    public GameObject curObject;
    // Start is called before the first frame update
    protected void Awake()
    {
        damage = 10;
        health = 0;
        lethal = true;
        directAction = false;
        uiChange = true;
        mapChange = true;
        reloadTime = 80;
        useLimit = 5;
        timerGadget = true;
        useTime = 20;
        
    }

    protected override void Start()
    {
        base.Start();
        // character = transform.parent.gameObject; Load Character object
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override void CallAction(string id = "")
    {
        base.CallAction();
        Client.clientInstance.character.GetComponent<NewPlayer>().enabled = false;
        Client.clientInstance.character.GetComponent<Shooting>().enabled = false;
        Client.clientInstance.character.GetComponent<PlayerAnimations>().enabled = false;
        int i = Mathf.FloorToInt((Client.clientInstance.character.transform.position.z-60f) / 6f);
        int j = Mathf.FloorToInt((60f + Client.clientInstance.character.transform.position.x) / 6f);
        Vector3 origin = new Vector3(6*j-57f,0.5f,57f-6*i);
        Quaternion q1 = Quaternion.identity;
        q1.eulerAngles = new Vector3(0,90f,0);
        curObject = GameObject.Instantiate(gadetPrefab, origin, q1);
        RectTransform temprect=UIElementsScript.instance.shootingButton.transform.GetComponent<RectTransform>();
        temprect.anchorMin = new Vector2(0, 0);
        temprect.anchorMax = new Vector2(0, 0);
        temprect.offsetMin = new Vector2(15, 15);
        temprect.offsetMax = new Vector2(95, 95);
        UIElementsScript.instance.joystick.interactable = false;
        UIElementsScript.instance.joystick.alpha = 0;
        UIElementsScript.instance.GadgetsGroup.interactable = false;
        UIElementsScript.instance.GadgetsGroup.alpha = 0;
        UIElementsScript.instance.crouchButton.interactable = false;
        UIElementsScript.instance.crouchButton.alpha = 0;
        UIElementsScript.instance.miniShootingButton.interactable = false;
        UIElementsScript.instance.miniShootingButton.alpha = 0;
    }

    public override void EndAction()
    {
        base.EndAction();

    }

    IEnumerator StartUseTime()
    {
        yield return new WaitForSeconds(useTime);

    }
}
