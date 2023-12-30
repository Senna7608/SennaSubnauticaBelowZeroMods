using UnityEngine;

namespace BZCommon.Helpers.RuntimeGUI
{
    public class GUI_scrollView : GUI_group
    {
        public GUI_scrollView
            (                   
            GUI_window guiWindow,
            Group group,
            Rect drawingArea,
            GUIEventHandler eventHandler                      
            ) : base(guiWindow, group, drawingArea, eventHandler)
        {             
            _maxShowItems = group.maxShowItems;
            _verticalSpace = 2;
            _baseItemCount = _guiItems.Count;

            CalculateClientRect();

            CalculateGroupGrid(_clientRect);

            CreateGroup();
        }
                
        private int _maxShowItems;
        private Vector2 scrollPos;
        private Rect _clientRect;        
        private Rect _drawRect;
        private bool _autoScroll = false;
        private int _baseItemCount;
        //private Rect _borderRect;

        private void CalculateClientRect()
        {
            _drawRect = new Rect(_drawingArea);            

            if (_groupLabel != string.Empty)
            {
                _groupLabelRect = new Rect(_drawRect.x + _group.horizontalSpace, _drawRect.y, _drawRect.width - (_group.horizontalSpace * 2), _group.itemHeight);

                _drawRect.y = _drawRect.y + (_group.itemHeight - (_verticalSpace / 2));                
            }

            if (_maxShowItems == 0)
            {
                _maxShowItems = (int)(_drawRect.height / (_group.itemHeight + _verticalSpace));
            }

            int totalRows = _group.GetTotalRows();

            //if (_maxShowItems < totalRows)
            //{
                _drawRect.height = _maxShowItems * (_group.itemHeight + _verticalSpace);
                _drawRect.width -= _group.horizontalSpace;
                _clientRect = new Rect(0, 0, _drawRect.width - 20, totalRows * (_group.itemHeight + _verticalSpace));
            //}
            //else
            //{
            //_drawRect.height = _maxShowItems * (_group.itemHeight + _verticalSpace);
            //_clientRect = new Rect(0, 0, _drawRect.width, totalRows * (_group.itemHeight + _verticalSpace));
            //}

            //_borderRect = new Rect(_drawRect.x, _drawRect.y - 2, _drawRect.width - (_group.horizontalSpace * 4), _drawRect.height + 4);

            float nextYpos = _drawRect.y + _drawRect.height;

            _guiWindow.RemainDrawableArea = new Rect(_drawingArea.x, nextYpos, _drawingArea.width, _guiWindow.WindowRect.height - (_drawRect.height + _group.itemHeight));                        
        }
        
        public override void DrawGroup()
        {
            if (_groupLabel != string.Empty)
            {
                GUI.Label(_groupLabelRect, _groupLabel, GUI_style.GetGuiStyle(GUI_Item_Type.LABEL, align: TextAnchor.MiddleLeft, colorNormal: GUI_Color.White));
            }

            //GUI.Box(_borderRect, "");

            scrollPos = GUI.BeginScrollView(_drawRect, scrollPos, _clientRect);

            base.DrawGroup();

            if (_autoScroll && _baseItemCount != _guiItems.Count)
            {
                scrollPos.y += Mathf.Infinity;
                _baseItemCount = _guiItems.Count;
            }

            GUI.EndScrollView();
        }

        public override void Refresh()
        {
            isRefresh = true;

            CalculateClientRect();            

            CalculateGroupGrid(_clientRect);

            CreateGroup();

            isRefresh = false;
        }

        public override void SetAutoScroll(bool value)
        {
            _autoScroll = value;
        }

        public override void IncrementClientHeight(float value)
        {
            _clientRect.Set(_clientRect.x, _clientRect.y, _clientRect.width, _clientRect.height + value);
        }
    }
}
