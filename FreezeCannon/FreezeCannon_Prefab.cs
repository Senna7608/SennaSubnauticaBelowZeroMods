using System.Collections.Generic;
using SMLHelper.V2.Crafting;
using UnityEngine;
using BZCommon.Helpers.SMLHelpers;
using SMLHelper.V2.Utility;
using SMLHelper.V2.Handlers;

namespace FreezeCannon
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
            TechTypeID = TechType;

            LanguageHandler.Main.SetLanguageLine("FreezeCannon_Targeting", "Targeting...");
            LanguageHandler.Main.SetLanguageLine("FreezeCannon_Lock", "Lock target");
            LanguageHandler.Main.SetLanguageLine("FreezeCannon_Release", "Release target");
            LanguageHandler.Main.SetLanguageLine("FreezeCannon_Freeze", "Freeze target");            
        }

        protected override void PostPatch()
        {
        }

        protected override RecipeData GetRecipe()
        {
            return new RecipeData()
            {
                craftAmount = 1,

                Ingredients = new List<Ingredient>(new Ingredient[3]
                {
                    new Ingredient(TechType.Titanium, 2),                    
                    new Ingredient(TechType.Battery, 1),
                    new Ingredient(TechType.AdvancedWiringKit, 1)
                })
            };
        }

        protected override EncyData GetEncyclopediaData()
        {
            Texture2D image = ImageUtils.LoadTextureFromFile($"{Main.modFolder}/Assets/Freeze_Cannon.png");

            return new EncyData()
            {
                title = "Freeze Cannon",
                description = "This cannon freezes the water or humidity around the living creature, making the creature incapable of moving for a while.\n\n'Alterra Freeze Cannon - Warning! It doesn't work in the desert!'",
                node = EncyNode.Equipment,
                image = image
            };            
        }

        protected override void ModifyGameObject()
        {
            GameObject freezeCannon = Object.Instantiate(Main.assetBundle.LoadAsset<GameObject>("FreezeCannonNew"), _GameObject.transform);
                                   
            PropulsionCannon propulsionCannon = _GameObject.GetComponent<PropulsionCannon>();           

            Object.DestroyImmediate(_GameObject.GetComponent<PropulsionCannonWeapon>());
            Object.DestroyImmediate(_GameObject.transform.Find("PropulsionCannonGrabbedEffect").gameObject);
            Object.DestroyImmediate(_GameObject.transform.Find("objectHolder").gameObject);            

            FreezeCannonWeapon cannonWeapon = _GameObject.AddComponent<FreezeCannonWeapon>();
            cannonWeapon.freezeCannon = _GameObject.AddComponent<FreezeCannon>();
            
            cannonWeapon.pickupable = _GameObject.GetComponent<Pickupable>();
            cannonWeapon.mainCollider = _GameObject.GetComponent<Collider>();
            
            cannonWeapon.freezeCannon.fxBeam = Object.Instantiate(propulsionCannon.fxBeam, cannonWeapon.transform);
            cannonWeapon.freezeCannon.fxTrailPrefab = Object.Instantiate(propulsionCannon.fxTrailPrefab, cannonWeapon.transform);            
            cannonWeapon.freezeCannon.muzzle = freezeCannon.FindChild("muzzle").transform;

            cannonWeapon.freezeCannon.fxControl = propulsionCannon.fxControl;
            cannonWeapon.freezeCannon.connectSound = propulsionCannon.grabbingSound;
            
            cannonWeapon.freezeCannon.shootSound = ScriptableObject.CreateInstance<FMODAsset>();
            cannonWeapon.freezeCannon.shootSound.name = "fire";
            cannonWeapon.freezeCannon.shootSound.path = "event:/tools/gravcannon/fire";

            cannonWeapon.freezeCannon.validTargetSound = ScriptableObject.CreateInstance<FMODAsset>();
            cannonWeapon.freezeCannon.validTargetSound.name = "ready";
            cannonWeapon.freezeCannon.validTargetSound.path = "event:/tools/gravcannon/ready";            

            //Object.DestroyImmediate(propulsionCannon);

            cannonWeapon.reloadMode = PlayerTool.ReloadMode.Direct;
            cannonWeapon.drawTime = 0;
            cannonWeapon.holsterTime = 0.1f;
            cannonWeapon.dropTime = 0;
            cannonWeapon.ikAimRightArm = true;
            cannonWeapon.ikAimLeftArm = true;
            cannonWeapon.useLeftAimTargetOnPlayer = true;
            
            cannonWeapon.leftHandIKTarget = freezeCannon.FindChild("leftHandTarget").transform;
            cannonWeapon.socket = PlayerTool.Socket.RightHand;

            cannonWeapon.reloadSound = ScriptableObject.CreateInstance<FMODAsset>();
            cannonWeapon.reloadSound.name = "reload";
            cannonWeapon.reloadSound.path = "event:/tools/gravcannon/reload";

            cannonWeapon.drawSound = ScriptableObject.CreateInstance<FMODAsset>();
            cannonWeapon.drawSound.name = "deploy";
            cannonWeapon.drawSound.path = "event:/tools/gravcannon/deploy";
                        
            freezeCannon.transform.localScale = new Vector3(0.14f, 0.14f, 0.14f);
            freezeCannon.transform.localPosition = new Vector3(0.0f, -0.04F, -0.13f);
            freezeCannon.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                        
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
        }
        protected override CrafTreeTypesData GetCraftTreeTypesData()
        {
            return new CrafTreeTypesData()
            {
                TreeTypes = new List<CraftTreeType>()
                {
                    new CraftTreeType(CraftTree.Type.Fabricator, new string[] { "Personal", "Tools", "FreezeCannon" } )                    
                }
            };
        }
        protected override TabNode GetTabNodeData()
        {
            return null;
        }
        protected override Sprite GetItemSprite()
        {
            return SpriteManager.Get(TechType.PropulsionCannon);
        }        
    }
}
