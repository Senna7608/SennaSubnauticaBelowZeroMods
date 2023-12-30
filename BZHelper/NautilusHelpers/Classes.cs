using System.Collections.Generic;
using UnityEngine;

namespace BZHelper.NautilusHelpers
{
#pragma warning disable CS1591 // Missing XML documentation

    public class TabNode
    {
        public readonly CraftTree.Type craftTree;
        public readonly string uniqueName;
        public readonly string displayName;
        public readonly Sprite sprite;

        public TabNode(CraftTree.Type craftTree, string uniqueName, string displayName, Sprite sprite)
        {
            this.craftTree = craftTree;
            this.uniqueName = uniqueName;
            this.displayName = displayName;
            this.sprite = sprite;
        }
    }

    public class CraftTreeType
    {
        public CraftTreeType(CraftTree.Type craftTreeType, string[] stepsToTab)
        {
            TreeType = craftTreeType;
            StepsToTab = stepsToTab;
        }

        public CraftTree.Type TreeType { get; }
        public string[] StepsToTab { get; }
    }

    public class CrafTreeTypesData
    {
        public List<CraftTreeType> TreeTypes;

        public CrafTreeTypesData() { }

        public CrafTreeTypesData(List<CraftTreeType> treeTypes)
        {
            TreeTypes = treeTypes;
        }
    }

    public class EncyData
    {
        public string title;
        public string description;
        public EncyNode node;
        public Texture2D image;

        public EncyData()
        {
        }

        public EncyData(string encyTitle, string encyText, EncyNode encyNode, Texture2D encyPic)
        {
            title = encyTitle;
            description = encyText;
            node = encyNode;
            image = encyPic;
        }
    }
}
