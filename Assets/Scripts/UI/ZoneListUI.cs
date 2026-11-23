using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class ZoneListUI : MonoBehaviour
{
    public Transform listContainer; // Scroll View Content
    public GameObject listItemPrefab; // ZoneListItem Prefab
    public TMP_InputField searchInput;

    private ZoneStatus? _statusFilter = null; // If null, display All
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

    // methods to connnected th filter button
    public void SetFilter(ZoneStatus? status)
    {
        _statusFilter = status;
        RefreshList();
    }

    void OnSearchChanged(string keyword)
    {
        _searchKeyword = keyword;
        RefreshList();
    }

    void RefreshList()
    {
        // Clear existing items
        foreach (Transform child in listContainer)
            Destroy(child.gameObject);

        // Filter, Search and Sort
        var filtered = ZoneManager.Instance.Zones
            .Where(z => _statusFilter == null || z.currentStatus == _statusFilter)
            .Where(z => string.IsNullOrEmpty(_searchKeyword) || z.zoneId.ToLower().Contains(_searchKeyword.ToLower()))
            .OrderBy(z => z.zoneId)
            .ToList();

        foreach(var zone in filtered)
        {
            GameObject item = Instantiate(listItemPrefab, listContainer);
            TMP_Text label = item.GetComponentInChildren<TMP_Text>();
            label.text = zone.zoneId;
            Button btn = item.GetComponent<Button>();
            btn.onClick.AddListener(() => ZoneManager.Instance.OnZoneClicked(zone));
        }
    }
}
