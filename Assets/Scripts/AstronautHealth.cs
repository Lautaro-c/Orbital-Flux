using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AstronautHealth : MonoBehaviour
{
    public static readonly List<AstronautHealth> All = new List<AstronautHealth>();

    [SerializeField] private float maxIntegrity = 100f;

    [Header("Cálculo de daño")]
    [SerializeField] private float minImpactSpeed = 3f;
    [SerializeField] private float damageScale = 1f;
    [SerializeField, Min(1f)] private float damageExponent = 2f;
    [SerializeField] private float invulnerabilityOnStart = 0.5f;

    [Header("Otros")]
    [Tooltip("Parte del cuerpo que cuenta para llegar a la salida. Si está vacío se usa el Rigidbody más pesado (torso/pelvis).")]
    [SerializeField] private Rigidbody coreBody;
    [Tooltip("Muestra en consola cada impacto: útil para calibrar los valores.")]
    [SerializeField] private bool logImpacts;
    [SerializeField] private TextMeshProUGUI percentText;

    private float integrity;
    private float pendingDamage;
    private float invulnerableUntil;
    private bool forcedInvulnerable;

    public event Action<float> OnIntegrityChanged;
    public event Action<AstronautHealth> OnDied;

    public float IntegrityPercent => integrity / maxIntegrity * 100f;
    public float TotalDamagePercent => 100f - IntegrityPercent;
    public bool IsDead => integrity <= 0f;
    public Rigidbody CoreBody => coreBody;

    private bool CanTakeDamage => !IsDead && !forcedInvulnerable && Time.time >= invulnerableUntil;


    private void Awake()
    {
        integrity = maxIntegrity;
        invulnerableUntil = Time.time + invulnerabilityOnStart;

        Rigidbody heaviest = null;
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            if (heaviest == null || rb.mass > heaviest.mass) heaviest = rb;

            LimbImpactReporter reporter = rb.GetComponent<LimbImpactReporter>();
            if (reporter == null) reporter = rb.gameObject.AddComponent<LimbImpactReporter>();
            reporter.Init(this);
        }

        if (coreBody == null) coreBody = heaviest;
        percentText.text = "Integrity: " + IntegrityPercent + "%";
    }

    private void OnEnable()
    {
        All.Add(this);
    }
    private void OnDisable()
    {
        All.Remove(this);
    }

    public void ReportImpact(float relativeSpeed, float limbMultiplier = 1f)
    {
        if (!CanTakeDamage || relativeSpeed <= minImpactSpeed) return;

        float damage = Mathf.Pow(relativeSpeed - minImpactSpeed, damageExponent) * damageScale * limbMultiplier;

        if (logImpacts)
            Debug.Log($"[{name}] Impacto {relativeSpeed:F1} m/s -> daño {damage:F1}");
        pendingDamage = Mathf.Max(pendingDamage, damage);
    }

    private void FixedUpdate()
    {
        if (pendingDamage <= 0f) return;

        float damage = pendingDamage;
        pendingDamage = 0f;
        ApplyDamage(damage);
    }

    private void ApplyDamage(float damage)
    {
        if (!CanTakeDamage) return;

        integrity = Mathf.Max(0f, integrity - damage);
        OnIntegrityChanged?.Invoke(IntegrityPercent);
        percentText.text = "Integrity: " + IntegrityPercent.ToString("F2") + "%";

        if (IsDead) OnDied?.Invoke(this);
    }

    public void SetInvulnerable(bool value)
    {
        forcedInvulnerable = value;
        if (value) pendingDamage = 0f;
    }
}