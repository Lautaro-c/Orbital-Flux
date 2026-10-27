using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ForceStrengthController : MonoBehaviour
{
    [Header("Ajuste con la rueda")]
    [SerializeField, Range(0.01f, 0.5f)] private float scrollStep = 0.1f;
    [SerializeField, Range(0f, 1f)] private float minStrength = 0.1f;
    [SerializeField] private bool invertScroll;

    [Header("Efecto real de la potencia")]
    [SerializeField, Min(1f)] private float responseExponent = 2f;

    public float PushStrength { get; private set; } = 1f;
    public float PullStrength { get; private set; } = 1f;

    public ForceDirection Selected { get; private set; } = ForceDirection.Push;

    public event Action OnChanged;

    private void Awake()
    {
        PushStrength = SaveManager.PushStrength;
        PullStrength = SaveManager.PullStrength;
    }

    private void OnDestroy()
    {
        SaveManager.Save();
    }

    public float GetStrength(ForceDirection direction)
    {
        switch (direction)
        {
            case ForceDirection.Push: return PushStrength;
            case ForceDirection.Pull: return PullStrength;
            default: return 0f;
        }
    }

    public float GetForceMultiplier(ForceDirection direction)
    {
        float value = GetStrength(direction);
        return Mathf.Pow(value, responseExponent);
    }

    public void SelectPush()
    {
        Select(ForceDirection.Push);
    }
    public void SelectPull()
    {
        Select(ForceDirection.Pull);
    }

    public void Select(ForceDirection direction)
    {
        if (direction == ForceDirection.Off || direction == Selected) return;
        Selected = direction;
        OnChanged?.Invoke();
    }

    public void OnScroll(InputAction.CallbackContext cb)
    {
        if (!cb.performed) return;

        float y = cb.ReadValue<Vector2>().y;
        if (Mathf.Approximately(y, 0f)) return;

        float notch = Mathf.Sign(y);
        if (invertScroll)
        {
            notch = -notch;
        }

        Adjust(notch * scrollStep);
    }

    private void Adjust(float delta)
    {
        float current = GetStrength(Selected);
        float next = Mathf.Clamp(current + delta, minStrength, 1f);
        if (Mathf.Approximately(next, current)) return;

        if (Selected == ForceDirection.Push)
        {
            PushStrength = next;
            SaveManager.PushStrength = next;
        }
        else
        {
            PullStrength = next;
            SaveManager.PullStrength = next;
        }

        OnChanged?.Invoke();
    }
}