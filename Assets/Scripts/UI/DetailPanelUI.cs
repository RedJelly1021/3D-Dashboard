using UnityEngine;
using TMPro;

public class DetailPanelUI : MonoBehaviour
{
    public static DetailPanelUI Instance { get; private set; }

    public GameObject panelRoot;
    public TMP_Text zoneIdText;
    public TMP_Text valueText;
    public TMP_Text statusText;

    private Zone _currentZone; // ref to zone currently displayed panel

    void Awake() => Instance = this;

    public void ShowZoneDetail(Zone zone)
    {
        _currentZone = zone;
        panelRoot.SetActive(true);
        RefreshDisplay();
    }

    private void Update()
    {
        // if panel is open and there is current zone, update to latest value each frame
        // TODO: modify to update only when zone value changes, using event/callback from Zone class
        if (panelRoot.activeSelf && _currentZone != null)
        {
            RefreshDisplay();
        }
    }

    private void RefreshDisplay()
    {
        zoneIdText.text = $"구역 : {_currentZone.zoneId}";
        valueText.text = $"현재값: {_currentZone.currentValue:F1}";
        statusText.text = $"상태: {_currentZone.currentStatus}";
    }

    public void ClosePanel()
    {
        panelRoot.SetActive(false);
        _currentZone = null;
    }
}
