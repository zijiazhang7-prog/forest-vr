using UnityEngine;

namespace ForestVR
{
    public class PlayerCollisionLayerSetup : MonoBehaviour
    {
        [SerializeField] private string playerLayerName = "Player";
        [SerializeField] private string ignoredLayerName = "NoPlayerCollision";

        private void Awake()
        {
            int playerLayer = LayerMask.NameToLayer(playerLayerName);
            int ignoredLayer = LayerMask.NameToLayer(ignoredLayerName);

            if (playerLayer >= 0)
            {
                gameObject.layer = playerLayer;
            }

            if (playerLayer >= 0 && ignoredLayer >= 0)
            {
                Physics.IgnoreLayerCollision(playerLayer, ignoredLayer, true);
            }
        }
    }
}
