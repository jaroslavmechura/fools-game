using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutSceneManager : MonoBehaviour
{
    public float timeToEnd;
    public string nextScene;

    

    private void Start()
    {
        Cursor.visible = false;
        Invoke("End", timeToEnd);
    }

    private void End() {
        Cursor.visible = true;
        SceneManager.LoadScene(nextScene);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space)) {
            End();
        }
    }

}
