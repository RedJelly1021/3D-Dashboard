using UnityEngine;
using System.Collections.Generic;

public class ZoneManager : MonoBehaviour
{
    public static ZoneManager Instance { get; private set; }
    public List<Zone> Zones = new List<Zone>();

    private void Awake()
    {
        Instance = this;
        Zones.AddRange(FindObjectsOfType<Zone>()); // TODO: look for another way to get all zones
    }

    public void OnZoneClicked(Zone zone)
    {
        CameraController.Instance.FocusOnZone(zone.transform);
        DetailPanelUI.Instance.ShowZoneDetail(zone);
    }
}
