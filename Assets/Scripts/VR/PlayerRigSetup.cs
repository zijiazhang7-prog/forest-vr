using UnityEngine;

namespace ForestVR
{
    public class PlayerRigSetup : MonoBehaviour
    {
        public static PlayerRigSetup Instance { get; private set; }

        [SerializeField] private bool inputLocked;

        public bool IsInputLocked => inputLocked;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void SetInputLocked(bool locked)
        {
            inputLocked = locked;
        }
    }
}
