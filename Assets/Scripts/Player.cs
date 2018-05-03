﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class Player : MovingObject {

	private const string NPC_TAG = "NPC";

	private Vector3 START_POS = new Vector3(0.5f, 0, 0);

	private Pocket mainItems;

	protected int money = 0;
	public Text moneyText;
	protected int health = 0;
	public Text healthText;
	protected int exp = 0;
	protected int strength = 0;

	protected override void Start () {
		base.Start();
		mainItems = FindObjectOfType<Pocket>();

		UpdateInfo();

		if (GetFloor() == 1) {
			GameManager.instance.HideFloor2();
		} else if (GetFloor() == 2) {
			GameManager.instance.ShowFloor2();
		}
	}

	public int GetMoney() { return money; }

	public int GetHealth() { return health; }

	public int GetExperience() { return exp; }

	public int GetStrength() { return strength; }

	protected override void FixedUpdate() {
		float moveHorizontal = 0;
		float moveVertical = 0;

		Vector3 movement;
		#if UNITY_STANDALONE || UNITY_WEBPLAYER

		moveHorizontal = Input.GetAxis ("Horizontal");
		moveVertical = Input.GetAxis ("Vertical");
		movement = new Vector3(moveHorizontal, moveVertical, 0f);

		#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE

		moveHorizontal = CrossPlatformInputManager.GetAxis("Horizontal");
		moveVertical = CrossPlatformInputManager.GetAxis("Vertical");
		movement = new Vector3(moveHorizontal, moveVertical, 0f);

		#endif

		Move(movement.normalized);
	}

	protected override void OnTriggerEnter2D(Collider2D other) {
		base.OnTriggerEnter2D (other);
		if (GetFloor () == 1) {
			GameManager.instance.HideFloor2();
		} else if (GetFloor () == 2) {
			GameManager.instance.ShowFloor2();
		}
	}

	protected override void OnTriggerExit2D(Collider2D other) {
		base.OnTriggerExit2D (other);
	}

	public string GetName() {
		return gameObject.name;
	}

	public Pocket GetPocket() {
		return mainItems;
	}
	
	public void AddItem(Item item) {
		mainItems.AddItem(item);
	}

	public Item RemoveItemAtIndex(int index) {
		if (index >= 0 || index < mainItems.GetNumItems()) {
			Item itemToRemove = mainItems.GetItem(index);
			mainItems.RemoveItem(itemToRemove);
			return itemToRemove;
		}
		return null;
	}

	public void RemoveItem(Item item) {
		mainItems.RemoveItem(item);
	}

	private void UpdateInfo() {
		if (moneyText != null) {
			moneyText.text = "" + money;
		}

		if (healthText != null) {
			healthText.text = "" + health;
		}
	}

    public override void Save() {
        PlayerData data = new PlayerData(base.onStairs, base.floor, base.rb2D.transform.position, 
		money, health, exp, strength);
		GameManager.Save(data, base.filename);
    }

    public override void Load() {
		PlayerData data = GameManager.Load<PlayerData>(base.filename);
		
        if (data != null) {
			base.LoadFromData(data);
			if (GetFloor () == 2) {
				GameManager.instance.ShowFloor2();
				gameObject.layer = 17 - gameObject.layer;
			}
			this.money = data.money;
			this.health = data.health;
			this.exp = data.exp;
			this.strength = data.strength;
		} else {
			base.LoadFromData(new PlayerData(
				0, 0, START_POS, 0, 0, 0, 0
			));
			this.money = 0;
			this.health = 100;
			this.exp = 0;
		}
    }
}

[System.Serializable]
public class PlayerData : MovingObjectData {
	public int money;
	public int health;
	public int exp;
	public int strength;

	public PlayerData(int onStairs, int floor, Vector3 position, int money, int health, int exp, int strength): 
		base(onStairs, floor, position) {
		this.money = money;
		this.health = health;
		this.exp = exp;
		this.strength = strength;
	}
}