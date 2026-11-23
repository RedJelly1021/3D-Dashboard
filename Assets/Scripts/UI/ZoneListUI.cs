using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class ZoneListUI : MonoBehaviour
{
    public Transform listContainer; // Scroll View Content
    public GameObject listItemPrefab; // ZoneListItem Prefab
    public TMP_InputField searchInput;

    private string _searchKeyword = "";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (searchInput != null)
        {
            searchInput.onValueChanged.AddListener(OnSearchChanged);
        }
        RefreshList();
    }


    void OnSearchChanged(string keyword)
    {
        _searchKeyword = keyword;
        RefreshList();
    }

    void RefreshList()
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
