using AssimilationSoftware.MediaSync.Core.Interfaces;
using AssimilationSoftware.MediaSync.Core.Mappers.Database;
using AssimilationSoftware.MediaSync.Core.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AssimilationSoftware.MediaSync.Core
{
    public class ViewModel : INotifyPropertyChanged
    {
        #region Events
        /// <summary>
        /// Fires when a data-bindable property has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event for a single property.
        /// </summary>
        /// <param name="propertyname">The name of the changed property. Default: caller.</param>
        private void NotifyPropertyChanged([CallerMemberName] string propertyname = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }

        /// <summary>
        /// Raises the PropertyChanged event for a list of properties.
        /// </summary>
        /// <param name="propertynames">The list of property names that have changed.</param>
        private void NotifyPropertiesChanged(params string[] propertynames)
        {
            foreach (string prop in propertynames)
            {
                NotifyPropertyChanged(prop);
            }
        }
        #endregion

        #region Constructors
        public ViewModel(IDataStore datacontext, string machineId)
        {
            _dataContext = datacontext;
            _machineId = machineId;
        }
        #endregion

        #region Methods
        public void CreateProfile(string name, ulong reserve, params string[] ignore)
        {
            if (!_dataContext.GetAllSyncProfile().Select(x => x.Name.ToLower()).Contains(name))
            {
                var profile = new Model.SyncProfile();
                profile.Name = name;
                profile.ReserveSpace = reserve;
                profile.IgnorePatterns = ignore.ToList();
                _dataContext.CreateSyncProfile(profile);
                _dataContext.SaveChanges();
            }
            else
            {
                StatusMessage = "Profile name already exists: " + name;
            }
        }

        public void JoinProfile(string profileName, string localpath, string sharedpath, bool contributor, bool consumer)
        {
            var profile = _dataContext.GetAllSyncProfile().Where(x => x.Name.ToLower() == profileName.ToLower()).First();
            JoinProfile(profile, localpath, sharedpath, contributor, consumer);
        }

        public void JoinProfile(SyncProfile profile, string localpath, string sharedpath, bool contributor, bool consumer)
        {
            if (!profile.ContainsParticipant(this.MachineId))
            {
                profile.Participants.Add(new Repository
                {
                    MachineName = this.MachineId,
                    LocalPath = localpath,
                    SharedPath = sharedpath,
                    Contributor = contributor,
                    Consumer = consumer
                });
                _dataContext.SaveChanges();
            }
        }

        public void LeaveProfile(SyncProfile profile)
        {
            if (profile.ContainsParticipant(this.MachineId))
            {
                profile.Participants.Remove((from p in profile.Participants where p.MachineName == this.MachineId select p).Single());
                _dataContext.SaveChanges();
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// The data context that saves all profile, index and configuration data.
        /// </summary>
        private IDataStore _dataContext;

        private string _machineId;
        public string MachineId
        {
            get
            {
                return _machineId;
            }
            set
            {
                _machineId = value;
                NotifyPropertyChanged();
            }
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get
            {
                return _statusMessage;
            }
            set
            {
                _statusMessage = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        public void SaveChanges()
        {
            _dataContext.SaveChanges();
        }

        public void LeaveProfile(string profileName)
        {
            var profile = _dataContext.GetAllSyncProfile().Where(x => x.Name.ToLower() == profileName.ToLower()).First();
            LeaveProfile(profile);
        }

        public List<string> GetProfileNames(bool p)
        {
            var names = _dataContext.GetAllSyncProfile().Select(x => x.Name).Distinct();
            return names.ToList();
        }

        public List<SyncProfile> Profiles
        {
            get
            {
                return _dataContext.GetAllSyncProfile().ToList();
            }
        }

        public List<Machine> Machines
        {
            get {
                return _dataContext.GetAllMachines().ToList();
            }
        }
    }
}
