using BZCommon;
using UnityEngine;

namespace SeaTruckArms.ArmControls
{
    public class SeaTruckGrapplingArmControl : MonoBehaviour, ISeaTruckArm
    {
        private SeaTruckArmManager manager;
        private SeaTruckHelper helper;

        public WorldForces worldForces;
        private const float energyCost = 0.5f;
        public Animator animator;

        public Transform front;
        public GameObject hookPrefab;

        public VFXGrapplingRope rope;
        public FMOD_CustomLoopingEmitter grapplingLoopSound;
        public FMODAsset shootSound;

        private SeaTruckGrapplingHook hook;

        private const float maxDistance = 50f;
        private const float damage = 5f;
        private const DamageType damageType = DamageType.Collide;
        private const float seamothGrapplingAccel = 25f;
        private const float targetGrapplingAccel = 400f;
        private Vector3 grapplingStartPos = Vector3.zero;

        private void Awake()
        {
            manager = GetComponentInParent<SeaTruckArmManager>();
            helper = manager.helper;

            animator = GetComponent<Animator>();

            front = manager.objectHelper.FindDeepChild(gameObject, "hook").transform;
            ExosuitGrapplingArm component = GetComponent<ExosuitGrapplingArm>();
            hookPrefab = Instantiate(component.hookPrefab);
            hookPrefab.GetComponent<Collider>().enabled = false;
            rope = manager.objectHelper.GetObjectClone(component.rope);

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

        private void Start()
        {
            worldForces = helper.thisSegment.worldForces;
        }

        GameObject ISeaTruckArm.GetGameObject()
        {
            return gameObject;
        }

        void ISeaTruckArm.SetSide(Arm arm)
        {
            if (arm == Arm.Right)
            {
                transform.localScale = new Vector3(-0.80f, 0.80f, 0.80f);
            }
            else
            {
                transform.localScale = new Vector3(0.80f, 0.80f, 0.80f);
            }
        }

        bool ISeaTruckArm.OnUseDown(out float cooldownDuration)
        {
            animator.SetBool("use_tool", true);

            if (!rope.isLaunching)
            {
                rope.LaunchHook(maxDistance);
            }

            cooldownDuration = 2f;

            return true;
        }

        bool ISeaTruckArm.OnUseHeld(out float cooldownDuration)
        {
            cooldownDuration = 0f;
            return false;
        }

        bool ISeaTruckArm.OnUseUp(out float cooldownDuration)
        {
            animator.SetBool("use_tool", false);
            ResetHook();
            cooldownDuration = 0f;
            return true;
        }

        bool ISeaTruckArm.OnAltDown()
        {
            return false;
        }

        void ISeaTruckArm.Update(ref Quaternion aimDirection)
        {
        }

        void ISeaTruckArm.Reset()
        {
            animator.SetBool("use_tool", false);
            ResetHook();
        }

        private void OnDestroy()
        {
            if (hook)
            {
                Destroy(hook.gameObject);
            }
            if (rope != null)
            {
                Destroy(rope.gameObject);
            }

        }

        private void ResetHook()
        {
            rope.Release();
            hook.Release();
            hook.SetFlying(false);
            hook.transform.parent = front;
            hook.transform.localScale = new Vector3(1f, 1f, 1f);
        }

        public void OnHit()
        {
            hook.transform.parent = null;
            hook.transform.position = front.transform.position;
            hook.SetFlying(true);
            GameObject x = null;
            Vector3 a = default(Vector3);

            UWE.Utils.TraceFPSTargetPosition(helper.MainCab, 100f, ref x, ref a, out Vector3 normal, false);

            if (x == null || x == hook.gameObject)
            {
                a = MainCamera.camera.transform.position + MainCamera.camera.transform.forward * 25f;
            }

            Vector3 a2 = Vector3.Normalize(a - hook.transform.position);

            hook.rb.velocity = a2 * 25f;
            Utils.PlayFMODAsset(shootSound, front, 15f);

            grapplingStartPos = helper.MainCab.transform.position;
        }

        public void FixedUpdate()
        {
            if (hook.attached)
            {
                grapplingLoopSound.Play();

                Vector3 value = hook.transform.position - front.position;
                Vector3 a = Vector3.Normalize(value);
                float magnitude = value.magnitude;

                if (magnitude > 1f)
                {
                    if (!IsUnderwater() && helper.MainCab.transform.position.y + 0.2f >= grapplingStartPos.y)
                    {
                        a.y = Mathf.Min(a.y, 0f);
                    }

                    helper.MainCab.GetComponent<Rigidbody>().AddForce(a * seamothGrapplingAccel, ForceMode.Acceleration);
                    hook.GetComponent<Rigidbody>().AddForce(-a * 400f, ForceMode.Force);
                }

                rope.SetIsHooked();
            }
            else if (hook.flying)
            {
                if ((hook.transform.position - front.position).magnitude > maxDistance)
                {
                    ResetHook();
                }
                grapplingLoopSound.Play();
            }
            else
            {
                grapplingLoopSound.Stop();
            }
        }

        bool ISeaTruckArm.HasClaw()
        {
            return false;
        }

        bool ISeaTruckArm.HasDrill()
        {
            return false;
        }

        void ISeaTruckArm.SetRotation(Arm arm, bool isDocked)
        {
            if (isDocked)
            {
                BaseRoot baseRoot = helper.MainCab.GetComponentInParent<BaseRoot>();

                if (baseRoot.isBase)
                {
                    if (arm == Arm.Right)
                    {
                        transform.localRotation = Quaternion.Euler(32, 6, 9);
                    }
                    else
                    {
                        transform.localRotation = Quaternion.Euler(32, 354, 351);
                    }
                }
            }
            else
            {
                if (arm == Arm.Right)
                {
                    transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
                else
                {
                    transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
            }
        }

        public float GetEnergyCost()
        {
            return energyCost;
        }

        private bool IsUnderwater()
        {
            return helper.MainCab.transform.position.y < worldForces.waterDepth + 2f;
        }

        public bool GetIsGrappling()
        {
            return hook != null && hook.attached;
        }
    }
}
