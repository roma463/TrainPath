using _Train.Scripts;
using _Train.Scripts.Root;
using UnityEngine;
using UnityEngine.UI;

public class CanisterFuel : Tool
{
    [SerializeField] private ObjectSlider slider;
    [SerializeField] private float fuelAmount;

    public float FuelAmount => fuelAmount;

    private void Start()
    {
        slider.Setup(0, fuelAmount);
        slider.SetValue(fuelAmount);
    }
    
    public void GetAmount(float amount)
    {
        fuelAmount -= amount;
        slider.SetValue(fuelAmount);
    }
}
