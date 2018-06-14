﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Quest {

	public string name;
	protected QuestStage[] stages;
	protected int currentStage;

	public NPC reporter;
	private bool active;

	public Quest(NPC reporter, string questName) {
		this.reporter = reporter;
		this.name = questName;
		this.stages = GenerateQuestStages();
		currentStage = 0;
		active = false;
	}

	protected abstract QuestStage[] GenerateQuestStages();

	public void OnAccept() {
		reporter.AcceptedQuest();
		QuestManager.instance.OnAcceptQuest(this);
		active = true;
	}

	public void OnReject() {
		reporter.RejectedQuest();
	}

	public T GetCurrentStage<T>() where T : QuestStage {
		if (HasCompletedAll()) {
			return null;
		}
		return stages[currentStage] as T;
	}

	public string GetCurrentDetails() {
		return stages[currentStage].GetDetails();
	}

	public int GetCurrentReward() {
		return stages[currentStage].GetReward();
	}

	public bool HasCompletedAll() {
		return currentStage >= stages.Length;
	}

	public virtual void CompleteQuestStage() {
		stages[currentStage].OnComplete(reporter);
		currentStage++;
		if (HasCompletedAll()) {
			OnCompletedAll();
		}
	}

	public virtual void OnCompletedAll() {
		active = false;
		reporter.CompletedEntireQuest();
		QuestManager.instance.OnCompleteQuest(this);
	}

	public abstract void OnStealItem(Player player, NPC npc, Item item);

	public abstract void OnCraftItem(Player player, Item item);

	public abstract void OnDefeatedNPC(Player player, NPC npc);

	public abstract void OnSellItem(Player player, NPC npc, Item item);

	public bool IsActive() {
		return active;
	}
}
