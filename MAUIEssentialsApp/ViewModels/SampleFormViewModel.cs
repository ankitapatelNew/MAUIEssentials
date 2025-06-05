using System.Collections.ObjectModel;
using MAUIEssentials.AppCode.Controls;
using MAUIEssentials.AppCode.Helpers;
using MAUIEssentials.AppResources;
using MAUIEssentials.Models;
using MAUIEssentials.ViewModels;

namespace MAUIEssentialsApp.ViewModels
{
    public class SampleFormViewModel : BaseViewModel
    {
        public DateofBirthModel dateofBirthData { get; set; }

        public SampleFormViewModel()
        {
            try
            {
                QuestionList = new ObservableCollection<QuestionModel>();

                GetDateofBirthData();
                GetDesignationData();
                GetCountryData();
                GetCheckBoxData();
                GetFlowRatesData();

                QuestionList = new ObservableCollection<QuestionModel>(GetQuestionsCheckBoxData());

                Quantity = QuantityList.FirstOrDefault();

                VideoSource = new HtmlWebViewSource
                {
                    Html = "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/ForBiggerEscapes.mp4",
                };
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        private void GetFlowRatesData()
        {
            try
            {
                var list = new List<FlowRatesList>
                {
                    new FlowRatesList
                    {
                        Description = "1",
                        IsBusy  = true,
                    },
                    new FlowRatesList
                    {
                        Description = "2",
                        IsBusy  = true,
                    },
                    new FlowRatesList
                    {
                        Description = "3",
                        IsBusy  = true,
                    },
                    new FlowRatesList
                    {
                        Description = "4",
                        IsBusy  = true,
                    },
                    new FlowRatesList
                    {
                        Description = "5",
                        IsBusy  = true,
                    },
                    new FlowRatesList
                    {
                        Description = "6",
                        IsBusy  = true,
                    },
                    new FlowRatesList
                    {
                        Description = "7",
                        IsBusy  = true,
                    },
                    new FlowRatesList
                    {
                        Description = "8",
                        IsBusy  = true,
                    },
                    new FlowRatesList
                    {
                        Description = "9",
                        IsBusy  = true,
                    },
                    new FlowRatesList
                    {
                        Description = "10",
                        IsBusy  = true,
                    },
                };

                FlowRatesLists = list;
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        public DateofBirthModel GetDateofBirthData()
        {
            var dateOfBirthValue = new DateofBirthModel
            {
                Name = "Date of Birth",
                Date = DateTime.Now,
                InfoText = "Please enter Date of Birth",
            };

            return dateOfBirthValue;
        }

        public DesignationModel GetDesignationData()
        {
            var desiginationList = new List<EnumMapping>
            {
                new EnumMapping
                {
                    Value = 0,
                    Text = "Bachelor of Commerce",
                    InfoText = "Bachelor of Commerce",
                },
                new EnumMapping
                {
                    Value = 1,
                    Text = "Bachelor of Education",
                    InfoText = "Bachelor of Education",
                },
                new EnumMapping
                {
                    Value = 2,
                    Text = "Bachelor of Technology",
                    InfoText = "Bachelor of Technology",
                },
                new EnumMapping
                {
                    Value = 3,
                    Text = "Bachelor of Arts",
                    InfoText = "Bachelor of Arts",
                },
                new EnumMapping
                {
                    Value = 4,
                    Text = "Bachelor of Business Administration",
                    InfoText = "Bachelor of Business Administration",
                },
                new EnumMapping
                {
                    Value = 5,
                    Text = "Bachelor of Computer Applications",
                    InfoText = "Bachelor of Computer Applications",
                },
            };

            var designationData = new DesignationModel
            {
                EnumMapping = desiginationList ?? new List<EnumMapping>(),
                Label = "Designation",
                InfoText = "Please enter your designation",
                Mandatory = true,
                Name = "Designation",
                ParameterValue = "Designation Value"
            };
            return designationData;
        }

        public List<CountryModel> GetCountryData()
        {
            var country = new List<CountryModel>
            {
                new CountryModel
                {
                    Id= 1,
                    CountryName = "India",
                },
                new CountryModel
                {
                    Id= 2,
                    CountryName = "Russia",
                },
                new CountryModel
                {
                    Id= 3,
                    CountryName = "America",
                },
                new CountryModel
                {
                    Id= 4,
                    CountryName = "Australia",
                },
                new CountryModel
                {
                    Id= 5,
                    CountryName = "maldives",
                },
            };

            Country = country;

            return country;
        }

        public CheckboxModel GetCheckBoxData()
        {
            var checkBoxValue = new CheckboxModel
            {
                Name = LocalizationResources.termsOfConditions,
                Label = LocalizationResources.termsOfConditions,
                Mandatory = true,
                ParameterValue = LocalizationResources.termsOfConditions,
                InfoText = "Please select terms of conditions",
            };

            return checkBoxValue;
        }

        public List<QuestionModel> GetQuestionsCheckBoxData()
        {
            var questionData = new List<QuestionModel>
            {
                new QuestionModel
                {
                    Id = 1,
                    Question = "What is .NET MAUI ?",
                    Required = true,
                    SelectedAnswer = ".NET MAUI (Multi-platform App UI) is a cross-platform framework that enables developers to build native mobile and desktop apps using a single C# codebase.",
                    Type = "tickbox",
                    PossibleAnswers = new List<PossibleAnswer>
                    {
                        new PossibleAnswer
                        {
                            Value = "MAUI is Scam",
                        },
                        new PossibleAnswer
                        {
                            Value = "MAUI is not Stable",
                        },
                    },
                },
                new QuestionModel
                {
                    Id = 2,
                    Question = ".net maui difference from xamarin.forms ?",
                    Required = true,
                    SelectedAnswer = ".NET MAUI (Multi-platform App UI) is a cross-platform framework that enables developers to build native mobile and desktop apps using a single C# codebase.",
                    Type = "multiplechoice",
                    PossibleAnswers = new List<PossibleAnswer>
                    {
                        new PossibleAnswer
                        {
                            Value = "MAUI is Scam",
                        },
                        new PossibleAnswer
                        {
                            Value = "MAUI is not Stable",
                        },
                    },
                },
                new QuestionModel
                {
                    Id = 3,
                    Question = "advantages of .net maui ",
                    Required = true,
                    SelectedAnswer = ".NET MAUI (Multi-platform App UI) is a cross-platform framework that enables developers to build native mobile and desktop apps using a single C# codebase.",
                    Type = "freetext",
                    PossibleAnswers = new List<PossibleAnswer>
                    {
                        new PossibleAnswer
                        {
                            Value = "MAUI is Scam",
                        },
                        new PossibleAnswer
                        {
                            Value = "MAUI is not Stable",
                        },
                    },
                },
            };
            return questionData;
        }

        string _firstName;
        public string FirstName
        {
            get => _firstName;
            set
            {
                _firstName = value;
                OnPropertyChanged();
            }
        }

        string _lastName;
        public string LastName
        {
            get => _lastName;
            set
            {
                _lastName = value;
                OnPropertyChanged();
            }
        }

        string _notes;
        public string Notes
        {
            get => _notes;
            set
            {
                _notes = value;
                OnPropertyChanged();
            }
        }

        string _confirmPassword;
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                _confirmPassword = value;
                OnPropertyChanged();
            }
        }

        string _email;
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        string _password;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        string _dateOfBirth;
        public string DateOfBirth
        {
            get => _dateOfBirth;
            set
            {
                _dateOfBirth = value;
                OnPropertyChanged();
            }
        }

        public string Calendar => "ic_calendar";

        List<string> _designation;
        public List<string> Designation
        {
            get => _designation;
            set
            {
                _designation = value;
                OnPropertyChanged();
            }
        }

        List<CountryModel> _country;
        public List<CountryModel> Country
        {
            get => _country;
            set
            {
                _country = value;
                OnPropertyChanged();
            }
        }

        List<string> _gender;
        public List<string> Gender
        {
            get => _gender;
            set
            {
                _gender = value;
                OnPropertyChanged();
            }
        }

        public bool switchSample { get; set; }

        public bool termsOfConditions { get; set; }

        ObservableCollection<QuestionModel> _questionList;
        public ObservableCollection<QuestionModel> QuestionList
        {
            get => _questionList;
            set
            {
                _questionList = value;
                OnPropertyChanged();
            }
        }

        double _rating;
        public double Rating
        {
            get => _rating;
            set
            {
                SetProperty(ref _rating, value);

                if (Rating.ToInt() != 0)
                {
                    IsRatingValid = true;
                }
            }
        }

        bool _isRatingValid = true;
        public bool IsRatingValid
        {
            get => _isRatingValid;
            set => SetProperty(ref _isRatingValid, value);
        }

        double _progress;
        public double Progress
        {
            get => _progress;
            set => SetProperty(ref _progress, value);
        }

        bool _isApproved = true;
        public bool IsApproved
        {
            get => _isApproved;
            set
            {
                SetProperty(ref _isApproved, value);
            }
        }

        bool _isCancelled = true;
        public bool IsCancelled
        {
            get => _isCancelled;
            set
            {
                SetProperty(ref _isCancelled, value);
            }
        }

        bool _isNotify = true;
        public bool IsNotify
        {
            get => _isNotify;
            set => SetProperty(ref _isNotify, value);
        }

        public ObservableCollection<string> QuantityList
        {
            get
            {
                var list = new List<string>();

                for (int i = 1; i < 101; i++)
                {
                    list.Add(i.ToString());
                }
                return new ObservableCollection<string>(list);
            }
        }

        string _quantity;
        public string Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(QuantitySelectedIndex));
            }
        }

        int _selectedIndex;
        public int QuantitySelectedIndex
        {
            get => _selectedIndex;
            set
            {
                _selectedIndex = value;
                OnPropertyChanged();
            }
        }

        HtmlWebViewSource _videoSource;
        public HtmlWebViewSource VideoSource
        {
            get => _videoSource;
            set
            {
                _videoSource = value;
                OnPropertyChanged();
            }
        }

        ObservableCollection<BottomLabelWithImage> _bottomLabelWithImageList;
        public ObservableCollection<BottomLabelWithImage> BottomLabelWithImageList
        {
            get => _bottomLabelWithImageList;
            set => SetProperty(ref _bottomLabelWithImageList, value);
        }

        List<FlowRatesList> _flowRatesLists;
        public List<FlowRatesList> FlowRatesLists
        {
            get => _flowRatesLists;
            set => SetProperty(ref _flowRatesLists, value);
        }
    }

    public class CountryModel
    {
        public int Id { get; set; }

        public string? CountryName { get; set; }
    }

    public class DateofBirthModel
    {
        public string? Name { get; set; }

        public DateTime Date { get; set; }

        public string? InfoText { get; set; }
    }

    public class DesignationModel
    {
        public bool Mandatory { get; set; }

        public object? ParameterValue { get; set; }

        public string? Name { get; set; }

        public string? InfoText { get; set; }

        public string? Label { get; set; }

        public List<EnumMapping>? EnumMapping { get; set; }
    }

    public class EnumMapping
    {
        public int? Value { get; set; }
        public string? Text { get; set; }
        public string? InfoText { get; set; }
    }

    public class CheckboxModel
    {
        public string? Name { get; set; }

        public string? Label { get; set; }

        public bool Mandatory { get; set; }

        public string? InfoText { get; set; }

        public object ParameterValue { get; set; }
    }

    public class QuestionModel
    {
        public int Id { get; set; }
        public bool Required { get; set; }
        public string Type { get; set; }
        public string Question { get; set; }
        public List<PossibleAnswer> PossibleAnswers { get; set; }

        public string SelectedAnswer { get; set; }
    }

    public class PossibleAnswer
    {
        public string Value { get; set; }
    }

    public class FlowRatesList
    {
        public string Description { get; set; }

        public bool IsBusy { get; set; }
    }
}
