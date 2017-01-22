using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Menus : MonoBehaviour {
    public Canvas startMenu;
    public Canvas quitMenu;

    public Button resume;

	void Start () {
        resume.enabled = false;
        ShowMenu();
    }

    void Update() {
        if (Input.GetButtonDown("Cancel")) {
            ShowMenu();
        }
    }

    public void OnePlayer() {
        resume.enabled = true;
        Resume();
    }

    public void TwoPlayer() {
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
    }
}
