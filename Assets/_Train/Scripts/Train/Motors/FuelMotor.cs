using UnityEngine;

namespace _Train.Scripts.Train.Motors
{
    public class FuelMotor : TrainMotor
    {
        [SerializeField] private TrainFuel electroPower;
        [SerializeField] private float fillPowerSpeed;

        private void Update()
        {
            if (IsActive)
            {
                electroPower.Fill(Mathf.Lerp(0f, electroPower.MaximumFuel, fillPowerSpeed * Time.deltaTime));
            }
        }
    }
}