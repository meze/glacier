using Aws;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UI.ViewModel;

namespace UI
{
    public class Upload : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        public string File { get; set;  }

        public bool HasVaults
        {
            get
            {
                return  _vaults.Result != null ? _vaults.Result.Count > 0 : false;
            }
        }

        private bool _success = false;
        public bool Success
        {
            get { return _success; }
            set
            {
                _success = value;
                RaisePropertyChanged();
            }
        }

        private int _progress = 0;
        public int Progress
        {
            get { return _progress; }
            set
            {
                _progress = value;
                RaisePropertyChanged();
            }
        }

        private bool _failure = false;
        public bool Failure
        {
            get { return _failure; }
            set
            {
                _failure = value;
                RaisePropertyChanged();
            }
        }
        
        private NotifyTaskCompletion<ObservableCollection<Vault>> _vaults;
        public NotifyTaskCompletion<ObservableCollection<Vault>> Vaults
        {
            get { return _vaults; }
        }

        public Upload(Glacier glacier)
        {
            _vaults = new NotifyTaskCompletion<ObservableCollection<Vault>>(GetVaults(glacier));
            _vaults.PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == "Result")
                {
                    RaisePropertyChanged("HasVaults");
                }
            };
        }

        public async Task<ObservableCollection<Vault>> GetVaults(Glacier glacier)
        {
            var collection = new ObservableCollection<Vault>();
            var vaults = await glacier.GetVaults().ConfigureAwait(continueOnCapturedContext: false);

            foreach (var v in vaults)
            {
                collection.Add(v);
            }

            return collection;
        }

        private void RaisePropertyChanged([CallerMemberName] string name = null)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
