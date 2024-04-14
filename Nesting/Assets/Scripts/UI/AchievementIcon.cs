using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class AchievementIcon : Button
{
	public Text title;
    public Text description;
	public Achievement achievement;
	
	BaseEventData m_BaseEvent;
	
	public void DisplayInfo() {
		title.text = achievement.mName;
		description.text = achievement.mDescription;	
	}

}
