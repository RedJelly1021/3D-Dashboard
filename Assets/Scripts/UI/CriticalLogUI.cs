using UnityEngine;
using TMPro;

public class CriticalLogUI : MonoBehaviour
{
    public static CriticalLogUI Instance { get; private set; }

    public Transform logContainer; // Right Panel Scroll View Content
    public GameObject logItemPrefab; // CriticalLogItem Prefab
    public int maxLogCount = 30; // log count limit

    void Awake() => Instance = this;

    public void AddEntry(string zoneId, ZoneStatus status, float value)
    {
        GameObject item = Instantiate(logItemPrefab, logContainer);
        TMP_Text[] texts = item.GetComponentsInChildren<TMP_Text>();
        texts[0].text = $"{System.DateTime.Now:HH:mm:ss}"; // time
        texts[1].text = zoneId; // Zone ID
        texts[2].text = status.ToString(); // status
        texts[3].text = value.ToString("F1"); // value

        item.transform.SetAsFirstSibling(); // Move the new log up

        // log count limit check
        if (logContainer.childCount > maxLogCount)
        {
            Destroy(logContainer.GetChild(logContainer.childCount - 1).gameObject);
        }
    }
}
