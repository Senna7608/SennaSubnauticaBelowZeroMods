extern alias SEZero;

using UnityEngine;
using SEZero::SlotExtenderZero.API;

namespace SeaTruckArms.API.ArmHandlers
{
    public abstract class GrapplingArmHandler : MonoBehaviour
    {
        public SeaTruckHelper TruckHelper;

        public Animator animator { get; private set; }
        public VFXGrapplingRope rope { get; private set; }
        public SeaTruckGrapplingHook hook { get; private set; }
        public FMOD_CustomLoopingEmitter grapplingLoopSound { get; private set; }
        public FMODAsset shootSound { get; private set; }
        public Transform front { get; private set; }
        public WorldForces worldForces { get; private set; }
        public GameObject hookPrefab { get; private set; }

        public DamageType damageType { get; set; } = DamageType.Collide;

        public float energyCost = 0.5f;
        public float maxDistance = 50f;
        public float damage = 5f;
        public float seamothGrapplingAccel = 25f;
        public float targetGrapplingAccel = 400f;
        public Vector3 grapplingStartPos = Vector3.zero;

        public virtual void Awake()
        {
            TruckHelper = GetComponentInParent<SeaTruckArmManager>().helper;

            animator = GetComponent<Animator>();

            front = ArmServices.main.objectHelper.FindDeepChild(gameObject, "hook").transform;
            ExosuitGrapplingArm component = GetComponent<ExosuitGrapplingArm>();
            hookPrefab = Instantiate(component.hookPrefab);
            hookPrefab.GetComponent<Collider>().enabled = false;
            rope = ArmServices.main.objectHelper.GetObjectClone(component.rope);

            DestroyImmediate(hookPrefab.GetComponent<GrapplingHook>());
            DestroyImmediate(component);

            hook = hookPrefab.AddComponent<SeaTruckGrapplingHook>();
            hook.transform.parent = front;
            hook.transform.localPosition = Vector3.zero;
            hook.transform.localRotation = Quaternion.identity;
            hook.transform.localScale = new Vector3(1f, 1f, 1f);

            rope.attachPoint = hook.transform;

            grapplingLoopSound = GetComponent<FMOD_CustomLoopingEmitter>();

            shootSound = ScriptableObject.CreateInstance<FMODAsset>();
            shootSound.path = "event:/sub/exo/hook_shoot";
            shootSound.name = "hook_shoot";
        }

        public virtual void Start()
        {
            worldForces = TruckHelper.TruckSegment.worldForces;
        }
    }
}
