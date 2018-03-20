﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Desk : Interactable {

    private UnityAction call;

    [SerializeField]
    private List<Item> items;

	// Use this for initialization
	void Start () {
        Debug.Log("player restart");		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void PlayerInteract(Player player) {        
        //base.button.RemoveAllListeners();
        call = delegate {
            Interact(player);
        };
        base.button.AddListener(call);
    }

    public override void PlayerLeave(Player player) {
        base.button.RemoveListener(call);
        call = null;
    }

    public void Interact(Player player) {
        Debug.Log(player.GetName() + " interacted with " + gameObject.name + " " + base.button.getListeners());
        if (items.Count > 0) {
            Item item = items[0];
            player.AddItem(item);
            items.Remove(item);
        }
    }
}
