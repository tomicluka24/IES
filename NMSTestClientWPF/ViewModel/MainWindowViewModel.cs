using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMSTestClientWPF.ViewModel
{
    public class MainWindowViewModel : BindableBase
    {
        private GetValuesViewModel getValuesViewModel;
        private GetExtentValuesViewModel getExtentValuesViewModel;
        private GetRelatedValuesViewModel getRelatedValuesViewModel;
        public MyICommand<string> NavCommand { get; private set; }

        private BindableBase currentViewModel;
        public BindableBase CurrentViewModel
        {
            get { return currentViewModel; }
            set
            {
                SetProperty(ref currentViewModel, value);
            }
        }


        public MainWindowViewModel()
        {
            getValuesViewModel = new GetValuesViewModel();
            getExtentValuesViewModel = new GetExtentValuesViewModel();
            getRelatedValuesViewModel = new GetRelatedValuesViewModel();
            NavCommand = new MyICommand<string>(OnNav);
            currentViewModel = getValuesViewModel;
        }

        public void OnNav(string arg)
        {
            switch (arg)
            {
                case "1": CurrentViewModel = getValuesViewModel; break;
                case "2": CurrentViewModel = getExtentValuesViewModel; break;
                case "3": CurrentViewModel = getRelatedValuesViewModel; break;
                default:
                    break;
            }
        }
    }
}
