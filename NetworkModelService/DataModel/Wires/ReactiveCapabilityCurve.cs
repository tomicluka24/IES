using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using FTN.Common;

namespace FTN.Services.NetworkModelService.DataModel.Core
{
    public class ReactiveCapabilityCurve : Curve
    {
        private List<long> synchronousMachines = new List<long>();

        public ReactiveCapabilityCurve(long globalId) : base(globalId)
        {
        }

        public List<long> SynchronousMachines
        {
            get
            {
                return synchronousMachines;
            }

            set
            {
                synchronousMachines = value;
            }
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                ReactiveCapabilityCurve x = (ReactiveCapabilityCurve)obj;
                return (CompareHelper.CompareLists(x.synchronousMachines, this.synchronousMachines, true));
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
                case ModelCode.REACTIVECAPCURVE_SYN_MACHINES:
                    return true;

                default:
                    return base.HasProperty(t);
            }
        }

        public override void GetProperty(Property prop)
        {
            switch (prop.Id)
            {
                case ModelCode.REACTIVECAPCURVE_SYN_MACHINES:
                    prop.SetValue(synchronousMachines);
                    break;

                default:
                    base.GetProperty(prop);
                    break;
            }
        }

        public override void SetProperty(Property property)
        {
            base.SetProperty(property);
        }

        #endregion IAccess implementation

        #region IReference implementation

        public override bool IsReferenced
        {
            get
            {
                return synchronousMachines.Count > 0 || base.IsReferenced;
            }
        }

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            if (synchronousMachines != null && synchronousMachines.Count > 0 && (refType == TypeOfReference.Target || refType == TypeOfReference.Both))
            {
                references[ModelCode.REACTIVECAPCURVE_SYN_MACHINES] = synchronousMachines.GetRange(0, synchronousMachines.Count);
            }

            base.GetReferences(references, refType);
        }

        public override void AddReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.SYNCMACHINE_REACTIVECAPCURVE:
                    synchronousMachines.Add(globalId);
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
                case ModelCode.SYNCMACHINE_REACTIVECAPCURVE:

                    if (synchronousMachines.Contains(globalId))
                    {
                        synchronousMachines.Remove(globalId);
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