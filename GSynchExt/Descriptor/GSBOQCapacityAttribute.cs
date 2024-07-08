using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.Common.Tools;
using PX.Objects.CS;
using PX.Objects.IN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using PX.Common;
using PX.Api;
using GSynchExt;
using static PX.Objects.SO.SOBehavior;

namespace GSynchExt
{
    public class GSBOQCapacityAttribute : PXUIFieldAttribute
    {
        private string _p2;
        private System.Type _fg;

        public GSBOQCapacityAttribute(string fieldName, System.Type bOQID)
        {

            _p2 = fieldName;
            _fg = bOQID;
            
        }
        public override void FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
        {
            if (sender == null) return;
            if (_fg == null) return;
            
            PXCache parentCache = sender.Graph.Caches[BqlCommand.GetItemType(_fg)];
            GSBOQ boq = (GSBOQ)parentCache.Current;
            if (parentCache == null) return;
            if(boq == null) return;
            var prefix = _p2.Remove(_p2.Length - 5, 5); // EPC Est. Qty
            var suffix = _p2.Remove(0, _p2.Length - 5); // Type5


            if (e.ReturnState != null && e.ReturnState is PXFieldState state)
            {

                if (suffix == "Type1" && boq.CapacityType1 != null)
                {

                    _DisplayName = prefix + " " + boq.CapacityType1;
                    _Visibility = PXUIVisibility.Visible;
                    _Visible = true;

                }
                else if (suffix == "Type2" && boq.CapacityType2 != null)
                {

                    _DisplayName = prefix + boq.CapacityType2;
                    _Visibility = PXUIVisibility.Visible;
                    _Visible = true;

                }
                else if (suffix == "Type3" && boq.CapacityType3 != null)
                {

                    _DisplayName = prefix + boq.CapacityType3;
                    _Visibility = PXUIVisibility.Visible;
                    _Visible = true;

                }

                else if (suffix == "Type4" && boq.CapacityType4 != null)
                {

                    _DisplayName = prefix + boq.CapacityType4;
                    _Visibility = PXUIVisibility.Visible;
                    _Visible = true;

                }
                else if (suffix == "Type5" && boq.CapacityType5 != null)
                {

                    _DisplayName = prefix + boq.CapacityType5;
                    _Visibility = PXUIVisibility.Visible;
                    _Visible = true;

                }
                else
                {
                    _DisplayName = _p2;
                    _Visibility = PXUIVisibility.Invisible;
                    _Visible = false;
                }

                state.DisplayName = _DisplayName;
                state.Visibility = _Visibility;
                state.Visible = _Visible;
                e.ReturnState = state;
            }

        }      
    }
}

