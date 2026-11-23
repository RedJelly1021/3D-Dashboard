using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class FilterButton : MonoBehaviour
{
    public bool showAll = true; 
    public ZoneStatus status = ZoneStatus.Normal; // Use this if showAll is false
    public ZoneListUI zoneListUI; // Reference to the ZoneListUI script

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        if (zoneListUI == null)
        {
            Debug.LogError("ZoneListUI reference is not set in FilterButton.");
            return;
        }

        if (showAll)
            zoneListUI.SetFilter(null);
        else
            zoneListUI.SetFilter(status);
    }
}
