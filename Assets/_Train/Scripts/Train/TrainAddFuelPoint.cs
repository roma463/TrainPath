using _Train.Scripts.Character;
using _Train.Scripts.Root;
using UnityEngine;

namespace _Train.Scripts.Train
{
    public class TrainAddFuelPoint : MonoBehaviour, IInteractable
    {
        public Transform RootTransform { get; }

        [SerializeField] private TrainFuel fuel;
        
        public string GetPromt(CharacterContext character)
        {
            return "Add fuel point";
        }

        public bool CanInteract(CharacterContext character)
        {
            return character.GrabSystem.CurrentGrabObject is CanisterFuel;
        }

        public void Interact(CharacterContext character)
        {
            var canister = character.GrabSystem.CurrentGrabObject as CanisterFuel;

            if (canister != null)
            {
                fuel.Fill(canister.FuelAmount);
                canister.GetAmount(canister.FuelAmount);
            }
        }
    }
}
