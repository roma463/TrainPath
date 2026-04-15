using System;
using UnityEngine;

namespace _Train.Scripts.Train.Motors
{
    public class FuelMotor : TrainMotor
    {
        [SerializeField] private TrainFuel electroPower;
        [SerializeField] private Engine[] engines;
        [SerializeField] private float fillPowerSpeed;

        private void Start()
        {
            foreach (var engine in engines)
            {
                engine.UpdatedPercent += UpdateClampedPower;
            }
        }

        private void OnDestroy()
        {
            foreach (var engine in engines)
            {
                engine.UpdatedPercent -= UpdateClampedPower;
            }
        }

        private void Update()
        {
            if (IsActive)
            {
                electroPower.Fill(Mathf.Lerp(0f, electroPower.MaximumFuel, fillPowerSpeed * Time.deltaTime));
            }
        }

        public override float GetCurrentMaxPower()
        {
            var result = 0f;

            foreach (var engine in engines)
            {
                result += engine.CurrentPower / engines.Length;
            }
            
            return result;
        }
    }
}