using _Train.Scripts.Root;
using UnityEngine;

namespace _Train.Scripts.Level
{
    public class Door : MonoBehaviour
    {
        [SerializeField] private InteractableObject interactableObject;
        [SerializeField] private Transform door;
        [SerializeField] private Vector3 openAngle;
        [SerializeField] private string open, closed;
    
        private bool _doorOpen;
        private Quaternion _doorOpenRotation;
        private Quaternion _doorClosedRotation;
    
        private void Start()
        {
            _doorClosedRotation = door.localRotation;
            _doorOpenRotation = Quaternion.Euler(openAngle);
        
            interactableObject.SetPromt(open);
            interactableObject.SetCanInteract(true);
        
            interactableObject.OnInteracting += OnInteract;
        }

        private void OnDestroy()
        {
            interactableObject.OnInteracting -= OnInteract;
        }

        private void OnInteract()
        {
            _doorOpen = !_doorOpen;
            interactableObject.SetPromt(_doorOpen? closed : open);
            door.localRotation = _doorOpen ? _doorOpenRotation : _doorClosedRotation;
        }
    }
}
