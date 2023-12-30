using System.Collections.Generic;
using UnityEngine;

namespace BZHelper
{
#pragma warning disable CS1591 // Missing XML documentation

    public class FreezeCannon : MonoBehaviour
    {
        public bool CanFreeze
        {
            get
            {
                return lastValidTarget != null;
            }
        }
        
        public string TargetTech
        {
            get
            {
                if (lastValidTarget != null)
                {
                    TechType techType = CraftData.GetTechType(lastValidTarget);

                    if (TechStringCache.ContainsKey(techType))
                    {
                        return TechStringCache[techType];
                    }
                    else
                    {
                        TechStringCache.Add(techType, Language.main.Get(TechTypeExtensions.AsString(techType, false)));
                    }

                    return TechStringCache[techType];
                }

                return string.Empty;
            }
        }        

        public GameObject LockedCreature { get; set; }

        public float freezeTime = 15f;
        public float consumption = 1f;

        public VFXController fxControl;
        public FMODAsset shootSound;
        public FMOD_CustomLoopingEmitter connectSound;
        public Animator animator;
        public FMODAsset validTargetSound;
        //public Renderer fpCannonModelRenderer;
        public EnergyInterface energyInterface;

        public GameObject fxBeam;
        public GameObject fxTrailPrefab;
        private GameObject fxTrailInstance;
        public Transform muzzle;
        private VFXElectricLine[] elecLines;
        private GameObject lastValidTarget;

        private Vector3 targetPosition = Vector3.zero;

        //private float cannonGlow;

        private int shaderParamID;

        private float timeLastValidTargetSoundPlayed;

        private static List<GameObject> checkedObjects = new List<GameObject>();

        public bool usingCannon;

        public Vector3 localObjectOffset = Vector3.zero;

        public float maxTargetDistance = 30f;

        public float attractionForce = 140f;
        public float massScalingFactor = 0.02f;

        private Vector3 targetObjectCenter = Vector3.zero;

        public Dictionary<TechType, string> TechStringCache = new Dictionary<TechType, string>();

        private void Start()
        {
            elecLines = fxBeam.GetComponentsInChildren<VFXElectricLine>(true);
            shaderParamID = Shader.PropertyToID("_OverlayStrength");
            
            if (energyInterface == null)
            {
                energyInterface = GetComponent<EnergyInterface>();
            }

            CreateTechStringCache();
        }

        private void CreateTechStringCache()
        {
            foreach (TechType techType in AllowedCreatures)
            {
                TechStringCache.Add(techType, Language.main.Get(TechTypeExtensions.AsString(techType, false)));
            }
        }

        private Bounds GetAABB(GameObject target)
        {
            FixedBounds fixedBounds = target.GetComponent<FixedBounds>();

            Bounds bounds;

            if (fixedBounds != null)
            {
                bounds = fixedBounds.bounds;
            }
            else
            {
                bounds = UWE.Utils.GetEncapsulatedAABB(target, 20);
            }

            return bounds;
        }

        private Vector3 GetObjectPosition(GameObject target)
        {
            Camera camera = MainCamera.camera;

            Vector3 vector = Vector3.zero;

            float absDistance = 0f;

            if (target != null)
            {
                Bounds bounds = GetAABB(target);

                vector = target.transform.position - bounds.center;

                Ray ray = new Ray(bounds.center, camera.transform.forward);

                if (bounds.IntersectRay(ray, out float distance))
                {
                    absDistance = Mathf.Abs(distance);
                }

                targetObjectCenter = bounds.center;
            }

            Vector3 position = Vector3.forward * (2.5f + absDistance) + localObjectOffset;

            return camera.transform.TransformPoint(position) + vector;
        }

        private bool CheckLineOfSight(GameObject newTarget, Vector3 origin, Vector3 direction)
        {
            bool result = true;

            int totalBuffers = UWE.Utils.RaycastIntoSharedBuffer(origin, Vector3.Normalize(direction - origin), (direction - origin).magnitude, ~(1 << LayerMask.NameToLayer("Player")), QueryTriggerInteraction.Ignore);

            bool flag = false;

            int currentBufer = 0;

            while (currentBufer < totalBuffers && !flag)
            {
                GameObject entityRoot = UWE.Utils.GetEntityRoot(UWE.Utils.sharedHitBuffer[currentBufer].collider.gameObject);
                
                if (!entityRoot)
                {
                    entityRoot = UWE.Utils.sharedHitBuffer[currentBufer].collider.gameObject;
                }

                if (entityRoot.GetComponentInChildren<Player>() == null && entityRoot != newTarget)
                {
                    result = false;
                    flag = true;
                }

                currentBufer++;
            }

            return result;
        }

        public bool ValidateObject(GameObject target)
        {
            if (!target.activeSelf || !target.activeInHierarchy)
            {
                return false;
            }

            bool canFreeze = false;

            if (target.GetComponent<CreatureDeath>() != null)
            {
                if (!BannedCreatures.Contains(CraftData.GetTechType(target)))
                {
                    canFreeze = true;
                }
            }

            return canFreeze && energyInterface.hasCharge;
        }

        public bool ValidateNewObject(GameObject newTarget, Vector3 hitPos, bool checkLineOfSight = true)
        {
            if (!ValidateObject(newTarget))
            {
                return false;
            }

            if (checkLineOfSight && !CheckLineOfSight(newTarget, MainCamera.camera.transform.position, hitPos))
            {
                return false;
            }

            return true;
        }

        public bool OnShoot()
        {
            if (LockedCreature != null)
            {
                Utils.PlayFMODAsset(shootSound, transform, 20f);

                energyInterface.ConsumeEnergy(consumption);

                fxControl.Play(0);
                //fxControl.Play();

                if (LockedCreature.TryGetComponent(out CreatureFrozenMixin creatureFrozenMixin))
                {
                    creatureFrozenMixin.FreezeInsideIce();
                    creatureFrozenMixin.FreezeForTime(freezeTime);
                }
                else if (LockedCreature.TryGetComponent(out CustomFrozenMixin customFrozenMixin))
                {
                    customFrozenMixin.FreezeInsideIce();
                    customFrozenMixin.FreezeForTime(freezeTime);
                }
                else
                {
                    CustomFrozenMixin component = LockedCreature.AddComponent<CustomFrozenMixin>();

                    component.FreezeInsideIce();
                    component.FreezeForTime(freezeTime);
                }

                ReleaseLockedObject();

                //fxControl.Stop();

                return true;
            }
            else
            {
                LockedCreature = TraceForTarget();

                return false;
            }
        }

        private void Update()
        {
            if (LockedCreature != null)
            {
                fxBeam.SetActive(true);

                if (LockedCreature.GetComponent<Rigidbody>() != null)
                {
                    connectSound.Play();

                    for (int i = 0; i < elecLines.Length; i++)
                    {
                        VFXElectricLine vfxelectricLine = elecLines[i];
                        vfxelectricLine.origin = muzzle.position;
                        vfxelectricLine.target = targetObjectCenter;
                        vfxelectricLine.originVector = muzzle.forward;                        
                    }
                }

                energyInterface.ConsumeEnergy(Time.deltaTime * 0.7f);
                UpdateTargetPosition();
            }
            else
            {
                connectSound.Stop();
                fxBeam.SetActive(false);
            }
        }

        private void UpdateTargetPosition()
        {
            targetPosition = GetObjectPosition(LockedCreature);
        }

        private void DrawTrail()
        {
            if (fxTrailPrefab != null)
            {
                fxTrailInstance = Utils.SpawnPrefabAt(fxTrailPrefab, null, LockedCreature.transform.position);

                if (fxTrailInstance != null)
                {
                    fxTrailInstance.SetActive(true);

                    ParticleSystem particleSystem = fxTrailInstance.GetComponent<ParticleSystem>();

                    if (particleSystem != null)
                    {
                        particleSystem.Play();
                    }
                }
            }
        }

        private void FixedUpdate()
        {
            if (LockedCreature != null)
            {
                if (!ValidateObject(LockedCreature) || maxTargetDistance * 1.5f < (LockedCreature.transform.position - MainCamera.camera.transform.position).magnitude)
                {
                    ReleaseLockedObject();
                }
                else
                {
                    targetPosition = GetObjectPosition(LockedCreature);
                    /*
                    Rigidbody component = LockedCreature.GetComponent<Rigidbody>();
                    Vector3 value = targetPosition - LockedCreature.transform.position;
                    float magnitude = value.magnitude;
                    float d = Mathf.Clamp(magnitude, 1f, 4f);
                    Vector3 vector = component.velocity + Vector3.Normalize(value) * attractionForce * d * Time.deltaTime / (1f + component.mass * massScalingFactor);
                    Vector3 amount = vector * (10f + Mathf.Pow(Mathf.Clamp01(1f - magnitude), 1.75f) * 40f) * Time.deltaTime;
                    vector = UWE.Utils.SlerpVector(vector, Vector3.zero, amount);
                    component.velocity = vector;
                    */
                }
            }
        }

        public void UpdateActive()
        {
            if (LockedCreature == null)
            {
                GameObject target = TraceForTarget();

                if (lastValidTarget != target && target != null && timeLastValidTargetSoundPlayed + 2f <= Time.time)
                {
                    Utils.PlayFMODAsset(validTargetSound, transform, 20f);

                    timeLastValidTargetSoundPlayed = Time.time;
                }
                
                lastValidTarget = target;
            }

            /*
            if (fpCannonModelRenderer != null)
            {
                if (TargetObject != null)
                {
                    cannonGlow = 1f;
                }
                else
                {
                    cannonGlow -= Time.deltaTime;
                }

                fpCannonModelRenderer.material.SetFloat(shaderParamID, Mathf.Clamp01(cannonGlow));
            }
            */

            if (animator != null)
            {
                animator.SetBool("use_tool", usingCannon);

                animator.SetBool("cangrab_propulsioncannon", CanFreeze || LockedCreature != null);                
            }
            
            HandReticle.main.SetIcon(HandReticle.IconType.Default, (CanFreeze && LockedCreature == null) ? 1.5f : 1f);
        }

        public bool HasChargeForShot()
        {
            return energyInterface.TotalCanProvide(out int num) > 4f;
        }

        public bool IsLockedObject()
        {
            return LockedCreature != null;
        }

        public GameObject TraceForTarget()
        {
            Vector3 position = MainCamera.camera.transform.position;

            int layerMask = ~(1 << LayerMask.NameToLayer("Player"));

            int totalBuffers = UWE.Utils.SpherecastIntoSharedBuffer(position, 1.2f, MainCamera.camera.transform.forward, maxTargetDistance, layerMask, QueryTriggerInteraction.UseGlobal);

            GameObject result = null;

            float num2 = float.PositiveInfinity;

            checkedObjects.Clear();

            for (int i = 0; i < totalBuffers; i++)
            {
                RaycastHit raycastHit = UWE.Utils.sharedHitBuffer[i];
                                
                if (!raycastHit.collider.isTrigger || raycastHit.collider.gameObject.layer == LayerMask.NameToLayer("Useable"))
                {
                    GameObject entityRoot = UWE.Utils.GetEntityRoot(raycastHit.collider.gameObject);
                     
                    if (entityRoot == null)
                    {
                        TechTag techTag = UnityHelper.GetTechTag(raycastHit.collider.gameObject);

                        if (techTag != null && AllowedCreatures.Contains(techTag.type))
                        {
                            entityRoot = raycastHit.collider.gameObject;
                        }
                    }

                    if (entityRoot != null && !checkedObjects.Contains(entityRoot))
                    {
                        float sqrMagnitude = (raycastHit.point - position).sqrMagnitude;

                        if (sqrMagnitude < num2 && ValidateNewObject(entityRoot, raycastHit.point, true))
                        {
                            result = entityRoot;
                            num2 = sqrMagnitude;
                        }

                        checkedObjects.Add(entityRoot);
                    }
                }
            }

            return result;
        }

        public void ReleaseLockedObject()
        {
            if (LockedCreature != null)
            {
                LockedCreature = null;
            }

            if (animator != null)
            {
                animator.SetBool("use_tool", false);

                animator.SetBool("cangrab_propulsioncannon", false);
            }
        }

        private readonly List<TechType> BannedCreatures = new List<TechType>()
        {
            TechType.BladderFishSchool,
            TechType.BoomerangFishSchool,
            TechType.HoopfishSchool,
            TechType.SpinefishSchool
        };

        private readonly List<TechType> AllowedCreatures = new List<TechType>()
        {
            TechType.DiscusFish,
            TechType.FeatherFish,
            TechType.FeatherFishRed,
            TechType.NootFish,
            TechType.SpinnerFish,
            TechType.Bladderfish,
            TechType.Hoopfish,
            TechType.SeaMonkey,
            TechType.ArcticPeeper,
            TechType.ArcticRay,
            TechType.GlowWhale,
            TechType.ShadowLeviathan,
            TechType.Chelicerate,
            TechType.Brinewing,
            TechType.Symbiote,
            TechType.Snowman,
            TechType.RockPuncher,
            TechType.Brinicle,
            TechType.BruteShark,
            TechType.Crash,
            TechType.Rockgrub,                        
            TechType.SeaEmperorJuvenile,
            TechType.TitanHolefish,            
            TechType.Penguin,
            TechType.PenguinBaby,                        
            TechType.Skyray,
            TechType.Boomerang,                               
            TechType.Spinefish,
            TechType.Cryptosuchus,
            TechType.SquidShark,
            TechType.ArrowRay,
            TechType.Jellyfish,
            TechType.SnowStalker,
            TechType.SnowStalkerBaby,
            TechType.IceWorm
        };
    }
}
