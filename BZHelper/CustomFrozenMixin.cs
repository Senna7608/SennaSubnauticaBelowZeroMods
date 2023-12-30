using System.Collections.Generic;
using UnityEngine;

namespace BZHelper
{
#pragma warning disable CS1591 // Missing XML documentation

    public class CustomFrozenMixin : FrozenMixin
    {
        private Animator animator;
        private Locomotion locomotion;
        private EcoTarget ecoTarget;
        private AnimatorLink animatorLink;
        private GameObject modelRoot;
        private Material iceMaterial;
        private VFXOverlayMaterial iceOverlay;
        private bool locomotionWasEnabled;

        private static readonly HashSet<FrozenMixin> frozenCreatures = new HashSet<FrozenMixin>();

        public void Awake()
        {
            animator = GetComponentInChildren<Animator>();

            locomotion = GetComponent<Locomotion>();
            locomotionWasEnabled = locomotion.isActiveAndEnabled;
            ecoTarget = GetComponent<EcoTarget>();
            animatorLink = GetComponentInChildren<AnimatorLink>();
            rb = GetComponent<Rigidbody>();
            modelRoot = GetComponentInChildren<Renderer>().gameObject;

            iceMaterial = Player.main.GetComponent<PlayerFrozenMixin>().GetPrivateField("iceMaterial") as Material;
        }

        public static FrozenMixin GetNearestFrozenCreature(Vector3 position, float maxRange, BehaviourType creatureType = BehaviourType.Unknown)
        {
            if (frozenCreatures.Count < 1)
            {
                return null;
            }

            FrozenMixin result = null;

            float num = maxRange * maxRange;

            foreach (FrozenMixin frozenMixin in frozenCreatures)
            {
                if (frozenMixin.IsFrozenInWater() && (creatureType == BehaviourType.Unknown || CreatureData.GetBehaviourType(frozenMixin.gameObject) == creatureType))
                {
                    float sqrMagnitude = (position - frozenMixin.transform.position).sqrMagnitude;

                    if (sqrMagnitude < num)
                    {
                        result = frozenMixin;
                        num = sqrMagnitude;
                    }
                }
            }

            return result;
        }

        public override void Start()
        {
            base.Start();
        }

        public override bool Freeze(float endTime, bool inIce)
        {
            bool frozen = this.frozen;

            if (!base.Freeze(endTime, inIce))
            {
                return false;
            }

            if (!frozen)
            {
                if (animatorLink != null)
                {
                    animatorLink.enabled = false;
                }

                animator.enabled = false;
                locomotionWasEnabled = false;

                if (locomotion.enabled)
                {
                    locomotionWasEnabled = true;
                    locomotion.enabled = false;
                }

                frozenCreatures.Add(this);
            }

            if (ecoTarget != null)
            {
                ecoTarget.enabled = !inIce;
            }

            float lifeTime = (endTime == float.PositiveInfinity) ? -1f : (endTime - Time.time);

            if (iceOverlay != null)
            {
                iceOverlay.RemoveOverlay();
            }

            iceOverlay = modelRoot.AddComponent<VFXOverlayMaterial>();

            iceOverlay.ApplyAndForgetOverlay(iceMaterial, "VFXOverlay: Frozen", Color.clear, lifeTime);

            return true;
        }

        public override void Unfreeze()
        {
            bool frozen = this.frozen;

            base.Unfreeze();

            if (frozen)
            {
                animator.enabled = true;

                if (animatorLink != null)
                {
                    animatorLink.enabled = true;
                }

                if (locomotionWasEnabled)
                {
                    locomotion.enabled = true;
                }

                frozenCreatures.Remove(this);

                if (iceOverlay != null)
                {
                    iceOverlay.RemoveOverlay();
                }

                if (ecoTarget != null)
                {
                    ecoTarget.enabled = true;
                }
            }
        }
    }
}
