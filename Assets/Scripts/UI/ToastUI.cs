using UnityEngine;
using System.Collections.Generic;

public class ToastUI : MonoBehaviour
{
    public static ToastUI Instance { get; private set; }

    public Transform toastContainer;
    public GameObject toastPrefab;
    public float displayDuration = 2.5f; 
    public float fadeDuration = 0.4f;
    public int maxVisibleToasts = 3;

    private List<ToastItem> _activeToasts = new List<ToastItem>();

    void Awake() => Instance = this;

    public void ShowToast(string zoneId, ZoneStatus status, float value)
    {
        // if too much toasts, request remove the oldest (processes all multiple requests come at once using while)
        while (_activeToasts.Count >= maxVisibleToasts)
        {
            ToastItem oldest = _activeToasts[0];
            _activeToasts.RemoveAt(0);
            if (oldest != null)
            {
                oldest.DismissImmediately(fadeDuration);
            }
        }

        GameObject toast = Instantiate(toastPrefab, toastContainer);
        ToastItem item = toast.GetComponent<ToastItem>();
        string message = $"{zoneId} 위험 상태 진입 (값: {value:F1})";
        item.Setup(message, displayDuration, fadeDuration);

        _activeToasts.Add(item);
    }
}
