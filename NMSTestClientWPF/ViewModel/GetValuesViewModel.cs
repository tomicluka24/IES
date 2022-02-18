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
    public class GetValuesViewModel : BindableBase
    {
        public MyICommand ShowCommand { get; set; }
        public MyICommand AddCommand { get; private set; }

        private ModelResourcesDesc modelResourcesDesc = new ModelResourcesDesc();

        private ObservableCollection<Item> bindList = new ObservableCollection<Item>();

        public GetValuesViewModel()
        {
            ShowCommand = new MyICommand(OnShow);
            ComboBoxGid = Channel.Instance.GdaQueryProxy.GetAllGids();
        }

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

        // TextBox property for binding to TextBox element in View
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

        private string gid;
        public string Gid
        {
            get { return gid; }
            set
            {
                if (gid != value)
                {
                    gid = value;
                    OnPropertyChanged("Gid");
                    BindList.Clear();
                    ImportProperties();
                }
            }
        }

        private List<long> comboBoxGid = new List<long>();
        public List<long> ComboBoxGid
        {
            get
            {
                return comboBoxGid;
            }
            set
            {
                comboBoxGid = value;
                OnPropertyChanged("ComboBoxGid");
            }
        }

        

        private void ImportProperties()
        {
            try
            {
                long GID = 0;
                string hexGid = gid;
                if (hexGid.StartsWith("0x", StringComparison.Ordinal))
                {
                    hexGid = gid.Remove(0, 2);

                    GID = Convert.ToInt64(Int64.Parse(hexGid, System.Globalization.NumberStyles.HexNumber));
                }
                else
                {
                    GID = Int64.Parse(gid);
                }
                short type = ModelCodeHelper.ExtractTypeFromGlobalId(GID);
                List<ModelCode> codes = modelResourcesDesc.GetAllPropertyIds((DMSType)type);
                foreach (var item in codes)
                {
                    BindList.Add(new Item(item));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.ToString()}");
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
            try
            {
                long GID = 0; // Globalni identifikator je 64 bitni, long => 64 bit
                string hexGid = gid;
                if (hexGid.StartsWith("0x", StringComparison.Ordinal))
                {
                    hexGid = gid.Remove(0, 2);

                    GID = Convert.ToInt64(Int64.Parse(hexGid, System.Globalization.NumberStyles.HexNumber));
                }
                else
                {
                    GID = Int64.Parse(hexGid);
                }
                short type = ModelCodeHelper.ExtractTypeFromGlobalId(GID);

                List<ModelCode> properties = modelResourcesDesc.GetAllPropertyIds((DMSType)type);
                ResourceDescription rd = null;

                List<ModelCode> modelCodes = new List<ModelCode>();
                foreach (var item in BindList)
                {
                    if (item.Checked)
                        modelCodes.Add(item.Code);
                }
                rd = Channel.Instance.GdaQueryProxy.GetValues(GID, modelCodes);
                TextBox = ((DMSType)type).ToString() + "\n";
                string retVal = string.Empty;
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
                TextBox += retVal;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.ToString()}");
            }
        }

    }
}
