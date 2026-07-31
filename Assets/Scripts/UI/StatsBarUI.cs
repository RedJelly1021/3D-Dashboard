using UnityEngine;
using System.Linq;
using TMPro;

public class StatsBarUI : MonoBehaviour
{
    public TMP_Text totalCount;
    public TMP_Text normalCount;
    public TMP_Text warningCount;
    public TMP_Text criticalCount;
    public TMP_Text averageValue;

    private float _timer;
    public float refreshInterval = 0.5f; // 매 프레임 갱신은 낭비

    // Update is called once per frame
    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= refreshInterval) 
        {
            _timer = 0f;
            RefreshStats();
        }
    }

    void RefreshStats()
    {
        var zones = ZoneManager.Instance.Zones;
        if (zones.Count == 0) return;

        int normal = zones.Count(z => z.currentStatus == ZoneStatus.Normal);
        int warning = zones.Count(z => z.currentStatus == ZoneStatus.Warning);
        int critical = zones.Count(z => z.currentStatus == ZoneStatus.Critical);
        float average = zones.Average(z => z.currentValue);

        totalCount.text = $"전체: {zones.Count}";
        normalCount.text = $"<color=green>정상: {normal}</color>";
        warningCount.text = $"<color=orange>경고: {warning}</color>";
        criticalCount.text = $"<color=red>위험: {critical}</color>";
        averageValue.text = $"평균: {average:F1}";
    }
}
