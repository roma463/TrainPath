using UnityEngine;

namespace _Train.Scripts.Train
{
    public class ViewTemperature : MonoBehaviour
    {
        [SerializeField] private MotorTemperature temperature;
        [SerializeField] private ObjectSlider slider;
        
        private void Start()
        {
            slider.SetValue(1);
            
            temperature.OnTemperatureChanged += OnChangeFuel;
        }

        private void OnDestroy()
        {
            temperature.OnTemperatureChanged -= OnChangeFuel;
        }

        private void OnChangeFuel(float currentTemperature, float normalizedTemperature)
        {
            slider.SetValue(normalizedTemperature);
        }
    }
}