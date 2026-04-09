using System;
using _Train.Scripts.Root;
using UnityEngine;

namespace _Train.Scripts.Train.Buttons
{
    public class MoveButton : MonoBehaviour, IInteractable, INotifyStateChanged
    {
        public event Action OnChange;

        [SerializeField] private TrainMotion train;
        [SerializeField] private float targetSpeed = 10f;
    
        private bool isMoving;
    
        public Transform RootTransform => transform;
    
        private void Awake()
        {
            if (!train) 
                GetComponentInParent<TrainMotion>();
        }

        public string GetPromt(Character.Character character)
        {
            return isMoving? "Stop" : "Start";   
        }

        public bool CanInteract(Character.Character character)
        {
            return true;
        }

        public void Interact(Character.Character character)
        {
            isMoving = !isMoving;
        
            if (isMoving)
                train.SetTargetPower(targetSpeed);
            else
                train.SetTargetPower(0f);
        
            OnChange?.Invoke();
        }
    }
}
