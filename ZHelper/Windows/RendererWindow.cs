using BZCommon.Helpers.RuntimeGUI;
using SMLHelper.V2.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ZHelper.FileHelper;
using ZHelper.Helpers;
using ZHelper.Renderers;
using static Zhelper.Objects.FieldHelper;
using static Zhelper.Objects.PropertyHelper;

namespace ZHelper
{
    public partial class ZHelper // Renderer Window
    {
        private Rect rendererWindowRect = new Rect(604, 30, 600, 400);

        private List<MaterialInfo> _materialInfos;

        private List<string> _materialInfo_ScrollItems = new List<string>();
        
        private IDictionary<int, Material[]> undoDictionary = new Dictionary<int, Material[]>();

        private bool undo = false;        

        private int materialIndex = -1;
        private int shaderIndex = -1;

        private List<GUI_content> RendererScrollContents = new List<GUI_content>();        

        private List<GUI_content> RendererWindowButtons = new List<GUI_content>()
        {
            new GUI_content((int)ButtonID.BTN_UNDO, GUI_Item_Type.NORMALBUTTON, "Undo", null, new GUI_textColor(), textAlign: TextAnchor.MiddleCenter, fixedWidth: 50f),
            new GUI_content((int)ButtonID.BTN_RONOFF, GUI_Item_Type.NORMALBUTTON, "Off", null, new GUI_textColor(), textAlign: TextAnchor.MiddleCenter, fixedWidth: 50f)            
        };

        private List<string> textureKeywords = new List<string>()
        {
            "_Illum",
            "_MainTex",
            "_Normal",
            "_BumpMap",
            "_SpecTex",
            "_ScrollTex",
            "_GlowScrollMask"
        };

        void RendererWindow_Awake()
        {
            GuiBase.gUIEventControl += RendererWindow_EventControl;            
        }

        private void RendererWindow_Start()
        {
            _materialInfos = selectedObject.GetRendererInfo();

            undo = IsUndoAvailable();

            RendererWindow_Refresh();
        }

        private void RendererWindow_EventControl(GUI_event guiEvent)
        {
            if (guiEvent.WindowID == 5)
            {
                switch (guiEvent.GroupID)
                {
                    case (int)GroupID.RENDERERSCROLLGROUP:
                        GetSelectedMaterialIndex(guiEvent.ItemID);

                        if (IsTextureItem(guiEvent.ItemID))
                        {
                            SystemFileRequester();
                        }
                        break;

                    case (int)GroupID.RENDERERBUTTONS:

                        switch (guiEvent.ItemID)
                        {
                            case (int)ButtonID.BTN_UNDO:

                                if (undo)
                                {
                                    ResetMaterials();
                                }

                                break;

                            case (int)ButtonID.BTN_RONOFF:
                                Renderer renderer = (Renderer)objects[selected_component];
                                renderer.enabled = !renderer.enabled;
                                GuiBase.GetGroupByID(5, 8).GetContentByItemID(11).SetText(renderer.enabled ? "Off" : "On");                                                               
                                GuiBase.RefreshGroup(5, 8);
                                break;

                        }

                        break;
                }
            }
        }

                     

        private void SystemFileRequester()
        {
            WindowsFileDialog wfd = new WindowsFileDialog();
            wfd.structSize = Marshal.SizeOf(wfd);
            wfd.filter = "All Files\0*.*\0\0";
            wfd.file = new string(new char[256]);
            wfd.maxFile = wfd.file.Length;
            wfd.fileTitle = new string(new char[64]);
            wfd.maxFileTitle = wfd.fileTitle.Length;
            wfd.initialDir = UnityEngine.Application.dataPath;
            wfd.title = "Select Texture Image";
            wfd.defExt = "PNG";
            wfd.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;
            //OFN_EXPLORER|OFN_FILEMUSTEXIST|OFN_PATHMUSTEXIST| OFN_ALLOWMULTISELECT|OFN_NOCHANGEDIR

            if (Dll_Comdlg32.GetOpenFileName(wfd))
            {
                StartCoroutine(WaitToLoad(wfd.file));
                Message(MESSAGE_TEXT[MESSAGES.SELECTED_FILE], wfd.file);
            }
        }

        private IEnumerator WaitToLoad(string fileName)
        {
            //Texture2D texture = ImageUtils.LoadTextureFromFile(fileName);
            Texture2D texture = Texture2D.blackTexture;
            yield return texture;

            string[] fileNameArray = fileName.Split('\\');

            texture.name = fileNameArray.GetLast();

            CreateNewMaterialForObject(texture);
        }

        private bool IsUndoAvailable()
        {
            Renderer renderer = selectedObject.GetComponent<Renderer>();

            return undoDictionary.Keys.Contains(renderer.GetInstanceID());
        }


        private bool GetSelectedMaterialIndex(int scrollIndex)
        {
            string[] splittedItem = _materialInfo_ScrollItems[scrollIndex].Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);

            if (int.TryParse(splittedItem[0], out materialIndex))
                return true;

            return false;
        }


        private bool IsTextureItem(int scrollIndex)
        {
            foreach (string keyword in textureKeywords)
            {
                if (_materialInfo_ScrollItems[scrollIndex].Contains(keyword))
                {
                    string[] splittedItem = _materialInfo_ScrollItems[scrollIndex].Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
                    int.TryParse(splittedItem[1].ToString(), out int sIndex);
                    shaderIndex = sIndex;
                    return true;
                }
            }

            shaderIndex = -1;
            return false;
        }

        private void ResetMaterials()
        {
            Renderer renderer = selectedObject.GetComponent<Renderer>();

            int rendererID = renderer.GetInstanceID();

            Material[] materials = (Material[])undoDictionary[rendererID].Clone();

            renderer.materials = materials;

            Message(MESSAGE_TEXT[MESSAGES.MATERIALS_TO_ORIGINAL]);

            RendererWindow_Start();
        }


        private void CreateNewMaterialForObject(Texture2D newTexture)
        {
            selectedObject.SetActive(false);

            Renderer renderer = selectedObject.GetComponent<Renderer>();

            string shaderKeyword = _materialInfos[materialIndex].ActiveShaders[shaderIndex].Keyword;
            string shaderName = _materialInfos[materialIndex].ActiveShaders[shaderIndex].Name;

            Shader newShader = Shader.Find(shaderName);

            Material newMaterial = new Material(newShader)
            {
                hideFlags = HideFlags.HideAndDontSave,
                shaderKeywords = _materialInfos[materialIndex].Material.shaderKeywords
            };

            newMaterial.SetTexture(Shader.PropertyToID(shaderKeyword), newTexture);
            newMaterial.name = "newMaterial";

            Material[] materials = renderer.materials;

            int rendererID = renderer.GetInstanceID();

            if (!undoDictionary.Keys.Contains(rendererID))
            {
                undoDictionary.Add(objects[selected_component].GetInstanceID(), (Material[])materials.Clone());
            }

            foreach (ShaderInfo shaderInfo in _materialInfos[materialIndex].ActiveShaders)
            {
                if (shaderInfo.Keyword == shaderKeyword)
                {
                    continue;
                }
                else
                {
                    Texture texture = materials[materialIndex].GetTexture(Shader.PropertyToID(shaderInfo.Keyword));
                    newMaterial.SetTexture(Shader.PropertyToID(shaderInfo.Keyword), texture);
                }
            }

            materials.SetValue(newMaterial, materialIndex);

            renderer.materials = materials;

            selectedObject.SetActive(true);

            Message(MESSAGE_TEXT[MESSAGES.NEW_MATERIAL_CREATED]);

            RendererWindow_Awake();
        }

        public void RendererWindow_Refresh()
        {
            _materialInfo_ScrollItems.Clear();

            foreach (MaterialInfo materialInfo in _materialInfos)
            {
                _materialInfo_ScrollItems.Add($"[{materialInfo.Index}] : {materialInfo.Material.name}");

                foreach (ShaderInfo shaderInfo in materialInfo.ActiveShaders)
                {
                    _materialInfo_ScrollItems.Add($"[{materialInfo.Index}][{shaderInfo.Index}]: S: {shaderInfo.Name}, P: {shaderInfo.Keyword}, V: {shaderInfo.KeywordValue}");
                }

                _materialInfo_ScrollItems.Add($"[{materialInfo.Index}] Color: {materialInfo.Material.color}");

                foreach (string shaderKeyword in materialInfo.ShaderKeywords)
                {
                    _materialInfo_ScrollItems.Add($"[{materialInfo.Index}] Shader Keywords: {shaderKeyword}");
                }
            }

            UnityEngine.Object _object = objects[selected_component];

            ObjectProperties _objectProperties = new ObjectProperties(_object);

            _materialInfo_ScrollItems.Add("#Green#Properties:");

            foreach (ObjectProperty objectProperty in _objectProperties)
            {
                _materialInfo_ScrollItems.Add(objectProperty.ToString());
            }

            ObjectFields _objectFields = new ObjectFields(_object);

            _materialInfo_ScrollItems.Add("#Green#Fields:");

            foreach (ObjectField objectField in _objectFields)
            {
                _materialInfo_ScrollItems.Add(objectField.ToString());
            }

            FillGroupContents(ref _materialInfo_ScrollItems, ref RendererScrollContents);

            //GuiBase.SetGroupLabel(2, 4, $"{selectedObject.name} components:");

            GuiBase.RefreshGroup(5, 7);
        }
    }
}
