extern alias SEZero;

using SEZero::SlotExtenderZero.API;
using UnityEngine;

namespace SeaTruckFlyModule
{
    public partial class FlyManager //Physics
    {
        private Rigidbody rigidbody;                

        private void FixedUpdate()
        {
            if (!isEnabled)
            {
                return;
            }

            if (helper == null || motor == null || !helper.IsPiloted())
                return;            

            if (telemetry.altitude >= 0 && rigidbody != null && helper.IsPowered() && !IsBusyAnimating())
            {
                Vector3 vector = (AvatarInputHandler.main.IsEnabled() || helper.TruckInputStackDummy.activeInHierarchy) ? GameInput.GetMoveDirection() : Vector3.zero;

                Int2 direction;

                if (vector.x > 0f)
                {                    
                    direction.x = 1;
                }
                else if (vector.x < 0f)
                {                    
                    direction.x = -1;
                }
                else
                {
                    direction.x = 0;
                }

                if (vector.z > 0f)
                {                    
                    direction.y = 1;
                }
                else if (vector.z < 0f)
                {                    
                    direction.y = -1;
                }
                else
                {
                    direction.y = 0;
                }

                if (telemetry.SeatruckPosition == TruckPosition.OnSurface && telemetry.SeatruckState == TruckState.Landed && vector.y > 0f)
                {                    
                    StartCoroutine(OnTakeOff());
                    //rigidbody.AddForce(new Vector3(0, 1 ,0), ForceMode.VelocityChange);                    
                }                

                helper.TruckLeverDirection = direction;

                vector = vector.normalized;

                if (motor.afterBurnerActive)
                {
                    vector = Vector3.forward;
                }

                Vector3 a = MainCameraControl.main.rotation * vector;

                float num = 1 / Mathf.Max(1f, helper.GetWeight() * 0.5f) * motor.acceleration;

                if (motor.afterBurnerActive)
                {
                    num += 7f;
                }
                
                if (telemetry.SeatruckState != TruckState.Landing)
                {
                    rigidbody.AddForce(num * a, ForceMode.Acceleration);
                }                

                if (motor.relay && vector != Vector3.zero)
                {
                    motor.relay.ConsumeEnergy(Time.deltaTime * motor.powerEfficiencyFactor * 0.12f, out float num2);
                }

                if (motor.animator)
                {
                    helper.TruckAnimAccel = Mathf.Lerp(helper.TruckAnimAccel, direction.y, Time.deltaTime * 3f);

                    motor.animator.SetFloat("move_speed_z", helper.TruckAnimAccel);
                }

                StabilizeRoll();

                /*
                if(SeatruckState == TruckState.Landing)
                {
                    StabilizePitch();
                }
                */
            }
        }

        public void StabilizeRoll()
        {
            float num = GetTruckRoll();

            if (num <= 180f)
            {
                float d = Mathf.Clamp01(1f - num / 180f) * 8f;

                rigidbody.AddTorque(mainCab.transform.forward * d * Time.deltaTime * Mathf.Sign(mainCab.transform.eulerAngles.z - 180f), ForceMode.VelocityChange);
            }
        }        

        private void StabilizePitch()
        {
            float num = GetTruckPitch();

            if (180f - num > 0f)
            {
                float d = Mathf.Clamp01(1f - num / 180f) * 8f;

                rigidbody.AddTorque(mainCab.transform.right * d * Time.deltaTime * Mathf.Sign(mainCab.transform.eulerAngles.x - 180f), ForceMode.VelocityChange);
            }
        }   

        public bool IsBusyAnimating()
        {
            return motor.waitForAnimation && motor.seatruckanimation != null && motor.seatruckanimation.currentAnimation > SeaTruckAnimation.Animation.Idle;
        }

        private bool IsFootsContactWithTerrain()
        {
            int footContacts = 0;

            foreach (Collider collider in footCollisions)
            {
                if (collider.gameObject.activeSelf)
                {
                    if (Physics.Raycast(collider.transform.position, Vector3.down, out RaycastHit raycastHit, 0.3f, -1, QueryTriggerInteraction.Ignore))
                    {
                        footContacts++;
                    }
                }
            }

            return rearFoots.activeSelf ? footContacts == 5 : footContacts == 3;
        }

        private float GetTruckPitch()
        {
            return Mathf.Abs(mainCab.transform.eulerAngles.x - 180f);
        }

        private float GetTruckRoll()
        {
            return Mathf.Abs(mainCab.transform.eulerAngles.z - 180f);
        }

        private bool IsSafePitch()
        {
            return 180f - GetTruckPitch() <= 12f;
        }

        private bool IsSafeRoll()
        {
            return 180f - GetTruckRoll() <= 12f;
        }
    }
}
