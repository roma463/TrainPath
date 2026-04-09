using UnityEngine;

namespace _Train.Scripts.Train
{
    public class TrainFuelView : MonoBehaviour
    {
        [SerializeField] private ObjectSlider slider;
        [SerializeField] private TrainFuel trainFuel;

        private void Start()
        {
            slider.SetValue(1);
            
            trainFuel.OnSpend += OnChangeFuel;
        }

        private void OnDestroy()
        {
            trainFuel.OnSpend -= OnChangeFuel;
        }

        private void OnChangeFuel(float value)
        {
            slider.SetValue(value / trainFuel.MaximumFuel);
        }
    }
}