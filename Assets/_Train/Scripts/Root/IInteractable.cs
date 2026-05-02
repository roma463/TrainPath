using _Train.Scripts.Character;
using UnityEngine;

namespace _Train.Scripts.Root
{
    public interface IInteractable
    {
        public string GetPromt(CharacterContext character);
        public bool CanInteract(CharacterContext character);
        public bool CanInteractWithTool(Tool tool) => false;
        public void InteractWithTool(CharacterContext characterContext, Tool tool) => Interact(characterContext);

        public void Interact(CharacterContext character);
    }
    
    public interface IToolInteractable : IInteractable
    {
        bool CanInteractWithTool(Tool tool);
        void InteractWithTool(Tool tool);
    }
}
