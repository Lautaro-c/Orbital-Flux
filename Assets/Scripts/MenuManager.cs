using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Serializable]
    public class LevelEntry
    {
        public Button button;

        [Tooltip("Nombre exacto de la escena, sin .unity")]
        public string sceneName;
    }

    [Header("Paneles")]
    [SerializeField] private GameObject levelsPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject audioAndVideoPanel;
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private GameObject creditsPanel;

    [Header("Menu principal")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button creditsButton;

    [Header("Selector de niveles")]
    [SerializeField] private LevelEntry[] levels = new LevelEntry[5];
    [SerializeField] private Button levelsBackButton;

    [Header("Settings")]
    [SerializeField] private Button audioAndVideoButton;
    [SerializeField] private Button controlsButton;
    [SerializeField] private Button settingsBackButton;

    [Header("Audio y video")]
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Button audioAndVideoBackButton;

    [Header("Controles")]
    [SerializeField] private Button controlsBackButton;

    private bool loadingLevel;

    private void Start()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Connect(playButton, () => ShowPanel(levelsPanel));
        Connect(settingsButton, () => ShowPanel(settingsPanel));
        Connect(exitButton, ExitGame);

        Connect(audioAndVideoButton, OpenAudioAndVideo);
        Connect(controlsButton, () => ShowPanel(controlsPanel));

        Connect(audioAndVideoBackButton, () => ShowPanel(settingsPanel));
        Connect(controlsBackButton, () => ShowPanel(settingsPanel));

        Connect(creditsButton, () => ShowPanel(creditsPanel));

        foreach (LevelEntry level in levels)
        {
            if (level == null)
                continue;

            // Una referencia independiente para cada boton.
            string scene = level.sceneName;
            Connect(level.button, () => LoadLevel(scene));
        }

        if (fullscreenToggle != null)
        {
            fullscreenToggle.SetIsOnWithoutNotify(Screen.fullScreen);
            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        }
    }

    private void Connect(Button button, UnityAction action)
    {
        if (button != null)
            button.onClick.AddListener(action);
    }

    private void ShowPanel(GameObject target)
    {
        if (target == null)
        {
            Debug.LogWarning(
                "Falta asignar el panel en el Inspector de MenuManager.",
                this
            );
            return;
        }

        SetPanel(levelsPanel, target);
        SetPanel(settingsPanel, target);
        SetPanel(audioAndVideoPanel, target);
        SetPanel(controlsPanel, target);
        SetPanel(creditsPanel, target);
    }

    private void SetPanel(GameObject panel, GameObject target)
    {
        if (panel != null)
            panel.SetActive(panel == target);
    }

    private void OpenAudioAndVideo()
    {
        if (fullscreenToggle != null)
            fullscreenToggle.SetIsOnWithoutNotify(Screen.fullScreen);

        ShowPanel(audioAndVideoPanel);
    }

    private void SetFullscreen(bool enabled)
    {
        Screen.fullScreen = enabled;
    }

    private void LoadLevel(string sceneName)
    {
        if (loadingLevel)
            return;

        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError(
                "Falta el nombre de la escena de este nivel.",
                this
            );
            return;
        }

        sceneName = sceneName.Trim();

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError(
                $"No se puede cargar '{sceneName}'. Comprueba el nombre " +
                "y que la escena este incluida y habilitada en la build.",
                this
            );
            return;
        }

        loadingLevel = true;
        Time.timeScale = 1f;

        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
    }

    private void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}