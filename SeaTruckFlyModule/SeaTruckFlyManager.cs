using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWE;
using BZCommon;
using BZCommon.Helpers;

namespace SeaTruckFlyModule
{
    public partial class SeaTruckFlyManager : MonoBehaviour
    {
        enum SeatruckFlyState
        {
            Diving,
            Flying,
            Landing,
            Landed
        };

        enum SeatruckPosition
        {
            BelowWater,
            AboveWater,
            AboveSurface,
            NearSurface,
            OnSurface
        };

        public SeaTruckHelper helper = null;

        private ObjectHelper objectHelper = new ObjectHelper();

        public SeaTruckMotor motor;

        public bool isEnabled = false;

        private bool isFirstCheckComplete = false;        

        public Utils.MonitoredValue<bool> isFlying = new Utils.MonitoredValue<bool>();        

        private FMOD_CustomLoopingEmitter engineSound;

        private FMODAsset engine;

        private FMODAsset engineDefault;              

        float distanceFromSurface;

        public float altitude;

        private SeatruckFlyState flyState;

        private SeatruckPosition seatruckPosition;

        private GameObject mainCab;

        private Vector3 mainCabExitPoint;

        private readonly Vector3 mainCabNewExitPoint1 = new Vector3(0.0f, -0.80f, -1.0f);

        private readonly Vector3 mainCabNewExitPoint2 = new Vector3(1.78f, -0.80f, 2.0f);

        float timer = 0.0f;

        Vector3 landingVelocity = new Vector3(0, -1, 0);

        Vector3 jump = new Vector3(0, 20, 0);

        public void Awake()
        {
            helper = new SeaTruckHelper(gameObject, false, false, false);
            mainCab = helper.MainCab;
            motor = helper.thisMotor;
            engineSound = motor.engineSound;
            rigidbody = helper.thisWorldForces.useRigidbody;

            Init_Graphics();

            engineDefault = ScriptableObject.CreateInstance<FMODAsset>();
            engineDefault.name = "engine";
            engineDefault.path = "event:/bz/vehicles/seatruck/engine";

            engine = ScriptableObject.CreateInstance<FMODAsset>();
            engine.name = "engine";
            engine.path = "event:/sub/drone/motor_loop";                                               

            isFlying.changedEvent.AddHandler(this, new Event<Utils.MonitoredValue<bool>>.HandleFunction(OnFlyModeChanged));

            mainCabExitPoint = helper.thisSegment.exitPosition.localPosition;
        }

        private void OnFlyModeChanged(Utils.MonitoredValue<bool> newValue)
        {
            if (newValue.value)
            {                
                detachLever.SetActive(false);
                SetWheelTriggers(false);

                engineSound.Stop();
                engineSound.asset = engine;

                helper.thisWorldForces.aboveWaterGravity = 0f;                
                rigidbody.drag = 1f;

                ErrorMessage.AddDebug("Seatruck has take off");

                if (!landingFoots.activeSelf)
                {
                    landingFoots.SetActive(true);
                    landingFootCollisions.SetActive(true);
                    SetRearFoots();

                    ErrorMessage.AddDebug("Landing foots extended");
                }
            }
            else
            {
                engineSound.Stop();
                engineSound.asset = engineDefault;

                if (seatruckPosition == SeatruckPosition.NearSurface && flyState == SeatruckFlyState.Landing)
                {
                    ErrorMessage.AddDebug("Landing sequence started");
                    engineSound.Stop();
                   
                    helper.thisWorldForces.aboveWaterGravity = 9.81f;
                    timer = 0.0f;
                    StartCoroutine(OnLanding(rigidbody.velocity));                    
                }
                else
                {                    
                    landingFoots.SetActive(false);
                    landingFootCollisions.SetActive(false);
                    detachLever.SetActive(true);
                    SetWheelTriggers(true);
                    helper.thisSegment.exitPosition.localPosition = mainCabExitPoint;
                    ErrorMessage.AddDebug("Landing foots retracted");
                }
            }
        }        

        IEnumerator OnLanding(Vector3 velocity)
        {
            while (!IsFootsContactWithTerrain())
            {
                timer += Mathf.Max(Mathf.Abs(rigidbody.velocity.y) * 0.009f, 0.008f);
                                
                if (timer > 3.0f)
                {
                    ErrorMessage.AddDebug("Landing sequence failed");
                    OnLandingFailed();
                    yield break;
                }
                else
                {
                    rigidbody.velocity = Vector3.Lerp(velocity, landingVelocity, timer);
                }                

                yield return null;
            }

            if (!IsSafePitch())
            {
                ErrorMessage.AddDebug("Pitch angle unsafe!");
                OnLandingFailed();
                yield break;
            }

            if (!IsSafeRoll())
            {
                ErrorMessage.AddDebug("Roll angle unsafe!");
                OnLandingFailed();
                yield break;
            }                       
           
            seatruckPosition = SeatruckPosition.OnSurface;
            flyState = SeatruckFlyState.Landed;

            rigidbody.velocity = Vector3.zero;

            yield return new WaitForSeconds(1);

            rigidbody.isKinematic = true;

            ErrorMessage.AddDebug("Seatruck has landed");

            if (!helper.IsSeatruckChained())
            {
                helper.thisSegment.exitPosition.localPosition = mainCabNewExitPoint1;
            }
            else
            {
                helper.thisSegment.exitPosition.localPosition = mainCabNewExitPoint2;
            }

            yield break;            
        }
        
        public void OnLandingFailed()
        {            
            rigidbody.AddForce(jump, ForceMode.VelocityChange);
        }


        public void Update()
        {
            if (!isFirstCheckComplete && helper != null)
            {
                CheckSlotsForFlyModule();
                isFirstCheckComplete = true;
            }
        }

        public void CheckSlotsForFlyModule()
        {
            foreach (string slot in helper.slotIDs)
            {
                TechType techType = helper.modules.GetTechTypeInSlot(slot);

                if (techType == SeaTruckFlyModule.TechTypeID)
                {
                    isEnabled = true;
                    
                    break;
                }
                else
                {                                        
                    isEnabled = false;
                    helper.thisWorldForces.aboveWaterGravity = 9.81f;
                }
            }

            ErrorMessage.AddDebug($"Seatruck fly mode {(isEnabled ? "enabled" : "disabled")}");
        }
                

        void LateUpdate()
        {
            if (!isEnabled)
            {
                return;
            }

            if (helper == null || motor == null || !helper.IsPiloted())
                return;
                        
            altitude = helper.MainCab.transform.position.y;            

            if (Physics.Raycast(altitudeMeter.transform.position, Vector3.down, out RaycastHit raycastHit, 1000f, -1, QueryTriggerInteraction.Ignore))
            {
                GameObject gameObject = raycastHit.collider.gameObject;

                if (gameObject != null && gameObject.GetComponent<LiveMixin>() == null)
                {
                    distanceFromSurface = (altitude - raycastHit.point.y) - 3;                    
                }
                else
                {
                    distanceFromSurface = altitude;
                }
            }
            
            SeatruckPosition prevPosition = seatruckPosition;

            if (altitude < 0)
            {
                seatruckPosition = SeatruckPosition.BelowWater;
                flyState = SeatruckFlyState.Diving;
            }
            else if (altitude > 0 && altitude <= distanceFromSurface)
            {                
                seatruckPosition = SeatruckPosition.AboveWater;
                flyState = SeatruckFlyState.Flying;
            }
            else if (altitude > distanceFromSurface && distanceFromSurface > 4.0f)
            {
                seatruckPosition = SeatruckPosition.AboveSurface;
                flyState = SeatruckFlyState.Flying;
            }            
            else if (altitude > 0 && distanceFromSurface < 4.0f && distanceFromSurface > 2.5f)
            {
                seatruckPosition = SeatruckPosition.NearSurface;
                flyState = SeatruckFlyState.Landing;
            }
            

            switch (seatruckPosition)
            {
                case SeatruckPosition.AboveWater:
                case SeatruckPosition.AboveSurface:
                    if (!isFlying.value)
                    {
                        isFlying.Update(true);
                    }
                    break;

                case SeatruckPosition.BelowWater:
                    if (isFlying.value)
                    {
                        isFlying.Update(false);
                    }
                    break;

                case SeatruckPosition.NearSurface:
                    if (isFlying.value)
                    {
                        isFlying.Update(false);
                    }
                    break;                    
            }                      

            if (altitude > 0)
            {
                hudTextAltitude.text = $"{(int)altitude}";                
            }
        }
                
        private void SetRearFoots()
        {
            if (helper.IsSeatruckChained())
            {
                float zShift = helper.GetSeatruckZShift();

                hatchTriggerCloneRear.SetActive(false);

                hatchTriggerCloneFront.SetActive(true);

                rearFoots.transform.localPosition = new Vector3(0, 0, zShift);

                rearFoots.SetActive(true);
                
                rearFootCollisions.transform.localPosition = new Vector3(0, 0, zShift);

                rearFootCollisions.SetActive(true);
            }
            else
            {
                rearFoots.SetActive(false);

                rearFootCollisions.SetActive(false);

                hatchTriggerCloneRear.SetActive(true);

                hatchTriggerCloneFront.SetActive(false);
            }
        }


        private void SetWheelTriggers(bool value)
        {
            if (!helper.IsSeatruckChained())
            {
                return;
            }

            List<GameObject> wheelTriggers = helper.GetWheelTriggers();
            
            foreach (GameObject wheelTrigger in wheelTriggers)
            {
                wheelTrigger.SetActive(value);
            }

            ErrorMessage.AddDebug($"Module wheel triggers {(value ? "enabled" : "disabled")}");
        }
    }
}
