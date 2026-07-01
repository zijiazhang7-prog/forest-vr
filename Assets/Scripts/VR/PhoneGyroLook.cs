using UnityEngine;

namespace ForestVR
{
    public class PhoneGyroLook : MonoBehaviour
    {
        [Header("Rig")]
        [SerializeField] private Transform yawRoot;
        [SerializeField] private bool splitYawToRoot = true;
        [SerializeField] private bool removeRoll = true;

        [Header("Gyro Correction")]
        [SerializeField] private Vector3 deviceCorrectionEuler = new Vector3(90f, 0f, 0f);
        [SerializeField] private float minPitch = -80f;
        [SerializeField] private float maxPitch = 80f;

        private bool gyroReady;
        private Quaternion yawRecenter = Quaternion.identity;

        public Transform YawRoot
        {
            get => yawRoot;
            set => yawRoot = value;
        }

        private void Awake()
        {
            if (yawRoot == null && transform.parent != null)
            {
                yawRoot = transform.parent;
            }
        }

        private void OnEnable()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            gyroReady = SystemInfo.supportsGyroscope;
            if (gyroReady)
            {
                Input.gyro.enabled = true;
                Input.gyro.updateInterval = 1f / 60f;
            }
#else
            gyroReady = false;
#endif
        }

        private void Start()
        {
            Recenter();
        }

        private void Update()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (!gyroReady)
            {
                return;
            }

            if (ForestVRInput.RecenterPressedThisFrame())
            {
                Recenter();
            }

            ApplyGyroRotation();
#endif
        }

        public void Recenter()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (!gyroReady)
            {
                return;
            }

            Quaternion corrected = GetCorrectedGyroRotation();
            float yaw = corrected.eulerAngles.y;
            yawRecenter = Quaternion.Euler(0f, -yaw, 0f);
#else
            yawRecenter = Quaternion.identity;
#endif
        }

        private void ApplyGyroRotation()
        {
            Quaternion rotation = yawRecenter * GetCorrectedGyroRotation();
            Vector3 euler = rotation.eulerAngles;

            float pitch = Mathf.Clamp(NormalizeAngle(euler.x), minPitch, maxPitch);
            float yaw = euler.y;
            float roll = removeRoll ? 0f : NormalizeAngle(euler.z);

            if (splitYawToRoot && yawRoot != null)
            {
                yawRoot.rotation = Quaternion.Euler(0f, yaw, 0f);
                transform.localRotation = Quaternion.Euler(pitch, 0f, roll);
            }
            else
            {
                transform.localRotation = Quaternion.Euler(pitch, yaw, roll);
            }
        }

        private Quaternion GetCorrectedGyroRotation()
        {
            Quaternion attitude = Input.gyro.attitude;

            // Unity and Android sensor coordinates use different handedness.
            Quaternion unityGyro = new Quaternion(attitude.x, attitude.y, -attitude.z, -attitude.w);
            return Quaternion.Euler(deviceCorrectionEuler) * unityGyro;
        }

        private static float NormalizeAngle(float angle)
        {
            return angle > 180f ? angle - 360f : angle;
        }
    }
}
