using System;
using _Train.Scripts.Character;
using _Train.Scripts.Root;
using _Train.Scripts.Train.Motors;
using UnityEngine;

namespace _Train.Scripts.Train.Buttons
{
    public class MotorButton : MonoBehaviour, IInteractable, INotifyStateChanged
    {
        public event Action OnChange;

        [SerializeField] private TrainMotor motor;
    
        private bool isActive;
    
        public Transform RootTransform => transform;

        public string GetPromt(CharacterContext character)
        {
            return isActive? "Stop" : "Start";
        }

        public bool CanInteract(CharacterContext character)
        {
            return true;
        }

        public void Interact(CharacterContext character)
        {
            if (isActive)
                motor.DeactivateMotor();
            else
                motor.ActivateMotor();
            
            isActive = !isActive;
        
            OnChange?.Invoke();
        }
    }
}