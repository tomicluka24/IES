using FTN.Common;
using NMSTestClientWPF.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMSTestClientWPF.ViewModel
{
    public class GetRelatedValuesViewModel : BindableBase
    {
        public MyICommand ShowCommand { get; set; }

        private ModelResourcesDesc modelResourcesDesc = new ModelResourcesDesc();

        private string textBox = "";
        public string TextBox
        {
            get { return textBox; }
            set
            {
                if (value != textBox)
                {
                    textBox = value;
                    OnPropertyChanged("TextBox");
                }
            }
        }

        private List<long> comboBoxGids = new List<long>();
        public List<long> ComboBoxGids
        {
            get
            {
                return comboBoxGids;
            }
            set
            {
                comboBoxGids = value;
                OnPropertyChanged("ComboBoxGids");
            }

        }

        private long selectedGid;
        public long SelectedGid
        {
            get
            {
                return selectedGid;
            }
            set
            {
                selectedGid = value;
                PropertyIDs.Clear();
                Types.Clear();
                BindList.Clear();
                OnPropertyChanged("SelectedGid");
                OnPropertyChanged("PropertyIDs");
                OnPropertyChanged("Types");
            }
        }


        private ModelCode slectedPropertyID;
        public ModelCode SlectedPropertyID
        {
            get
            {
                return slectedPropertyID;
            }
            set
            {
                slectedPropertyID = value;
                OnPropertyChanged("SlectedPropertyID");
                FindTypes(slectedPropertyID);
                OnPropertyChanged("Types");
            }
        }


        private ObservableCollection<ModelCode> propertyIDs = new ObservableCollection<ModelCode>();
        public ObservableCollection<ModelCode> PropertyIDs
        {
            get
            {
                if (selectedGid != 0)
                {
                    return FindPropertyIDs(selectedGid);
                }
                return null;
            }
            set
            {
                propertyIDs = value;
                OnPropertyChanged("PropertyIDs");
                OnPropertyChanged("Types");
                OnPropertyChanged("BindList");
            }
        }

        private ModelCode type;
        public ModelCode Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
                ImportProperties();
                OnPropertyChanged("Type");
                OnPropertyChanged("BindList");
            }
        }

        private ObservableCollection<ModelCode> types = new ObservableCollection<ModelCode>();
        public ObservableCollection<ModelCode> Types
        {
            get
            {
                return types;
            }
            set
            {
                types = value;
                OnPropertyChanged("Types");
                OnPropertyChanged("BindList");
            }
        }








        private ObservableCollection<Item> bindList = new ObservableCollection<Item>();
        public ObservableCollection<Item> BindList
        {
            get { return bindList; }
            set
            {
                if (value != bindList)
                {
                    bindList = value;
                    OnPropertyChanged("BindList");
                }
            }
        }





        public GetRelatedValuesViewModel()
        {
            ShowCommand = new MyICommand(OnShow);
            ComboBoxGids = Channel.Instance.GdaQueryProxy.GetAllGids();
        }

        public static ObservableCollection<ModelCode> FindPropertyIDs(long selectedGid)
        {
            ModelResourcesDesc rd = new ModelResourcesDesc();
            List<ModelCode> modelCodes = rd.GetAllPropertyIdsForEntityId(selectedGid);
            ObservableCollection<ModelCode> retVal = new ObservableCollection<ModelCode>();

            foreach (ModelCode mc in modelCodes)
            {
                if (Property.GetPropertyType(mc) == PropertyType.Reference || Property.GetPropertyType(mc) == PropertyType.ReferenceVector)
                {
                    retVal.Add(mc);
                }

            }
            return retVal;
        }
        private List<ModelCode> FindTypes(ModelCode kodProp)
        {
            ModelResourcesDesc modResDes = new ModelResourcesDesc();

            string[] props = (kodProp.ToString()).Split('_');
            props[1] = props[1].TrimEnd('S');

            DMSType propertyCode = ModelResourcesDesc.GetTypeFromModelCode(kodProp);

            ModelCode mc;
            ModelCodeHelper.GetModelCodeFromString(propertyCode.ToString(), out mc);

            long systemID = ModelCodeHelper.ExtractEntityIdFromGlobalId(selectedGid);

            foreach (ModelCode modelCode in Enum.GetValues(typeof(ModelCode)))
            {
                if (String.Compare(props[1], "REGULATINGCONDEQ") == 0 && systemID == 1)
                {
                    types = new ObservableCollection<ModelCode>();
                    types.Add(ModelCode.SHUNTCOMP);
                }
                else if (String.Compare(props[1], "REGULATINGCONDEQ") == 0 && systemID == 2)
                {
                    types = new ObservableCollection<ModelCode>();
                    types.Add(ModelCode.STATICVARCOMP);
                }
                else if (String.Compare(props[1], "REGULATINGCONDEQ") == 0 && systemID == 3)
                {
                    types = new ObservableCollection<ModelCode>();
                    types.Add(ModelCode.SYNCMACHINE);
                }
                else if (String.Compare(props[1], "SYN") == 0 && String.Compare(props[2], "MACHINES") == 0)
                {
                    types = new ObservableCollection<ModelCode>();
                    types.Add(ModelCode.SYNCMACHINE);
                }
                else if (String.Compare(props[1], modelCode.ToString()) == 0)
                {
                    DMSType type = ModelCodeHelper.GetTypeFromModelCode(modelCode);
                    if (type == 0)
                    {
                        types = new ObservableCollection<ModelCode>();
                        List<DMSType> r = modResDes.GetLeaves(modelCode);
                        foreach (DMSType ff in r)
                        {
                            types.Add(modResDes.GetModelCodeFromType(ff));
                        }
                    }
                    else
                    {
                        types = new ObservableCollection<ModelCode>();
                        types.Add(modelCode);
                    }
                }
            }

            return new List<ModelCode>();
        }

        private void ImportProperties()
        {
            try
            {
                List<ModelCode> modelCodes = modelResourcesDesc.GetAllPropertyIds(type);
                foreach (var item in modelCodes)
                {
                    var newItem = new Item(item);
                    BindList.Add(newItem);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //        List<ResourceDescription> IteratorNext(int n, int id); - pozivom ove metode klijent
        //        zahteva od servisa da mu prosledi narednih n ResourceDescription-a za upit čiji je identifikator id
        //        bool IteratorRewind(int id); - zahtev servisu da se vrati na početak rezultujućeg niza
        //        int IteratorResourcesTotal(int id); - koliko ukupno postoji ResourceDescription-a u rezultujućem nizu
        //        int IteratorResourcesLeft(int id); - koliko je ostalo nepročitanih ResourceDescription-a
        //        bool IteratorClose(int id); - oslobađanje resursa, poziva se kada se završi rad nad upitom čiji je identifikator id

        private void OnShow()
        {
            TextBox = string.Empty;
            try
            {
                List<ModelCode> modelCodes = new List<ModelCode>();

                foreach (var item in BindList)
                {
                    if (item.Checked)
                        modelCodes.Add(item.Code);
                }

                int iteratorId = Channel.Instance.GdaQueryProxy.GetExtentValues(type, modelCodes);
                int resourcesLeft = Channel.Instance.GdaQueryProxy.IteratorResourcesLeft(iteratorId);

                List<ResourceDescription> myResourceDesc = new List<ResourceDescription>();
                int numberOfResources = 2;

                while (resourcesLeft > 0)
                {
                    List<ResourceDescription> rds = Channel.Instance.GdaQueryProxy.IteratorNext(numberOfResources, iteratorId);

                    for (int i = 0; i < rds.Count; i++)
                        myResourceDesc.Add(rds[i]);

                    resourcesLeft = Channel.Instance.GdaQueryProxy.IteratorResourcesLeft(iteratorId);
                }

                Channel.Instance.GdaQueryProxy.IteratorClose(iteratorId);

                string retVal = string.Empty;

                foreach (var rd in myResourceDesc)
                {
                    retVal += type.ToString() + "\n";

                    foreach (var item in rd.Properties)
                    {
                        if (item.Type == PropertyType.ReferenceVector)
                        {
                            List<long> gids = item.AsReferences();
                            if (gids.Count <= 0)
                                continue;

                            retVal += item.Id.ToString() + " : ";
                            foreach (var gid in gids)
                            {
                                //s += $"{gid.ToString("X")}, "; sasece ga iz nekog razloga..
                                retVal += (String.Format("0x{0:x16}, ", gid)); // from GetValues metode
                            }
                            retVal += "\n";
                        }
                        else if (item.Type == PropertyType.Reference)
                        {
                            retVal += item.Id.ToString() + " : ";
                            retVal += (String.Format("0x{0:x16}\n", item.GetValue())); // from GetValues metode
                        }
                        else if (item.Type == PropertyType.Enum)
                        {
                            switch (item.Id)
                            {
                                case ModelCode.REGCONTROL_MODE:
                                    retVal += item.Id.ToString() + " : " + "MODE" + "\n";
                                    break;
                                case ModelCode.REGCONTROL_MONITOREDPAHSE:
                                    retVal += item.Id.ToString() + " : " + "MONITOREDPAHSE" + "\n";
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            retVal += item.Id.ToString() + " : " + item.GetValue().ToString() + "\n";
                        }
                    }
                }

                TextBox += retVal;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.ToString()}");
            }
        }
    }
}
