using System.ComponentModel;
using System.Runtime.CompilerServices;
using AssimilationSoftware.MediaSync.WpfGui.Annotations;

namespace AssimilationSoftware.MediaSync.WpfGui
{
    public class SyncResultViewModel : INotifyPropertyChanged
    {
        private int _copiedToPrimary;
        private int _copiedFromShared;
        private int _unchanged;
        private int _conflicted;
        private int _deletedLocal;
        private int _deletedToPrimary;
        private string _syncSetName;

        public SyncResultViewModel(string syncSetName)
        {
            SyncSetName = syncSetName;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string SyncSetName
        {
            get => _syncSetName;
            set
            {
                if (value == _syncSetName) return;
                _syncSetName = value;
                OnPropertyChanged();
            }
        }

        public int CopiedToPrimary
        {
            get => _copiedToPrimary;
            set
            {
                if (value == _copiedToPrimary) return;
                _copiedToPrimary = value;
                OnPropertyChanged();
            }
        }

        public int CopiedFromShared
        {
            get => _copiedFromShared;
            set
            {
                if (value == _copiedFromShared) return;
                _copiedFromShared = value;
                OnPropertyChanged();
            }
        }

        public int DeletedToPrimary
        {
            get => _deletedToPrimary;
            set
            {
                if (value == _deletedToPrimary) return;
                _deletedToPrimary = value;
                OnPropertyChanged();
            }
        }

        public int DeletedLocal
        {
            get => _deletedLocal;
            set
            {
                if (value == _deletedLocal) return;
                _deletedLocal = value;
                OnPropertyChanged();
            }
        }

        public int Conflicted
        {
            get => _conflicted;
            set
            {
                if (value == _conflicted) return;
                _conflicted = value;
                OnPropertyChanged();
            }
        }

        public int Unchanged
        {
            get => _unchanged;
            set
            {
                if (value == _unchanged) return;
                _unchanged = value;
                OnPropertyChanged();
            }
        }
    }
}
