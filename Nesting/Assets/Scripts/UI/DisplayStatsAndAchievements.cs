using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class DisplayStatsAndAchievements : MonoBehaviour
{

	public Text completedAchievementList;

    void OnEnable()
    {
		string list = "";
        List<Achievement> completed = UIObjects.achievements.GetCompletedAchievments();
		foreach (Achievement a in completed) {
			list += a.mName+"\n";
		}
		completedAchievementList.text = list;
    }
}
