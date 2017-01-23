using System.Collections;
using System.Collections.Generic;
//using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menus : MonoBehaviour {
    public Canvas startMenu;
    public Canvas quitMenu;
	public Canvas background; 
	public Image backgroundImage;
    public Button resume;
	public Button newGame;
	public AudioSource audio;
	public float maxScrollDistance;
	public Button quitNo;

	public Vector2 backgroundScrollSpeed;
	public Vector2 backgroundScrollDirection;
	public Image foreground1;
	public Image foreground2;
	private float lastswitch;
	private Vector3 originalPosition;

	void Start () {
		foreground2.enabled = false;
		originalPosition = backgroundImage.transform.position;
		resume.enabled = false;
		ShowMenu();
    }

    void Update() {		
		float distance = System.Math.Abs (backgroundImage.transform.position.x - originalPosition.x);
		if (distance < maxScrollDistance) {
			var movement = new Vector3(
				backgroundScrollSpeed.x * backgroundScrollDirection.x,
				backgroundScrollSpeed.y * backgroundScrollDirection.y,
				0);
			
			movement *= Time.deltaTime;
			backgroundImage.transform.Translate (movement);
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

    public void NewGame() {
		resume.enabled = true;
        Resume();
    }

    public void Resume() {
		audio.enabled = false;
        quitMenu.enabled = false;
        startMenu.enabled = false;
		background.enabled = false;
        SceneManager.LoadScene("ArmadilloZ");
    }

    public void Quit() {
        quitMenu.enabled = true;
        startMenu.enabled = false;
		quitNo.Select ();
    }

    public void QuitNo() {
        quitMenu.enabled = false;
        startMenu.enabled = true;        
    }

    public void QuitYes() {
        //EditorApplication.isPlaying = false;
        Application.Quit();
    }

    public void ShowMenu() {        
		audio.enabled = true;
        quitMenu.enabled = false;
        startMenu.enabled = true;
		background.enabled = true;
		backgroundImage.transform.position = originalPosition;
		if (resume.enabled) {
			resume.Select();
		} else {
			newGame.Select();
		}
    }
}
