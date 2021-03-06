﻿using ForumDEG.Interfaces;
using ForumDEG.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ForumDEG.ViewModels {
    public class FormsViewModel : BaseViewModel {
        public ObservableCollection<FormDetailViewModel> Forms { get; private set; }

        private FormDetailViewModel _selectedForm;
        public FormDetailViewModel SelectedForm {
            get { return _selectedForm; }
            set { SetValue(ref _selectedForm, value); }
        }

        private bool _formVisibility;
        public bool FormVisibility {
            get {
                return _formVisibility;
            }
            set {
                if (_formVisibility != value) {
                    _formVisibility = value;

                    OnPropertyChanged("FormVisibility");
                }
            }
        }

        private bool _noFormWarning;
        public bool NoFormWarning {
            get {
                return _noFormWarning;
            }
            set {
                if (_noFormWarning != value) {
                    _noFormWarning = value;

                    OnPropertyChanged("NoFormWarning");
                }
            }
        }

        private bool _activityIndicator;
        public bool ActivityIndicator {
            get {
                return _activityIndicator;
            }
            set {
                if (_activityIndicator != value) {
                    _activityIndicator = value;

                    OnPropertyChanged("ActivityIndicator");
                }
            }
        }

        private readonly IPageService _pageService;
        private readonly Helpers.Form _formService;

        public ICommand SelectFormCommand { get; private set; }

        private static FormsViewModel _instance = null;
        public FormsViewModel(IPageService pageService) {
            _pageService = pageService;
            _formService = new Helpers.Form();
            ActivityIndicator = false;
            SelectFormCommand = new Command<FormDetailViewModel>(async vm => await SelectForm(vm));
            FormVisibility = true;
            NoFormWarning = false;
        }

        public static FormsViewModel GetInstance() {
            if (_instance == null)
                _instance = new FormsViewModel(new PageService());
            return _instance;
        }

        public async Task SelectForm(FormDetailViewModel form) {
            if (form == null)
                return;
            SelectedForm = form;
            await _pageService.PushAsync(new Views.Forms.FormDetailPage(SelectedForm));
        }

        public async void UpdateFormsList() {
            ActivityIndicator = true;
            Forms = new ObservableCollection<FormDetailViewModel>();
            try {
                var formsList = await _formService.GetFormsAsync();

                foreach (Form _form in formsList) {
                    var formViewModel = new FormDetailViewModel(new PageService()) {
                        Id = _form.Id,
                        RemoteId = _form.RemoteId,
                        Title = _form.Title,
                        DiscursiveQuestions = _form.DiscursiveQuestions,
                        MultipleChoiceQuestions = _form.MultipleChoiceQuestions
                    };
                    ActivityIndicator = false;
                    formViewModel.SplitMultipleChoiceQuestions();
                    Forms.Add(formViewModel);
                }

                if(formsList.Count == 0) {
                    ActivityIndicator = false;
                    FormVisibility = false;
                    NoFormWarning = true;
                }
                else {
                    ActivityIndicator = false;
                    FormVisibility = true;
                    NoFormWarning = false;
                }
            }
            catch (Exception ex) {
                ActivityIndicator = false;
                Debug.WriteLine("[Update forms list] " + ex.Message + "\n" + ex.StackTrace);
                await _pageService.DisplayAlert("Falha ao carregar formulários",
                                          "Houve um erro ao estabelecer conexão com o servidor. Por favor, tente novamente.",
                                          null, "OK");
                await _pageService.PopAsync();
            }
        }
    }
}
