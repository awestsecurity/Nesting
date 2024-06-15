using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;


[RequireComponent(typeof(ScrollRect))]
public class AutoScroll : MonoBehaviour {
	
    private ScrollRect         m_scrollRect;
    public Button[]            m_buttons;
    public int                 m_index;
    private float              m_horizontallPosition;
    private bool               m_right;
    private bool               m_left;
	private GameObject			m_lastButton;

	public Text nameDisplay;
	public Text statusDisplay;
	public Text flavorDisplay;
	public Text speedDisplay;
	
    public void Start()
    {
        m_scrollRect        = GetComponent<ScrollRect>();
        m_buttons           = GetComponentsInChildren<Button>();
		m_index = m_buttons.Length - 1;
        //m_horizontallPosition  = 1f - ((float)m_index / (m_buttons.Length - 1));
    }
	
	public void Refresh() {
        m_buttons = GetComponentsInChildren<Button>();
		m_index = m_buttons.Length - 1;
		//Debug.Log("I'm triggered - AutoScroll enabled");
	}

    public void Update()
    {
		GameObject b = EventSystem.current.currentSelectedGameObject;
		if ( m_lastButton == null || m_lastButton != b ) {
			m_lastButton = b;
			Scroll(b);
			BirdStats s = b.GetComponent<BirdStats>();
			nameDisplay.text = s.name;
			statusDisplay.text = $"Status: {s.status}";
			flavorDisplay.text = s.flavortext;
			speedDisplay.text = $"Speed: {s.speed.ToString()} - Just a place holder. Will change."; 
		}
    }
	
	void Scroll(GameObject button) {
		m_index = System.Array.IndexOf(m_buttons,button.GetComponent<Button>());
		float f = (float)m_index;
		// to offset for the smaller close and random buttons
		if (m_index > 0 && m_index <= 5) { 
			f -= 1f - (m_index*0.15f);
		} 
		//position 0-1f
        m_horizontallPosition = f / (m_buttons.Length - 1);

		
 /*       m_right  = CrossPlatformInputManager.GetAxis("Horizontal") > 0.1f;
        m_left  = CrossPlatformInputManager.GetAxis("Horizontal") < -0.1f;

        if (m_right ^ m_left)
        {
            if (m_right)
                m_index = Mathf.Clamp(m_index - 1, 0, m_buttons.Length - 1);
            else
                m_index = Mathf.Clamp(m_index + 1, 0, m_buttons.Length - 1);

            m_horizontallPosition = 1f - ((float)m_index / (m_buttons.Length - 1));
        }
*/
        m_scrollRect.horizontalNormalizedPosition = m_horizontallPosition;		
	}
	
}