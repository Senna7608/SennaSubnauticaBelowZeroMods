using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using BZHelper;
using BZHelper.NautilusHelpers;
using Nautilus.Crafting;
using Nautilus.Handlers;
using Nautilus.Utility;

namespace FreezeCannonTool
{
    internal class FreezeCannon_Prefab : ModPrefab_Craftable
    {
        public static TechType TechTypeID { get; private set; }

        internal FreezeCannon_Prefab(ModPrefab_Fragment fragment)
            : base(
                  techTypeName: "FreezeCannon",                  
                  friendlyName: "Freeze Cannon",
                  description: "Freeze the creatures for a while.",
                  template: TechType.PropulsionCannon,
                  gamerResourceFileName: null,
                  requiredAnalysis: TechType.None,
                  groupForPDA: TechGroup.Personal,
                  categoryForPDA: TechCategory.Tools,
                  equipmentType: EquipmentType.Hand,
                  quickSlotType: QuickSlotType.Selectable,
                  backgroundType: CraftData.BackgroundType.Normal,
                  itemSize: new Vector2int(2, 2),                  
                  fragment: fragment
                  )
        {
        }
                
        protected override void PrePatch()
        {
            LanguageHandler.SetLanguageLine("FreezeCannon_Targeting", "Targeting...");
            LanguageHandler.SetLanguageLine("FreezeCannon_Lock", "Lock target");
            LanguageHandler.SetLanguageLine("FreezeCannon_Release", "Release target");
            LanguageHandler.SetLanguageLine("FreezeCannon_Freeze", "Freeze target");            
        }

        protected override void PostPatch()
        {
            TechTypeID = Info.TechType;
        }

        protected override RecipeData GetRecipe()
        {
            return new RecipeData()
            {
                craftAmount = 1,

                Ingredients = new List<Ingredient>(new Ingredient[4]
                {
                    new Ingredient(TechType.Titanium, 2),                    
                    new Ingredient(TechType.Battery, 1),
                    new Ingredient(TechType.Magnetite, 1),
                    new Ingredient(TechType.AdvancedWiringKit, 1)
                })
            };
        }

        protected override EncyData GetEncyclopediaData()
        {
            Texture2D image = ImageUtils.LoadTextureFromFile($"{Main.modFolder}/Assets/FreezeCannon_ency.png");

            return new EncyData()
            {
                title = "Freeze Cannon",
                description = "This cannon freezes the water or humidity around the living creature, making the creature incapable of moving for a while.\n\n" +
                "Alterra Freeze Cannon - Warning:\n\nIn case of accident or death due to careless use, the Alterra company cannot be held responsible!",
                node = EncyNode.Equipment,
                image = image
            };            
        }

        protected override IEnumerator ModifyGameObjectAsync(IOut<bool> success)
        {            
            BZLogger.Trace($"ModifyGameObjectAsync started for TechType: [{Info.ClassID}]...");

            PropulsionCannon propulsionCannon = GameObjectClone.GetComponent<PropulsionCannon>();           

            Object.DestroyImmediate(GameObjectClone.GetComponent<PropulsionCannonWeapon>());
            Object.DestroyImmediate(GameObjectClone.transform.Find("PropulsionCannonGrabbedEffect").gameObject);
            Object.DestroyImmediate(GameObjectClone.transform.Find("objectHolder").gameObject);

            FreezeCannonWeapon cannonWeapon = GameObjectClone.AddComponent<FreezeCannonWeapon>();           
            cannonWeapon.freezeCannon = GameObjectClone.AddComponent<FreezeCannon>();

            cannonWeapon.pickupable = GameObjectClone.GetComponent<Pickupable>();
            cannonWeapon.mainCollider = GameObjectClone.GetComponent<Collider>();
            
            //cannonWeapon.freezeCannon.fxBeam = Object.Instantiate(propulsionCannon.fxBeam, cannonWeapon.transform);
            cannonWeapon.freezeCannon.fxBeam = propulsionCannon.fxBeam;

            //cannonWeapon.freezeCannon.fxTrailPrefab = Object.Instantiate(propulsionCannon.fxTrailPrefab, cannonWeapon.transform);
            cannonWeapon.freezeCannon.fxTrailPrefab = propulsionCannon.fxTrailPrefab;

            cannonWeapon.freezeCannon.muzzle = GameObjectClone.transform.Find("1st person model/Propulsion_Cannon_anim/body/muzzle");

            cannonWeapon.freezeCannon.fxControl = propulsionCannon.fxControl;
            cannonWeapon.freezeCannon.connectSound = propulsionCannon.grabbingSound;

            cannonWeapon.freezeCannon.shootSound = propulsionCannon.shootSound;
            //cannonWeapon.freezeCannon.shootSound = ScriptableObject.CreateInstance<FMODAsset>();
            //cannonWeapon.freezeCannon.shootSound.name = "fire";
            //cannonWeapon.freezeCannon.shootSound.path = "event:/tools/gravcannon/fire";

            cannonWeapon.freezeCannon.validTargetSound = propulsionCannon.validTargetSound;
            //cannonWeapon.freezeCannon.validTargetSound = ScriptableObject.CreateInstance<FMODAsset>();
            //cannonWeapon.freezeCannon.validTargetSound.name = "ready";
            //cannonWeapon.freezeCannon.validTargetSound.path = "event:/tools/gravcannon/ready";            

            cannonWeapon.reloadMode = PlayerTool.ReloadMode.Direct;
            cannonWeapon.drawTime = 0;
            cannonWeapon.holsterTime = 0.1f;
            cannonWeapon.dropTime = 0;
            cannonWeapon.ikAimRightArm = true;
            cannonWeapon.ikAimLeftArm = true;
            cannonWeapon.useLeftAimTargetOnPlayer = true;
            
            cannonWeapon.leftHandIKTarget = GameObjectClone.transform.Find("1st person model/Propulsion_Cannon_anim/leftAttach/left_hand_target");
            cannonWeapon.socket = PlayerTool.Socket.RightHand;
                        
            cannonWeapon.reloadSound = ScriptableObject.CreateInstance<FMODAsset>();
            cannonWeapon.reloadSound.name = "reload";
            cannonWeapon.reloadSound.path = "event:/tools/gravcannon/reload";

            cannonWeapon.drawSound = ScriptableObject.CreateInstance<FMODAsset>();
            cannonWeapon.drawSound.name = "deploy";
            cannonWeapon.drawSound.path = "event:/tools/gravcannon/deploy";

            Texture2D _Illum = ImageUtils.LoadTextureFromFile($"{Main.modFolder}/Assets/FreezeCannon_illum.png");
            _Illum.name = "FreezeCannon_Illum";

            Texture2D _MainTex = ImageUtils.LoadTextureFromFile($"{Main.modFolder}/Assets/FreezeCannon_MainTex.png");
            _MainTex.name = "FreezeCannon_MainTex";

            GameObject fp_model = GameObjectClone.transform.Find("1st person model/Propulsion_Cannon_anim/Propulsion_Cannon_geo").gameObject;           
            GraphicsHelper.ChangeObjectTexture(fp_model, 0, mainTex: _MainTex, illumTex: _Illum);

            GameObject tp_model = GameObjectClone.transform.Find("3rd person model/model/Propulsion_Cannon").gameObject;            

            GraphicsHelper.ChangeObjectTexture(tp_model, 0, mainTex: _MainTex, illumTex: _Illum);
            
            /*            
            Texture2D _MainTex = Main.assetBundle.LoadAsset<Texture2D>("freezeCannon_main");
            Texture2D _Illum = Main.assetBundle.LoadAsset<Texture2D>("freezeCannon_illum");
            Texture2D _BumpMap = Main.assetBundle.LoadAsset<Texture2D>("freezeCannon_bump");
            Texture2D _SpecTex = Main.assetBundle.LoadAsset<Texture2D>("freezeCannon_spec");

            Shader marmoShader = Shader.Find("MarmosetUBER");
            
            MeshRenderer freezeRenderer = freezeCannon.GetComponent<MeshRenderer>();

            for (int i = 0; i < freezeRenderer.materials.Length; i++)
            {
                Material material = freezeRenderer.materials[i];

                switch (i)
                {
                    case 0:
                        material.shader = marmoShader;
                        //material.EnableKeyword("UWE_3COLOR");
                        //material.EnableKeyword("MARMO_NORMALMAP");
                        material.EnableKeyword("_ZWRITE_ON");
                        material.SetTexture(Shader.PropertyToID("_MainTex"), _MainTex);
                        material.SetTexture(Shader.PropertyToID("_BumpMap"), _BumpMap);
                        break;

                    case 1:
                        material.shader = marmoShader;
                        //material.EnableKeyword("UWE_3COLOR");
                        //material.EnableKeyword("MARMO_NORMALMAP");
                        material.EnableKeyword("MARMO_EMISSION");
                        material.EnableKeyword("_ZWRITE_ON");
                        material.SetTexture(Shader.PropertyToID("_MainTex"), _MainTex);
                        material.SetTexture(Shader.PropertyToID("_BumpMap"), _BumpMap);
                        material.SetTexture(Shader.PropertyToID("_Illum"), _Illum);
                        break;

                    default:
                        material.shader = marmoShader;
                        //material.EnableKeyword("UWE_3COLOR");
                        break;
                }
                
                
                material.DisableKeyword("_NORMALMAP");
                
                //material.EnableKeyword("MARMO_SPECMAP");                
                //material.SetTexture(Shader.PropertyToID("_SpecTex"), _SpecTex);
            }
            */

            success.Set(true);
            yield break;
        }

        protected override CrafTreeTypesData GetCraftTreeTypesData()
        {
            return new CrafTreeTypesData()
            {
                TreeTypes = new List<CraftTreeType>()
                {
                    new CraftTreeType(CraftTree.Type.Fabricator, new string[] { "Personal", "Tools" } )                    
                }
            };
        }

        protected override TabNode GetTabNodeData()
        {
            return null;
        }

        protected override Sprite GetItemSprite()
        {
            return ImageUtils.LoadSpriteFromFile($"{Main.modFolder}/Assets/FreezeCannon_icon.png");
        }
    }
}
