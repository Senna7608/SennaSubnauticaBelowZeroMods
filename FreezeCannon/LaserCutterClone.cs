using System;
using FMODUnity;
using UnityEngine;
using UWE;

namespace FreezeCannon
{
    [RequireComponent(typeof(EnergyMixin))]
    public class LaserCutterClone : PlayerTool
    {
        public override string animToolName
        {
            get
            {
                return "lasercutter";
            }
        }

        public FireSource fireSource;                
        public StudioEventEmitter sfx_laserLoop;                
        public StudioEventEmitter sfx_oneShot;               
        public VFXController fxControl;                
        public Light fxLight;

        public float laserEnergyCost = 1f;

        private float healthPerWeld = 25f;

        private bool usedThisFrame;

        private Sealed activeCuttingTarget;

        private bool fxIsPlaying;

        private bool firedOneShot;        

        public float maxLightIntensity = 1f;

        private float lightIntensity;

        private AimIKTarget playerIKTarget;

        private float totalTimeActive;

        private Color lightbarColor = Color.black;

        private void Start()
        {
            fireSource = GetComponent<FireSource>();

            GameObject AudioAssets = gameObject.FindChild("AudioAssets");

            StudioEventEmitter[] array = AudioAssets.GetComponents<StudioEventEmitter>();
            for (int i = 0; i < array.Length; i++)
            {
                StudioEventEmitter emitter = array[i];

                switch (i)
                {
                    case 0: sfx_laserLoop = emitter;
                        break;

                    case 1:
                        sfx_oneShot = emitter;
                        break;
                }                
            }

            GameObject SparkEmit = gameObject.FindChild("SparkEmit");

            fxControl = SparkEmit.GetComponent<VFXController>();

            fxLight = SparkEmit.GetComponentInChildren<Light>();

            playerIKTarget = Player.main.armsController.lookTargetTransform.GetComponent<AimIKTarget>();
        }

        private void OnDisable()
        {
            activeCuttingTarget = null;

            if (playerIKTarget != null)
            {
                playerIKTarget.enabled = true;
            }
        }

        public override void OnToolUseAnim(GUIHand hand)
        {
            LaserCut();
        }

        public override void OnHolster()
        {
            base.OnHolster();
            StopLaserCuttingFX();
            fxLight.intensity = lightIntensity;
            fireSource.AttachToPlayer(null);
            fireSource.enabled = false;
        }

        private void LaserCut()
        {
            bool flag = true;

            if (activeCuttingTarget && activeCuttingTarget.requireOpenFromFront && !Utils.CheckObjectInFront(activeCuttingTarget.transform, Player.main.transform, 90f))
            {
                flag = false;
            }

            if (energyMixin.IsDepleted())
            {
                flag = false;
            }

            if (flag && activeCuttingTarget != null)
            {
                bool flag2 = false;

                activeCuttingTarget.Weld(healthPerWeld);

                if (activeCuttingTarget.openedAmount < activeCuttingTarget.maxOpenedAmount)
                {
                    flag2 = true;
                }

                if (flag2)
                {
                    if (activeCuttingTarget.GetComponent<LaserCutObject>())
                    {
                        activeCuttingTarget.GetComponent<LaserCutObject>().ActivateFX();
                    }

                    if (playerIKTarget != null)
                    {
                        playerIKTarget.enabled = false;
                    }

                    StartLaserCuttingFX();

                    energyMixin.ConsumeEnergy(laserEnergyCost);

                    return;
                }

                if (playerIKTarget != null)
                {
                    playerIKTarget.enabled = true;
                    return;
                }
            }
            else
            {
                StopLaserCuttingFX();
            }
        }

        private void UpdateTarget()
        {            
            activeCuttingTarget = null;

            if (usingPlayer != null)
            {
                Vector3 vector = default(Vector3);

                GameObject gameObject = null;

                Vector3 vector2;

                UWE.Utils.TraceFPSTargetPosition(Player.main.gameObject, 2f, ref gameObject, ref vector, out vector2, true);

                if (gameObject == null)
                {
                    InteractionVolumeUser component = Player.main.gameObject.GetComponent<InteractionVolumeUser>();
                    if (component != null && component.GetMostRecent() != null)
                    {
                        gameObject = component.GetMostRecent().gameObject;
                    }
                }

                if (gameObject)
                {
                    Sealed exists = gameObject.FindAncestor<Sealed>();

                    if (exists)
                    {
                        activeCuttingTarget = exists;
                    }
                }
            }
            
        }

        private void StartLaserCuttingFX()
        {
            if (fxControl != null && !fxIsPlaying)
            {
                int i = Player.main.IsUnderwater() ? 1 : 0;
                fxControl.Play(i);
                fxControl.Play(3);
                fxIsPlaying = true;
                fxLight.enabled = true;
                InvokeRepeating("RandomizeIntensity", 0f, 0.05f);
                totalTimeActive = 0f;
            }
        }

        private void StopLaserCuttingFX()
        {
            sfx_laserLoop.Stop();

            if (playerIKTarget != null)
            {
                playerIKTarget.enabled = true;
            }

            if (fxControl != null && this.fxIsPlaying)
            {
                fxControl.StopAndDestroy(0f);
                fxIsPlaying = false;
                CancelInvoke("RandomizeIntensity");
                fxLight.enabled = false;
            }            
        }

        private void Update()
        {
            usedThisFrame = false;

            if (!isDrawn)
            {
                return;
            }

            if (AvatarInputHandler.main.IsEnabled() && Player.main.GetRightHandHeld() && !Player.main.IsSpikyTrapAttached())
            {
                usedThisFrame = true;
            }

            if (!usedThisFrame)
            {
                firedOneShot = false;
            }

            fireSource.enabled = usedThisFrame;

            if (usedThisFrame && activeCuttingTarget && !sfx_laserLoop.IsPlaying())
            {
                sfx_laserLoop.Play();
            }

            else if (usedThisFrame && !firedOneShot && !sfx_laserLoop.IsPlaying())
            {
                sfx_oneShot.Play();
                firedOneShot = true;
            }

            else if (!usedThisFrame && fxIsPlaying)
            {
                StopLaserCuttingFX();
            }

            else if (!usedThisFrame)
            {
                sfx_laserLoop.Stop();
            }

            if (fxIsPlaying)
            {
                fxLight.intensity = Mathf.MoveTowards(fxLight.intensity, lightIntensity, Time.deltaTime * 25f);
            }

            UpdateTarget();            
        }

        public override void OnDraw(Player p)
        {
            base.OnDraw(p);

            fireSource.AttachToPlayer(p);

            if (firstUseAnimationStarted)
            {
                fxControl.Play(2);
            }
        }

        protected override void OnFirstUseAnimationStop()
        {
            base.OnFirstUseAnimationStop();
            fxControl.StopAndDestroy(2, 0f);
        }

        private void RandomizeIntensity()
        {
            lightIntensity = UnityEngine.Random.Range(0f, maxLightIntensity);
        }

        public override bool GetUsedToolThisFrame()
        {
            return usedThisFrame;
        }

        private void UpdateLightbar()
        {
            totalTimeActive += Time.deltaTime;

            float num = UWE.Utils.SineWaveNegOneToOne(totalTimeActive * 0.5f) * 100f;

            int num2 = Mathf.FloorToInt(150f + num);

            num2 = Mathf.Clamp(num2, 13, 255);

            lightbarColor.r = (float)num2;

            PlatformUtils.SetLightbarColor(lightbarColor, 0);
        }        

    }
}
