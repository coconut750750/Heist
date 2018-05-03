﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Desk : Interactable {

    private UnityAction call;

    private Inventory items;

	void Start () {
        items = gameObject.GetComponent<Inventory>();
	}
	
    public override void Interact(Player player) {
        Debug.Log(Interactable.player.GetName() + " interacted with " + gameObject.name + " " + Interactable.button.GetListeners());

        GameManager.instance.stashDisplayer.DisplayInventory(items);
    }
}
