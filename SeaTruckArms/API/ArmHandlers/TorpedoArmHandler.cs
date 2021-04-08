extern alias SEZero;

using UnityEngine;
using BZCommon.Helpers;
using SEZero::SlotExtenderZero.API;

namespace SeaTruckArms.API.ArmHandlers
{
    public abstract class TorpedoArmHandler : MonoBehaviour
    {
        public SeaTruckHelper TruckHelper => GetComponentInParent<SeaTruckArmManager>().helper;

        public GenericHandTarget handTarget { get; private set; }
        public Animator animator { get; private set; }
        public Transform siloFirst { get; private set; }
        public Transform siloSecond { get; private set; }
        public GameObject visualTorpedoFirst { get; private set; }
        public GameObject visualTorpedoSecond { get; private set; }
        public GameObject visualTorpedoReload { get; private set; }
        public FMODAsset fireSound { get; private set; }
        public FMODAsset torpedoDisarmed { get; private set; }

        public ItemsContainer container;
        public float cooldownTime = 5f;
        public float cooldownInterval = 1f;
        public float timeFirstShot = float.NegativeInfinity;
        public float timeSecondShot = float.NegativeInfinity;

        public virtual void Awake()
        {
            ObjectHelper objectHelper = ArmServices.main.objectHelper;

            animator = GetComponent<Animator>();

            siloFirst = objectHelper.FindDeepChild(gameObject, "TorpedoSiloFirst").transform;
            siloSecond = objectHelper.FindDeepChild(gameObject, "TorpedoSiloSecond").transform;

            visualTorpedoFirst = objectHelper.FindDeepChild(gameObject, "TorpedoFirst");
            visualTorpedoSecond = objectHelper.FindDeepChild(gameObject, "TorpedoSecond");
            visualTorpedoReload = objectHelper.FindDeepChild(gameObject, "TorpedoReload");

            handTarget = gameObject.GetComponentInChildren<GenericHandTarget>(true);            

            fireSound = ScriptableObject.CreateInstance<FMODAsset>();
            fireSound.path = "event:/sub/seamoth/torpedo_fire";
            fireSound.name = "torpedo_fire";

            torpedoDisarmed = ScriptableObject.CreateInstance<FMODAsset>();
            torpedoDisarmed.path = "event:/sub/seamoth/torpedo_disarmed";
            torpedoDisarmed.name = "torpedo_disarmed";
        }
    }
}
