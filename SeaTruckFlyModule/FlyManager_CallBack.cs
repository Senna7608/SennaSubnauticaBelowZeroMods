﻿extern alias SEZero;
using BZCommon;
using SEZero::SlotExtenderZero.API;
using System.Collections;
using UnityEngine;

namespace SeaTruckFlyModule
{
    public partial class FlyManager //CallBack
    {
        private IEnumerator SeatruckCallBack()
        {
            //isEnabled = true;
            //enabled = true;
            StartCoroutine(OnTakeOff());
            //yield return StartCoroutine(OnTakeOff());

            /*
            while (SeatruckState != TruckState.Flying)
            {
                yield return null;
            }
            */

            while (telemetry.SeatruckState == TruckState.TakeOff)
            {
                yield return null;
            }

            telemetry.ForceStateChange(TruckState.AutoFly);
            
            Vector3 playerDirection = Player.main.transform.position - mainCab.transform.position;            
            
            BZLogger.Debug($"[SeatruckCallBack] playerDirection.normalized: {playerDirection.normalized}");
            
            BZLogger.Debug("[SeatruckCallBack] Rotating Seatruck towards to player direction");

            bool rotateComplete = false;

            while (!rotateComplete)
            {
                mainCab.transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(mainCab.transform.forward, playerDirection, Time.deltaTime, 0.0f));

                rotateComplete = mainCab.transform.forward == playerDirection.normalized;

                yield return null;
            }

            BZLogger.Debug("[SeatruckCallBack] Rotating finished");            

            while (Vector3.Distance(mainCab.transform.position, Player.main.transform.position) > 15f)
            {
                mainCab.transform.Translate(Vector3.forward * 8 * Time.deltaTime, Space.Self);

                if (Physics.Raycast(telemetry.altitudeMeter.transform.position, telemetry.altitudeMeter.transform.TransformDirection(Vector3.forward), out RaycastHit raycastForward, 30f, -1, QueryTriggerInteraction.Ignore))
                {
                    GameObject gameObject = raycastForward.collider.gameObject;

                    if (gameObject != null && gameObject.GetComponent<LiveMixin>() == null)
                    {
                        rigidbody.AddRelativeForce(jumpUp, ForceMode.VelocityChange);

                        mainCab.transform.LookAt(Player.main.transform.position);
                    }
                }                

                yield return null;
            }

            /*
            while (distanceFromSurface > 15)
            {
                mainCab.transform.Translate(Vector3.down * 10 * Time.deltaTime, Space.Self);

                yield return null;
            }
            */

            StartCoroutine(OnLanding(new Vector3(0, telemetry.distanceFromSurface * -1, 0)));

            yield break;
        }

    }
}
