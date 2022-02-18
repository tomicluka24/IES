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
    public class GetExtentValuesViewModel : BindableBase
    {
        public MyICommand ShowCommand { get; set; }

        private ModelResourcesDesc modelResourcesDesc = new ModelResourcesDesc();

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

        private ModelCode modelCode;
        public ModelCode ModelCode
        {
            get { return modelCode; }
            set
            {
                if (modelCode != value)
                {
                    modelCode = value;
                    OnPropertyChanged("ModelCode");
                    BindList.Clear();
                    ImportProperties();
                }
            }
        }

        private ObservableCollection<ModelCode> comboBoxModelCode;
        public ObservableCollection<ModelCode> ComboBoxModelCode
        {
            get { return comboBoxModelCode; }
            set
            {
                comboBoxModelCode = value;
                OnPropertyChanged("ComboBoxModelCode");
            }
        }


        public GetExtentValuesViewModel()
        {
            ShowCommand = new MyICommand(OnShow);
            ComboBoxModelCode = PopulateModelCodes();
        }

        public static List<ModelCode> ImportModelCodes(DMSType dmsType)
        {
            ModelResourcesDesc rd = new ModelResourcesDesc();
            List<ModelCode> modelCodesList = rd.GetAllPropertyIds(dmsType);

            return modelCodesList;
        }

        public static ObservableCollection<ModelCode> PopulateModelCodes()
        {
            var list = new ObservableCollection<ModelCode>
            {
                FTN.Common.ModelCode.TERMINAL,
                FTN.Common.ModelCode.REGCONTROL,
                FTN.Common.ModelCode.REACTIVECAPCURVE,
                FTN.Common.ModelCode.SYNCMACHINE,
                FTN.Common.ModelCode.SHUNTCOMP,
                FTN.Common.ModelCode.STATICVARCOMP,
                FTN.Common.ModelCode.CONTROL
            };

            return list;
        }

        private void ImportProperties()
        {
            try
            {
                List<ModelCode> modelCodes = modelResourcesDesc.GetAllPropertyIds(modelCode);
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

                int iteratorId = Channel.Instance.GdaQueryProxy.GetExtentValues(modelCode, modelCodes);
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
                    retVal += modelCode.ToString() + "\n";

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
