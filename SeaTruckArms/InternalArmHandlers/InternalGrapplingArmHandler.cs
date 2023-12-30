using UnityEngine;
using ModdedArmsHelperBZ.API;
using ModdedArmsHelperBZ.API.Interfaces;
using ModdedArmsHelperBZ.API.ArmHandlers;

namespace SeaTruckArms.InternalArmHandlers
{
    internal class InternalGrapplingArmHandler : SeatruckGrapplingArmHandler, ISeatruckArm
    {
        public override void Start()
        {
        }

        GameObject ISeatruckArm.GetGameObject()
        {
            return gameObject;
        }

        GameObject ISeatruckArm.GetInteractableRoot(GameObject target)
        {
            return null;
        }

        void ISeatruckArm.SetSide(SeatruckArm arm)
        {
            if (arm == SeatruckArm.Right)
            {
                transform.localScale = new Vector3(-0.80f, 0.80f, 0.80f);
            }
            else
            {
                transform.localScale = new Vector3(0.80f, 0.80f, 0.80f);
            }
        }

        bool ISeatruckArm.OnUseDown(out float cooldownDuration)
        {
            animator.SetBool("use_tool", true);

            if (!rope.isLaunching)
            {
                rope.LaunchHook(maxDistance);
            }

            cooldownDuration = 2f;

            return true;
        }

        bool ISeatruckArm.OnUseHeld(out float cooldownDuration)
        {
            cooldownDuration = 0f;
            return false;
        }

        bool ISeatruckArm.OnUseUp(out float cooldownDuration)
        {
            animator.SetBool("use_tool", false);
            ResetHook();
            cooldownDuration = 0f;
            return true;
        }

        bool ISeatruckArm.OnAltDown()
        {
            return false;
        }

        void ISeatruckArm.Update(ref Quaternion aimDirection)
        {
        }

        void ISeatruckArm.ResetArm()
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

            UWE.Utils.TraceFPSTargetPosition(seatruck.mainCab, 100f, ref x, ref a, out Vector3 normal, false);

            if (x == null || x == hook.gameObject)
            {
                a = MainCamera.camera.transform.position + MainCamera.camera.transform.forward * 25f;
            }

            Vector3 a2 = Vector3.Normalize(a - hook.transform.position);

            hook.rb.velocity = a2 * 25f;
            Utils.PlayFMODAsset(shootSound, front, 15f);

            grapplingStartPos = seatruck.mainCab.transform.position;
        }

        public void FixedUpdate()
        {
            if (hook.attached)
            {
                grapplingLoopSound.Play();

                Vector3 value = hook.transform.position - front.position;
                Vector3 vector = Vector3.Normalize(value);
                //float magnitude = value.magnitude;

                if (value.magnitude > 1f)
                {
                    if (!IsUnderwater() && seatruck.mainCab.transform.position.y + 0.2f >= grapplingStartPos.y)
                    {
                        vector.y = Mathf.Min(vector.y, 0f);
                    }

                    seatruck.mainCab.GetComponent<Rigidbody>().AddForce(vector * seatruckGrapplingAccel, ForceMode.Acceleration);
                    hook.GetComponent<Rigidbody>().AddForce(-vector * 400f, ForceMode.Force);
                }

                rope.SetIsHooked();

                return;
            }

            if (hook.flying)
            {
                if ((hook.transform.position - front.position).magnitude > maxDistance)
                {
                    ResetHook();
                }
                grapplingLoopSound.Play();

                return;
            }
            
            grapplingLoopSound.Stop();
            
        }

        bool ISeatruckArm.HasClaw()
        {
            return false;
        }

        bool ISeatruckArm.HasDrill()
        {
            return false;
        }

        bool ISeatruckArm.HasPropCannon()
        {
            return false;
        }

        void ISeatruckArm.SetRotation(SeatruckArm arm, bool isDocked)
        {
            if (isDocked && !seatruck.seatruckHelper.TruckDockable.isInTransition)
            {
                BaseRoot baseRoot = seatruck.mainCab.GetComponentInParent<BaseRoot>();

                if (baseRoot.isBase)
                {
                    if (arm == SeatruckArm.Right)
                    {
                        transform.localRotation = Quaternion.Euler(20, 6, 0);
                    }
                    else
                    {
                        transform.localRotation = Quaternion.Euler(20, 354, 0);
                    }
                }
            }
            else
            {
                if (arm == SeatruckArm.Right)
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
            return seatruck.mainCab.transform.position.y < seatruck.worldForces.waterDepth + 2f;
        }

        public bool GetIsGrappling()
        {
            return hook != null && hook.attached;
        }

        bool ISeatruckArm.GetCustomUseText(out string customText)
        {
            customText = string.Empty;
            return false;
        }
    }
}
