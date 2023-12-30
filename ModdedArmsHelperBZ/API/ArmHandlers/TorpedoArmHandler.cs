using BZHelper;
using UnityEngine;

namespace ModdedArmsHelperBZ.API.ArmHandlers
{
    /// <summary>
    /// The abstract class to inherit for handling the Torpedo arm base mechanics.
    /// </summary>
    public abstract class TorpedoArmHandler : ArmHandler
    {
#pragma warning disable CS1591 //XML documentation

        public GenericHandTarget handTarget { get; private set; }        
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
        public TorpedoType[] TorpedoTypes { get => ModdedArmsHelperBZ_Main.armsCacheManager.TorpedoTypes; }

#pragma warning restore CS1591 //XML documentation

        /// <summary>
        /// If you implement this method in your modded arm handler, the first line should be 'base.Awake()'
        /// </summary>
        public override void Awake()
        {
            siloFirst = UnityHelper.FindDeepChild(gameObject, "TorpedoSiloFirst").transform;
            siloSecond = UnityHelper.FindDeepChild(gameObject, "TorpedoSiloSecond").transform;

            visualTorpedoFirst = UnityHelper.FindDeepChild(gameObject, "TorpedoFirst");
            visualTorpedoSecond = UnityHelper.FindDeepChild(gameObject, "TorpedoSecond");
            visualTorpedoReload = UnityHelper.FindDeepChild(gameObject, "TorpedoReload");

            handTarget = GetComponentInChildren<GenericHandTarget>(true);            

            fireSound = ScriptableObject.CreateInstance<FMODAsset>();
            fireSound.path = "event:/sub/seamoth/torpedo_fire";
            fireSound.name = "torpedo_fire";

            torpedoDisarmed = ScriptableObject.CreateInstance<FMODAsset>();
            torpedoDisarmed.path = "event:/sub/seamoth/torpedo_disarmed";
            torpedoDisarmed.name = "torpedo_disarmed";
        }

        /// <summary>
        /// If you implement this method in your modded arm handler, the first line should be 'base.Start()'
        /// </summary>
        public override void Start()
        {
        }

        /// <summary>
        /// This method examines which torpedo types can be used of this torpedo arm.
        /// </summary>
        /// <returns>
        /// Returns a <see cref="TechType"/>.<br/> array. 
        /// </returns>
        public TechType[] GetTorpedoTypes()
        {
            TechType[] techTypes = new TechType[TorpedoTypes.Length];

            for (int i = 0; i < TorpedoTypes.Length; i++)
            {
                techTypes[i] = TorpedoTypes[i].techType;
            }

            return techTypes;
        }
    }
}
