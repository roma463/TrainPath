using System;
using _Train.Scripts.Character;
using UnityEngine;

namespace _Train.Scripts.Root
{
    public class InteractableObject : MonoBehaviour, IInteractable, INotifyStateChanged
    {
        public event Action OnInteracting;
        public event Action OnChange;
        
        private string _promt;
        private bool _canInteract;
        
        public void SetPromt(string promt)
        {
            _promt = promt;       
            OnChange?.Invoke();
        }

        public void SetCanInteract(bool canInteract)
        {
            _canInteract = canInteract;
        }
        
        public string GetPromt(CharacterContext character)
        {
            return _promt;
        }

        public bool CanInteract(CharacterContext character)
        {
            return _canInteract;
        }

        public void Interact(CharacterContext character)
        {
            OnInteracting?.Invoke();
        }
    }
}