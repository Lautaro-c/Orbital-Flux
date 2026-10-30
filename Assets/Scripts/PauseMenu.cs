using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Menú de pausa del nivel. Al abrirlo frena el juego (Time.timeScale = 0) y muestra
/// directo el panel Settings. Esc abre y cierra la pausa.
///
/// IMPORTANTE: este script va en un objeto que SIEMPRE esté activo (no en el menú).
/// El menú visual es "pauseRoot", un hijo que este script muestra y oculta.
/// </summary>
public class PauseMenu : MonoBehaviour
{
    [Header("Entrada")]
    [Tooltip("Tecla que abre y cierra la pausa, con la sintaxis del Input System (ej: <Keyboard>/escape). La acción es propia de este script y no depende del PlayerInput.")]
    [SerializeField] private string pauseBinding = "<Keyboard>/escape";

    private InputAction pauseAction;

    [Header("Paneles")]
    [Tooltip("Contenedor de todo el menú (fondo + paneles). Debe ser un objeto distinto al que tiene este script.")]
    [SerializeField] private GameObject pauseRoot;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject audioAndVideoPanel;
    [SerializeField] private GameObject controlsPanel;

    [Header("Panel Settings")]
    [SerializeField] private Button audioAndVideoButton;
    [SerializeField] private Button controlsButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;

    [Header("Audio y video")]
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Button audioAndVideoBackButton;

    [Header("Controles")]
    [SerializeField] private Button controlsBackButton;

    private bool isPaused;

    public bool IsPaused => isPaused;

    private void OnEnable()
    {
        pauseAction = new InputAction("Pause", InputActionType.Button, pauseBinding);
        pauseAction.performed += OnPausePerformed;
        pauseAction.Enable();
    }

    private void OnDisable()
    {
        if (pauseAction == null)
            return;

        pauseAction.performed -= OnPausePerformed;
        pauseAction.Disable();
        pauseAction.Dispose();
        pauseAction = null;
    }

    private void Start()
    {
        Connect(audioAndVideoButton, OpenAudioAndVideo);
        Connect(controlsButton, () => ShowPanel(controlsPanel));
        Connect(resumeButton, Resume);
        Connect(restartButton, RestartLevel);
        Connect(mainMenuButton, GoToMainMenu);

        Connect(audioAndVideoBackButton, () => ShowPanel(settingsPanel));
        Connect(controlsBackButton, () => ShowPanel(settingsPanel));

        if (fullscreenToggle != null)
        {
            fullscreenToggle.SetIsOnWithoutNotify(Screen.fullScreen);
            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        }

        if (pauseRoot == null)
        {
            Debug.LogError("Falta asignar pauseRoot en el Inspector de PauseMenu.", this);
            return;
        }

        // El menú arranca oculto
        pauseRoot.SetActive(false);
    }

    private void OnDestroy()
    {
        // Por si la escena se destruye estando en pausa
        if (isPaused)
            Time.timeScale = 1f;
    }

    private void OnPausePerformed(InputAction.CallbackContext context)
    {
        if (!isPaused)
        {
            Pause();
            return;
        }

        // Ya en pausa: Esc primero vuelve de Audio/Video o Controles al panel Settings,
        // y recién desde ahí reanuda la partida.
        if (IsPanelOpen(audioAndVideoPanel) || IsPanelOpen(controlsPanel))
        {
            ShowPanel(settingsPanel);
        }
        else
        {
            Resume();
        }
    }

    /// <summary>Abre la pausa. También se puede conectar a un botón en pantalla.</summary>
    public void Pause()
    {
        if (isPaused)
            return;

        // No se puede pausar si el nivel ya terminó (pop-up de victoria o derrota)
        if (LevelManager.Instance != null && !LevelManager.Instance.IsRunning)
            return;

        if (pauseRoot == null)
        {
            Debug.LogError("Falta asignar pauseRoot en el Inspector de PauseMenu.", this);
            return;
        }

        isPaused = true;
        Time.timeScale = 0f;

        pauseRoot.SetActive(true);
        ShowPanel(settingsPanel);
    }

    /// <summary>Cierra la pausa y sigue el juego.</summary>
    public void Resume()
    {
        if (!isPaused)
            return;

        isPaused = false;
        Time.timeScale = 1f;

        if (pauseRoot != null)
            pauseRoot.SetActive(false);
    }

    private void RestartLevel()
    {
        if (LevelManager.Instance == null)
        {
            Debug.LogError("No hay un LevelManager en la escena para reiniciar el nivel.", this);
            return;
        }

        // LevelManager restaura el timeScale y guarda antes de cargar la escena
        LevelManager.Instance.RetryLevel();
    }

    private void GoToMainMenu()
    {
        if (LevelManager.Instance == null)
        {
            Debug.LogError("No hay un LevelManager en la escena para volver al menú principal.", this);
            return;
        }

        // Usa el campo "Menu Scene Name" del LevelManager
        LevelManager.Instance.GoToMenu();
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
                "Falta asignar el panel en el Inspector de PauseMenu.",
                this
            );
            return;
        }

        SetPanel(settingsPanel, target);
        SetPanel(audioAndVideoPanel, target);
        SetPanel(controlsPanel, target);
    }

    private void SetPanel(GameObject panel, GameObject target)
    {
        if (panel != null)
            panel.SetActive(panel == target);
    }

    private bool IsPanelOpen(GameObject panel)
    {
        return panel != null && panel.activeSelf;
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
}