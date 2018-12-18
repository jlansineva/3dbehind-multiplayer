using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerState : NetworkBehaviour {
    [SyncVar]
    [SerializeField]
    private bool jumping = false;
    [SyncVar]
    [SerializeField]
    private bool moving = false;
    [SyncVar]
    [SerializeField]
    private bool teleporting = false;
    [SyncVar]
    [SerializeField]
    private bool attacking = false;

    [Command]
    public void CmdUpdateState(string stateVar, bool newValue)
    {
        switch(stateVar)
        {
            case "jumping":
                jumping = newValue;
                break;
            case "teleporting":
                teleporting = newValue;
                break;
            case "moving":
                moving = newValue;
                break;
            case "attacking":
                attacking = newValue;
                break;
            default:
                break;
        }
    }

    public bool GetState(string stateVar)
    {
        switch (stateVar)
        {
            case "jumping":
                return jumping;
            case "teleporting":
                return teleporting;
            case "moving":
                return moving;
            case "attacking":
                return attacking;
            default:
                return false;
        }
    }
    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
