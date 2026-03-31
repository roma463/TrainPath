using System;
using UnityEngine;

namespace _Train.Scripts.Train
{
    public class TrainFuel : MonoBehaviour
    {
        public event Action<float> OnSpend;
        
        [SerializeField] private float maximumFuel;
        [SerializeField] private float onePercent;
        [SerializeField] private TrainMotor motor;

        private float _currentFuel;
        
        public float MaximumFuel => maximumFuel;
        
        
        private void Start()
        {
            _currentFuel = maximumFuel;
            
            motor.OnStart += StartUse;
            motor.OnStop += StopUse;
        }

        private void OnDestroy()
        {
            motor.OnStart -= StartUse;
            motor.OnStop -= StopUse;
        }

        private void StopUse()
        {
            
        }
        
        private void StartUse()
        {
            
        }

        public bool CanSpend(float amount)
        {
            return _currentFuel >= amount;
        }

        public void Spend(float amount)
        {
            if (!CanSpend(amount))
                Debug.LogError($"Can't spend {amount}");
            
            _currentFuel -= amount;
            _currentFuel = Mathf.Clamp(_currentFuel, 0, maximumFuel);
            
            OnSpend?.Invoke(_currentFuel);
        }
    }
}