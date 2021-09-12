using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;


[RequireComponent(typeof(ScrollRect))]
public class AutoScroll : MonoBehaviour {
	
    private ScrollRect          m_scrollRect;
    public Button[]            m_buttons;
    public int                 m_index;
    private float               m_horizontallPosition;
    private bool                m_right;
    private bool                m_left;
	private GameObject			m_lastButton;

    public void Start()
    {
        m_scrollRect        = GetComponent<ScrollRect>();
        m_buttons           = GetComponentsInChildren<Button>();
		m_index = m_buttons.Length - 1;
        //m_horizontallPosition  = 1f - ((float)m_index / (m_buttons.Length - 1));
    }
	
	void OnEnabled() {
        m_buttons = GetComponentsInChildren<Button>();
		m_index = m_buttons.Length - 1;
	}

    public void Update()
    {
		Scroll(EventSystem.current.currentSelectedGameObject);
    }
	
	void Scroll(GameObject button) {
		m_index = System.Array.IndexOf(m_buttons,button.GetComponent<Button>());
        m_horizontallPosition = ((float)m_index / (m_buttons.Length - 1));
		
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