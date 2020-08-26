
declare module 'csharp' {
    interface $Ref<T> {}
    
    interface $Task<T> {}
    
    namespace UnityEngine {
        class Debug extends System.Object {
            public static unityLogger: UnityEngine.ILogger;
            public static developerConsoleVisible: boolean;
            public static isDebugBuild: boolean;
            public static logger: UnityEngine.ILogger;
            public constructor();
            public static DrawLine(start: UnityEngine.Vector3, end: UnityEngine.Vector3, color: UnityEngine.Color, duration: number, depthTest: boolean):void;
            public static DrawLine(start: UnityEngine.Vector3, end: UnityEngine.Vector3, color: UnityEngine.Color, duration: number):void;
            public static DrawLine(start: UnityEngine.Vector3, end: UnityEngine.Vector3, color: UnityEngine.Color):void;
            public static DrawLine(start: UnityEngine.Vector3, end: UnityEngine.Vector3):void;
            public static DrawRay(start: UnityEngine.Vector3, dir: UnityEngine.Vector3, color: UnityEngine.Color, duration: number):void;
            public static DrawRay(start: UnityEngine.Vector3, dir: UnityEngine.Vector3, color: UnityEngine.Color):void;
            public static DrawRay(start: UnityEngine.Vector3, dir: UnityEngine.Vector3):void;
            public static DrawRay(start: UnityEngine.Vector3, dir: UnityEngine.Vector3, color: UnityEngine.Color, duration: number, depthTest: boolean):void;
            public static Break():void;
            public static DebugBreak():void;
            public static Log(message: any):void;
            public static Log(message: any, context: UnityEngine.Object):void;
            public static LogFormat(format: string, ...args: any[]):void;
            public static LogFormat(context: UnityEngine.Object, format: string, ...args: any[]):void;
            public static LogError(message: any):void;
            public static LogError(message: any, context: UnityEngine.Object):void;
            public static LogErrorFormat(format: string, ...args: any[]):void;
            public static LogErrorFormat(context: UnityEngine.Object, format: string, ...args: any[]):void;
            public static ClearDeveloperConsole():void;
            public static LogException(exception: System.Exception):void;
            public static LogException(exception: System.Exception, context: UnityEngine.Object):void;
            public static LogWarning(message: any):void;
            public static LogWarning(message: any, context: UnityEngine.Object):void;
            public static LogWarningFormat(format: string, ...args: any[]):void;
            public static LogWarningFormat(context: UnityEngine.Object, format: string, ...args: any[]):void;
            public static Assert(condition: boolean):void;
            public static Assert(condition: boolean, context: UnityEngine.Object):void;
            public static Assert(condition: boolean, message: any):void;
            public static Assert(condition: boolean, message: string):void;
            public static Assert(condition: boolean, message: any, context: UnityEngine.Object):void;
            public static Assert(condition: boolean, message: string, context: UnityEngine.Object):void;
            public static AssertFormat(condition: boolean, format: string, ...args: any[]):void;
            public static AssertFormat(condition: boolean, context: UnityEngine.Object, format: string, ...args: any[]):void;
            public static LogAssertion(message: any):void;
            public static LogAssertion(message: any, context: UnityEngine.Object):void;
            public static LogAssertionFormat(format: string, ...args: any[]):void;
            public static LogAssertionFormat(context: UnityEngine.Object, format: string, ...args: any[]):void;
            
        }
        interface ILogger {
            
        }
        class Vector3 extends System.ValueType {
            public static kEpsilon: number;
            public x: number;
            public y: number;
            public z: number;
            public Item: number;
            public normalized: UnityEngine.Vector3;
            public magnitude: number;
            public sqrMagnitude: number;
            public static zero: UnityEngine.Vector3;
            public static one: UnityEngine.Vector3;
            public static forward: UnityEngine.Vector3;
            public static back: UnityEngine.Vector3;
            public static up: UnityEngine.Vector3;
            public static down: UnityEngine.Vector3;
            public static left: UnityEngine.Vector3;
            public static right: UnityEngine.Vector3;
            public static positiveInfinity: UnityEngine.Vector3;
            public static negativeInfinity: UnityEngine.Vector3;
            public static fwd: UnityEngine.Vector3;
            public constructor(x: number, y: number, z: number);
            public constructor(x: number, y: number);
            public static Slerp(a: UnityEngine.Vector3, b: UnityEngine.Vector3, t: number):UnityEngine.Vector3;
            public static SlerpUnclamped(a: UnityEngine.Vector3, b: UnityEngine.Vector3, t: number):UnityEngine.Vector3;
            public static OrthoNormalize(normal: $Ref<UnityEngine.Vector3>, tangent: $Ref<UnityEngine.Vector3>):void;
            public static OrthoNormalize(normal: $Ref<UnityEngine.Vector3>, tangent: $Ref<UnityEngine.Vector3>, binormal: $Ref<UnityEngine.Vector3>):void;
            public static RotateTowards(current: UnityEngine.Vector3, target: UnityEngine.Vector3, maxRadiansDelta: number, maxMagnitudeDelta: number):UnityEngine.Vector3;
            public static Exclude(excludeThis: UnityEngine.Vector3, fromThat: UnityEngine.Vector3):UnityEngine.Vector3;
            public static Lerp(a: UnityEngine.Vector3, b: UnityEngine.Vector3, t: number):UnityEngine.Vector3;
            public static LerpUnclamped(a: UnityEngine.Vector3, b: UnityEngine.Vector3, t: number):UnityEngine.Vector3;
            public static MoveTowards(current: UnityEngine.Vector3, target: UnityEngine.Vector3, maxDistanceDelta: number):UnityEngine.Vector3;
            public static SmoothDamp(current: UnityEngine.Vector3, target: UnityEngine.Vector3, currentVelocity: $Ref<UnityEngine.Vector3>, smoothTime: number, maxSpeed: number):UnityEngine.Vector3;
            public static SmoothDamp(current: UnityEngine.Vector3, target: UnityEngine.Vector3, currentVelocity: $Ref<UnityEngine.Vector3>, smoothTime: number):UnityEngine.Vector3;
            public static SmoothDamp(current: UnityEngine.Vector3, target: UnityEngine.Vector3, currentVelocity: $Ref<UnityEngine.Vector3>, smoothTime: number, maxSpeed: number, deltaTime: number):UnityEngine.Vector3;
            public get_Item(index: number):number;
            public set_Item(index: number, value: number):void;
            public Set(newX: number, newY: number, newZ: number):void;
            public static Scale(a: UnityEngine.Vector3, b: UnityEngine.Vector3):UnityEngine.Vector3;
            public Scale(scale: UnityEngine.Vector3):void;
            public static Cross(lhs: UnityEngine.Vector3, rhs: UnityEngine.Vector3):UnityEngine.Vector3;
            public GetHashCode():number;
            public Equals(other: any):boolean;
            public static Reflect(inDirection: UnityEngine.Vector3, inNormal: UnityEngine.Vector3):UnityEngine.Vector3;
            public static Normalize(value: UnityEngine.Vector3):UnityEngine.Vector3;
            public Normalize():void;
            public static Dot(lhs: UnityEngine.Vector3, rhs: UnityEngine.Vector3):number;
            public static Project(vector: UnityEngine.Vector3, onNormal: UnityEngine.Vector3):UnityEngine.Vector3;
            public static ProjectOnPlane(vector: UnityEngine.Vector3, planeNormal: UnityEngine.Vector3):UnityEngine.Vector3;
            public static Angle(from: UnityEngine.Vector3, to: UnityEngine.Vector3):number;
            public static SignedAngle(from: UnityEngine.Vector3, to: UnityEngine.Vector3, axis: UnityEngine.Vector3):number;
            public static Distance(a: UnityEngine.Vector3, b: UnityEngine.Vector3):number;
            public static ClampMagnitude(vector: UnityEngine.Vector3, maxLength: number):UnityEngine.Vector3;
            public static Magnitude(vector: UnityEngine.Vector3):number;
            public static SqrMagnitude(vector: UnityEngine.Vector3):number;
            public static Min(lhs: UnityEngine.Vector3, rhs: UnityEngine.Vector3):UnityEngine.Vector3;
            public static Max(lhs: UnityEngine.Vector3, rhs: UnityEngine.Vector3):UnityEngine.Vector3;
            public static op_Addition(a: UnityEngine.Vector3, b: UnityEngine.Vector3):UnityEngine.Vector3;
            public static op_Subtraction(a: UnityEngine.Vector3, b: UnityEngine.Vector3):UnityEngine.Vector3;
            public static op_UnaryNegation(a: UnityEngine.Vector3):UnityEngine.Vector3;
            public static op_Multiply(a: UnityEngine.Vector3, d: number):UnityEngine.Vector3;
            public static op_Multiply(d: number, a: UnityEngine.Vector3):UnityEngine.Vector3;
            public static op_Division(a: UnityEngine.Vector3, d: number):UnityEngine.Vector3;
            public static op_Equality(lhs: UnityEngine.Vector3, rhs: UnityEngine.Vector3):boolean;
            public static op_Inequality(lhs: UnityEngine.Vector3, rhs: UnityEngine.Vector3):boolean;
            public ToString():string;
            public ToString(format: string):string;
            public static AngleBetween(from: UnityEngine.Vector3, to: UnityEngine.Vector3):number;
            
        }
        class Color {
            
        }
        class Object extends System.Object {
            public name: string;
            public hideFlags: UnityEngine.HideFlags;
            public constructor();
            public static Destroy(obj: UnityEngine.Object, t: number):void;
            public static Destroy(obj: UnityEngine.Object):void;
            public static DestroyImmediate(obj: UnityEngine.Object, allowDestroyingAssets: boolean):void;
            public static DestroyImmediate(obj: UnityEngine.Object):void;
            public static FindObjectsOfType(type: System.Type):UnityEngine.Object[];
            public static DontDestroyOnLoad(target: UnityEngine.Object):void;
            public static DestroyObject(obj: UnityEngine.Object, t: number):void;
            public static DestroyObject(obj: UnityEngine.Object):void;
            public static FindSceneObjectsOfType(type: System.Type):UnityEngine.Object[];
            public static FindObjectsOfTypeIncludingAssets(type: System.Type):UnityEngine.Object[];
            public static FindObjectsOfTypeAll(type: System.Type):UnityEngine.Object[];
            public ToString():string;
            public GetInstanceID():number;
            public GetHashCode():number;
            public Equals(other: any):boolean;
            public static op_Implicit(exists: UnityEngine.Object):boolean;
            public static Instantiate(original: UnityEngine.Object, position: UnityEngine.Vector3, rotation: UnityEngine.Quaternion):UnityEngine.Object;
            public static Instantiate(original: UnityEngine.Object, position: UnityEngine.Vector3, rotation: UnityEngine.Quaternion, parent: UnityEngine.Transform):UnityEngine.Object;
            public static Instantiate(original: UnityEngine.Object):UnityEngine.Object;
            public static Instantiate(original: UnityEngine.Object, parent: UnityEngine.Transform):UnityEngine.Object;
            public static Instantiate(original: UnityEngine.Object, parent: UnityEngine.Transform, instantiateInWorldSpace: boolean):UnityEngine.Object;
            public static FindObjectOfType(type: System.Type):UnityEngine.Object;
            public static op_Equality(x: UnityEngine.Object, y: UnityEngine.Object):boolean;
            public static op_Inequality(x: UnityEngine.Object, y: UnityEngine.Object):boolean;
            
        }
        class Time extends System.Object {
            public static time: number;
            public static timeSinceLevelLoad: number;
            public static deltaTime: number;
            public static fixedTime: number;
            public static unscaledTime: number;
            public static fixedUnscaledTime: number;
            public static unscaledDeltaTime: number;
            public static fixedUnscaledDeltaTime: number;
            public static fixedDeltaTime: number;
            public static maximumDeltaTime: number;
            public static smoothDeltaTime: number;
            public static maximumParticleDeltaTime: number;
            public static timeScale: number;
            public static frameCount: number;
            public static renderedFrameCount: number;
            public static realtimeSinceStartup: number;
            public static captureFramerate: number;
            public static inFixedTimeStep: boolean;
            public constructor();
            
        }
        class Transform extends UnityEngine.Component {
            public position: UnityEngine.Vector3;
            public localPosition: UnityEngine.Vector3;
            public eulerAngles: UnityEngine.Vector3;
            public localEulerAngles: UnityEngine.Vector3;
            public right: UnityEngine.Vector3;
            public up: UnityEngine.Vector3;
            public forward: UnityEngine.Vector3;
            public rotation: UnityEngine.Quaternion;
            public localRotation: UnityEngine.Quaternion;
            public localScale: UnityEngine.Vector3;
            public parent: UnityEngine.Transform;
            public worldToLocalMatrix: UnityEngine.Matrix4x4;
            public localToWorldMatrix: UnityEngine.Matrix4x4;
            public root: UnityEngine.Transform;
            public childCount: number;
            public lossyScale: UnityEngine.Vector3;
            public hasChanged: boolean;
            public hierarchyCapacity: number;
            public hierarchyCount: number;
            public SetParent(parent: UnityEngine.Transform):void;
            public SetParent(parent: UnityEngine.Transform, worldPositionStays: boolean):void;
            public SetPositionAndRotation(position: UnityEngine.Vector3, rotation: UnityEngine.Quaternion):void;
            public Translate(translation: UnityEngine.Vector3):void;
            public Translate(translation: UnityEngine.Vector3, relativeTo: UnityEngine.Space):void;
            public Translate(x: number, y: number, z: number):void;
            public Translate(x: number, y: number, z: number, relativeTo: UnityEngine.Space):void;
            public Translate(translation: UnityEngine.Vector3, relativeTo: UnityEngine.Transform):void;
            public Translate(x: number, y: number, z: number, relativeTo: UnityEngine.Transform):void;
            public Rotate(eulerAngles: UnityEngine.Vector3):void;
            public Rotate(eulerAngles: UnityEngine.Vector3, relativeTo: UnityEngine.Space):void;
            public Rotate(xAngle: number, yAngle: number, zAngle: number):void;
            public Rotate(xAngle: number, yAngle: number, zAngle: number, relativeTo: UnityEngine.Space):void;
            public Rotate(axis: UnityEngine.Vector3, angle: number):void;
            public Rotate(axis: UnityEngine.Vector3, angle: number, relativeTo: UnityEngine.Space):void;
            public RotateAround(point: UnityEngine.Vector3, axis: UnityEngine.Vector3, angle: number):void;
            public LookAt(target: UnityEngine.Transform):void;
            public LookAt(target: UnityEngine.Transform, worldUp: UnityEngine.Vector3):void;
            public LookAt(worldPosition: UnityEngine.Vector3, worldUp: UnityEngine.Vector3):void;
            public LookAt(worldPosition: UnityEngine.Vector3):void;
            public TransformDirection(direction: UnityEngine.Vector3):UnityEngine.Vector3;
            public TransformDirection(x: number, y: number, z: number):UnityEngine.Vector3;
            public InverseTransformDirection(direction: UnityEngine.Vector3):UnityEngine.Vector3;
            public InverseTransformDirection(x: number, y: number, z: number):UnityEngine.Vector3;
            public TransformVector(vector: UnityEngine.Vector3):UnityEngine.Vector3;
            public TransformVector(x: number, y: number, z: number):UnityEngine.Vector3;
            public InverseTransformVector(vector: UnityEngine.Vector3):UnityEngine.Vector3;
            public InverseTransformVector(x: number, y: number, z: number):UnityEngine.Vector3;
            public TransformPoint(position: UnityEngine.Vector3):UnityEngine.Vector3;
            public TransformPoint(x: number, y: number, z: number):UnityEngine.Vector3;
            public InverseTransformPoint(position: UnityEngine.Vector3):UnityEngine.Vector3;
            public InverseTransformPoint(x: number, y: number, z: number):UnityEngine.Vector3;
            public DetachChildren():void;
            public SetAsFirstSibling():void;
            public SetAsLastSibling():void;
            public SetSiblingIndex(index: number):void;
            public GetSiblingIndex():number;
            public Find(name: string):UnityEngine.Transform;
            public IsChildOf(parent: UnityEngine.Transform):boolean;
            public FindChild(name: string):UnityEngine.Transform;
            public GetEnumerator():System.Collections.IEnumerator;
            public RotateAround(axis: UnityEngine.Vector3, angle: number):void;
            public RotateAroundLocal(axis: UnityEngine.Vector3, angle: number):void;
            public GetChild(index: number):UnityEngine.Transform;
            public GetChildCount():number;
            
        }
        class Quaternion {
            
        }
        class Matrix4x4 {
            
        }
        enum Space { World = 0, Self = 1 }
        class Component extends UnityEngine.Object {
            public transform: UnityEngine.Transform;
            public gameObject: UnityEngine.GameObject;
            public tag: string;
            public constructor();
            public GetComponent(type: System.Type):UnityEngine.Component;
            public GetComponent(type: string):UnityEngine.Component;
            public GetComponentInChildren(t: System.Type, includeInactive: boolean):UnityEngine.Component;
            public GetComponentInChildren(t: System.Type):UnityEngine.Component;
            public GetComponentsInChildren(t: System.Type):UnityEngine.Component[];
            public GetComponentsInChildren(t: System.Type, includeInactive: boolean):UnityEngine.Component[];
            public GetComponentInParent(t: System.Type):UnityEngine.Component;
            public GetComponentsInParent(t: System.Type):UnityEngine.Component[];
            public GetComponentsInParent(t: System.Type, includeInactive: boolean):UnityEngine.Component[];
            public GetComponents(type: System.Type):UnityEngine.Component[];
            public GetComponents(type: System.Type, results: System.Collections.Generic.List$1<UnityEngine.Component>):void;
            public CompareTag(tag: string):boolean;
            public SendMessageUpwards(methodName: string, value: any, options: UnityEngine.SendMessageOptions):void;
            public SendMessageUpwards(methodName: string, value: any):void;
            public SendMessageUpwards(methodName: string):void;
            public SendMessageUpwards(methodName: string, options: UnityEngine.SendMessageOptions):void;
            public SendMessage(methodName: string, value: any, options: UnityEngine.SendMessageOptions):void;
            public SendMessage(methodName: string, value: any):void;
            public SendMessage(methodName: string):void;
            public SendMessage(methodName: string, options: UnityEngine.SendMessageOptions):void;
            public BroadcastMessage(methodName: string, parameter: any, options: UnityEngine.SendMessageOptions):void;
            public BroadcastMessage(methodName: string, parameter: any):void;
            public BroadcastMessage(methodName: string):void;
            public BroadcastMessage(methodName: string, options: UnityEngine.SendMessageOptions):void;
            
        }
        class GameObject extends UnityEngine.Object {
            public transform: UnityEngine.Transform;
            public layer: number;
            public active: boolean;
            public activeSelf: boolean;
            public activeInHierarchy: boolean;
            public isStatic: boolean;
            public tag: string;
            public scene: UnityEngine.SceneManagement.Scene;
            public gameObject: UnityEngine.GameObject;
            public constructor(name: string);
            public constructor();
            public constructor(name: string, ...components: System.Type[]);
            public static CreatePrimitive(type: UnityEngine.PrimitiveType):UnityEngine.GameObject;
            public GetComponent(type: System.Type):UnityEngine.Component;
            public GetComponent(type: string):UnityEngine.Component;
            public GetComponentInChildren(type: System.Type, includeInactive: boolean):UnityEngine.Component;
            public GetComponentInChildren(type: System.Type):UnityEngine.Component;
            public GetComponentInParent(type: System.Type):UnityEngine.Component;
            public GetComponents(type: System.Type):UnityEngine.Component[];
            public GetComponents(type: System.Type, results: System.Collections.Generic.List$1<UnityEngine.Component>):void;
            public GetComponentsInChildren(type: System.Type):UnityEngine.Component[];
            public GetComponentsInChildren(type: System.Type, includeInactive: boolean):UnityEngine.Component[];
            public GetComponentsInParent(type: System.Type):UnityEngine.Component[];
            public GetComponentsInParent(type: System.Type, includeInactive: boolean):UnityEngine.Component[];
            public SetActive(value: boolean):void;
            public SetActiveRecursively(state: boolean):void;
            public CompareTag(tag: string):boolean;
            public static FindGameObjectWithTag(tag: string):UnityEngine.GameObject;
            public static FindWithTag(tag: string):UnityEngine.GameObject;
            public static FindGameObjectsWithTag(tag: string):UnityEngine.GameObject[];
            public SendMessageUpwards(methodName: string, value: any, options: UnityEngine.SendMessageOptions):void;
            public SendMessageUpwards(methodName: string, value: any):void;
            public SendMessageUpwards(methodName: string):void;
            public SendMessageUpwards(methodName: string, options: UnityEngine.SendMessageOptions):void;
            public SendMessage(methodName: string, value: any, options: UnityEngine.SendMessageOptions):void;
            public SendMessage(methodName: string, value: any):void;
            public SendMessage(methodName: string):void;
            public SendMessage(methodName: string, options: UnityEngine.SendMessageOptions):void;
            public BroadcastMessage(methodName: string, parameter: any, options: UnityEngine.SendMessageOptions):void;
            public BroadcastMessage(methodName: string, parameter: any):void;
            public BroadcastMessage(methodName: string):void;
            public BroadcastMessage(methodName: string, options: UnityEngine.SendMessageOptions):void;
            public AddComponent(componentType: System.Type):UnityEngine.Component;
            public static Find(name: string):UnityEngine.GameObject;
            
        }
        enum SendMessageOptions { RequireReceiver = 0, DontRequireReceiver = 1 }
        enum PrimitiveType { Sphere = 0, Capsule = 1, Cylinder = 2, Cube = 3, Plane = 4, Quad = 5 }
        enum HideFlags { None = 0, HideInHierarchy = 1, HideInInspector = 2, DontSaveInEditor = 4, NotEditable = 8, DontSaveInBuild = 16, DontUnloadUnusedAsset = 32, DontSave = 52, HideAndDontSave = 61 }
        class ParticleSystem extends UnityEngine.Component {
            public startDelay: number;
            public isPlaying: boolean;
            public isEmitting: boolean;
            public isStopped: boolean;
            public isPaused: boolean;
            public loop: boolean;
            public playOnAwake: boolean;
            public time: number;
            public duration: number;
            public playbackSpeed: number;
            public particleCount: number;
            public enableEmission: boolean;
            public emissionRate: number;
            public startSpeed: number;
            public startSize: number;
            public startColor: UnityEngine.Color;
            public startRotation: number;
            public startRotation3D: UnityEngine.Vector3;
            public startLifetime: number;
            public gravityModifier: number;
            public maxParticles: number;
            public simulationSpace: UnityEngine.ParticleSystemSimulationSpace;
            public scalingMode: UnityEngine.ParticleSystemScalingMode;
            public randomSeed: number;
            public useAutoRandomSeed: boolean;
            public main: UnityEngine.ParticleSystem.MainModule;
            public emission: UnityEngine.ParticleSystem.EmissionModule;
            public shape: UnityEngine.ParticleSystem.ShapeModule;
            public velocityOverLifetime: UnityEngine.ParticleSystem.VelocityOverLifetimeModule;
            public limitVelocityOverLifetime: UnityEngine.ParticleSystem.LimitVelocityOverLifetimeModule;
            public inheritVelocity: UnityEngine.ParticleSystem.InheritVelocityModule;
            public forceOverLifetime: UnityEngine.ParticleSystem.ForceOverLifetimeModule;
            public colorOverLifetime: UnityEngine.ParticleSystem.ColorOverLifetimeModule;
            public colorBySpeed: UnityEngine.ParticleSystem.ColorBySpeedModule;
            public sizeOverLifetime: UnityEngine.ParticleSystem.SizeOverLifetimeModule;
            public sizeBySpeed: UnityEngine.ParticleSystem.SizeBySpeedModule;
            public rotationOverLifetime: UnityEngine.ParticleSystem.RotationOverLifetimeModule;
            public rotationBySpeed: UnityEngine.ParticleSystem.RotationBySpeedModule;
            public externalForces: UnityEngine.ParticleSystem.ExternalForcesModule;
            public noise: UnityEngine.ParticleSystem.NoiseModule;
            public collision: UnityEngine.ParticleSystem.CollisionModule;
            public trigger: UnityEngine.ParticleSystem.TriggerModule;
            public subEmitters: UnityEngine.ParticleSystem.SubEmittersModule;
            public textureSheetAnimation: UnityEngine.ParticleSystem.TextureSheetAnimationModule;
            public lights: UnityEngine.ParticleSystem.LightsModule;
            public trails: UnityEngine.ParticleSystem.TrailModule;
            public customData: UnityEngine.ParticleSystem.CustomDataModule;
            public safeCollisionEventSize: number;
            public constructor();
            public SetParticles(particles: UnityEngine.ParticleSystem.Particle[], size: number):void;
            public GetParticles(particles: UnityEngine.ParticleSystem.Particle[]):number;
            public SetCustomParticleData(customData: System.Collections.Generic.List$1<UnityEngine.Vector4>, streamIndex: UnityEngine.ParticleSystemCustomData):void;
            public GetCustomParticleData(customData: System.Collections.Generic.List$1<UnityEngine.Vector4>, streamIndex: UnityEngine.ParticleSystemCustomData):number;
            public Simulate(t: number, withChildren: boolean, restart: boolean, fixedTimeStep: boolean):void;
            public Simulate(t: number, withChildren: boolean, restart: boolean):void;
            public Simulate(t: number, withChildren: boolean):void;
            public Simulate(t: number):void;
            public Play(withChildren: boolean):void;
            public Play():void;
            public Pause(withChildren: boolean):void;
            public Pause():void;
            public Stop(withChildren: boolean, stopBehavior: UnityEngine.ParticleSystemStopBehavior):void;
            public Stop(withChildren: boolean):void;
            public Stop():void;
            public Clear(withChildren: boolean):void;
            public Clear():void;
            public IsAlive(withChildren: boolean):boolean;
            public IsAlive():boolean;
            public Emit(count: number):void;
            public Emit(position: UnityEngine.Vector3, velocity: UnityEngine.Vector3, size: number, lifetime: number, color: UnityEngine.Color32):void;
            public Emit(particle: UnityEngine.ParticleSystem.Particle):void;
            public Emit(emitParams: UnityEngine.ParticleSystem.EmitParams, count: number):void;
            public static EnableMeshNonUniformScaleFix(enabled: boolean):void;
            
        }
        enum ParticleSystemSimulationSpace { Local = 0, World = 1, Custom = 2 }
        enum ParticleSystemScalingMode { Hierarchy = 0, Local = 1, Shape = 2 }
        class Vector4 {
            
        }
        enum ParticleSystemCustomData { Custom1 = 0, Custom2 = 1 }
        enum ParticleSystemStopBehavior { StopEmittingAndClear = 0, StopEmitting = 1 }
        class Color32 {
            
        }
        class Canvas extends UnityEngine.Behaviour {
            public renderMode: UnityEngine.RenderMode;
            public isRootCanvas: boolean;
            public worldCamera: UnityEngine.Camera;
            public pixelRect: UnityEngine.Rect;
            public scaleFactor: number;
            public referencePixelsPerUnit: number;
            public overridePixelPerfect: boolean;
            public pixelPerfect: boolean;
            public planeDistance: number;
            public renderOrder: number;
            public overrideSorting: boolean;
            public sortingOrder: number;
            public targetDisplay: number;
            public sortingGridNormalizedSize: number;
            public normalizedSortingGridSize: number;
            public sortingLayerID: number;
            public cachedSortingLayerValue: number;
            public additionalShaderChannels: UnityEngine.AdditionalCanvasShaderChannels;
            public sortingLayerName: string;
            public rootCanvas: UnityEngine.Canvas;
            public constructor();
            public static GetDefaultCanvasMaterial():UnityEngine.Material;
            public static GetETC1SupportedCanvasMaterial():UnityEngine.Material;
            public static GetDefaultCanvasTextMaterial():UnityEngine.Material;
            public static add_willRenderCanvases(value: UnityEngine.Canvas.WillRenderCanvases):void;
            public static remove_willRenderCanvases(value: UnityEngine.Canvas.WillRenderCanvases):void;
            public static ForceUpdateCanvases():void;
            
        }
        enum RenderMode { ScreenSpaceOverlay = 0, ScreenSpaceCamera = 1, WorldSpace = 2 }
        class Camera {
            
        }
        class Rect {
            
        }
        enum AdditionalCanvasShaderChannels { None = 0, TexCoord1 = 1, TexCoord2 = 2, TexCoord3 = 4, Normal = 8, Tangent = 16 }
        class Material {
            
        }
        class Behaviour extends UnityEngine.Component {
            public enabled: boolean;
            public isActiveAndEnabled: boolean;
            public constructor();
            
        }
        class MonoBehaviour extends UnityEngine.Behaviour {
            public useGUILayout: boolean;
            public runInEditMode: boolean;
            public constructor();
            public Invoke(methodName: string, time: number):void;
            public InvokeRepeating(methodName: string, time: number, repeatRate: number):void;
            public CancelInvoke():void;
            public CancelInvoke(methodName: string):void;
            public IsInvoking(methodName: string):boolean;
            public IsInvoking():boolean;
            public StartCoroutine(routine: System.Collections.IEnumerator):UnityEngine.Coroutine;
            public StartCoroutine_Auto(routine: System.Collections.IEnumerator):UnityEngine.Coroutine;
            public StartCoroutine(methodName: string, value: any):UnityEngine.Coroutine;
            public StartCoroutine(methodName: string):UnityEngine.Coroutine;
            public StopCoroutine(methodName: string):void;
            public StopCoroutine(routine: System.Collections.IEnumerator):void;
            public StopCoroutine(routine: UnityEngine.Coroutine):void;
            public StopAllCoroutines():void;
            public static print(message: any):void;
            
        }
        class Coroutine {
            
        }
        class Animator {
            
        }
        enum TouchScreenKeyboardType { Default = 0, ASCIICapable = 1, NumbersAndPunctuation = 2, URL = 3, NumberPad = 4, PhonePad = 5, NamePhonePad = 6, EmailAddress = 7, NintendoNetworkAccount = 8, Social = 9, Search = 10 }
        class Vector2 {
            
        }
        class Event {
            
        }
        
    }
    namespace System {
        class Void {
            
        }
        class Single {
            
        }
        class Boolean {
            
        }
        class Object {
            public constructor();
            public Equals(obj: any):boolean;
            public static Equals(objA: any, objB: any):boolean;
            public GetHashCode():number;
            public GetType():System.Type;
            public ToString():string;
            public static ReferenceEquals(objA: any, objB: any):boolean;
            
        }
        class String {
            
        }
        class Exception {
            
        }
        class Int32 {
            
        }
        class ValueType {
            
        }
        type Converter$2<TInput,TOutput> = (input: TInput) => TOutput;
        type Predicate$1<T> = (obj: T) => boolean;
        type Action$1<T> = (obj: T) => void;
        type Comparison$1<T> = (x: T, y: T) => number;
        class Double {
            
        }
        class Enum {
            
        }
        class Type extends System.Reflection.MemberInfo {
            public static Delimiter: System.Char;
            public static EmptyTypes: System.Type[];
            public static FilterAttribute: System.Reflection.MemberFilter;
            public static FilterName: System.Reflection.MemberFilter;
            public static FilterNameIgnoreCase: System.Reflection.MemberFilter;
            public static Missing: any;
            public Assembly: System.Reflection.Assembly;
            public AssemblyQualifiedName: string;
            public Attributes: System.Reflection.TypeAttributes;
            public BaseType: System.Type;
            public DeclaringType: System.Type;
            public static DefaultBinder: System.Reflection.Binder;
            public FullName: string;
            public GUID: System.Guid;
            public HasElementType: boolean;
            public IsAbstract: boolean;
            public IsAnsiClass: boolean;
            public IsArray: boolean;
            public IsAutoClass: boolean;
            public IsAutoLayout: boolean;
            public IsByRef: boolean;
            public IsClass: boolean;
            public IsCOMObject: boolean;
            public IsContextful: boolean;
            public IsEnum: boolean;
            public IsExplicitLayout: boolean;
            public IsImport: boolean;
            public IsInterface: boolean;
            public IsLayoutSequential: boolean;
            public IsMarshalByRef: boolean;
            public IsNestedAssembly: boolean;
            public IsNestedFamANDAssem: boolean;
            public IsNestedFamily: boolean;
            public IsNestedFamORAssem: boolean;
            public IsNestedPrivate: boolean;
            public IsNestedPublic: boolean;
            public IsNotPublic: boolean;
            public IsPointer: boolean;
            public IsPrimitive: boolean;
            public IsPublic: boolean;
            public IsSealed: boolean;
            public IsSerializable: boolean;
            public IsSpecialName: boolean;
            public IsUnicodeClass: boolean;
            public IsValueType: boolean;
            public MemberType: System.Reflection.MemberTypes;
            public Module: System.Reflection.Module;
            public Namespace: string;
            public ReflectedType: System.Type;
            public TypeHandle: System.RuntimeTypeHandle;
            public TypeInitializer: System.Reflection.ConstructorInfo;
            public UnderlyingSystemType: System.Type;
            public ContainsGenericParameters: boolean;
            public IsGenericTypeDefinition: boolean;
            public IsGenericType: boolean;
            public IsGenericParameter: boolean;
            public IsNested: boolean;
            public IsVisible: boolean;
            public GenericParameterPosition: number;
            public GenericParameterAttributes: System.Reflection.GenericParameterAttributes;
            public DeclaringMethod: System.Reflection.MethodBase;
            public StructLayoutAttribute: System.Runtime.InteropServices.StructLayoutAttribute;
            public Equals(o: any):boolean;
            public Equals(o: System.Type):boolean;
            public static GetType(typeName: string):System.Type;
            public static GetType(typeName: string, throwOnError: boolean):System.Type;
            public static GetType(typeName: string, throwOnError: boolean, ignoreCase: boolean):System.Type;
            public static GetTypeArray(args: any[]):System.Type[];
            public static GetTypeCode(type: System.Type):System.TypeCode;
            public static GetTypeFromCLSID(clsid: System.Guid):System.Type;
            public static GetTypeFromCLSID(clsid: System.Guid, throwOnError: boolean):System.Type;
            public static GetTypeFromCLSID(clsid: System.Guid, server: string):System.Type;
            public static GetTypeFromCLSID(clsid: System.Guid, server: string, throwOnError: boolean):System.Type;
            public static GetTypeFromHandle(handle: System.RuntimeTypeHandle):System.Type;
            public static GetTypeFromProgID(progID: string):System.Type;
            public static GetTypeFromProgID(progID: string, throwOnError: boolean):System.Type;
            public static GetTypeFromProgID(progID: string, server: string):System.Type;
            public static GetTypeFromProgID(progID: string, server: string, throwOnError: boolean):System.Type;
            public static GetTypeHandle(o: any):System.RuntimeTypeHandle;
            public GetType():System.Type;
            public IsSubclassOf(c: System.Type):boolean;
            public FindInterfaces(filter: System.Reflection.TypeFilter, filterCriteria: any):System.Type[];
            public GetInterface(name: string):System.Type;
            public GetInterface(name: string, ignoreCase: boolean):System.Type;
            public GetInterfaceMap(interfaceType: System.Type):System.Reflection.InterfaceMapping;
            public GetInterfaces():System.Type[];
            public IsAssignableFrom(c: System.Type):boolean;
            public IsInstanceOfType(o: any):boolean;
            public GetArrayRank():number;
            public GetElementType():System.Type;
            public GetEvent(name: string):System.Reflection.EventInfo;
            public GetEvent(name: string, bindingAttr: System.Reflection.BindingFlags):System.Reflection.EventInfo;
            public GetEvents():System.Reflection.EventInfo[];
            public GetEvents(bindingAttr: System.Reflection.BindingFlags):System.Reflection.EventInfo[];
            public GetField(name: string):System.Reflection.FieldInfo;
            public GetField(name: string, bindingAttr: System.Reflection.BindingFlags):System.Reflection.FieldInfo;
            public GetFields():System.Reflection.FieldInfo[];
            public GetFields(bindingAttr: System.Reflection.BindingFlags):System.Reflection.FieldInfo[];
            public GetHashCode():number;
            public GetMember(name: string):System.Reflection.MemberInfo[];
            public GetMember(name: string, bindingAttr: System.Reflection.BindingFlags):System.Reflection.MemberInfo[];
            public GetMember(name: string, type: System.Reflection.MemberTypes, bindingAttr: System.Reflection.BindingFlags):System.Reflection.MemberInfo[];
            public GetMembers():System.Reflection.MemberInfo[];
            public GetMembers(bindingAttr: System.Reflection.BindingFlags):System.Reflection.MemberInfo[];
            public GetMethod(name: string):System.Reflection.MethodInfo;
            public GetMethod(name: string, bindingAttr: System.Reflection.BindingFlags):System.Reflection.MethodInfo;
            public GetMethod(name: string, types: System.Type[]):System.Reflection.MethodInfo;
            public GetMethod(name: string, types: System.Type[], modifiers: System.Reflection.ParameterModifier[]):System.Reflection.MethodInfo;
            public GetMethod(name: string, bindingAttr: System.Reflection.BindingFlags, binder: System.Reflection.Binder, types: System.Type[], modifiers: System.Reflection.ParameterModifier[]):System.Reflection.MethodInfo;
            public GetMethod(name: string, bindingAttr: System.Reflection.BindingFlags, binder: System.Reflection.Binder, callConvention: System.Reflection.CallingConventions, types: System.Type[], modifiers: System.Reflection.ParameterModifier[]):System.Reflection.MethodInfo;
            public GetMethods():System.Reflection.MethodInfo[];
            public GetMethods(bindingAttr: System.Reflection.BindingFlags):System.Reflection.MethodInfo[];
            public GetNestedType(name: string):System.Type;
            public GetNestedType(name: string, bindingAttr: System.Reflection.BindingFlags):System.Type;
            public GetNestedTypes():System.Type[];
            public GetNestedTypes(bindingAttr: System.Reflection.BindingFlags):System.Type[];
            public GetProperties():System.Reflection.PropertyInfo[];
            public GetProperties(bindingAttr: System.Reflection.BindingFlags):System.Reflection.PropertyInfo[];
            public GetProperty(name: string):System.Reflection.PropertyInfo;
            public GetProperty(name: string, bindingAttr: System.Reflection.BindingFlags):System.Reflection.PropertyInfo;
            public GetProperty(name: string, returnType: System.Type):System.Reflection.PropertyInfo;
            public GetProperty(name: string, types: System.Type[]):System.Reflection.PropertyInfo;
            public GetProperty(name: string, returnType: System.Type, types: System.Type[]):System.Reflection.PropertyInfo;
            public GetProperty(name: string, returnType: System.Type, types: System.Type[], modifiers: System.Reflection.ParameterModifier[]):System.Reflection.PropertyInfo;
            public GetProperty(name: string, bindingAttr: System.Reflection.BindingFlags, binder: System.Reflection.Binder, returnType: System.Type, types: System.Type[], modifiers: System.Reflection.ParameterModifier[]):System.Reflection.PropertyInfo;
            public GetConstructor(types: System.Type[]):System.Reflection.ConstructorInfo;
            public GetConstructor(bindingAttr: System.Reflection.BindingFlags, binder: System.Reflection.Binder, types: System.Type[], modifiers: System.Reflection.ParameterModifier[]):System.Reflection.ConstructorInfo;
            public GetConstructor(bindingAttr: System.Reflection.BindingFlags, binder: System.Reflection.Binder, callConvention: System.Reflection.CallingConventions, types: System.Type[], modifiers: System.Reflection.ParameterModifier[]):System.Reflection.ConstructorInfo;
            public GetConstructors():System.Reflection.ConstructorInfo[];
            public GetConstructors(bindingAttr: System.Reflection.BindingFlags):System.Reflection.ConstructorInfo[];
            public GetDefaultMembers():System.Reflection.MemberInfo[];
            public FindMembers(memberType: System.Reflection.MemberTypes, bindingAttr: System.Reflection.BindingFlags, filter: System.Reflection.MemberFilter, filterCriteria: any):System.Reflection.MemberInfo[];
            public InvokeMember(name: string, invokeAttr: System.Reflection.BindingFlags, binder: System.Reflection.Binder, target: any, args: any[]):any;
            public InvokeMember(name: string, invokeAttr: System.Reflection.BindingFlags, binder: System.Reflection.Binder, target: any, args: any[], culture: System.Globalization.CultureInfo):any;
            public InvokeMember(name: string, invokeAttr: System.Reflection.BindingFlags, binder: System.Reflection.Binder, target: any, args: any[], modifiers: System.Reflection.ParameterModifier[], culture: System.Globalization.CultureInfo, namedParameters: string[]):any;
            public ToString():string;
            public GetGenericArguments():System.Type[];
            public GetGenericTypeDefinition():System.Type;
            public MakeGenericType(...typeArguments: System.Type[]):System.Type;
            public GetGenericParameterConstraints():System.Type[];
            public MakeArrayType():System.Type;
            public MakeArrayType(rank: number):System.Type;
            public MakeByRefType():System.Type;
            public MakePointerType():System.Type;
            public static ReflectionOnlyGetType(typeName: string, throwIfNotFound: boolean, ignoreCase: boolean):System.Type;
            
        }
        class Delegate extends System.Object {
            public Method: System.Reflection.MethodInfo;
            public Target: any;
            public static CreateDelegate(type: System.Type, firstArgument: any, method: System.Reflection.MethodInfo, throwOnBindFailure: boolean):Function;
            public static CreateDelegate(type: System.Type, firstArgument: any, method: System.Reflection.MethodInfo):Function;
            public static CreateDelegate(type: System.Type, method: System.Reflection.MethodInfo, throwOnBindFailure: boolean):Function;
            public static CreateDelegate(type: System.Type, method: System.Reflection.MethodInfo):Function;
            public static CreateDelegate(type: System.Type, target: any, method: string):Function;
            public static CreateDelegate(type: System.Type, target: System.Type, method: string, ignoreCase: boolean, throwOnBindFailure: boolean):Function;
            public static CreateDelegate(type: System.Type, target: System.Type, method: string):Function;
            public static CreateDelegate(type: System.Type, target: System.Type, method: string, ignoreCase: boolean):Function;
            public static CreateDelegate(type: System.Type, target: any, method: string, ignoreCase: boolean, throwOnBindFailure: boolean):Function;
            public static CreateDelegate(type: System.Type, target: any, method: string, ignoreCase: boolean):Function;
            public DynamicInvoke(...args: any[]):any;
            public Clone():any;
            public Equals(obj: any):boolean;
            public GetHashCode():number;
            public GetObjectData(info: System.Runtime.Serialization.SerializationInfo, context: System.Runtime.Serialization.StreamingContext):void;
            public GetInvocationList():Function[];
            public static Combine(a: Function, b: Function):Function;
            public static Combine(...delegates: Function[]):Function;
            public static Remove(source: Function, value: Function):Function;
            public static RemoveAll(source: Function, value: Function):Function;
            public static op_Equality(d1: Function, d2: Function):boolean;
            public static op_Inequality(d1: Function, d2: Function):boolean;
            
        }
        class Char {
            
        }
        class Guid {
            
        }
        class RuntimeTypeHandle {
            
        }
        enum TypeCode { Empty = 0, Object = 1, DBNull = 2, Boolean = 3, Char = 4, SByte = 5, Byte = 6, Int16 = 7, UInt16 = 8, Int32 = 9, UInt32 = 10, Int64 = 11, UInt64 = 12, Single = 13, Double = 14, Decimal = 15, DateTime = 16, String = 18 }
        class UInt32 {
            
        }
        
    }
    namespace PuertsTest {
        class TestClass extends System.Object {
            public constructor();
            public AddEventCallback1(callback1: PuertsTest.Callback1):void;
            public RemoveEventCallback1(callback1: PuertsTest.Callback1):void;
            public AddEventCallback2(callback2: PuertsTest.Callback2):void;
            public Trigger():void;
            public Foo():void;
            
        }
        type Callback1 = (obj: PuertsTest.TestClass) => void;
        var Callback1: {new (func: (obj: PuertsTest.TestClass) => void): Callback1;}
        type Callback2 = (str: number) => void;
        var Callback2: {new (func: (str: number) => void): Callback2;}
        class BaseClass extends System.Object {
            public static BSF: number;
            public BMF: number;
            public constructor();
            public static BSFunc():void;
            public BMFunc():void;
            
        }
        class DerivedClass extends PuertsTest.BaseClass {
            public static DSF: number;
            public MyCallback: PuertsTest.MyCallback;
            public DMF: number;
            public constructor();
            public static DSFunc():void;
            public DMFunc():void;
            public DMFunc(myEnum: PuertsTest.MyEnum):PuertsTest.MyEnum;
            public add_MyEvent(value: PuertsTest.MyCallback):void;
            public remove_MyEvent(value: PuertsTest.MyCallback):void;
            public static add_MyStaticEvent(value: PuertsTest.MyCallback):void;
            public static remove_MyStaticEvent(value: PuertsTest.MyCallback):void;
            public Trigger():void;
            public ParamsFunc(a: number, ...b: string[]):number;
            public InOutArgFunc(a: number, b: $Ref<number>, c: $Ref<number>):number;
            public PrintList(lst: System.Collections.Generic.List$1<number>):void;
            
        }
        type MyCallback = (msg: string) => void;
        var MyCallback: {new (func: (msg: string) => void): MyCallback;}
        enum MyEnum { E1 = 0, E2 = 1 }
        
    }
    namespace System.Collections.Generic {
        class List$1<T> extends System.Object {
            public Capacity: number;
            public Count: number;
            public Item: T;
            public constructor();
            public constructor(collection: System.Collections.Generic.IEnumerable$1<T>);
            public constructor(capacity: number);
            public Add(item: T):void;
            public AddRange(collection: System.Collections.Generic.IEnumerable$1<T>):void;
            public AsReadOnly():System.Collections.ObjectModel.ReadOnlyCollection$1<T>;
            public BinarySearch(item: T):number;
            public BinarySearch(item: T, comparer: System.Collections.Generic.IComparer$1<T>):number;
            public BinarySearch(index: number, count: number, item: T, comparer: System.Collections.Generic.IComparer$1<T>):number;
            public Clear():void;
            public Contains(item: T):boolean;
            public CopyTo(array: T[]):void;
            public CopyTo(array: T[], arrayIndex: number):void;
            public CopyTo(index: number, array: T[], arrayIndex: number, count: number):void;
            public Exists(match: System.Predicate$1<T>):boolean;
            public Find(match: System.Predicate$1<T>):T;
            public FindAll(match: System.Predicate$1<T>):System.Collections.Generic.List$1<T>;
            public FindIndex(match: System.Predicate$1<T>):number;
            public FindIndex(startIndex: number, match: System.Predicate$1<T>):number;
            public FindIndex(startIndex: number, count: number, match: System.Predicate$1<T>):number;
            public FindLast(match: System.Predicate$1<T>):T;
            public FindLastIndex(match: System.Predicate$1<T>):number;
            public FindLastIndex(startIndex: number, match: System.Predicate$1<T>):number;
            public FindLastIndex(startIndex: number, count: number, match: System.Predicate$1<T>):number;
            public ForEach(action: System.Action$1<T>):void;
            public GetEnumerator():System.Collections.Generic.List$1.Enumerator<T>;
            public GetRange(index: number, count: number):System.Collections.Generic.List$1<T>;
            public IndexOf(item: T):number;
            public IndexOf(item: T, index: number):number;
            public IndexOf(item: T, index: number, count: number):number;
            public Insert(index: number, item: T):void;
            public InsertRange(index: number, collection: System.Collections.Generic.IEnumerable$1<T>):void;
            public LastIndexOf(item: T):number;
            public LastIndexOf(item: T, index: number):number;
            public LastIndexOf(item: T, index: number, count: number):number;
            public Remove(item: T):boolean;
            public RemoveAll(match: System.Predicate$1<T>):number;
            public RemoveAt(index: number):void;
            public RemoveRange(index: number, count: number):void;
            public Reverse():void;
            public Reverse(index: number, count: number):void;
            public Sort():void;
            public Sort(comparer: System.Collections.Generic.IComparer$1<T>):void;
            public Sort(comparison: System.Comparison$1<T>):void;
            public Sort(index: number, count: number, comparer: System.Collections.Generic.IComparer$1<T>):void;
            public ToArray():T[];
            public TrimExcess():void;
            public TrueForAll(match: System.Predicate$1<T>):boolean;
            public get_Item(index: number):T;
            public set_Item(index: number, value: T):void;
            
        }
        interface IEnumerable$1<T> {
            
        }
        interface IComparer$1<T> {
            
        }
        
    }
    namespace System.Collections.ObjectModel {
        class ReadOnlyCollection$1<T> {
            
        }
        
    }
    namespace System.Collections.Generic.List$1 {
        class Enumerator<T> {
            
        }
        
    }
    namespace System.Collections {
        interface IEnumerator {
            
        }
        
    }
    namespace UnityEngine.SceneManagement {
        class Scene {
            
        }
        
    }
    namespace System.Reflection {
        class MethodInfo {
            
        }
        type MemberFilter = (m: System.Reflection.MemberInfo, filterCriteria: any) => boolean;
        var MemberFilter: {new (func: (m: System.Reflection.MemberInfo, filterCriteria: any) => boolean): MemberFilter;}
        class Assembly {
            
        }
        enum TypeAttributes { VisibilityMask = 7, NotPublic = 0, Public = 1, NestedPublic = 2, NestedPrivate = 3, NestedFamily = 4, NestedAssembly = 5, NestedFamANDAssem = 6, NestedFamORAssem = 7, LayoutMask = 24, AutoLayout = 0, SequentialLayout = 8, ExplicitLayout = 16, ClassSemanticsMask = 32, Class = 0, Interface = 32, Abstract = 128, Sealed = 256, SpecialName = 1024, Import = 4096, Serializable = 8192, StringFormatMask = 196608, AnsiClass = 0, UnicodeClass = 65536, AutoClass = 131072, BeforeFieldInit = 1048576, ReservedMask = 264192, RTSpecialName = 2048, HasSecurity = 262144, CustomFormatClass = 196608, CustomFormatMask = 12582912 }
        class Binder {
            
        }
        enum MemberTypes { Constructor = 1, Event = 2, Field = 4, Method = 8, Property = 16, TypeInfo = 32, Custom = 64, NestedType = 128, All = 191 }
        class Module {
            
        }
        class ConstructorInfo {
            
        }
        type TypeFilter = (m: System.Type, filterCriteria: any) => boolean;
        var TypeFilter: {new (func: (m: System.Type, filterCriteria: any) => boolean): TypeFilter;}
        class InterfaceMapping {
            
        }
        class EventInfo {
            
        }
        enum BindingFlags { Default = 0, IgnoreCase = 1, DeclaredOnly = 2, Instance = 4, Static = 8, Public = 16, NonPublic = 32, FlattenHierarchy = 64, InvokeMethod = 256, CreateInstance = 512, GetField = 1024, SetField = 2048, GetProperty = 4096, SetProperty = 8192, PutDispProperty = 16384, PutRefDispProperty = 32768, ExactBinding = 65536, SuppressChangeType = 131072, OptionalParamBinding = 262144, IgnoreReturn = 16777216 }
        class FieldInfo {
            
        }
        class MemberInfo {
            
        }
        class ParameterModifier {
            
        }
        enum CallingConventions { Standard = 1, VarArgs = 2, Any = 3, HasThis = 32, ExplicitThis = 64 }
        class PropertyInfo {
            
        }
        enum GenericParameterAttributes { Covariant = 1, Contravariant = 2, VarianceMask = 3, None = 0, ReferenceTypeConstraint = 4, NotNullableValueTypeConstraint = 8, DefaultConstructorConstraint = 16, SpecialConstraintMask = 28 }
        class MethodBase {
            
        }
        
    }
    namespace System.Runtime.Serialization {
        class SerializationInfo {
            
        }
        class StreamingContext {
            
        }
        
    }
    namespace System.Globalization {
        class CultureInfo {
            
        }
        
    }
    namespace System.Runtime.InteropServices {
        class StructLayoutAttribute {
            
        }
        
    }
    namespace UnityEngine.ParticleSystem {
        class MainModule {
            
        }
        class EmissionModule {
            
        }
        class ShapeModule {
            
        }
        class VelocityOverLifetimeModule {
            
        }
        class LimitVelocityOverLifetimeModule {
            
        }
        class InheritVelocityModule {
            
        }
        class ForceOverLifetimeModule {
            
        }
        class ColorOverLifetimeModule {
            
        }
        class ColorBySpeedModule {
            
        }
        class SizeOverLifetimeModule {
            
        }
        class SizeBySpeedModule {
            
        }
        class RotationOverLifetimeModule {
            
        }
        class RotationBySpeedModule {
            
        }
        class ExternalForcesModule {
            
        }
        class NoiseModule {
            
        }
        class CollisionModule {
            
        }
        class TriggerModule {
            
        }
        class SubEmittersModule {
            
        }
        class TextureSheetAnimationModule {
            
        }
        class LightsModule {
            
        }
        class TrailModule {
            
        }
        class CustomDataModule {
            
        }
        class Particle {
            
        }
        class EmitParams {
            
        }
        
    }
    namespace UnityEngine.Canvas {
        type WillRenderCanvases = () => void;
        var WillRenderCanvases: {new (func: () => void): WillRenderCanvases;}
        
    }
    namespace UnityEngine.EventSystems {
        class UIBehaviour extends UnityEngine.MonoBehaviour {
            public IsActive():boolean;
            public IsDestroyed():boolean;
            
        }
        class AxisEventData {
            
        }
        class PointerEventData {
            
        }
        class BaseEventData {
            
        }
        
    }
    namespace UnityEngine.UI {
        class Selectable extends UnityEngine.EventSystems.UIBehaviour {
            public static allSelectables: System.Collections.Generic.List$1<UnityEngine.UI.Selectable>;
            public navigation: UnityEngine.UI.Navigation;
            public transition: UnityEngine.UI.Selectable.Transition;
            public colors: UnityEngine.UI.ColorBlock;
            public spriteState: UnityEngine.UI.SpriteState;
            public animationTriggers: UnityEngine.UI.AnimationTriggers;
            public targetGraphic: UnityEngine.UI.Graphic;
            public interactable: boolean;
            public image: UnityEngine.UI.Image;
            public animator: UnityEngine.Animator;
            public IsInteractable():boolean;
            public FindSelectable(dir: UnityEngine.Vector3):UnityEngine.UI.Selectable;
            public FindSelectableOnLeft():UnityEngine.UI.Selectable;
            public FindSelectableOnRight():UnityEngine.UI.Selectable;
            public FindSelectableOnUp():UnityEngine.UI.Selectable;
            public FindSelectableOnDown():UnityEngine.UI.Selectable;
            public OnMove(eventData: UnityEngine.EventSystems.AxisEventData):void;
            public OnPointerDown(eventData: UnityEngine.EventSystems.PointerEventData):void;
            public OnPointerUp(eventData: UnityEngine.EventSystems.PointerEventData):void;
            public OnPointerEnter(eventData: UnityEngine.EventSystems.PointerEventData):void;
            public OnPointerExit(eventData: UnityEngine.EventSystems.PointerEventData):void;
            public OnSelect(eventData: UnityEngine.EventSystems.BaseEventData):void;
            public OnDeselect(eventData: UnityEngine.EventSystems.BaseEventData):void;
            public Select():void;
            
        }
        class Navigation {
            
        }
        class ColorBlock {
            
        }
        class SpriteState {
            
        }
        class AnimationTriggers {
            
        }
        class Graphic {
            
        }
        class Image {
            
        }
        class Button extends UnityEngine.UI.Selectable {
            public onClick: UnityEngine.UI.Button.ButtonClickedEvent;
            public OnPointerClick(eventData: UnityEngine.EventSystems.PointerEventData):void;
            public OnSubmit(eventData: UnityEngine.EventSystems.BaseEventData):void;
            
        }
        class InputField extends UnityEngine.UI.Selectable {
            public shouldHideMobileInput: boolean;
            public text: string;
            public isFocused: boolean;
            public caretBlinkRate: number;
            public caretWidth: number;
            public textComponent: UnityEngine.UI.Text;
            public placeholder: UnityEngine.UI.Graphic;
            public caretColor: UnityEngine.Color;
            public customCaretColor: boolean;
            public selectionColor: UnityEngine.Color;
            public onEndEdit: UnityEngine.UI.InputField.SubmitEvent;
            public onValueChange: UnityEngine.UI.InputField.OnChangeEvent;
            public onValueChanged: UnityEngine.UI.InputField.OnChangeEvent;
            public onValidateInput: UnityEngine.UI.InputField.OnValidateInput;
            public characterLimit: number;
            public contentType: UnityEngine.UI.InputField.ContentType;
            public lineType: UnityEngine.UI.InputField.LineType;
            public inputType: UnityEngine.UI.InputField.InputType;
            public keyboardType: UnityEngine.TouchScreenKeyboardType;
            public characterValidation: UnityEngine.UI.InputField.CharacterValidation;
            public readOnly: boolean;
            public multiLine: boolean;
            public asteriskChar: System.Char;
            public wasCanceled: boolean;
            public caretPosition: number;
            public selectionAnchorPosition: number;
            public selectionFocusPosition: number;
            public minWidth: number;
            public preferredWidth: number;
            public flexibleWidth: number;
            public minHeight: number;
            public preferredHeight: number;
            public flexibleHeight: number;
            public layoutPriority: number;
            public MoveTextEnd(shift: boolean):void;
            public MoveTextStart(shift: boolean):void;
            public ScreenToLocal(screen: UnityEngine.Vector2):UnityEngine.Vector2;
            public OnBeginDrag(eventData: UnityEngine.EventSystems.PointerEventData):void;
            public OnDrag(eventData: UnityEngine.EventSystems.PointerEventData):void;
            public OnEndDrag(eventData: UnityEngine.EventSystems.PointerEventData):void;
            public OnPointerDown(eventData: UnityEngine.EventSystems.PointerEventData):void;
            public ProcessEvent(e: UnityEngine.Event):void;
            public OnUpdateSelected(eventData: UnityEngine.EventSystems.BaseEventData):void;
            public ForceLabelUpdate():void;
            public Rebuild(update: UnityEngine.UI.CanvasUpdate):void;
            public LayoutComplete():void;
            public GraphicUpdateComplete():void;
            public ActivateInputField():void;
            public OnSelect(eventData: UnityEngine.EventSystems.BaseEventData):void;
            public OnPointerClick(eventData: UnityEngine.EventSystems.PointerEventData):void;
            public DeactivateInputField():void;
            public OnDeselect(eventData: UnityEngine.EventSystems.BaseEventData):void;
            public OnSubmit(eventData: UnityEngine.EventSystems.BaseEventData):void;
            public CalculateLayoutInputHorizontal():void;
            public CalculateLayoutInputVertical():void;
            
        }
        class Text {
            
        }
        enum CanvasUpdate { Prelayout = 0, Layout = 1, PostLayout = 2, PreRender = 3, LatePreRender = 4, MaxUpdateValue = 5 }
        class Toggle extends UnityEngine.UI.Selectable {
            public toggleTransition: UnityEngine.UI.Toggle.ToggleTransition;
            public graphic: UnityEngine.UI.Graphic;
            public onValueChanged: UnityEngine.UI.Toggle.ToggleEvent;
            public group: UnityEngine.UI.ToggleGroup;
            public isOn: boolean;
            public Rebuild(executing: UnityEngine.UI.CanvasUpdate):void;
            public LayoutComplete():void;
            public GraphicUpdateComplete():void;
            public OnPointerClick(eventData: UnityEngine.EventSystems.PointerEventData):void;
            public OnSubmit(eventData: UnityEngine.EventSystems.BaseEventData):void;
            
        }
        class ToggleGroup {
            
        }
        
    }
    namespace UnityEngine.UI.Selectable {
        enum Transition { None = 0, ColorTint = 1, SpriteSwap = 2, Animation = 3 }
        
    }
    namespace UnityEngine.UI.Button {
        class ButtonClickedEvent extends UnityEngine.Events.UnityEvent {
            public constructor();
            
        }
        
    }
    namespace UnityEngine.Events {
        class UnityEvent extends UnityEngine.Events.UnityEventBase {
            public constructor();
            public AddListener(call: UnityEngine.Events.UnityAction):void;
            public RemoveListener(call: UnityEngine.Events.UnityAction):void;
            public Invoke():void;
            
        }
        class UnityEventBase {
            
        }
        type UnityAction = () => void;
        var UnityAction: {new (func: () => void): UnityAction;}
        class UnityEvent$1<T0> extends UnityEngine.Events.UnityEventBase {
            public AddListener(call: UnityEngine.Events.UnityAction$1<T0>):void;
            public RemoveListener(call: UnityEngine.Events.UnityAction$1<T0>):void;
            public Invoke(arg0: T0):void;
            
        }
        type UnityAction$1<T0> = (arg0: T0) => void;
        
    }
    namespace UnityEngine.UI.InputField {
        class SubmitEvent {
            
        }
        class OnChangeEvent {
            
        }
        type OnValidateInput = (text: string, charIndex: number, addedChar: System.Char) => System.Char;
        var OnValidateInput: {new (func: (text: string, charIndex: number, addedChar: System.Char) => System.Char): OnValidateInput;}
        enum ContentType { Standard = 0, Autocorrected = 1, IntegerNumber = 2, DecimalNumber = 3, Alphanumeric = 4, Name = 5, EmailAddress = 6, Password = 7, Pin = 8, Custom = 9 }
        enum LineType { SingleLine = 0, MultiLineSubmit = 1, MultiLineNewline = 2 }
        enum InputType { Standard = 0, AutoCorrect = 1, Password = 2 }
        enum CharacterValidation { None = 0, Integer = 1, Decimal = 2, Alphanumeric = 3, Name = 4, EmailAddress = 5 }
        
    }
    namespace UnityEngine.UI.Toggle {
        enum ToggleTransition { None = 0, Fade = 1 }
        class ToggleEvent extends UnityEngine.Events.UnityEvent$1<boolean> {
            public constructor();
            
        }
        
    }
    
}