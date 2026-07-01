ForestVR 功能移交包

复制方式：
1. 解压后，把 Assets 文件夹里的内容合并到目标 Unity 项目的 Assets 目录下。
2. 不要覆盖目标项目已有同名资源，除非确认要替换。

必需依赖：
- com.unity.inputsystem

Player Settings：
- Active Input Handling = Both
- Android Orientation = Landscape Right
- Minimum API Level = 26+
- Android Graphics API = OpenGLES3 only

不要安装：
- Cardboard / OpenXR / Oculus / XR Interaction Toolkit / XRI Starter Kit

接入步骤：
1. 打开目标场景
2. Tools -> Forest VR -> Setup Current Scene
3. Tools -> Forest VR -> Mark Leaf Colliders NoPlayerCollision
4. Build APK 测试

包含脚本：
- ForestVRInput.cs
- SimplePlayerLocomotion.cs
- PhoneGyroLook.cs
- VrStereoCamera.cs
- BluetoothButtonInput.cs
- EditorMouseLook.cs
- PlayerCollisionLayerSetup.cs
- ForestVRSceneSetup.cs

按键：
- 移动：WASD / 左摇杆
- 加速：Shift / 左摇杆按下
- 确认：Space / Enter / E / A键
- 重置视角：R / Start
