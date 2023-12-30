using BZHelper;
using UnityEngine;

namespace FreezeCannonTool
{
    [RequireComponent(typeof(EnergyMixin))]
    public class FreezeCannonWeapon : PlayerTool, IEquippable
    {
        public override string animToolName
        {
            get
            {
                return "propulsioncannon";               
            }
        }        

        public FreezeCannon freezeCannon;        

        private string cachedPrimaryUseText = string.Empty;

        private string cachedAltUseText = string.Empty;

        private string cachedCustomUseText = string.Empty;

        public override string GetCustomUseText()
        {
            bool flag = freezeCannon.IsLockedObject();
            bool flag2 = freezeCannon.HasChargeForShot();

            if (usingPlayer == null || usingPlayer.IsInSub() || (!flag && !flag2))
            {
                return base.GetCustomUseText();
            }

            string text = string.Empty;
            string text2 = string.Empty;

            if (flag)
            {     
                text = $"{Language.main.Get("FreezeCannon_Freeze")} ({uGUI.FormatButton(GameInput.Button.RightHand, false, " / ", false)})";
                text2 = $"{Language.main.Get("FreezeCannon_Release")} ({uGUI.FormatButton(GameInput.Button.AltTool, false, " / ", false)})";
            }
            else
            {               
                if (freezeCannon.CanFreeze)
                {                                        
                    text = $"{Language.main.Get("FreezeCannon_Lock")} ({uGUI.FormatButton(GameInput.Button.RightHand, false, " / ", false)})";
                    text2 = freezeCannon.TargetTech;
                }
                else
                {
                    text = Language.main.Get("FreezeCannon_Targeting");
                    text2 = freezeCannon.TargetTech;
                }
            }            

            if (text != cachedPrimaryUseText || text2 != cachedAltUseText)
            {
                cachedCustomUseText = $"{text}\n{text2}";
                cachedPrimaryUseText = text;
                cachedAltUseText = text2;
            }

            return cachedCustomUseText;
        }

        public override void OnDraw(Player p)
        {            
            base.OnDraw(p);
        }

        public override void OnHolster()
        {
            base.OnHolster();
            freezeCannon.ReleaseLockedObject();
        }

        public override void OnToolBleederHitAnim(GUIHand guiHand)
        {
            if (usingPlayer != null)
            {
                Bleeder bleeder = usingPlayer.GetComponentInChildren<BleederAttachTarget>().bleeder;

                if (bleeder != null)
                {
                    bleeder.attachAndSuck.SetDetached();
                    freezeCannon.ReleaseLockedObject();                    
                }
            }
        }

        public override FMODAsset GetBleederHitSound(FMODAsset defaultSound)
        {
            return null;
        }

        public override bool OnExitDown()
        {
            if (usingPlayer != null && !usingPlayer.IsInSub())
            {
                freezeCannon.ReleaseLockedObject();
                return true;
            }

            return false;
        }

        public override bool OnAltDown()
        {
            if (usingPlayer != null && usingPlayer.IsInSub())
            {
                return false;
            }

            if (freezeCannon.IsLockedObject())
            {
                freezeCannon.ReleaseLockedObject();
            }

            return true;
        }        

        public override bool OnRightHandDown()
        {
            if (usingPlayer != null && usingPlayer.IsInSub())
            {
                return false;
            }

            return freezeCannon.OnShoot();
        }        

        public override void OnToolReloadBeginAnim(GUIHand guiHand)
        {
            base.OnToolReloadBeginAnim(guiHand);            
        }

        public override void OnFirstUseAnimationStop()
        {
            
        }

        public void OnEquip(GameObject sender, string slot)
        {
            
        }

        public void OnUnequip(GameObject sender, string slot)
        {
            
        }
        
        public void UpdateEquipped(GameObject sender, string slot)
        {
            if (usingPlayer != null && !usingPlayer.IsInSub())
            {
                freezeCannon.usingCannon = GameInput.GetButtonDown(GameInput.Button.RightHand);

                freezeCannon.UpdateActive();

                SafeAnimator.SetBool(Player.main.armsController.GetComponent<Animator>(), "cangrab_propulsioncannon", freezeCannon.CanFreeze || freezeCannon.LockedCreature != null);
            }
        }                
    }
}
