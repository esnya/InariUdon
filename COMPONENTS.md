# InariUdon Components



## Uncategorized

### ClockAnimationStarter




#### Public Variables
| Name | Type | Description |
|:--|:--|:--|
| clockAnimator | UnityEngine.Animator |  |
| useUTC | System.Boolean |  |
| layerName | System.String |  |
| stateName | System.String |  |



### EventBroadcaster




#### Public Variables
| Name | Type | Description |
|:--|:--|:--|
| parent | UnityEngine.GameObject |  |
| eventName | System.String |  |
| includeInactive | System.Boolean |  |
| networked | System.Boolean |  |
| networkEventTarget | VRC.Udon.Common.Interfaces.NetworkEventTarget |  |



#### Public Events
| Name | Description |
|:--|:--|
| _Trigger |  |



### GripTransporeter




#### Public Variables
| Name | Type | Description |
|:--|:--|:--|
| origin | UnityEngine.Transform | Set null to use Transform.parent |
| jumpOnDisable | System.Boolean |  |



### SimplePlaylist




#### Public Variables
| Name | Type | Description |
|:--|:--|:--|
| playlist | VRC.SDKBase.VRCUrl[] |  |
| titles | System.String[] | Optional |
| autoPlay | System.Boolean |  |
| timeSkipThreshold | System.Single |  |
| text | TMPro.TextMeshPro | Optional |



#### Public Events
| Name | Description |
|:--|:--|
| LoadURL |  |
| Play |  |
| Stop |  |
| PlayNext |  |
| PlayPrevious |  |



### AnimatorDriver




#### Public Variables
| Name | Type | Description |
|:--|:--|:--|
| animator | UnityEngine.Animator |  |
| parameterName | System.String |  |
| floatValue | System.Single |  |



#### Public Events
| Name | Description |
|:--|:--|
| SetFloat |  |



### FloatMultiValueDriver




#### Public Variables
| Name | Type | Description |
|:--|:--|:--|
| value | System.Single |  |
| castToInt | System.Boolean |  |
| writeProgramVariables | System.Boolean |  |
| sendEvent | System.Boolean |  |
| ignoreFirstEvent | System.Boolean |  |
| findTargetFromChildren | System.Boolean |  |
| behaviours | UdonSharp.UdonSharpBehaviour[] |  |
| variableNames | System.String[] |  |
| eventNames | System.String[] |  |
| behaviourParent | UnityEngine.Transform |  |
| useFind | System.Boolean |  |
| findPath | System.String |  |
| variableName | System.String |  |
| eventName | System.String |  |



#### Public Events
| Name | Description |
|:--|:--|
| _ValueChanged |  |



### Float Value Driver
Drives float parameters of animators by one float value calculated from scene.



#### Public Variables
| Name | Type | Description |
|:--|:--|:--|
| modeString | System.String |  |
| sourceTransform | UnityEngine.Transform |  |
| transformOrigin | UnityEngine.Transform |  |
| localVector | UnityEngine.Vector3 |  |
| worldVector | UnityEngine.Vector3 |  |
| axisVector | UnityEngine.Vector3 |  |
| valueMultiplier | System.Single |  |
| valueBias | System.Single |  |
| clampValue | System.Boolean |  |
| minValue | System.Single |  |
| maxValue | System.Single |  |
| driveAnimatorParameters | System.Boolean |  |
| targetAnimators | UnityEngine.Animator[] |  |
| targetAnimatorParameters | System.String[] |  |



#### Public Events
| Name | Description |
|:--|:--|
| GetModeOptions |  |
| HideTransformSource |  |
| HideTransformOrigin |  |
| HideLocalVector |  |
| HideWorldVector |  |
| HideAxisVector |  |



### RotationDriver




#### Public Variables
| Name | Type | Description |
|:--|:--|:--|
| target | UnityEngine.Transform |  |
| axis | UnityEngine.Vector3 |  |
| localSpace | System.Boolean |  |
| applyAngleOnStart | System.Boolean |  |
| startAngle | System.Single |  |
| endAngle | System.Single |  |
| speed | System.Single |  |



#### Public Events
| Name | Description |
|:--|:--|
| _Trigger |  |



### SyncedSpinner




#### Public Variables
| Name | Type | Description |
|:--|:--|:--|
| angle | System.Single |  |
| axis | UnityEngine.Vector3 |  |
| speed | System.Single |  |
| randomizeStartAngle | System.Boolean |  |



### KeyboardInput




#### Public Variables
| Name | Type | Description |
|:--|:--|:--|
| keyCodes | System.Int32[] |  |
| modes | System.Int32[] |  |
| eventTargets | UdonSharp.UdonSharpBehaviour[] |  |
| onKeyDownEvents | System.String[] |  |
| holdTime | System.Single |  |
| holdInterval | System.Single |  |
| audioSource | UnityEngine.AudioSource |  |



### Snappable




#### Public Variables
| Name | Type | Description |
|:--|:--|:--|
| layerMask | UnityEngine.LayerMask |  |
| snapTargetPrefixes | System.String[] |  |
| reparent | System.Boolean |  |



#### Public Events
| Name | Description |
|:--|:--|
| _TakeOwnership |  |
| _Snap |  |
| _Release |  |



### TouchSwitch




#### Public Variables
| Name | Type | Description |
|:--|:--|:--|
| radius | System.Single |  |
| thickness | System.Single |  |
| maxPlayerDistance | System.Single |  |
| awakeCheckInterval | System.Int32 |  |
| ownerOnly | System.Boolean |  |
| sentToOwner | System.Boolean | Deprecated: Use networked |
| networked | System.Boolean |  |
| networkEventTarget | VRC.Udon.Common.Interfaces.NetworkEventTarget |  |
| eventTarget | UdonSharp.UdonSharpBehaviour |  |
| eventName | System.String |  |
| disableInteractInVR | System.Boolean |  |
| throttlingDelay | System.Single |  |
| fingerMode | System.Boolean |  |
| grip | System.Boolean |  |
| onTouchStart | System.String |  |
| onTouchEnd | System.String |  |
| knobMode | System.Boolean |  |
| onKnobRight | System.String |  |
| onKnobLeft | System.String |  |
| knobStep | System.Single |  |
| directionalMode | System.Boolean |  |
| directionalThreshold | System.Single |  |
| onUp | System.String |  |
| onDown | System.String |  |
| onLeft | System.String |  |
| onRight | System.String |  |
| wheelMode | System.Boolean |  |
| onWheelRight | System.String |  |
| onWheelLeft | System.String |  |
| wheelStep | System.Single |  |
| enableDesktopKey | System.Boolean | Deprecated: Use KeyboardInput |
| desktopKey | UnityEngine.KeyCode | Deprecated: Use KeyboardInput |
| audioSource | UnityEngine.AudioSource |  |
| enableHaptics | System.Boolean |  |
| hapticDuration | System.Single |  |
| hapticAmplitude | System.Single |  |
| hapticFrequency | System.Single |  |
| debugTouhSource | UnityEngine.Transform |  |



#### Public Events
| Name | Description |
|:--|:--|
| _PostStart |  |
| _AwakeCheck |  |



### AmbientController




#### Public Variables
| Name | Type | Description |
|:--|:--|:--|
| ambientMode | UnityEngine.Rendering.AmbientMode |  |
| ambientEquatorColor | UnityEngine.Color |  |
| ambientGroundColor | UnityEngine.Color |  |
| ambientIntensity | System.Single |  |
| ambientLight | UnityEngine.Color |  |
| ambientSkyColor | UnityEngine.Color |  |



#### Public Events
| Name | Description |
|:--|:--|
| Apply |  |



### TumblerSwitch




#### Public Variables
| Name | Type | Description |
|:--|:--|:--|
| localAnimator | UnityEngine.Animator |  |
| localParameter | System.String |  |
| audioSource | UnityEngine.AudioSource |  |
| externalAnimator | UnityEngine.Animator |  |
| externalParameter | System.String |  |
| state | System.Boolean |  |
| sync | System.Boolean |  |
| customEventTarget | VRC.Udon.UdonBehaviour |  |
| customTurnOnEventName | System.String |  |
| customTurnOffEventName | System.String |  |



#### Public Events
| Name | Description |
|:--|:--|
| Sync |  |
| PlayAudio |  |
| TurnOn |  |
| TurnOff |  |



### UpdateFPSVisualizer




#### Public Variables
| Name | Type | Description |
|:--|:--|:--|
| text | UnityEngine.UI.Text |  |
| particle | UnityEngine.ParticleSystem |  |
| df | System.Int32 |  |



### Pickup Controller
Enhancement VRC_Pickup such as relay events or expose Respawn event.



#### Public Variables
| Name | Type | Description |
|:--|:--|:--|
| respawnTarget | UnityEngine.Transform | Set null to use initial world transform |
| respawnOnDrop | System.Boolean |  |
| overrideIsTrigger | System.Boolean |  |
| fireOnPickup | System.Boolean |  |
| onPickupTargets | UdonSharp.UdonSharpBehaviour[] |  |
| onPickupEvents | System.String[] |  |
| fireOnDrop | System.Boolean |  |
| onDropNetworkTarget | VRC.Udon.Common.Interfaces.NetworkEventTarget |  |
| onDropTargets | UdonSharp.UdonSharpBehaviour[] |  |
| onDropEvents | System.String[] |  |
| fireOnPickupUseDown | System.Boolean |  |
| onPickupUseDownTargets | UdonSharp.UdonSharpBehaviour[] |  |
| onPickupUseDownEvents | System.String[] |  |



#### Public Events
| Name | Description |
|:--|:--|
| _TakeOwnership |  |
| _Respawn |  |



### Pickup Event Trigger
SendCustomEvents on pickup events.



#### Public Variables
| Name | Type | Description |
|:--|:--|:--|
| networked | System.Boolean |  |
| networkTarget | VRC.Udon.Common.Interfaces.NetworkEventTarget |  |
| fireOnPickup | System.Boolean |  |
| onPickupTargets | UdonSharp.UdonSharpBehaviour[] |  |
| onPickupEvents | System.String[] |  |
| fireOnDrop | System.Boolean |  |
| onDropTargets | UdonSharp.UdonSharpBehaviour[] |  |
| onDropEvents | System.String[] |  |



### Entrance Sound Player
Play sound using AudioSource when player joined or left. To disable either of them, select None.



#### Public Variables
| Name | Type | Description |
|:--|:--|:--|
| joinedSoundSource | UnityEngine.AudioSource |  |
| leftSoundSource | UnityEngine.AudioSource |  |



### Player Counter
Display number of players in the instance with TextMeshPro. Alos show world max capacity if provided.



#### Public Variables
| Name | Type | Description |
|:--|:--|:--|
| text | TMPro.TextMeshPro | TextMeshPro component to display counts. |
| maxCapacity | System.Int32 | Max capacity of world. Set 0 to disable. |



### PlayerEventLogger




#### Public Variables
| Name | Type | Description |
|:--|:--|:--|
| logger | InariUdon.UI.UdonLogger |  |
| level | System.String |  |
| joinedFormat | System.String |  |
| leftFormat | System.String |  |



### PlayerPositionRecovery




#### Public Variables
| Name | Type | Description |
|:--|:--|:--|
| maxDistanceFromOrigin | System.Single |  |
| timeoutSeconds | System.Single |  |
| bufferSize | System.Int32 |  |
| logger | InariUdon.UI.UdonLogger |  |



### MaterialPropertyBlock Writer

Apply a `MaterialPropertyBlock`.
Override the material properties with various values, but they can share the same material. This is a first step for GPU instancing.


![image](https://user-images.githubusercontent.com/2088693/121310202-160c6b00-c93e-11eb-92ec-91583c3f69f0.png)
![image](https://user-images.githubusercontent.com/2088693/121310283-2cb2c200-c93e-11eb-9834-c99a901a0f1a.png)

#### Public Variables
| Name | Type | Description |
|:--|:--|:--|
| onStart | System.Boolean | Apply on start |
| onEnable | System.Boolean | Apply on enable |
| writeColors | System.Boolean |  |
| colorTargets | UnityEngine.Renderer[] |  |
| colorIndices | System.Int32[] |  |
| colorNames | System.String[] |  |
| colorValues | UnityEngine.Color[] |  |
| writeFloats | System.Boolean |  |
| floatTargets | UnityEngine.Renderer[] |  |
| floatIndices | System.Int32[] |  |
| floatNames | System.String[] |  |
| floatValues | System.Single[] |  |
| writeTextures | System.Boolean |  |
| textureTargets | UnityEngine.Renderer[] |  |
| textureIndices | System.Int32[] |  |
| textureNames | System.String[] |  |
| textureValues | UnityEngine.Texture[] |  |
| writeVectors | System.Boolean |  |
| vectorTargets | UnityEngine.Renderer[] |  |
| vectorIndices | System.Int32[] |  |
| vectorNames | System.String[] |  |
| vectorValues | UnityEngine.Vector4[] |  |



#### Public Events
| Name | Description |
|:--|:--|
| Trigger | Apply overrides |
| ClearTargetProperties |  |



### Reflection Probe Driver
Controls ReflectionProbe at runtime. Currently, only the "RenderProbe" event is available to update the in real-time mode.



#### Public Variables
| Name | Type | Description |
|:--|:--|:--|
| reflectionProbe | UnityEngine.GameObject |  |
| renderOnStart | System.Boolean |  |



#### Public Events
| Name | Description |
|:--|:--|
| RenderProbe |  |



### ObjectSyncRespawner




#### Public Variables
| Name | Type | Description |
|:--|:--|:--|
| findObjectSyncFromChildren | System.Boolean |  |
| targets | VRC.SDK3.Components.VRCObjectSync[] |  |
| targetsParent | UnityEngine.GameObject |  |
| includeDisabled | System.Boolean |  |



#### Public Events
| Name | Description |
|:--|:--|
| _Trigger |  |



### SyncedBooleanLatch




#### Public Variables
| Name | Type | Description |
|:--|:--|:--|
| value | System.Boolean |  |
| onSetEventListeners | UdonSharp.UdonSharpBehaviour[] |  |
| onSetEventNames | System.String[] |  |
| onResetEventListeners | UdonSharp.UdonSharpBehaviour[] |  |
| onResetEventNames | System.String[] |  |
| onToggleEventListeners | UdonSharp.UdonSharpBehaviour[] |  |
| onToggleEventNames | System.String[] |  |



#### Public Events
| Name | Description |
|:--|:--|
| _TakeOwnership |  |
| _Set |  |
| _Reset |  |
| _Toggle |  |



### SyncedFloat




#### Public Variables
| Name | Type | Description |
|:--|:--|:--|
| value | System.Single |  |
| castToInt | System.Boolean |  |
| clampValue | System.Boolean |  |
| minValue | System.Single |  |
| maxValue | System.Single |  |
| incrementStep | System.Single |  |
| writeProgramVariables | System.Boolean |  |
| sendEvents | System.Boolean |  |
| writeAsArray | System.Boolean |  |
| programVariablesFromChildren | System.Boolean |  |
| targets | UdonSharp.UdonSharpBehaviour[] |  |
| variableNames | System.String[] |  |
| eventNames | System.String[] |  |
| targetsParent | UnityEngine.Transform |  |
| variableName | System.String |  |
| eventName | System.String |  |
| writeAnimatorParameters | System.Boolean |  |
| animators | UnityEngine.Animator[] |  |
| animatorParameterNames | System.String[] |  |
| slider | UnityEngine.UI.Slider |  |
| wholeNumbers | System.Boolean |  |
| exp | System.Boolean |  |



#### Public Events
| Name | Description |
|:--|:--|
| _TakeOwnership |  |
| _Sync |  |
| _Increment |  |
| _Decrement |  |
| HideSliderOptions |  |



### SyncedMultiObjectToggle




#### Public Variables
| Name | Type | Description |
|:--|:--|:--|
| targets | UnityEngine.GameObject[] |  |
| initialState | System.Boolean |  |
| initializeDelay | System.Single |  |



#### Public Events
| Name | Description |
|:--|:--|
| _Activate |  |
| _Deactivate |  |
| _ToggleActive |  |
| _Initialize |  |



### SyncedObjectMultiplexer




#### Public Variables
| Name | Type | Description |
|:--|:--|:--|
| initialIndex | System.Int32 |  |
| targets | UnityEngine.GameObject[] |  |



#### Public Events
| Name | Description |
|:--|:--|
| get_Index |  |
| _Increment |  |
| _Decrement |  |



### SyncedObjectToggle




#### Public Variables
| Name | Type | Description |
|:--|:--|:--|
| target | UnityEngine.GameObject |  |



#### Public Events
| Name | Description |
|:--|:--|
| _Activate |  |
| _Deactivate |  |
| _ToggleActive |  |



### AutoAdjustedChair




#### Public Variables
| Name | Type | Description |
|:--|:--|:--|
| seatTopFront | UnityEngine.Transform |  |



### Local Space Tracker
Track source transform as local position/rotation. You can translate and scale by parent transform. Call "Trigger" custome event to update manually. All fields are optional.



#### Public Variables
| Name | Type | Description |
|:--|:--|:--|
| source | UnityEngine.Transform |  |
| sourceOrigin | UnityEngine.Transform |  |
| positionTarget | UnityEngine.Transform |  |
| rotationTarget | UnityEngine.Transform |  |
| updateMode | System.String |  |



#### Public Events
| Name | Description |
|:--|:--|
| Trigger |  |



### Scaled Multi Follower

Drive multiple transform of targets by source transforms in single Update loop.
Scale of positions and origin of transforms can be changed.
This component allows you to display the position of an object on the minimap,  object placement or etc.
        


![image](https://user-images.githubusercontent.com/2088693/121690092-5d425980-cb00-11eb-9518-a19896cbabd5.png)

#### Public Variables
| Name | Type | Description |
|:--|:--|:--|
| sourceParent | UnityEngine.Transform | Find source from children |
| sources | UnityEngine.Transform[] | Use specified sources |
| sourceOrigin | UnityEngine.Transform | Position origin of sources |
| findSourceChild | System.Boolean | Find sources by path |
| sourceChildPath | System.String | Find sources by path |
| targetParent | UnityEngine.Transform | Find targets from children |
| targets | UnityEngine.Transform[] | Use specified targets |
| targetOrigin | UnityEngine.Transform | Position origin of targets |
| findTargetChild | System.Boolean | Find targets by path |
| targetChildPath | System.String | Find targets by path |
| positionScale | UnityEngine.Vector3 | Scale positions |
| scaleMultiplier | System.Single | Scale positions |
| inverseScale | System.Boolean |  |
| rotation | System.Boolean | Enable rotation copy |
| updateFrequency | System.Single |  |
| copyActive | System.Boolean | Copy `GameObject.activeSelf` |
| deactivateExcessiveTargets | System.Boolean |  |
| ownerOnly | System.Boolean | Follow if owenr of source |
| toggleTargetColliders | System.Boolean | Disable collider while `pickup.IsHeld == true` of source |
| freezeTargetWhileSoruceHeld | System.Boolean | Stop following while `pickup.IsHeld == true` of source |



#### Public Events
| Name | Description |
|:--|:--|
| _Trigger |  |



### Set Parent
Modify parent in hierarchy ay runtime



#### Public Variables
| Name | Type | Description |
|:--|:--|:--|
| parent | UnityEngine.Transform |  |
| parentName | System.String |  |
| findParentByName | System.Boolean | Find parent by `GameObject.Find(parentName)` |
| target | UnityEngine.Transform | None to use `this.transform` |
| keepGrobalTransform | System.Boolean |  |
| triggerOnStart | System.Boolean |  |



#### Public Events
| Name | Description |
|:--|:--|
| Trigger | Set parent |



### EnabledTrigger
Trigger by Enable/Disable Component and GameObject



#### Public Variables
| Name | Type | Description |
|:--|:--|:--|
| enabledEventTargets | UdonSharp.UdonSharpBehaviour[] |  |
| enabledEvents | System.String[] |  |
| disabledEventTargets | UdonSharp.UdonSharpBehaviour[] |  |
| disabledEvents | System.String[] |  |
| ownerOnly | System.Boolean |  |



### InputTrigger
Trigger by input



#### Public Variables
| Name | Type | Description |
|:--|:--|:--|
| keyName | System.String |  |
| buttonName | System.String |  |
| keyDown | System.Boolean |  |
| buttonDown | System.Boolean |  |
| eventTargets | UdonSharp.UdonSharpBehaviour[] |  |
| eventNames | System.String[] |  |
| keyUp | System.Boolean |  |
| buttonUp | System.Boolean |  |
| onUpEventTargets | UdonSharp.UdonSharpBehaviour[] |  |
| onUpEventNames | System.String[] |  |



#### Public Events
| Name | Description |
|:--|:--|
| _Enable |  |
| _Disable |  |



### InteractRelay
Overrides Interact, or integrates multiple.



#### Public Variables
| Name | Type | Description |
|:--|:--|:--|
| relayTargets | UdonSharp.UdonSharpBehaviour[] |  |
| eventTargets | UdonSharp.UdonSharpBehaviour[] |  |
| eventNames | System.String[] |  |



#### Public Events
| Name | Description |
|:--|:--|
| _Trigger |  |
| _Enable |  |
| _Disable |  |



### AudioModeToggle




#### Public Variables
| Name | Type | Description |
|:--|:--|:--|
| usharpVideoPlayer | UdonSharp.UdonSharpBehaviour |  |
| audioSources | UnityEngine.AudioSource[] |  |
| sliderTransform | UnityEngine.RectTransform |  |
| lockedOverlay | UnityEngine.GameObject |  |
| lockIcon | UnityEngine.GameObject |  |
| default2DMode | System.Boolean |  |



#### Public Events
| Name | Description |
|:--|:--|
| Toggle |  |



### UdonToggle




#### Public Variables
| Name | Type | Description |
|:--|:--|:--|
| target | UdonSharp.UdonSharpBehaviour |  |
| buttonImage | UnityEngine.UI.Image |  |



#### Public Events
| Name | Description |
|:--|:--|
| _Toggle |  |



### VideoScreenBrightness




#### Public Variables
| Name | Type | Description |
|:--|:--|:--|
| slider | UnityEngine.UI.Slider |  |
| screen | UnityEngine.MeshRenderer |  |
| subMesh | System.Int32 |  |
| colorPropertyName | System.String |  |
| uiBrightness | System.Boolean |  |
| icon | UnityEngine.UI.Image |  |
| uiMaterial | UnityEngine.Material |  |



#### Public Events
| Name | Description |
|:--|:--|
| OnSliderValueChanged |  |



### Udon Logger
Rich log viewer in world with colord log-levels, timestamp and etc.



#### Public Variables
| Name | Type | Description |
|:--|:--|:--|
| maxCharacters | System.Int32 |  |
| text | TMPro.TextMeshProUGUI |  |
| levels | System.String[] |  |
| colors | UnityEngine.Color[] |  |
| levelsIgnoreCase | System.Boolean |  |



#### Public Events
| Name | Description |
|:--|:--|

