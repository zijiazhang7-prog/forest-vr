using ForestVR;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

public static class ForestVRSceneSetup
{
    private const string PlayerRigName = "PlayerRig";
    private const string PlayerLayerName = "Player";
    private const string NoPlayerCollisionLayerName = "NoPlayerCollision";

    [MenuItem("Tools/Forest VR/Setup Current Scene")]
    public static void SetupCurrentScene()
    {
        EnsureLayer(PlayerLayerName);
        EnsureLayer(NoPlayerCollisionLayerName);
        Physics.IgnoreLayerCollision(
            LayerMask.NameToLayer(PlayerLayerName),
            LayerMask.NameToLayer(NoPlayerCollisionLayerName),
            true);

        GameObject rig = GameObject.Find(PlayerRigName);
        if (rig == null)
        {
            rig = new GameObject(PlayerRigName);
            Undo.RegisterCreatedObjectUndo(rig, "Create Forest VR PlayerRig");
        }

        Undo.RecordObject(rig, "Setup Forest VR PlayerRig");
        rig.transform.position = GetSpawnPosition();
        rig.transform.rotation = Quaternion.identity;
        rig.layer = LayerMask.NameToLayer(PlayerLayerName);

        CharacterController controller = GetOrAdd<CharacterController>(rig);
        controller.height = 1.65f;
        controller.radius = 0.28f;
        controller.center = new Vector3(0f, 0.825f, 0f);
        controller.stepOffset = 0.28f;
        controller.slopeLimit = 45f;
        controller.skinWidth = 0.04f;

        Transform cameraTransform = GetOrCreateChild(rig.transform, "Main Camera");
        cameraTransform.localPosition = new Vector3(0f, 1.55f, 0f);
        cameraTransform.localRotation = Quaternion.identity;

        GameObject cameraObject = cameraTransform.gameObject;
        cameraObject.tag = "MainCamera";
        cameraObject.layer = rig.layer;

        Camera camera = GetOrAdd<Camera>(cameraObject);
        camera.nearClipPlane = 0.05f;
        camera.farClipPlane = 100f;
        camera.fieldOfView = 90f;

        GetOrAdd<AudioListener>(cameraObject);
        GetOrAdd<EditorMouseLook>(cameraObject);
        PhoneGyroLook gyroLook = GetOrAdd<PhoneGyroLook>(cameraObject);
        gyroLook.YawRoot = rig.transform;
        GetOrAdd<VrStereoCamera>(cameraObject);

        SimplePlayerLocomotion locomotion = GetOrAdd<SimplePlayerLocomotion>(rig);
        locomotion.ViewTransform = cameraTransform;
        GetOrAdd<BluetoothButtonInput>(rig);
        GetOrAdd<PlayerRigSetup>(rig);
        GetOrAdd<PlayerCollisionLayerSetup>(rig);

        DisableOtherCameras(camera);
        ApplyAndroidProjectSettings();

        Selection.activeGameObject = rig;
        EditorSceneManager.MarkSceneDirty(rig.scene);
        Debug.Log("Forest VR scene setup complete. Use WASD/left stick to move, right mouse in Editor to look, R/start to recenter gyro on device.");
    }

    [MenuItem("Tools/Forest VR/Mark Leaf Colliders NoPlayerCollision")]
    public static void MarkLeafCollidersNoPlayerCollision()
    {
        EnsureLayer(NoPlayerCollisionLayerName);
        int layer = LayerMask.NameToLayer(NoPlayerCollisionLayerName);
        int changed = 0;

        foreach (Collider collider in Object.FindObjectsByType<Collider>(FindObjectsSortMode.None))
        {
            string name = collider.gameObject.name.ToLowerInvariant();
            if (!LooksLikeSoftFoliage(name))
            {
                continue;
            }

            Undo.RecordObject(collider.gameObject, "Mark foliage collider ignored by player");
            collider.gameObject.layer = layer;
            changed++;
        }

        EditorSceneManager.MarkAllScenesDirty();
        Debug.Log($"Marked {changed} foliage collider object(s) as {NoPlayerCollisionLayerName}.");
    }

    private static void ApplyAndroidProjectSettings()
    {
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeRight;
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
        PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
        PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] { GraphicsDeviceType.OpenGLES3 });
    }

    private static Vector3 GetSpawnPosition()
    {
        if (Selection.activeTransform != null)
        {
            return ProjectToGround(Selection.activeTransform.position);
        }

        Terrain terrain = Terrain.activeTerrain;
        if (terrain != null && terrain.terrainData != null)
        {
            Vector3 terrainPosition = terrain.transform.position;
            Vector3 size = terrain.terrainData.size;
            Vector3 center = terrainPosition + new Vector3(size.x * 0.5f, 0f, size.z * 0.5f);
            center.y = terrain.SampleHeight(center) + terrainPosition.y + 0.1f;
            return center;
        }

        if (SceneView.lastActiveSceneView != null)
        {
            return ProjectToGround(SceneView.lastActiveSceneView.pivot);
        }

        return Vector3.up;
    }

    private static Vector3 ProjectToGround(Vector3 position)
    {
        Vector3 origin = position + Vector3.up * 100f;
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 250f))
        {
            return hit.point + Vector3.up * 0.1f;
        }

        return position;
    }

    private static void DisableOtherCameras(Camera activeCamera)
    {
        foreach (Camera camera in Object.FindObjectsByType<Camera>(FindObjectsSortMode.None))
        {
            if (camera == activeCamera)
            {
                continue;
            }

            Undo.RecordObject(camera, "Disable non-player camera");
            camera.enabled = false;

            AudioListener listener = camera.GetComponent<AudioListener>();
            if (listener != null)
            {
                Undo.RecordObject(listener, "Disable non-player audio listener");
                listener.enabled = false;
            }
        }
    }

    private static Transform GetOrCreateChild(Transform parent, string childName)
    {
        Transform child = parent.Find(childName);
        if (child != null)
        {
            return child;
        }

        GameObject childObject = new GameObject(childName);
        Undo.RegisterCreatedObjectUndo(childObject, $"Create {childName}");
        childObject.transform.SetParent(parent, false);
        return childObject.transform;
    }

    private static T GetOrAdd<T>(GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponent<T>();
        if (component != null)
        {
            return component;
        }

        return Undo.AddComponent<T>(gameObject);
    }

    private static bool LooksLikeSoftFoliage(string objectName)
    {
        return objectName.Contains("leaf")
            || objectName.Contains("leaves")
            || objectName.Contains("foliage")
            || objectName.Contains("grass")
            || objectName.Contains("fern")
            || objectName.Contains("bush")
            || objectName.Contains("shrub");
    }

    private static void EnsureLayer(string layerName)
    {
        if (LayerMask.NameToLayer(layerName) >= 0)
        {
            return;
        }

        SerializedObject tagManager = new SerializedObject(
            AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty layers = tagManager.FindProperty("layers");

        for (int i = 8; i < layers.arraySize; i++)
        {
            SerializedProperty layer = layers.GetArrayElementAtIndex(i);
            if (!string.IsNullOrEmpty(layer.stringValue))
            {
                continue;
            }

            layer.stringValue = layerName;
            tagManager.ApplyModifiedProperties();
            return;
        }

        Debug.LogWarning($"No free Unity layer slot is available for {layerName}.");
    }
}
