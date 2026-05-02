using System;
using _Train.Scripts.Root;
using _Train.Scripts.UI;
using Mirror;
using UnityEngine;

namespace _Train.Scripts.Character
{
    public class NewInteractionSystem : MonoBehaviour
    {
        [SerializeField] private CharacterContext character;
        [SerializeField] private float interactDistance = 5f;
        [SerializeField] private LayerMask interactableLayer;
        [SerializeField] private InteractionSystem interactionSystem;
        
        private INPUTE _input;
        
        private IInteractable _currentInteractable;
        private IHoverable _currentHoverable;
        private INotifyStateChanged _currentNotifyStateChangedObject;
        private Tool _currentTool;

        private void Start()
        {
            _input = INPUTE.instance;
            interactionSystem.OnInteractionStart += OnInteractionStarted;
            interactionSystem.OnInteractionStop += OnInteractionStopped;
            _input.OnPerformedGrab += OnPerformedGrabButton;
            character.GrabSystem.OnItemGrabbed += OnGrabbedItem;
            _input.OnLeftMouseButtonPerformed += TryStartUseTool;
            _input.OnLeftMouseButtonCanceled += TryStopUseTool;
        }

        private void OnDestroy()
        {
            interactionSystem.OnInteractionStart -= OnInteractionStarted;
            interactionSystem.OnInteractionStop -= OnInteractionStopped;
            _input.OnPerformedGrab -= OnPerformedGrabButton;
            character.GrabSystem.OnItemGrabbed -= OnGrabbedItem;
            _input.OnLeftMouseButtonPerformed -= TryStartUseTool;
            _input.OnLeftMouseButtonCanceled -= TryStopUseTool;
        }

        private void OnInteractionStarted(GameObject detectedObject)
        {
            if (detectedObject.TryGetComponent(out IInteractable interactable))
            {
                if (interactable is INotifyStateChanged interactableChange)
                {
                     _currentNotifyStateChangedObject = interactableChange;
                     _currentNotifyStateChangedObject.OnChange += OnChangeCurrentObject;
                }
                
                InteractableView.Instance.Show(interactable.GetPromt(character));
                
                if (interactable.CanInteract(character))
                {
                    _currentInteractable = interactable; 
                }
            }
            else if (detectedObject.TryGetComponent(out IHoverable hoverable))
            {
                _currentHoverable = hoverable;
                _currentHoverable.OnHoverEnter();
            }
        }

        private void OnChangeCurrentObject()
        {
            InteractableView.Instance.Show(_currentInteractable.GetPromt(character));
        }

        private void OnInteractionStopped()
        {
            if (_currentInteractable != null)
            {
                InteractableView.Instance.Hide();

                if (_currentNotifyStateChangedObject != null)
                {
                    _currentNotifyStateChangedObject.OnChange -= OnChangeCurrentObject;
                    _currentNotifyStateChangedObject = null;
                }
                
                _currentInteractable = null;
            }

            if (_currentHoverable != null)
            {
                _currentHoverable.OnHoverExit();
                _currentHoverable = null;
            }
        }
        
        private void OnPerformedGrabButton()
        {
            if (_currentInteractable != null && _currentInteractable.CanInteract(character))
                _currentInteractable.Interact(character);
        }
        
        private void OnGrabbedItem(PickupObject pickupObject)
        {
            if (pickupObject.gameObject.TryGetComponent(out Tool tool))
            {
                _currentTool = tool;
            }
        }

        private void TryStartUseTool()
        {
            if (_currentTool == null || _currentInteractable == null)
                return;

            if (_currentTool.CanUse(_currentInteractable))
            {
                
            }
        }

        private void TryStopUseTool()
        {
            if (_currentTool == null)
                return;
        }
    }
}