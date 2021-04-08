using System.Collections.Generic;
using UnityEngine;

namespace FreezeCannon
{
    public class FreezeCannon : MonoBehaviour
    {
        public bool CanFreeze
        {
            get
            {
                return lastValidTarget != null ? true : false;
            }
        }

        /*
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
        */

        public GameObject TargetObject { get; set; }

        public VFXController fxControl;
        public FMODAsset shootSound;
        public FMOD_CustomLoopingEmitter connectSound;
        //public Animator animator;
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

        private Vector3 targetObjectCenter = Vector3.zero;

        //public Dictionary<TechType, string> TechStringCache = new Dictionary<TechType, string>();

        private void Start()
        {
            elecLines = fxBeam.GetComponentsInChildren<VFXElectricLine>(true);
            shaderParamID = Shader.PropertyToID("_OverlayStrength");
            energyInterface = GetComponent<EnergyInterface>();
        }

        private Bounds GetAABB(GameObject target)
        {
            FixedBounds component = target.GetComponent<FixedBounds>();

            Bounds result;

            if (component != null)
            {
                result = component.bounds;
            }
            else
            {
                result = UWE.Utils.GetEncapsulatedAABB(target, 20);
            }            

            return result;
        }

        private Vector3 GetObjectPosition(GameObject go)
        {
            Camera camera = MainCamera.camera;

            Vector3 b = Vector3.zero;

            float num = 0f;

            if (go != null)
            {
                Bounds aabb = GetAABB(go);

                b = go.transform.position - aabb.center;

                Ray ray = new Ray(aabb.center, camera.transform.forward);

                if (aabb.IntersectRay(ray, out float f))
                {
                    num = Mathf.Abs(f);
                }

                targetObjectCenter = aabb.center;
            }

            Vector3 position = Vector3.forward * (2.5f + num) + localObjectOffset;

            return camera.transform.TransformPoint(position) + b;
        }        

        private bool CheckLineOfSight(GameObject obj, Vector3 a, Vector3 b)
        {            
            bool result = true;

            int num = UWE.Utils.RaycastIntoSharedBuffer(a, Vector3.Normalize(b - a), (b - a).magnitude, ~(1 << LayerMask.NameToLayer("Player")), QueryTriggerInteraction.Ignore);

            bool flag = false;

            int num2 = 0;

            while (num2 < num && !flag)
            {
                GameObject gameObject = UWE.Utils.GetEntityRoot(UWE.Utils.sharedHitBuffer[num2].collider.gameObject);

                if (!gameObject)
                {
                    gameObject = UWE.Utils.sharedHitBuffer[num2].collider.gameObject;
                }

                if (gameObject.GetComponentInChildren<Player>() == null && gameObject != obj)
                {
                    result = false;
                    flag = true;
                }
                
                num2++;
            }
            
            return result;
        }

        private bool ValidateObject(GameObject target)
        {            
            if (!target.activeSelf || !target.activeInHierarchy)
            {                
                return false;
            }

            bool canFreeze = false;

            if (target.GetComponent<Locomotion>() != null)
            {          
                if (!BannedCreatures.Contains(CraftData.GetTechType(target)))
                {
                    canFreeze = true;
                }
            }            

            return canFreeze && energyInterface.hasCharge;
        }

        public bool ValidateNewObject(GameObject go, Vector3 hitPos, bool checkLineOfSight = true)
        {
            if (!ValidateObject(go))
            {
                return false;
            }

            if (checkLineOfSight && !CheckLineOfSight(go, MainCamera.camera.transform.position, hitPos))
            {                
                return false;
            }            

            return true;
        }        

        public bool OnShoot()
        {
            if (TargetObject != null)
            {
                Utils.PlayFMODAsset(shootSound, transform, 20f);
                
                energyInterface.GetValues(out float charge, out float capacity);

                float d = Mathf.Min(1f, charge / 4f);

                energyInterface.ConsumeEnergy(4f);
                                
                fxControl.Play(0);
                fxControl.Play();
                
                if (TargetObject.TryGetComponent(out CreatureFrozenMixin creatureFrozenMixin))
                {
                    creatureFrozenMixin.FreezeInsideIce();
                    creatureFrozenMixin.FreezeForTime(20);
                }
                else if (TargetObject.TryGetComponent(out FreezeCannonFrozenMixin freezeCannonFrozenMixin))
                {
                    freezeCannonFrozenMixin.FreezeInsideIce();
                    freezeCannonFrozenMixin.FreezeForTime(20);
                }
                else
                {
                    FreezeCannonFrozenMixin component = TargetObject.AddComponent<FreezeCannonFrozenMixin>();

                    component.FreezeInsideIce();
                    component.FreezeForTime(20);
                }              

                ReleaseTargetObject();

                fxControl.Stop();            

                return true;
            }            
            else
            {
                TargetObject = TraceForTarget();

                return false;
            }
            
            
        }

        private void Update()
        {
            if (TargetObject != null)
            {
                fxBeam.SetActive(true);

                if (TargetObject.GetComponent<Rigidbody>() != null)
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
            }
            else
            {
                connectSound.Stop();                
                fxBeam.SetActive(false);
            }
        }

        private void DrawTrail()
        {
            if (fxTrailPrefab != null)
            {
                fxTrailInstance = Utils.SpawnPrefabAt(fxTrailPrefab, null, TargetObject.transform.position);
                if (fxTrailInstance != null)
                {
                    fxTrailInstance.SetActive(true);
                    ParticleSystem component = fxTrailInstance.GetComponent<ParticleSystem>();
                    if (component != null)
                    {
                        component.Play();
                    }
                }
            }
        }

        private void FixedUpdate()
        {
            if (TargetObject != null)
            {
                if (!ValidateObject(TargetObject) || maxTargetDistance * 1.5f < (TargetObject.transform.position - MainCamera.camera.transform.position).magnitude)
                {
                    ReleaseTargetObject();
                }
                else
                {
                    targetPosition = GetObjectPosition(TargetObject);
                }
            }
        }        

        public void UpdateActive()
        {
            if (TargetObject == null)
            {
                GameObject gameObject = TraceForTarget();

                if (lastValidTarget != gameObject && gameObject != null && timeLastValidTargetSoundPlayed + 2f <= Time.time)
                {
                    Utils.PlayFMODAsset(validTargetSound, transform, 20f);

                    timeLastValidTargetSoundPlayed = Time.time;
                }

                lastValidTarget = gameObject;
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
            //animator.SetBool("use_tool", usingCannon);

            //animator.SetBool("cangrab_propulsioncannon", CanFreeze || TargetObject != null);

            HandReticle.main.SetIcon(HandReticle.IconType.Default, (CanFreeze && TargetObject == null) ? 1.5f : 1f);
        }

        public bool HasChargeForShot()
        {
            return energyInterface.TotalCanProvide(out int num) > 4f;
        }

        public bool IsTargetingObject()
        {
            return TargetObject != null;
        }

        public GameObject TraceForTarget()
        {            
            Vector3 position = MainCamera.camera.transform.position;

            int layerMask = ~(1 << LayerMask.NameToLayer("Player"));
            
            int num = UWE.Utils.SpherecastIntoSharedBuffer(position, 1.2f, MainCamera.camera.transform.forward, maxTargetDistance, layerMask, QueryTriggerInteraction.UseGlobal);

            GameObject result = null;

            float num2 = float.PositiveInfinity;

            checkedObjects.Clear();

            for (int i = 0; i < num; i++)
            {
                RaycastHit raycastHit = UWE.Utils.sharedHitBuffer[i];

                if (!raycastHit.collider.isTrigger || raycastHit.collider.gameObject.layer == LayerMask.NameToLayer("Useable"))
                {
                    GameObject entityRoot = UWE.Utils.GetEntityRoot(raycastHit.collider.gameObject);
                    
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

        public void ReleaseTargetObject()
        {
            if (TargetObject != null)
            {
                TargetObject = null;
            }
        }

        private readonly HashSet<TechType> BannedCreatures = new HashSet<TechType>()
        {            
            TechType.BladderFishSchool,
            TechType.BoomerangFishSchool,            
            TechType.HoopfishSchool,
            TechType.SpinefishSchool
        };
    }
}
