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

    private static NPCInteractable activeInstance = null;

    public HoverName hoverNameText;
    private HoverName hoverTextInstance = null;

    public SpeechBubble speechBubble;
    private SpeechBubble speechBubbleInstance = null;

    public NPCOptions npcOptions;
    private NPCOptions npcOptionsInstance = null;

    public Alert exclaimIcon;
    private Alert exclaimInstance = null;

    public Alert quest;
    private Alert questInstance = null;

	private NPC npc;
    private bool interacted = false;

    // Use this for initialization
    void Start () {
		npc = gameObject.GetComponent<NPC>();
	}

	public override void Interact(Player player) {
        // ensures that if another npc comes when player is interacting, the first one
        // is closed
        if (activeInstance != null && activeInstance != this) {
            activeInstance.Interact(player);
            return;
        }

        interacted = !interacted;
        if (!interacted) { 
            FinishInteraction();
        } else {
            StartInteraction();
        }
    }

    public void ShowFightAlert(Character opponent) {
        DestroyHoverText();
        DestroyNPCOptions();
        DestroySpeechBubble();
        if (questInstance != null) {
            questInstance.Disable();
        }
        InitExclaimIcon();
    }

    public void HideFightAlert() {
        if (questInstance != null) {
            questInstance.Enable();
        }
        DestroyExclaimIcon();
    }

    public override void EnterRange(Player player)
    {
        if (speechBubbleInstance != null) {
            // if there is a speech bubble already, don't want to overlap it
            // with a hovername
            return;
        }

        InitHoverText();
    }

    public override void ExitRange(Player player)
    {
        DestroyHoverText();
    }

    // disable button b so player can't attack
    private void StartInteraction() {
        activeInstance = this;
        player.DisableButtonB();

        npc.Pause();
        player.Pause();

        InitSpeechBubble();
        InitNPCOptions();
        DestroyHoverText(); // avoid overlap with speech bubble
    }

    private void FinishInteraction() {
        activeInstance = null;
        player.EnableButtonB();

        DestroySpeechBubble();
        DestroyNPCOptions();

        npc.Resume();
        player.Resume();
        InitHoverText();
    }

    private void InitHoverText() {
        hoverTextInstance = Instantiate(hoverNameText);
        hoverTextInstance.Display(npc.GetName(), gameObject);
    }

    private void DestroyHoverText() {
        if (hoverTextInstance != null) {
            hoverTextInstance.Destroy();
            hoverTextInstance = null;
        }
    }

    private void InitSpeechBubble() {
        if (speechBubbleInstance == null) {
            speechBubbleInstance = Instantiate(speechBubble);
            speechBubbleInstance.Display();
            speechBubbleInstance.UpdateText(npc.Greet());
            speechBubbleInstance.UpdatePosition(gameObject.transform.position);
        }
    }

    private void DestroySpeechBubble() {
        if (speechBubbleInstance != null) {
            speechBubbleInstance.Destroy();
            speechBubbleInstance = null;
        }
    }

    private void InitNPCOptions() {
        if (npcOptionsInstance == null) {
            npcOptionsInstance = Instantiate(npcOptions);
            npcOptionsInstance.Display();
            npcOptionsInstance.SetCallbacks(ShowInventory, ShowQuest, ShowInfo);
            npcOptionsInstance.UpdatePosition(gameObject.transform.position);
        }
    }

    private void DestroyNPCOptions() {
        if (npcOptionsInstance != null) {
            npcOptionsInstance.Destroy();
            npcOptionsInstance = null;
        }
    }

    private void InitExclaimIcon() {
        exclaimInstance = Instantiate(exclaimIcon);
        exclaimInstance.Display(gameObject);
    }

    private void DestroyExclaimIcon() {
        if (exclaimInstance != null) {
            exclaimInstance.Destroy();
            exclaimInstance = null;
        }
    }

    public void InitQuestIcon() {
        questInstance = Instantiate(quest);
        questInstance.Display(gameObject);
    }

    public void DestroyQuestIcon() {
        if (questInstance != null) {
            questInstance.Destroy();
            questInstance = null;
        }
    }

    public void DestroyAllPopUps() {
        DestroyHoverText();
        DestroySpeechBubble();
        DestroyNPCOptions();
        DestroyExclaimIcon();
        DestroyQuestIcon();
    }

    public override void Enable() {
        base.Enable();
        if (exclaimInstance != null) {
            InitExclaimIcon();
        } else if (questInstance != null) {
            InitQuestIcon();
        }
    }

    public override void Disable() {
        base.Disable();
        DestroyHoverText();
        DestroySpeechBubble();
        DestroyNPCOptions();
        if (exclaimInstance != null) {
            exclaimInstance.Disable();
        }
        if (questInstance != null) {
            questInstance.Disable();
        }
    }

    public void ShowInventory() {
        NPCTrade.instance.Display(npc);
    }

    public void ShowQuest() {
        NPCQuest.instance.Display(npc);
    }

    public void ShowInfo() {
        NPCInfo.instance.Display(npc);
    }
}
