using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class ZoneListUI : MonoBehaviour
{
    public Transform listContainer; // Scroll View Content
    public GameObject listItemPrefab; // ZoneListItem Prefab

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PopulateList();
    }

    void PopulateList()
    {
        var sortedZones = ZoneManager.Instance.Zones.OrderBy(z => z.zoneId).ToList();

        foreach(var zone in sortedZones)
        {
            GameObject item = Instantiate(listItemPrefab, listContainer);
            TMP_Text label = item.GetComponentInChildren<TMP_Text>();
            label.text = zone.zoneId;
            Button btn = item.GetComponent<Button>();
            btn.onClick.AddListener(() => ZoneManager.Instance.OnZoneClicked(zone));
        }
    }
}
