using System;
using _Train.Scripts.Character;
using _Train.Scripts.Root;
using _Train.Scripts.Train;
using UnityEngine;

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

    public string GetPromt(Character character)
    {
        return isMoving? "Stop" : "Start";   
    }

    public bool CanInteract(Character character)
    {
        return true;
    }

    public void Interact(Character character)
    {
        isMoving = !isMoving;
        
        if (isMoving)
            train.SetTargetPower(targetSpeed);
        else
            train.SetTargetPower(0f);
        
        OnChange?.Invoke();
    }
}
