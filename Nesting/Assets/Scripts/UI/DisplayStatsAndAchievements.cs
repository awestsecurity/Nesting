using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;

public class DisplayStatsAndAchievements : MonoBehaviour
{

	public GameObject IconPrefab;
	public Text achievementTitle;
	public Text achievementInfo;
	
	private Transform p;
	private List<Achievement> completeGoals = new List<Achievement>();
	private List<Achievement> incompleteGoals = new List<Achievement>();
	private Dictionary<string, Selectable> icons = new Dictionary<string, Selectable>();
	private Sprite[] sprites;

    void OnEnable()
    {
		if (icons.Count <= 0) {
			Initialize();
		}
		string list = "";
		List<string> doneGoals = UIObjects.achievements.GetCompletedAchievments().Select(o => o.mName).ToList();
		for (int i = 0; i < incompleteGoals.Count; i++) {
			if (doneGoals.Contains(incompleteGoals[i].mName)) {
				icons[incompleteGoals[i].mName].interactable = true;
			}
		}
		for (int i = 0; i < completeGoals.Count; i++) {
			if (doneGoals.Contains(completeGoals[i].mName)) {
				icons[completeGoals[i].mName].interactable = true;
			}
		}
    }
	
	void Initialize() {
		sprites = Resources.LoadAll<Sprite>("AchievementBadges");
		completeGoals = UIObjects.achievements.GetCompletedAchievments();
		incompleteGoals = UIObjects.achievements.GetIncompleteAchievments();
		int x = 100; //Top Left x Axis
		int y = -100;
		foreach (Achievement a in completeGoals) {
			GenerateIcon(a, x, y);
			y -= 90;
			if (y < -400) {
				x += 90;
				y = -100;
			}
		}
		foreach (Achievement a in incompleteGoals) {
			GenerateIcon(a, x, y);
			y -= 90;
			if (y < -400) {
				x += 90;
				y = -100;
			}
		}
	}
	
	Sprite GetBadgeSprite(string name) {
		foreach (Sprite sp in sprites) {
			if (sp.name == name) {
				return sp;
			} 
		}
		return sprites[0];
	}
	
	void GenerateIcon(Achievement a, int x, int y) {
		GameObject icon = Instantiate(IconPrefab);
		icon.transform.SetParent(transform);
		icon.GetComponent<Image>().sprite = GetBadgeSprite(a.mIcon);
		Selectable s = icon.GetComponent<Selectable>();
		s.interactable = a.mComplete ? true : false;
		RectTransform rectTransform = icon.GetComponent<RectTransform>();
		rectTransform.localScale = new Vector3(1,1,1);
		rectTransform.anchoredPosition = new Vector2(x,y);
		AchievementIcon ai = icon.GetComponent<AchievementIcon>();
		ai.title = achievementTitle;
		ai.description = achievementInfo;
		ai.achievement = a;
		icons.Add(a.mName, s);
	}
}
