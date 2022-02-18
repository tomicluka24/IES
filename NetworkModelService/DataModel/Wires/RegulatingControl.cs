﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;

namespace FTN.Services.NetworkModelService.DataModel.Wires
{
    public class RegulatingControl : PowerSystemResource
    {

        private bool discrete;

        private RegulatingControlModeKind mode;

        private PhaseCode monitoredPhase;

        private float targetRange;

        private float targetValue;


        private long terminal = 0;

        private List<long> regulatingCondEqs = new List<long>();

        public RegulatingControl(long globalId)
            : base(globalId)
        {
        }
        public bool Discrete
        {
            get
            {
                return discrete;
            }

            set
            {
                discrete = value;
            }
        }
        public RegulatingControlModeKind Mode
        {
            get
            {
                return mode;
            }

            set
            {
                mode = value;
            }
        }
        public PhaseCode MonitoredPhase
        {
            get
            {
                return monitoredPhase;
            }

            set
            {
                monitoredPhase = value;
            }
        }
        public float TargetRange
        {
            get
            {
                return targetRange;
            }

            set
            {
                targetRange = value;
            }
        }
        public float TargetValue
        {
            get
            {
                return targetValue;
            }

            set
            {
                targetValue = value;
            }
        }
        public long Terminal
        {
            get { return terminal; }
            set { terminal = value; }
        }

        public List<long> RegulatingCondEqs
        {
            get { return regulatingCondEqs; }
            set { regulatingCondEqs = value; }
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                RegulatingControl x = (RegulatingControl)obj;
                return (x.discrete == this.discrete &&
                        x.mode == this.mode &&
                        x.monitoredPhase == this.monitoredPhase &&
                        x.targetRange == this.targetRange &&
                        x.targetValue == this.targetValue &&
                        x.terminal == this.terminal &&
                        CompareHelper.CompareLists(x.regulatingCondEqs, this.regulatingCondEqs));
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #region IAccess implementation

        public override bool HasProperty(ModelCode t)
        {
            switch (t)
            {
                case ModelCode.REGCONTROL_DISCRETE:
                case ModelCode.REGCONTROL_MODE:
                case ModelCode.REGCONTROL_MONITOREDPAHSE:
                case ModelCode.REGCONTROL_TARGET_RANGE:
                case ModelCode.REGCONTROL_TARGET_VALUE:
                case ModelCode.REGCONTROL_TERMINAL:
                case ModelCode.REGCONTROL_REGCONDEQS:
                    return true;

                default:
                    return base.HasProperty(t);

            }
        }

        public override void GetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.REGCONTROL_DISCRETE:
                    property.SetValue(discrete);
                    break;

                case ModelCode.REGCONTROL_MODE:
                    property.SetValue((short)mode);
                    break;

                case ModelCode.REGCONTROL_MONITOREDPAHSE:
                    property.SetValue((short)monitoredPhase);
                    break;

                case ModelCode.REGCONTROL_TARGET_RANGE:
                    property.SetValue(targetRange);
                    break;

                case ModelCode.REGCONTROL_TARGET_VALUE:
                    property.SetValue(targetValue);
                    break;

                case ModelCode.REGCONTROL_TERMINAL:
                    property.SetValue(terminal);
                    break;

                case ModelCode.REGCONTROL_REGCONDEQS:
                    property.SetValue(regulatingCondEqs);
                    break;

                default:
                    base.GetProperty(property);
                    break;
            }
        }

        public override void SetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.REGCONTROL_DISCRETE:
                    discrete = property.AsBool();
                    break;

                case ModelCode.REGCONTROL_MODE:
                    mode = (RegulatingControlModeKind)property.AsEnum();
                    break;

                case ModelCode.REGCONTROL_MONITOREDPAHSE:
                    monitoredPhase = (PhaseCode)property.AsEnum();
                    break;

                case ModelCode.REGCONTROL_TARGET_RANGE:
                    targetRange = property.AsFloat();
                    break;


                case ModelCode.REGCONTROL_TARGET_VALUE:
                    targetValue = property.AsFloat();
                    break;

                case ModelCode.REGCONTROL_TERMINAL:
                    terminal = property.AsReference();
                    break;

                default:
                    base.SetProperty(property);
                    break;
            }
        }

        #endregion IAccess implementation

        #region IReference implementation

        public override bool IsReferenced
        {
            get
            {
                return regulatingCondEqs.Count != 0 || base.IsReferenced;
            }
        }

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            if (terminal != 0 && (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
            {
                references[ModelCode.REGCONTROL_TERMINAL] = new List<long>();
                references[ModelCode.REGCONTROL_TERMINAL].Add(terminal);
            }

            if (regulatingCondEqs != null && regulatingCondEqs.Count != 0 && (refType == TypeOfReference.Target || refType == TypeOfReference.Both))
            {
                references[ModelCode.REGCONTROL_REGCONDEQS] = regulatingCondEqs.GetRange(0, regulatingCondEqs.Count);
            }

            base.GetReferences(references, refType);
        }

        public override void AddReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.REGCONDEQ_REGCONTROL:
                    regulatingCondEqs.Add(globalId);
                    break;

                default:
                    base.AddReference(referenceId, globalId);
                    break;
            }
        }

        public override void RemoveReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.REGCONDEQ_REGCONTROL:

                    if (regulatingCondEqs.Contains(globalId))
                    {
                        regulatingCondEqs.Remove(globalId);
                    }
                    else
                    {
                        CommonTrace.WriteTrace(CommonTrace.TraceWarning, "Entity (GID = 0x{0:x16}) doesn't contain reference 0x{1:x16}.", this.GlobalId, globalId);
                    }

                    break;

                default:
                    base.RemoveReference(referenceId, globalId);
                    break;
            }
        }

        #endregion IReference implementation
    }
}
