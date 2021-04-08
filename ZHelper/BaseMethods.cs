using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZHelper.Components;
using static ZHelper.Helpers.SceneHelper;

namespace ZHelper
{
    public partial class Zhelper
    {
        private void GetRoots()
        {
            GetRootTransforms(ref TRANSFORMS);

            TRANSFORMS.Sort(TransformsHelper.SortByName);

            guiItems_transforms.SetScrollViewItems(TRANSFORMS.InitTransformNamesList(), 278f);

            RefreshTransformIndex();

            UpdateVisuals();

            OutputWindow_Log(MESSAGE_TEXT[MESSAGES.GET_ROOTS]);

            isRootList = true;

            SetDirty(false);
        }








    }
}
