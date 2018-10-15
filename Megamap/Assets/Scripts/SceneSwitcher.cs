using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour {

    [SerializeField]
    private string[] conditionSceneNames;

    [SerializeField]
    private int initialSceneIndex = 0;

    private int currentSceneIndex = 0;

    public void LoadNextScene()
    {
        if (currentSceneIndex == conditionSceneNames.Length - 1) {
            return;
        }

        ++currentSceneIndex;
        SceneManager.LoadScene(currentSceneIndex, LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync(currentSceneIndex - 1);
    }

    public void LoadPreviousScene()
    {
        if (currentSceneIndex == 0) {
            return;
        }

        --currentSceneIndex;
        SceneManager.LoadScene(currentSceneIndex, LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync(currentSceneIndex + 1);
    }

    private void Start()
    {
        if (conditionSceneNames == null || conditionSceneNames.Length == 0) {
            Debug.LogError("SceneSwitcher: Must provide scene names to switch; disabling script");
            enabled = false;
            return;
        }

        if (initialSceneIndex > conditionSceneNames.Length - 1) {
            Debug.LogWarning("SceneSwitcher: Initial index exceeds scene name list; using 0.");
            initialSceneIndex = 0;
        }

        currentSceneIndex = initialSceneIndex;
        SceneManager.LoadScene(conditionSceneNames[currentSceneIndex], LoadSceneMode.Additive);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N)) {
            LoadNextScene();
        }
        else if (Input.GetKeyDown(KeyCode.P)) {
            LoadPreviousScene();
        }
    }
}
