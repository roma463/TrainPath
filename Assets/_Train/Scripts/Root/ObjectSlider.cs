using UnityEngine;

public class ObjectSlider : MonoBehaviour
{
    [SerializeField] private float min = 0, max = 1;
    [SerializeField] private Transform view;
    [SerializeField, Range(0, 1)] private float sliderValue;

    private void OnValidate()
    {
        if(view == null)
            return;

        SetValue(sliderValue);

    }

    public void Setup(float min, float max)
    {
        this.min = min;
        this.max = max;
    }
    
    public void SetValue(float value)
    {
        value = Mathf.Clamp(value, min, max);
        float normalizedValue = Mathf.Lerp(min, max, value);
        view.localScale = new Vector3(view.localScale.x, normalizedValue / max, view.localScale.z);
    }
}
