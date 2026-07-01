using UnityEngine;

namespace ForestVR
{
    [RequireComponent(typeof(Camera))]
    public class VrStereoCamera : MonoBehaviour
    {
        [Header("Mode")]
        [SerializeField] private bool forceStereoInEditor;

        [Header("Eyes")]
        [SerializeField] private Camera centerCamera;
        [SerializeField] private Camera leftEyeCamera;
        [SerializeField] private Camera rightEyeCamera;
        [SerializeField] private float interpupillaryDistance = 0.064f;

        [Header("Camera")]
        [SerializeField] private float fieldOfView = 90f;
        [SerializeField] private float nearClipPlane = 0.05f;
        [SerializeField] private float farClipPlane = 100f;

        private void Awake()
        {
            EnsureCameras();
            ApplyCameraSettings();
            ApplyMode();
        }

        private void OnValidate()
        {
            interpupillaryDistance = Mathf.Max(0.01f, interpupillaryDistance);
            fieldOfView = Mathf.Clamp(fieldOfView, 60f, 110f);
            nearClipPlane = Mathf.Max(0.01f, nearClipPlane);
            farClipPlane = Mathf.Max(nearClipPlane + 1f, farClipPlane);
        }

        private void LateUpdate()
        {
            ApplyMode();
        }

        private void EnsureCameras()
        {
            if (centerCamera == null)
            {
                centerCamera = GetComponent<Camera>();
            }

            if (leftEyeCamera == null)
            {
                leftEyeCamera = FindChildCamera("LeftEyeCamera") ?? CreateEyeCamera("LeftEyeCamera");
            }

            if (rightEyeCamera == null)
            {
                rightEyeCamera = FindChildCamera("RightEyeCamera") ?? CreateEyeCamera("RightEyeCamera");
            }
        }

        private Camera FindChildCamera(string childName)
        {
            Transform child = transform.Find(childName);
            return child != null ? child.GetComponent<Camera>() : null;
        }

        private Camera CreateEyeCamera(string childName)
        {
            GameObject eyeObject = new GameObject(childName);
            eyeObject.transform.SetParent(transform, false);
            Camera eyeCamera = eyeObject.AddComponent<Camera>();
            return eyeCamera;
        }

        private void ApplyCameraSettings()
        {
            ConfigureCamera(centerCamera);
            ConfigureCamera(leftEyeCamera);
            ConfigureCamera(rightEyeCamera);

            float halfIpd = interpupillaryDistance * 0.5f;
            leftEyeCamera.transform.localPosition = new Vector3(-halfIpd, 0f, 0f);
            rightEyeCamera.transform.localPosition = new Vector3(halfIpd, 0f, 0f);
            leftEyeCamera.transform.localRotation = Quaternion.identity;
            rightEyeCamera.transform.localRotation = Quaternion.identity;

            leftEyeCamera.rect = new Rect(0f, 0f, 0.5f, 1f);
            rightEyeCamera.rect = new Rect(0.5f, 0f, 0.5f, 1f);
            centerCamera.rect = new Rect(0f, 0f, 1f, 1f);
        }

        private void ConfigureCamera(Camera target)
        {
            if (target == null)
            {
                return;
            }

            if (centerCamera != null && target != centerCamera)
            {
                target.CopyFrom(centerCamera);
            }

            target.fieldOfView = fieldOfView;
            target.nearClipPlane = nearClipPlane;
            target.farClipPlane = farClipPlane;
        }

        private void ApplyMode()
        {
            bool stereo = ShouldUseStereo();

            if (centerCamera != null)
            {
                centerCamera.enabled = !stereo;
            }

            if (leftEyeCamera != null)
            {
                leftEyeCamera.enabled = stereo;
            }

            if (rightEyeCamera != null)
            {
                rightEyeCamera.enabled = stereo;
            }
        }

        private bool ShouldUseStereo()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return true;
#else
            return forceStereoInEditor;
#endif
        }
    }
}
