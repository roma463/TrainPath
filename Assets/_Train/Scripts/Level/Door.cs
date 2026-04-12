using System.Collections;
using _Train.Scripts.Root;
using UnityEngine;

namespace _Train.Scripts.Level
{
    public class Door : MonoBehaviour
    {
        [SerializeField] private InteractableObject interactableObject;
        [SerializeField] private Transform door;
        [SerializeField] private Vector3 openAngle;
        [SerializeField] private float speedAnimation;
        [SerializeField] private string open, closed;
    
        private bool _doorOpen;
        private Quaternion _doorOpenRotation;
        private Quaternion _doorClosedRotation;
        
        private Coroutine _coroutine;
    
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
            StopAnimation();
            interactableObject.OnInteracting -= OnInteract;
        }

        private void OnInteract()
        {
            _doorOpen = !_doorOpen;
            
            interactableObject.SetPromt(_doorOpen ? closed : open);
            StopAnimation();
            _coroutine = StartCoroutine(Animation(_doorOpen ? _doorOpenRotation : _doorClosedRotation));
        }

        private void StopAnimation()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }
        }

        private IEnumerator Animation(Quaternion target)
        {
            while (door.localRotation != target)
            {
                door.localRotation = Quaternion.Lerp(door.localRotation, target, speedAnimation * Time.deltaTime);
                yield return null;
            }

            _coroutine = null;
        }
    }
}
