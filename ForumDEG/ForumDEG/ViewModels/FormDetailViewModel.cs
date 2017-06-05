using ForumDEG.Interfaces;
using ForumDEG.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ForumDEG.ViewModels {
    public class FormDetailViewModel {
        private IPageService _pageService;

        public int Id { get; set; }
        public string RemoteId { get; set; }
        public string Title { get; set; }

        public int QuestionsAmount {
            get { return (DiscursiveQuestions.Count + MultipleChoiceQuestions.Count); }
        }

        public ICommand CancelCommand { get; set; }
        public ICommand SubmitCommand { get; set; }

        public List<Models.DiscursiveQuestion> DiscursiveQuestions { get; set; }
        public List<Models.MultipleChoiceQuestion> MultipleChoiceQuestions { get; set; }
        public List<Models.MultipleAnswersQuestion> MultipleAnswersQuestions;
        public List<Models.SingleAnswerQuestion> SingleAnswerQuestions;

        public FormDetailViewModel(IPageService pageService) {
            _pageService = pageService;
            MultipleAnswersQuestions = new List<Models.MultipleAnswersQuestion>();
            SingleAnswerQuestions = new List<Models.SingleAnswerQuestion>();

            CancelCommand = new Command(async () => await Cancel());
            SubmitCommand = new Command(Submit);
        }
        public void SplitMultipleChoiceQuestions() {

            var multipleChoiceQuestions = MultipleChoiceQuestions;

            foreach (Models.MultipleChoiceQuestion multipleQuestion in multipleChoiceQuestions) {
                if (multipleQuestion.MultipleAnswers) {
                    var multipleAnswerQuestion = new Models.MultipleAnswersQuestion(multipleQuestion.Question);
                    foreach (Models.Option option in multipleQuestion) {
                        multipleAnswerQuestion.Add(option);
                    }
                    MultipleAnswersQuestions.Add(multipleAnswerQuestion);
                } else if (!multipleQuestion.MultipleAnswers) {
                    var singleAnswerQuestion = new Models.SingleAnswerQuestion {
                        Question = multipleQuestion.Question,
                        Options = new ObservableCollection<string>()
                    };
                    foreach (Models.Option option in multipleQuestion) {
                        singleAnswerQuestion.Options.Add(option.OptionText);
                    }
                    SingleAnswerQuestions.Add(singleAnswerQuestion);
                }
            }
        }

        private async Task Cancel() {
            await _pageService.PopAsync();
        }

        private void Submit() {
            foreach (DiscursiveQuestion discursiveQuestion in DiscursiveQuestions) {
                Debug.WriteLine("[Submit] Question: " + discursiveQuestion.Question);
                Debug.WriteLine("[Submit] Answer: " + discursiveQuestion.Answer);
            }
            foreach (MultipleAnswersQuestion checkBoxQuestion in MultipleAnswersQuestions) {
                Debug.WriteLine("[Submit] Question: " + checkBoxQuestion.Question);
                foreach (Option option in checkBoxQuestion) {
                    if (option.IsSelected) {
                        Debug.WriteLine("[Submit] Answer: " + option.OptionText);
                    }
                }
            }
            foreach (SingleAnswerQuestion radioButtonQuestion in SingleAnswerQuestions) {
                int answerIndex = radioButtonQuestion.SelectedOption;

                Debug.WriteLine("[Submit] Question: " + radioButtonQuestion.Question);
                Debug.WriteLine("[Submit] Answer: " + radioButtonQuestion.Options[answerIndex]);
            }
        }
    }
}
