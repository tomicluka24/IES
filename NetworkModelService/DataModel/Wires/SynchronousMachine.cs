using FTN.Services.NetworkModelService.DataModel.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FTN.Common;

namespace FTN.Services.NetworkModelService.DataModel.Wires
{
    public class SynchronousMachine : RotatingMachine
    {
        private long reactiveCapabilityCurve = 0;

        public SynchronousMachine(long globalId)
            : base(globalId)
        {
        }

        public long ReactiveCapabilityCurve
        {
            get
            {
                return reactiveCapabilityCurve;
            }

            set
            {
                reactiveCapabilityCurve = value;
            }
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                SynchronousMachine x = (SynchronousMachine)obj;
                return (x.reactiveCapabilityCurve == this.reactiveCapabilityCurve);
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

        public override bool HasProperty(ModelCode property)
        {
            switch (property)
            {
                case ModelCode.SYNCMACHINE_REACTIVECAPCURVE:
                    return true;

                default:
                    return base.HasProperty(property);
            }
        }

        public override void GetProperty(Property property)
        {
            switch (property.Id)
            {

                case ModelCode.SYNCMACHINE_REACTIVECAPCURVE:
                    property.SetValue(reactiveCapabilityCurve);
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

                case ModelCode.SYNCMACHINE_REACTIVECAPCURVE:
                    reactiveCapabilityCurve = property.AsReference();
                    break;

                default:
                    base.SetProperty(property);
                    break;
            }
        }

        #endregion IAccess implementation

        #region IReference implementation

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            if (reactiveCapabilityCurve != 0 && (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
            {
                references[ModelCode.SYNCMACHINE_REACTIVECAPCURVE] = new List<long>();
                references[ModelCode.SYNCMACHINE_REACTIVECAPCURVE].Add(reactiveCapabilityCurve);
            }

            base.GetReferences(references, refType);
        }

        #endregion IReference implementation		
    }
}
