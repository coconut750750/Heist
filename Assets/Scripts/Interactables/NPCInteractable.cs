﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>  
///		This is the NPCInteractable class.
/// 	When players come into range with an NPC, a hover name object appears.
///     When players interact with an NPC, a speech bubble object appears that hides the hover name.
///     Only one pop up can be visible at a time.
/// 	SAVING AND LOADING: None
/// </summary>  
public class NPCInteractable : Interactable {

    public HoverName hoverNameText;
    private HoverName hoverTextInstance = null;

    public SpeechBubble speechBubble;
    private SpeechBubble speechBubbleInstance = null;

    public NPCOptions npcOptions;
    private NPCOptions npcOptionsInstance = null;

	private NPC npcObject;
    private bool interacted = false;

    // Use this for initialization
    void Start () {
		npcObject = gameObject.GetComponent<NPC>();
	}

    void Update () {
        if (!base.enabled) {
            return;
        }

        // updates hovername's position so that it follows the npc
        if (hoverTextInstance != null) {
            hoverTextInstance.UpdatePosition(gameObject.transform.position);
        }
    }
	
	public override void Interact(Player player) {
        interacted = !interacted;
        if (!interacted) { 
            // interacted twice so resume game, hide pop ups, and enable button b
            player.EnableButtonB();
            GameManager.instance.UnpauseGame();
            HideSpeechBubble();
            HideNPCOptions();
            return;
        } else {
            // disable button b so player can't attack
            // there isnt a cover when player interacts with npc
            player.DisableButtonB();
            GameManager.instance.PauseGame();
        }

        // greet player
        if (speechBubbleInstance == null) {
            speechBubbleInstance = Instantiate(speechBubble);
            speechBubbleInstance.Display(GameManager.instance.canvas.transform);
            speechBubbleInstance.UpdateText(npcObject.Greet());
            speechBubbleInstance.UpdatePosition(gameObject.transform.position);
        }

        if (npcOptionsInstance == null) {
            npcOptionsInstance = Instantiate(npcOptions);
            npcOptionsInstance.Display(GameManager.instance.canvas.transform);
            npcOptionsInstance.SetCallbacks(ShowInventory, ShowQuest, ShowInfo);
            npcOptionsInstance.UpdatePosition(gameObject.transform.position);
        }

        // destory hover name so that it doesn't overlap with speech bubble
        HideHoverText();
    }

    public override void EnterRange(Player player)
    {
        if (speechBubbleInstance != null) {
            // if there is a speech bubble already, don't want to overlap it
            // with a hovername
            return;
        }

        hoverTextInstance = Instantiate(hoverNameText);
        hoverTextInstance.Display(npcObject.GetName(), GameManager.instance.canvas.transform);
    }

    public override void ExitRange(Player player)
    {
        HideHoverText();
    }

    public void HideHoverText() {
        if (hoverTextInstance != null) {
            hoverTextInstance.Destroy();
            hoverTextInstance = null;
        }
    }

    public void HideSpeechBubble() {
        if (speechBubbleInstance != null) {
            speechBubbleInstance.Destroy();
            speechBubbleInstance = null;
        }
    }

    public void HideNPCOptions() {
        if (npcOptionsInstance != null) {
            npcOptionsInstance.Destroy();
            npcOptionsInstance = null;
        }
    }
            
    public void HideAllPopUps() {
        HideHoverText();
        HideSpeechBubble();
        HideNPCOptions();
    }

    public override void Disable() {
        base.Disable();
        HideAllPopUps();
    }

    public void ShowInventory() {
        GameManager.instance.npcDisplayer.Display(npcObject);
    }

    public void ShowQuest() {

    }

    public void ShowInfo() {

    }
}
