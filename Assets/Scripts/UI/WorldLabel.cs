using UnityEngine;
using TMPro;

[DefaultExecutionOrder(100)]
public class WorldLabel : MonoBehaviour
{
    public TMP_Text text;
    public float minDistance = 5f;
    public bool scaleWithDistance = true;
    public float scaleFactor = 0.05f; // factor to scale the label with distance

    private Transform _cam;
    private Transform _anchor; // zone's transform
    private Vector3 _offset; // world offset

    private void Awake()
    {
        if (text == null) text = GetComponent<TMP_Text>();
        _cam = Camera.main.transform;
        _anchor = transform.parent;
        _offset = transform.position - _anchor.position;
        transform.SetParent(null); // detach from parent to avoid scaling/rotation issues
    }

    public void SetText(string zoneId) => text.text = zoneId;
    
    private void LateUpdate()
    {
        if (_anchor == null) { Destroy(gameObject); return; }

        // billboard : always face the camera
        transform.position = _anchor.position + _offset;
        transform.rotation = _cam.rotation;

        // scale with distance
        if (scaleWithDistance)
        {
            float dist = Vector3.Distance(_cam.position, transform.position);
            float s = Mathf.Max(dist, minDistance) * scaleFactor;
            transform.localScale = Vector3.one * s;
        }
    }
}
