using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Menus : MonoBehaviour {
    public Canvas startMenu;
    public Canvas quitMenu;
	public Image background;
    public Button resume;
	public float maxScrollDistance;

	public Vector2 backgroundScrollSpeed;
	public Vector2 backgroundScrollDirection;
	public Image foreground1;
	public Image foreground2;
	private float lastswitch;
	private Vector3 originalPosition;

	void Start () {
        resume.enabled = false;
		foreground2.enabled = false;
		originalPosition = background.transform.position;
		ShowMenu();
    }

    void Update() {		
		float distance = System.Math.Abs (background.transform.position.x - originalPosition.x);
		if (distance < maxScrollDistance) {
			var movement = new Vector3(
				backgroundScrollSpeed.x * backgroundScrollDirection.x,
				backgroundScrollSpeed.y * backgroundScrollDirection.y,
				0);
			
			movement *= Time.deltaTime;
			background.transform.Translate (movement);
		}
		lastswitch += Time.deltaTime;

		if (lastswitch > 1.0f) {
			foreground1.enabled = !foreground1.enabled;
			foreground2.enabled = !foreground1.enabled;
			lastswitch = 0f;
		}

        if (Input.GetButtonDown("Cancel")) {
            ShowMenu();
        }
    }

    public void OnePlayer() {
        resume.enabled = true;
        Resume();
    }

    public void Resume() {        
        quitMenu.enabled = false;
        startMenu.enabled = false;
    }

    public void Quit() {
        quitMenu.enabled = true;
        startMenu.enabled = false;
    }

    public void QuitNo() {
        quitMenu.enabled = false;
        startMenu.enabled = true;        
    }

    public void QuitYes() {
        EditorApplication.isPlaying = false;
        Application.Quit();
    }

    public void ShowMenu() {        
        quitMenu.enabled = false;
        startMenu.enabled = true;
		background.transform.position = originalPosition;
    }
}
