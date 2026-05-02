using UnityEngine;

public class LineEqulizer : MonoBehaviour
{
    [SerializeField] private Transform _viewSound;
    [SerializeField] private Transform _trailSound;

    [SerializeField] private float _speedMoveLine;
    [SerializeField] private float _speedMoveTrail;

    public void UpdateView(float delta)
    {
        var scale = new Vector3(delta, _viewSound.localScale.y, _viewSound.localScale.z);
        _viewSound.transform.localScale = Vector3.MoveTowards(_viewSound.transform.localScale, scale, _speedMoveLine * Time.deltaTime);

        // if (_trailSound.localScale.x < _viewSound.localScale.x)
        // {
        //     _trailSound.localScale = _viewSound.transform.localScale;
        // }
        // else
        // {
        //     _trailSound.localScale = Vector3.MoveTowards(_trailSound.transform.localScale, new Vector3(0, _trailSound.localScale.y, _trailSound.localScale.z), _speedMoveTrail * Time.deltaTime);
        // }
    }
}
