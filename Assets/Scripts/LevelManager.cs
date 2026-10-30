using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Nivel")]
    [SerializeField, Min(1)] private int levelNumber = 1;
    [SerializeField] private string nextSceneName;
    [SerializeField] private string menuSceneName = "MainMenu";

    [Header("Puntaje")]
    [SerializeField, Min(1f)] private float maxTime = 120f;

    [Header("Estrellas (1 por completar + 1 por cada objetivo)")]
    [SerializeField, Range(1f, 100f)] private float integrityForStar = 60f;
    [SerializeField] private float timeForStar = 90f;

    [Header("UI")]
    [SerializeField] private LevelCompletePopup completePopup;
    [SerializeField] private GameObject defeatPanel;

    private readonly HashSet<AstronautHealth> rescued = new HashSet<AstronautHealth>();
    private List<AstronautHealth> astronauts;
    private float elapsed;
    private bool running;

    public float ElapsedTime => elapsed;

    public bool IsRunning => running;

    private void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;
    }

    private void Start()
    {
        astronauts = new List<AstronautHealth>(AstronautHealth.All);
        foreach (AstronautHealth astronaut in astronauts)
        {
            astronaut.OnDied += HandleAstronautDied;
        }

        if (completePopup != null) completePopup.gameObject.SetActive(false);
        if (defeatPanel != null) defeatPanel.SetActive(false);

        running = true;
    }

    private void OnDestroy()
    {
        if (astronauts != null)
        {
            foreach (AstronautHealth astronaut in astronauts)
            {
                if (astronaut != null) astronaut.OnDied -= HandleAstronautDied;
            }
        }

        if (Instance == this) Instance = null;
    }

    private void Update()
    {
        if (running) elapsed += Time.deltaTime;
    }

    public void NotifyRescued(AstronautHealth astronaut)
    {
        if (!running || astronaut.IsDead || !rescued.Add(astronaut)) return;

        astronaut.SetInvulnerable(true);

        if (rescued.Count >= astronauts.Count)
            CompleteLevel();
    }

    private void CompleteLevel()
    {
        running = false;

        float integrity = AverageIntegrityPercent();
        int stars = CalculateStars(integrity, elapsed);
        int score = CalculateScore(integrity, elapsed);
        LevelResult result = new LevelResult(levelNumber, stars, elapsed, integrity, score);

        bool newBest = SaveManager.SubmitResult(result);
        SaveManager.UnlockLevel(levelNumber + 1);
        SaveManager.Save();

        Time.timeScale = 0f;

        if (completePopup != null)
            completePopup.Show(result, newBest, !string.IsNullOrEmpty(nextSceneName));
    }

    private void HandleAstronautDied(AstronautHealth astronaut)
    {
        if (!running) return;
        running = false;

        Time.timeScale = 0f;
        if (defeatPanel != null) defeatPanel.SetActive(true);
    }

    private int CalculateScore(float integrityPercent, float time)
    {
        float timeLeft = maxTime - time;
        if (timeLeft < 0f)
        {
            timeLeft = 0f;
        }

        return Mathf.RoundToInt(integrityPercent * timeLeft);
    }

    private int CalculateStars(float integrityPercent, float time)
    {
        int stars = 1;
        if (integrityPercent >= integrityForStar) stars++;
        if (time <= timeForStar) stars++;
        return stars;
    }

    private float AverageIntegrityPercent()
    {
        float sum = 0f;
        foreach (AstronautHealth astronaut in astronauts)
            sum += astronaut.IntegrityPercent;
        return sum / astronauts.Count;
    }

    public void RetryLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void NextLevel()
    {
        if (!string.IsNullOrEmpty(nextSceneName)) Load(nextSceneName);
    }

    public void GoToMenu()
    {
        Load(menuSceneName);
    }

    private void Load(string sceneName)
    {
        Time.timeScale = 1f;
        SaveManager.Save();
        SceneManager.LoadScene(sceneName);
    }
}