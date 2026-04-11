using _Train.Scripts.Character;
using UnityEngine;

namespace _Train.Scripts.Root
{
    public interface IInteractable
    {

        public string GetPromt(CharacterContext character);

        public bool CanInteract(CharacterContext character);

        public void Interact(CharacterContext character);
    }
}
