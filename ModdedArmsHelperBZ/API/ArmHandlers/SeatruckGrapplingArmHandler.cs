﻿using BZHelper;
using UnityEngine;

namespace ModdedArmsHelperBZ.API.ArmHandlers
{
    /// <summary>
    /// The abstract class to inherit for handling the Seatruck grappling arm base mechanics.
    /// </summary>
    public abstract class SeatruckGrapplingArmHandler : ArmHandler
    {
#pragma warning disable CS1591 //XML documentation

        public VFXGrapplingRope rope { get; private set; }
        public SeatruckGrapplingHook hook { get; private set; }
        public FMOD_CustomLoopingEmitter grapplingLoopSound { get; private set; }
        public FMODAsset shootSound { get; private set; }
        public Transform front { get; private set; }
        
        public GameObject hookPrefab { get; private set; }

        public DamageType damageType { get; set; } = DamageType.Collide;

        public float energyCost = 0.5f;        
        public float maxDistance = 50f;
        public float damage = 5f;
        public float seatruckGrapplingAccel = 25f;
        public float targetGrapplingAccel = 400f;
        public Vector3 grapplingStartPos = Vector3.zero;

#pragma warning restore CS1591 //XML documentation

        /// <summary>
        /// If you implement this method in your modded arm handler, the first line should be 'base.Awake()'
        /// </summary>
        public override void Awake()
        {
            front = UnityHelper.FindDeepChild(gameObject, "hook").transform;            
            hookPrefab = Instantiate(ModdedArmsHelperBZ_Main.armsCacheManager.HookPrefab);

            DestroyImmediate(hookPrefab.GetComponent<GrapplingHook>());
            hook = hookPrefab.AddComponent<SeatruckGrapplingHook>();
            hook.transform.parent = front;
            hook.transform.localPosition = Vector3.zero;
            hook.transform.localRotation = Quaternion.identity;
            hook.transform.localScale = new Vector3(1f, 1f, 1f);            

            rope = FindObjectOfType(typeof(VFXGrapplingRope)) as VFXGrapplingRope;
            rope.origin = front.parent;
            rope.attachPoint = hook.transform;

            grapplingLoopSound = GetComponent<FMOD_CustomLoopingEmitter>();

            shootSound = ScriptableObject.CreateInstance<FMODAsset>();
            shootSound.path = "event:/sub/exo/hook_shoot";
            shootSound.name = "hook_shoot";
        }

        /// <summary>
        /// If you implement this method in your modded arm handler, the first line should be 'base.Start()'
        /// </summary>
        public override void Start()
        {            
        }
    }
}
