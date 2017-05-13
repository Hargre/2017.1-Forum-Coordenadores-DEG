using ForumDEG.Interfaces;
using ForumDEG.Models;
using ForumDEG.Utils;
using ForumDEG.Views;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ForumDEG.ViewModels {
    public class UsersViewModel : BaseViewModel {
        public ObservableCollection<UserDetailViewModel> Administrators { get; private set; }
        public ObservableCollection<UserDetailViewModel> Coordinators { get; private set; }

        private UserDetailViewModel _selectedAdministrator;
        public UserDetailViewModel SelectedAdministrator {
            get { return _selectedAdministrator; }
            set { SetValue(ref _selectedAdministrator, value); }
        }

        private UserDetailViewModel _selectedCoordinator;
        public UserDetailViewModel SelectedCoordinator {
            get { return _selectedCoordinator; }
            set { SetValue(ref _selectedCoordinator, value); }
        }

        private readonly IPageService _pageService;

        public ICommand SelectAdministratorCommand { get; private set; }
        public ICommand SelectCoordinatorCommand { get; private set; }

        private static UsersViewModel _instance = null;

        private UsersViewModel(IPageService pageService) {
            _pageService = pageService;
            SelectAdministratorCommand = new Command<UserDetailViewModel>(async vm => await SelectAdministrator(vm));
            SelectCoordinatorCommand = new Command<UserDetailViewModel>(async vm => await SelectCoordinator(vm));
        }

        public static UsersViewModel GetInstance() {
            if (_instance == null) _instance = new UsersViewModel(new PageService());
            return _instance;
        }

        private async Task SelectAdministrator(UserDetailViewModel administrator) {
            if (administrator == null)
                return;
            SelectedAdministrator = administrator;
            await _pageService.PushAsync(new UserDetailPage(administrator));
        }

        private async Task SelectCoordinator(UserDetailViewModel coordinator) {
            if (coordinator == null)
                return;
            SelectedCoordinator = coordinator;
            await _pageService.PushAsync(new UserDetailPage(coordinator));
        }

        public void UpdateUsersList() {
            Administrators = new ObservableCollection<UserDetailViewModel>();
            Coordinators = new ObservableCollection<UserDetailViewModel>();

            Task<List<Administrator>> administratorslisttask = AdministratorDatabase.getAdmDB.GetAll();
            administratorslisttask.Wait();

            List<Administrator> administratorslist = administratorslisttask.Result;

            Task<List<Coordinator>> coordinatorslisttask = CoordinatorDatabase.getCoordinatorDB.GetAll();
            coordinatorslisttask.Wait();

            List<Coordinator> coordinatorslist = coordinatorslisttask.Result;

            foreach (Coordinator coordinator in coordinatorslist) {
                Coordinators.Add(new UserDetailViewModel {
                    Name = coordinator.Name,
                    Id = coordinator.Id,
                    Email = coordinator.Email,
                    Registration = coordinator.Registration,
                    Course = coordinator.Course,
                    IsAdministrator = false,
                    IsCoordinator = true
                });
            }

            foreach (Administrator administrator in administratorslist) {
                Administrators.Add(new UserDetailViewModel {
                    Name = administrator.Name,
                    Id = administrator.Id,
                    Email = administrator.Email,
                    Registration = administrator.Registration,
                    IsAdministrator = true,
                    IsCoordinator = false
                });
            }
        }
    }
}
