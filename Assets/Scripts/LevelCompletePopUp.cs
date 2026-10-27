using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelCompletePopup : MonoBehaviour
{
    [Header("Estrellas")]
    [SerializeField] private Image[] starImages;
    [SerializeField] private Sprite starOn;
    [SerializeField] private Sprite starOff;

    [Header("Datos")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text integrityText;
    [SerializeField] private GameObject newRecordLabel;

    [Header("Botones")]
    [SerializeField] private GameObject nextButton;

    public void Show(LevelResult result, bool newBest, bool hasNextLevel)
    {
        gameObject.SetActive(true);

        for (int i = 0; i < starImages.Length; i++)
        {
            if (i < result.stars)
            {
                starImages[i].sprite = starOn;
            }
            else
            {
                starImages[i].sprite = starOff;
            }
        }

        if (scoreText != null) scoreText.text = result.score.ToString();
        if (timeText != null) timeText.text = FormatTime(result.time);
        if (integrityText != null) integrityText.text = $"{result.integrityPercent:0}%";
        if (newRecordLabel != null) newRecordLabel.SetActive(newBest);
        if (nextButton != null) nextButton.SetActive(hasNextLevel);
    }

    private static string FormatTime(float seconds)
    {
        int minutes = (int)(seconds / 60f);
        float rest = seconds - minutes * 60f;
        return $"{minutes:00}:{rest:00.0}";
    }
}