using UnityEngine;
using SeaTruckArms.API;
using SeaTruckArms.API.ArmHandlers;
using SeaTruckArms.API.Interfaces;

namespace SeaTruckArms.InternalArmHandlers
{
    internal class InternalGrapplingArmHandler : GrapplingArmHandler, ISeaTruckArmHandler
    {
        GameObject ISeaTruckArmHandler.GetGameObject()
        {
            return gameObject;
        }

        void ISeaTruckArmHandler.SetSide(SeaTruckArm arm)
        {
            if (arm == SeaTruckArm.Right)
            {
                transform.localScale = new Vector3(-0.80f, 0.80f, 0.80f);
            }
            else
            {
                transform.localScale = new Vector3(0.80f, 0.80f, 0.80f);
            }
        }

        bool ISeaTruckArmHandler.OnUseDown(out float cooldownDuration)
        {
            animator.SetBool("use_tool", true);

            if (!rope.isLaunching)
            {
                rope.LaunchHook(maxDistance);
            }

            cooldownDuration = 2f;

            return true;
        }

        bool ISeaTruckArmHandler.OnUseHeld(out float cooldownDuration)
        {
            cooldownDuration = 0f;
            return false;
        }

        bool ISeaTruckArmHandler.OnUseUp(out float cooldownDuration)
        {
            animator.SetBool("use_tool", false);
            ResetHook();
            cooldownDuration = 0f;
            return true;
        }

        bool ISeaTruckArmHandler.OnAltDown()
        {
            return false;
        }

        void ISeaTruckArmHandler.Update(ref Quaternion aimDirection)
        {
        }

        void ISeaTruckArmHandler.Reset()
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

            UWE.Utils.TraceFPSTargetPosition(TruckHelper.MainCab, 100f, ref x, ref a, out Vector3 normal, false);

            if (x == null || x == hook.gameObject)
            {
                a = MainCamera.camera.transform.position + MainCamera.camera.transform.forward * 25f;
            }

            Vector3 a2 = Vector3.Normalize(a - hook.transform.position);

            hook.rb.velocity = a2 * 25f;
            Utils.PlayFMODAsset(shootSound, front, 15f);

            grapplingStartPos = TruckHelper.MainCab.transform.position;
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
                    if (!IsUnderwater() && TruckHelper.MainCab.transform.position.y + 0.2f >= grapplingStartPos.y)
                    {
                        vector.y = Mathf.Min(vector.y, 0f);
                    }

                    TruckHelper.MainCab.GetComponent<Rigidbody>().AddForce(vector * seamothGrapplingAccel, ForceMode.Acceleration);
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

        bool ISeaTruckArmHandler.HasClaw()
        {
            return false;
        }

        bool ISeaTruckArmHandler.HasDrill()
        {
            return false;
        }

        void ISeaTruckArmHandler.SetRotation(SeaTruckArm arm, bool isDocked)
        {
            if (isDocked)
            {
                BaseRoot baseRoot = TruckHelper.MainCab.GetComponentInParent<BaseRoot>();

                if (baseRoot.isBase)
                {
                    if (arm == SeaTruckArm.Right)
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
                if (arm == SeaTruckArm.Right)
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
            return TruckHelper.MainCab.transform.position.y < worldForces.waterDepth + 2f;
        }

        public bool GetIsGrappling()
        {
            return hook != null && hook.attached;
        }
    }
}
