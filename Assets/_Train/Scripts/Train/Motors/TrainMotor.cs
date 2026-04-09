using System;
using TMPro;
using UnityEngine;

namespace _Train.Scripts.Train.Motors
{
    public class TrainMotor : MonoBehaviour
    {
        public event Action<float> OnUsed;
        
        public event Action OnStart;
        public event Action OnStop;
        
        [SerializeField] private float speedChange;
        [SerializeField] private float speedSpendPowerByDisable;
        [SerializeField] private float fuelConsumption;
        [SerializeField] private TrainFuel trainFuel;
        [Header("Debug")] 
        [SerializeField] private TMP_Text viewSpeed;

        private float _targetPower;
        private float _currentMaxPower = MAX_POWER;
        
        private const float MAX_POWER = 100f;
        
        public float CurrentPower { get; private set; }
        public float NormalPower => CurrentPower / MAX_POWER;
        public bool IsActive { get; private set; }
        
        public void SetTargetPower(float power)
        {
            _targetPower = Mathf.Clamp(power, 0, _currentMaxPower);
        }

        private void FixedUpdate()
        { 
            if (IsActive)
            {
                if (!CanUpdateMotor())
                {
                    IsActive = false;
                }
                else
                {
                    trainFuel.Spend(NormalPower * fuelConsumption * Time.fixedDeltaTime);
                    if (!Mathf.Approximately(CurrentPower, _targetPower))
                    {
                        CurrentPower = Mathf.MoveTowards(CurrentPower, _targetPower, speedChange * Time.fixedDeltaTime);
                    }
                }
                
                OnUsed?.Invoke(NormalPower);
            }
            else if (NormalPower != 0)
            {
                CurrentPower = Mathf.MoveTowards(CurrentPower, 0, speedSpendPowerByDisable * Time.deltaTime);
            }
            
            viewSpeed.text = $"current power: <color=yellow>{(float)Math.Round(CurrentPower, 1)}</color>";
        }

        public void ActivateMotor()
        {
            IsActive = true;
        }

        public void DeactivateMotor()
        {
            IsActive = false;
        }

        private bool CanUpdateMotor()
        {
            return trainFuel.CanSpend(NormalPower * fuelConsumption);
        }
    }
}