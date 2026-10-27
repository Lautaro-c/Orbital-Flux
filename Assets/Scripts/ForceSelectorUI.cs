using UnityEngine;
using UnityEngine.UI;

public class ForceSelectorUI : MonoBehaviour
{
    [SerializeField] private ForceStrengthController controller;

    [Header("Repulsión (Push)")]
    [SerializeField] private Button pushButton;
    [SerializeField] private Image pushFill;
    [SerializeField] private GameObject pushSelectedMarker;

    [Header("Atracción (Pull)")]
    [SerializeField] private Button pullButton;
    [SerializeField] private Image pullFill;
    [SerializeField] private GameObject pullSelectedMarker;

    private void Awake()
    {
        pushButton.onClick.AddListener(controller.SelectPush);
        pullButton.onClick.AddListener(controller.SelectPull);
    }

    private void OnEnable()
    {
        controller.OnChanged += Refresh;
    }

    private void OnDisable()
    {
        controller.OnChanged -= Refresh;
    }

    private void Start()
    {
        Refresh();
    }

    private void Refresh()
    {
        pushFill.fillAmount = controller.PushStrength;
        pullFill.fillAmount = controller.PullStrength;

        pushSelectedMarker.SetActive(controller.Selected == ForceDirection.Push);
        pullSelectedMarker.SetActive(controller.Selected == ForceDirection.Pull);
    }
}