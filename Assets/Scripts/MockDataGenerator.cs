using System.Text.Json;
using UnityEngine;

public class MockDataGenerator : MonoBehaviour
{
    [Range(0.5f, 5f)] public float updateInterval = 1f;
    private float _timer;
    
    // Update is called once per frame
    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= updateInterval) 
        {
            _timer = 0f;
            GenerateAndBroadcast();
        }
    }

    void GenerateAndBroadcast()
    {
        foreach (var zone in ZoneManager.Instance.Zones) 
        {
            // create natural transitions with sine waves and noise
            // The key point is each zone is offset by zoneId.GetHashCode(), so not all zones appear to have same value at same time   
            float t = Time.time + zone.zoneId.GetHashCode() % 100;
            float baseValue = 25f + Mathf.Sin(t * 0.1f) * 10f;
            float noise = Random.Range(-3f, 3f);
            float value = baseValue + noise;

            ZoneStatus status = value switch
            {
                < 22f => ZoneStatus.Normal,
                < 32f => ZoneStatus.Warning,
                _ => ZoneStatus.Critical,
            };

            zone.UpdateStatus(value, status);
        }
    }
}
