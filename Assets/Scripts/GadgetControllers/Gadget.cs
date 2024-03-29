﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gadget : MonoBehaviour
{
    public bool lethal;
    public int damage;
    public int health;
    public bool directAction;
    public bool uiChange;
    public bool mapChange;
    public int reloadTime;
    public int useLimit;
    public bool timerGadget;
    public int useTime;
    public GameObject gadetPrefab;
    public GameObject character;


    public bool timerMode = false;
    public float timer = 0f;

    public bool enable = true;

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {

    }

    public virtual void CallMapChange()
    {

    }

    public virtual void CallUiChange()
    {

    }

    public virtual bool CanCall()
    {
        return useLimit > 0 && enable;
    }

    public virtual void CallAction(string id="")
    {
        if (useLimit == 0)
            return;
        useLimit--;
        enable = false;
        StartCoroutine(Reloading());
    }

    public virtual void EndAction()
    {
        if (useLimit > 0)
        {
            timerMode = false;
            enable = true;
        }
    }

    public virtual Dictionary<string,GameObject> GetActiveGadgets()
    {
        return null;
    }

    public virtual void AddNewAutoState(AutoGadgetState gadgetState,string id)
    {

    }

    public virtual void AddNewTimerState(AutoGadgetState gadgetState, string id)
    {

    }

    IEnumerator Reloading()
    {
        timer = reloadTime;
        while (timer > 0)
        {
            timer = timer - 0.2f;
            yield return new WaitForSeconds(0.2f);
        }
        EndAction();
    }

}
