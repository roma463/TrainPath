using _Train.Scripts.Level.Items.Data;
using UnityEngine;

namespace _Train.Scripts.Root
{
    public class GameEntryPoint : MonoBehaviour
    {
        public static GameEntryPoint Instance { get; private set; }
        
        [SerializeField] private ItemsConfig itemsConfig;

        private INPUTE _inpute;
            
        public ItemsConfig ItemsConfig => itemsConfig;
        
        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(this);
            
            QualitySettings.vSyncCount = 1;
            Application.targetFrameRate = 80;
            _inpute = new INPUTE();
            _inpute.Enable();
        }

        private void Start()
        {
            // SceneLoader.Instance.StartLoadingGame();
        }

        private void OnDestroy()
        {
            _inpute.Disable();
        }
    }
}
