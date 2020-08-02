using UnityEngine;

namespace SeaTruckFlyModule
{
    public partial class SeaTruckFlyManager //Physics
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

            if (altitude >= 0 && rigidbody != null && helper.IsPowered() && !IsBusyAnimating())
            {
                Vector3 vector = (AvatarInputHandler.main.IsEnabled() || helper.thisInputStackDummy.activeInHierarchy) ? GameInput.GetMoveDirection() : Vector3.zero;

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

                if (seatruckPosition == SeatruckPosition.OnSurface && vector.y > 0f)
                {
                    rigidbody.isKinematic = false;
                    rigidbody.AddForce(new Vector3(0,1,0), ForceMode.VelocityChange);                                     
                }                

                helper.thisLeverDirection = direction;

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
                
                if (flyState != SeatruckFlyState.Landing)
                {
                    rigidbody.AddForce(num * a, ForceMode.Acceleration);
                }                

                if (motor.relay && vector != Vector3.zero)
                {
                    motor.relay.ConsumeEnergy(Time.deltaTime * motor.powerEfficiencyFactor * 0.12f, out float num2);
                }

                if (motor.animator)
                {
                    helper.thisAnimAccel = Mathf.Lerp(helper.thisAnimAccel, direction.y, Time.deltaTime * 3f);

                    motor.animator.SetFloat("move_speed_z", helper.thisAnimAccel);
                }

                StabilizeRoll();

                if(flyState == SeatruckFlyState.Landing)
                {
                    StabilizePitch();
                }
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

            foreach(Collider collider in footCollisions)
            {
                if (collider.gameObject.activeSelf)
                {
                    if (Physics.Raycast(collider.transform.position, Vector3.down, out RaycastHit raycastHit, 0.3f, -1, QueryTriggerInteraction.Ignore))
                    {
                        footContacts++;
                    }
                }
            }            

            if (rearFoots.activeSelf)
            {
                return footContacts == 5 ? true : false;
            }
            else
            {
                return footContacts == 3 ? true : false;
            }            
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
            float num = GetTruckPitch();

            float diff = 180f - num;

            if (diff > 12f)
            {                
                return false;
            }

            return true;
        }


        private bool IsSafeRoll()
        {
            float num = GetTruckRoll();

            float diff = 180f - num;

            if (diff > 12f)
            {                
                return false;
            }

            return true;
        }
    }
}
