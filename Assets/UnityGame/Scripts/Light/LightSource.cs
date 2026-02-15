using UnityEngine;

public class LightSource : MonoBehaviour
{
    private float _range;
    public float range
    {
        get { return _range; }
        set
        {
            _range = value;
            transform.localScale = Vector3.one * (_range * 2f);
        }
    }
}
