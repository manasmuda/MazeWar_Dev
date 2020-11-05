using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinCollectBotGadget : Gadget
{
    // Start is called before the first frame update
    public Dictionary<string,GameObject> activeObjects = new Dictionary<string,GameObject>();
    // Start is called before the first frame update
    protected void Awake()
    {
        damage = 0;
        health = 100;
        lethal = false;
        directAction = true;
        uiChange = false;
        mapChange = true;
        reloadTime = 60;
        useLimit = 5;
        timerGadget = false;
        useTime = 0;
        gadetPrefab = Resources.Load<GameObject>("Gadgets/Bots/CoinCollectorBot_"+MyData.team);
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

    public override void CallAction(string id="")
    {
        base.CallAction();
        if (character == null)
            character = Client.clientInstance.character;
        GameObject gadgetObject = Instantiate(gadetPrefab, character.transform.position, Quaternion.identity);
        activeObjects.Add(id, gadgetObject);
    }

    public override void EndAction()
    {
        base.EndAction();
    }

    public override Dictionary<string, GameObject> GetActiveGadgets()
    {
        return activeObjects;
    }

    public override void AddNewAutoState(AutoGadgetState gadgetState, string id)
    {
        activeObjects[id].GetComponent<CollectBotSyncScript>().AddNewMove(new Vector3(gadgetState.position[0], gadgetState.position[1], gadgetState.position[2]), new Vector3(gadgetState.angle[0],gadgetState.angle[1],gadgetState.angle[2]));
    }
}
