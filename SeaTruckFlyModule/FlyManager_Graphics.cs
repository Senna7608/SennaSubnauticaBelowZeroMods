using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering;

namespace SeaTruckFlyModule
{
    public partial class FlyManager //Graphics
    {
        GameObject HUD;
        //uGUI.main -> ScreenCanvas -> HUD -> Content -> DepthCompass -> SubmersibleDepth -> DepthTextArea -> DepthValue.TextMeshProUGUI.text
        //uGUI.main -> ScreenCanvas -> HUD -> Content -> Seatruck -> Indicators
        
        private GameObject altitudeMeter;
        private GameObject landingFoots;
        private GameObject rearFoots;
        private GameObject landingFootCollisions;
        private GameObject frontFootCollisions;
        private GameObject rearFootCollisions;
        
        private GameObject hatchTriggerCloneRear;
        private GameObject hatchTriggerCloneFront;
        private GameObject detachLever;
        private List<Collider> footCollisions = new List<Collider>();
       
        TextMeshProUGUI hudTextAltitude;       

        private void Init_Graphics()
        {
            HUD = helper.TruckHUD.root;

            InitDebugHUD();

            GameObject DepthCompass = HUD.transform.parent.gameObject.FindChild("DepthCompass");
            GameObject SubmersibleDepth = DepthCompass.FindChild("SubmersibleDepth");
            GameObject DepthTextArea = SubmersibleDepth.FindChild("DepthTextArea");
            GameObject DepthValue = DepthTextArea.FindChild("DepthValue");
            
            hudTextAltitude = DepthValue.GetComponent<TextMeshProUGUI>();
            
            altitudeMeter = objectHelper.CreateGameObject("altitudeMeter", transform);
            Utils.ZeroTransform(altitudeMeter.transform);
            altitudeMeter.transform.localPosition = new Vector3(0f, -3f, 0.94f);

            /*
            Shader shader = Shader.Find("Hidden/Internal-Colored");

            Material lineMaterial = new Material(shader)
            {
                hideFlags = HideFlags.HideAndDontSave
            };

            lineMaterial.SetInt(ShaderPropertyID._SrcBlend, 5);
            lineMaterial.SetInt(ShaderPropertyID._DstBlend, 10);

            LineRenderer lineRenderer = altitudeMeter.EnsureComponent<LineRenderer>();
            lineRenderer.material = lineMaterial;
            lineRenderer.useWorldSpace = true;
            lineRenderer.startWidth = 0.008f;
            lineRenderer.endWidth = 0.008f;
            lineRenderer.startColor = Color.green;
            lineRenderer.endColor = Color.red;
            lineRenderer.receiveShadows = false;
            lineRenderer.loop = false;
            lineRenderer.textureMode = LineTextureMode.Stretch;
            lineRenderer.alignment = LineAlignment.View;
            lineRenderer.shadowCastingMode = ShadowCastingMode.Off;
            lineRenderer.positionCount = 2;            
            */

            landingFoots = objectHelper.CreateGameObject("landingFoots", transform);

            GameObject frontFoots = objectHelper.CreateGameObject("frontFoots", landingFoots.transform);            

            var resource = Resources.Load<GameObject>("worldentities/alterra/base/biodome_robot_arm");

            GameObject biodome_robot_arm = Instantiate(resource.FindChild("biodome_Robot_Arm"), null);

            objectHelper.GetPrefabClone(ref biodome_robot_arm, frontFoots.transform, true, "frontFootLeft", out GameObject frontFootLeft);

            frontFootLeft.transform.localPosition = new Vector3(-1.79f, -3.23f, 0.75f);
            frontFootLeft.transform.localScale = new Vector3(2f, 0.60f, 1f);
            frontFootLeft.transform.localRotation = Quaternion.Euler(0, 90, 180);

            objectHelper.GetPrefabClone(ref biodome_robot_arm, frontFoots.transform, true, "frontFootRight", out GameObject frontFootRight);

            frontFootRight.transform.localPosition = new Vector3(1.79f, -3.23f, 0.75f);
            frontFootRight.transform.localScale = new Vector3(2f, 0.60f, 1f);
            frontFootRight.transform.localRotation = Quaternion.Euler(0, 270, 180);

            objectHelper.GetPrefabClone(ref biodome_robot_arm, frontFoots.transform, true, "frontFootCenter", out GameObject frontFootCenter);

            frontFootCenter.transform.localPosition = new Vector3(0f, -3.23f, 2.84f);
            frontFootCenter.transform.localScale = new Vector3(2f, 0.60f, 1f);
            frontFootCenter.transform.localRotation = Quaternion.Euler(0, 180, 180);

            rearFoots = objectHelper.CreateGameObject("rearFoots", landingFoots.transform);

            rearFoots.SetActive(false);

            objectHelper.GetPrefabClone(ref biodome_robot_arm, rearFoots.transform, true, "rearFootLeft", out GameObject rearFootLeft);

            rearFootLeft.transform.localPosition = new Vector3(-2.07f, -3.31f, 0.94f);
            rearFootLeft.transform.localScale = new Vector3(2f, 0.60f, 1f);
            rearFootLeft.transform.localRotation = Quaternion.Euler(0, 90, 180);

            objectHelper.GetPrefabClone(ref biodome_robot_arm, rearFoots.transform, true, "rearFootRight", out GameObject rearFootRight);

            rearFootRight.transform.localPosition = new Vector3(2.07f, -3.31f, 0.94f);
            rearFootRight.transform.localScale = new Vector3(2f, 0.60f, 1f);
            rearFootRight.transform.localRotation = Quaternion.Euler(0, 270, 180);

            var collision = mainCab.FindChild("collision");

            landingFootCollisions = objectHelper.CreateGameObject("landingFootCollisions", collision.transform);

            landingFootCollisions.transform.localPosition = new Vector3(0f, -2.10f, 0.94f);

            landingFootCollisions.SetActive(false);

            frontFootCollisions = objectHelper.CreateGameObject("frontFootCollisions", landingFootCollisions.transform);

            BoxCollider frontLeftCollider = frontFootCollisions.AddComponent<BoxCollider>();
            frontLeftCollider.center = new Vector3(-1.52f, 0.10f, -0.19f);
            frontLeftCollider.size = new Vector3(0.54f, 0.15f, 0.95f);
            frontLeftCollider.contactOffset = 0.01f;
            footCollisions.Add(frontLeftCollider);

            BoxCollider frontRightCollider = frontFootCollisions.AddComponent<BoxCollider>();
            frontRightCollider.center = new Vector3(1.52f, 0.10f, -0.19f);
            frontRightCollider.size = new Vector3(0.54f, 0.15f, 0.95f);
            frontRightCollider.contactOffset = 0.01f;
            footCollisions.Add(frontRightCollider);

            BoxCollider frontCenterCollider = frontFootCollisions.AddComponent<BoxCollider>();
            frontCenterCollider.center = new Vector3(0f, 0.10f, 1.63f);
            frontCenterCollider.size = new Vector3(0.95f, 0.15f, 0.54f);
            frontCenterCollider.contactOffset = 0.01f;
            footCollisions.Add(frontCenterCollider);
                       
            rearFootCollisions = objectHelper.CreateGameObject("rearFootCollisions", landingFootCollisions.transform);

            rearFootCollisions.SetActive(false);

            BoxCollider rearLeftCollider = rearFootCollisions.AddComponent<BoxCollider>();
            rearLeftCollider.center = new Vector3(-1.80f, 0.10f, 0);
            rearLeftCollider.size = new Vector3(0.54f, 0.15f, 0.95f);
            footCollisions.Add(rearLeftCollider);

            BoxCollider rearRightCollider = rearFootCollisions.AddComponent<BoxCollider>();
            rearRightCollider.center = new Vector3(1.80f, 0.10f, 0);
            rearRightCollider.size = new Vector3(0.54f, 0.15f, 0.95f);
            footCollisions.Add(rearRightCollider);
            
            GameObject hatchTrigger = helper.MainCab.FindChild("hatchTrigger");

            objectHelper.GetPrefabClone(ref hatchTrigger, landingFoots.transform, true, "hatchTriggerCloneRear", out hatchTriggerCloneRear);
            hatchTriggerCloneRear.transform.localPosition = new Vector3(0f, -0.47f, 0.20f);
            hatchTriggerCloneRear.transform.localScale = new Vector3(0.93f, 1.87f, 0.24f);

            objectHelper.GetPrefabClone(ref hatchTrigger, landingFoots.transform, true, "hatchTriggerCloneFront", out hatchTriggerCloneFront);
            hatchTriggerCloneFront.transform.localPosition = new Vector3(0f, -1.36f, 2.76f);
            hatchTriggerCloneFront.transform.localScale = new Vector3(0.41f, 0.50f, 0.12f);
            hatchTriggerCloneFront.transform.localRotation = Quaternion.Euler(32, 0, 0);

            detachLever = mainCab.FindChild("detachlever");

            landingFoots.SetActive(false);

            Destroy(biodome_robot_arm);
        }
    }
}
