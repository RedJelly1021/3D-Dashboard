using UnityEngine;
using System.Collections.Generic;

public enum ZoneStatus { Normal, Warning, Critical }

public class Zone : MonoBehaviour
{

    public string zoneId;
    public ZoneStatus currentStatus = ZoneStatus.Normal;
    public float currentValue; // temperature or electric value

    public Queue<float> recentHistory = new Queue<float>();
    public const int MaxHistoryCount = 30; // only last 30 data points

    private Renderer _renderer;
    /*
    * Why use Material Property Block
    * Because if multiple prefabs share same material, can assign different colors to each instance.
    * Prevents the material from being duplicated and consuming more memory.(draw call management)
    */
    private MaterialPropertyBlock _mpb;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _mpb = new MaterialPropertyBlock();
        if (string.IsNullOrEmpty(zoneId))
            zoneId = gameObject.name;
    }

    public void UpdateStatus(float value, ZoneStatus status)
    {
        currentValue = value;

        bool enteringCritical = (status == ZoneStatus.Critical && currentStatus != ZoneStatus.Critical);

        currentStatus = status;
        ApplyColor();

        recentHistory.Enqueue(value);
        if (recentHistory.Count > MaxHistoryCount)
            recentHistory.Dequeue();

        if (enteringCritical)
            CriticalLogUI.Instance.AddEntry(zoneId, status, value);
    }

    private void ApplyColor()
    {
        Color c = currentStatus switch
        {
            ZoneStatus.Normal => Color.green,
            ZoneStatus.Warning => new Color(1f, 0.55f, 0f), // orange
            ZoneStatus.Critical => Color.red,
            _ => Color.gray,
        };

        _renderer.GetPropertyBlock(_mpb);
        _mpb.SetColor("_BaseColor", c); // URP Lit shader uses _BaseColor, Standard uses _Color
        _renderer.SetPropertyBlock(_mpb);
    }

    // On Mouse Down(), if there collider, automatically registers click event
    private void OnMouseDown()
    {
        ZoneManager.Instance.OnZoneClicked(this);
    }
}
