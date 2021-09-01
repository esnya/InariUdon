# InariUdon
Useful prefabs and Udon scripts for VRChat World SDK 3.0

**NOTICE: Sun Controller is now separated into a standalone packge. Checkout from https://github.com/esnya/UdonSunController !!**

## Usage
Place prefabs into Scene in [Assets/InariUdon/Components/<COMPONENT_NAME>](Assets/InariUdon/Components) or add UdonScript in Script directory.

## Setup
1. Install requirements in below.
2. Download latest non-pre-release `InariUdon-<VERSION>.zip` from [Releases](https://github.com/esnya/InariUdon/releases).
3. Unzip and drag and drop .unitypackage into Project window of Unity.

## Requirements
* VRC SDK3 Worlds
* UdonSharp
* UdonToolkt

## Usage
See also [COMPONENTS.md](COMPONENTS.md).

### USharpVideo+
![image](https://user-images.githubusercontent.com/2088693/131600243-9bc85ee3-ab77-43c7-aaf3-7f4b215f2852.png)

Enhanced Version of [USharpVideo](https://github.com/MerlinVR/USharpVideo) by [MerlinVR](https://github.com/MerlinVR). Requires [MerlinVR/USharpVideo](https://github.com/MerlinVR/USharpVideo).

#### Features
* Slider for Screen/UI Brightness
* 3D/2S AudioModeToggle (Synced)
* Toggle UpdateGIMaterials
  * Workaround to avoid VRAM leaking issue: https://feedback.vrchat.com/bug-reports/p/1121-vram-leak-when-using-vrccustomrendererbehaviourrendererextensionsupdategima
