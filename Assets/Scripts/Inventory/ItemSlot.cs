﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IDropHandler {
	private Image itemImage;
	private Image itemBack;
	public Item item;

	public static Text text = null;

	private ItemStash parentStash;
	[SerializeField]
	private int index;
	private bool selected;

	private static Color SELECTED_COLOR = new Color(255/255f, 147/255f, 76/255f);
	private static Color DEFAULT_COLOR = new Color(1, 1, 1);

	private const string ITEM_IMAGE = "ItemImage";
	private const string BACKGROUND_IMAGE = "BackgroundImage";
	private const string INVENTORY_ITEM_TEXT = "SelectedInventoryItemText";

	void Awake() {
		Refresh();
	}

	public void Refresh() {
		if (text == null) {
			text = GameObject.Find(INVENTORY_ITEM_TEXT).GetComponent<Text>();
		}
		itemImage = transform.Find(ITEM_IMAGE).gameObject.GetComponent<Image>();
		itemBack = transform.Find(BACKGROUND_IMAGE).gameObject.GetComponent<Image>();
		Reset();
	}

	public void SetIndex(int i) {
		index = i;
	}

	public int GetIndex() {
		return index;
	}

	public void SetParentStash(ItemStash parent) {
		parentStash = parent;
	}

	public ItemStash GetParentStash() {
		return parentStash;
	}

	public void InsertItem(Item item, ItemStash parent) {
		SetParentStash(parent);
		SetItem(item);
	}

	public void SetItem(Item item) {
		if (item == null) {
			return;
		}
		
		itemImage.sprite = item.sprite;
		itemImage.enabled = true;
		this.item = item;
		
	}

	public Item GetItem() {
		return this.item;
	}

	public void Select() {
		if (parentStash != GameManager.instance.stashDisplayer) {
			GameManager.instance.stashDisplayer.DeselectAll();
		}
		if (parentStash != GameManager.instance.mainPlayer.GetPocket()) {
			GameManager.instance.mainPlayer.GetPocket().DeselectAll();
		}
		parentStash.DeselectAll();
		
		text.text = item.name;
		itemBack.color = SELECTED_COLOR;

		selected = true;
	}

	public void ToggleSelect() {
		if (selected) {
			Deselect();
		} else {
			Select();
		}
	}

	public void Deselect() {
		if (selected) {
			selected = false;
			text.text = "";
			itemBack.color = DEFAULT_COLOR;
		}
	}

	public void Reset() {
		itemImage.sprite = null;
		itemImage.enabled = false;
		this.item = null;
		Deselect();
	}

	public bool IsEmpty() {
		return this.item == null;
	}

    public void OnDrop(PointerEventData eventData)
    {
		// swap images
		Sprite tempSprite = itemImage.sprite;
		bool tempEnabled = itemImage.enabled;
		Image imageDragged = ItemDragger.itemBeingDragged.GetImage();
		itemImage.sprite = imageDragged.sprite;
		itemImage.enabled = true;

		imageDragged.sprite = tempSprite;
		imageDragged.enabled = tempEnabled;

		// swap item slot items
		ItemSlot itemSlotOther = imageDragged.GetComponent<ItemDragger>().GetParentSlot();

		Item tempItem1 = GetItem();
		Item tempItem2 = itemSlotOther.GetItem();
		
		SetItem(tempItem2);
		itemSlotOther.SetItem(tempItem1);

		// swap parent stash positions
		int indexOther = itemSlotOther.GetIndex();
		if (parentStash == itemSlotOther.GetParentStash()) {
			parentStash.SwapItemPositions(indexOther, index);
		} else {
			itemSlotOther.GetParentStash().RemoveItemAtIndex(indexOther);
			itemSlotOther.GetParentStash().AddItemAtIndex(tempItem1, indexOther);

			parentStash.RemoveItemAtIndex(index);
			parentStash.AddItemAtIndex(tempItem2, index);
		}
		Select();
    }
}
