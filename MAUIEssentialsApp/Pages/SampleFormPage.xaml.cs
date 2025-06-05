using System.Globalization;
using MAUIEssentials.AppCode.Controls;
using MAUIEssentials.AppCode.Helpers;
using MAUIEssentials.AppResources;
using MAUIEssentials.Pages;
using MAUIEssentialsApp.ViewModels;

namespace MAUIEssentialsApp.Pages;

public partial class SampleFormPage : BaseContentPage
{
	double fontSize14 = 14, fontSize13 = 13, fontSize15 = 15, fontSize12 = 12;

	public SampleFormPage()
	{
		try
		{
			InitializeComponent();
			ratingView.Spacing = (float)Math.Floor(ScreenSize.ScreenWidth - 70) / 5;
			webView.IsVisible = true;

			lblHtml.Text = "<h1>Lorem ipsum dolor sit amet consectetuer adipiscing elit</h1>";
		}
		catch (Exception ex)
		{
			ex.LogException();
		}
	}

	protected override void OnAppearing()
	{
		try
		{
			datePickerView.Children.Add(GetDatePicker(viewModel.GetDateofBirthData()));
			pickerInfoControlView.Children.Add(GetPickerView(viewModel.GetDesignationData()));
			checkBoxView.Children.Add(GetCheckbox(viewModel.GetCheckBoxData()));
			if (viewModel.QuestionList.Any())
			{
				SetQuestionUI();
			}
		}
		catch (Exception ex)
		{
			ex.LogException();
		}
	}

	DatePickerControl GetDatePicker(DateofBirthModel parameter)
	{
		string date = string.Empty;

		if (parameter.Date != null)
		{
			try
			{
				var dateTime = parameter.Date.ToString("dd'/'MM'/'yyyy", CultureInfo.InvariantCulture);
				date = dateTime;
			}
			catch (Exception ex)
			{
				ex.LogException();
			}
		}

		var datePicker = new DatePickerControl
		{
			ClassId = parameter.Name,
			Text = date,
			TextColor = AppColorResources.blackTextColor.ToColor(),
			FontSize = fontSize14,
			Placeholder = "dd / MM / yyyy",
			PlaceholderColor = AppColorResources.grayColor.ToColor(),
			DateFormat = "dd'/'MM'/'yyyy",
			MaximumDate = DateTime.Today,
			HasInfoText = !string.IsNullOrEmpty(parameter.InfoText),
			InfoText = parameter.InfoText
		};

		datePicker.DateSelected -= DatePicker_DateSelected;
		datePicker.DateSelected += DatePicker_DateSelected;
		return datePicker;
	}

	async void DatePicker_DateSelected(object sender, EventArgs e)
	{
		try
		{
			var datePicker = sender as DatePickerControl;

			datePicker.Text = datePicker.SelectedDate.ToString("dd'/'MM'/'yyyy");
			viewModel.DateOfBirth = datePicker.SelectedDate.ToString("dd'/'MM'/'yyyy");
		}
		catch (Exception ex)
		{
			ex.LogException();
		}
	}

	PickerInfoControl GetPickerView(DesignationModel parameter)
	{
		var selectedValue = string.Empty;

		if (parameter != null && parameter.EnumMapping != null)
		{
			var stringValue = parameter.ParameterValue?.ToString();
			if (int.TryParse(stringValue, out var intValue))
			{
				var mapping = parameter.EnumMapping.FirstOrDefault(x => x.Value == intValue);
				selectedValue = mapping?.Text ?? string.Empty;
			}
		}

		var pickerControl = new PickerInfoControl
		{
			Margin = new Thickness(0, 24, 0, 0),
			ClassId = parameter.Name,
			Text = selectedValue,
			TextColor = AppColorResources.blackTextColor.ToColor(),
			FontSize = fontSize14,
			HeaderText = parameter.Label,
			HeaderTextColor = AppColorResources.grayColor.ToColor(),
			HeaderFontSize = fontSize13,
			HasInfoText = !string.IsNullOrEmpty(parameter.InfoText),
			InfoText = parameter.InfoText,
			IsMandatory = parameter.Mandatory,
			ItemList = parameter.EnumMapping?.Where(x => x.Value != null).Select(x => x.Text).ToList()
		};

		pickerControl.ItemSelected -= DesignationPickerItemSelected;
		pickerControl.ItemSelected += DesignationPickerItemSelected;
		return pickerControl;
	}

	private void DesignationPickerItemSelected(object sender, EventArgs e)
	{
		try
		{
			var pickerView = sender as PickerInfoControl;
			if (pickerView == null)
				return;

			string selectedText = pickerView.Text;
		}
		catch (Exception ex)
		{
			ex.LogException();
		}
	}

	CheckboxControl GetCheckbox(CheckboxModel parameter)
	{
		var control = new CheckboxControl
		{
			Margin = new Thickness(0, 20, 0, 0),
			ClassId = parameter.Name,
			Text = parameter.Label,
			TextColor = AppColorResources.blackTextColor.ToColor(),
			FontSize = fontSize14,
			IsMandatory = parameter.Mandatory,
			HasInfoText = !string.IsNullOrEmpty(parameter.InfoText),
			InfoText = parameter.InfoText,
			IsSelected = parameter.ParameterValue != null,
		};

		control.Selected -= CheckboxSelected;
		control.Selected += CheckboxSelected;
		return control;
	}

	private async void CheckboxSelected(object sender, EventArgs e)
	{
		try
		{
			var checkBoxView = sender as CheckboxControl;
			if (checkBoxView == null)
				return;

			string selectedText = checkBoxView.Text;
		}
		catch (Exception ex)
		{
			ex.LogException();
		}
	}

	private void SetQuestionUI()
	{
		try
		{
			if (questionCheckboxView.Children.Any())
			{
				questionCheckboxView.Children.Clear();
			}

			foreach (var item in viewModel.QuestionList)
			{
				switch (item.Type.ToLower())
				{
					case "multiplechoice":
						questionCheckboxView.Children.Add(GetQuestionLabel(item));
						questionCheckboxView.Children.Add(GetRadioControl(item, true));
						break;

					case "tickbox":
						if (item.SelectedAnswer == null)
						{
							item.SelectedAnswer = "false";
						}
						questionCheckboxView.Children.Add(GetQuestionCheckBox(item));
						break;

					case "freetext":
						questionCheckboxView.Children.Add(GetQuestionLabel(item));
						questionCheckboxView.Children.Add(GetEditor(item));
						questionCheckboxView.Children.Add(new Border
						{
							ClassId = item.Id.ToString(),
							Style = Application.Current?.Resources["borderStyle"] as Style
						});
						break;
				}
			}
		}
		catch (Exception ex)
		{
			ex.LogException();
		}
	}

	Label GetQuestionLabel(QuestionModel model)
	{
		var label = new Label
		{
			Margin = new Thickness(0, 20, 0, 0),
			ClassId = model.Id.ToString(),
			HorizontalTextAlignment = TextAlignment.Start,
			VerticalTextAlignment = TextAlignment.Center
		};

		if (model.Required)
		{
			var formattedString = new FormattedString();
			formattedString.Spans.Add(new Span
			{
				Text = model.Question,
				FontSize = fontSize15,
				TextColor = AppColorResources.blueColor.ToColor()
			});

			formattedString.Spans.Add(new Span
			{
				Text = " *",
				FontSize = fontSize12,
				TextColor = AppColorResources.redColor.ToColor()
			});
			label.FormattedText = formattedString;
		}
		else
		{
			label.Text = model.Question;
			label.TextColor = AppColorResources.blueColor.ToColor();
			label.FontSize = fontSize15;
		}
		return label;
	}

	Label GetValidationLabel(QuestionModel model)
	{
		return new Label
		{
			IsVisible = false,
			Margin = new Thickness(0, 3, 0, 0),
			ClassId = model.Id.ToString(),
			TextColor = AppColorResources.redColor.ToColor(),
			FontSize = fontSize12,
			HorizontalTextAlignment = TextAlignment.Start,
			VerticalTextAlignment = TextAlignment.Center,
			Text = string.Format("{0} is required", model.Question),
		};
	}

	RadioControl GetRadioControl(QuestionModel model, bool isCheckbox = false)
	{
		var distinctList = model.PossibleAnswers.Where(x => x.Value != null).Select(x => x.Value).ToList();

		var list = distinctList.Distinct().Select(item => new RadioModel
		{
			Name = item,
			IsCheckbox = isCheckbox,
			IsSelected = !string.IsNullOrEmpty(model.SelectedAnswer) && model.SelectedAnswer.Contains(item)
		});

		var radioControl = new RadioControl
		{
			Margin = new Thickness(0, 10, 0, 0),
			ClassId = model.Id.ToString(),
			ItemSource = list,
			FontSize = fontSize14,
			TextColor = AppColorResources.blackTextColor.ToColor()
		};

		radioControl.ValueSelected -= QuestionsRadioValueSelected;
		radioControl.ValueSelected += QuestionsRadioValueSelected;
		return radioControl;
	}

	QuestionCheckbox GetQuestionCheckBox(QuestionModel model)
	{
		var control = new QuestionCheckbox
		{
			Margin = new Thickness(0, 20, 0, 0),
			ClassId = model.Id.ToString(),
			Text = model.Question,
			TextColor = AppColorResources.blueColor.ToColor(),
			FontSize = fontSize15,
			IsMandatory = model.Required,
			IsSelected = !string.IsNullOrEmpty(model.SelectedAnswer) && model.SelectedAnswer == "true"
		};

		control.Selected -= QuestionsCheckboxSelected;
		control.Selected += QuestionsCheckboxSelected;
		return control;
	}

	CommanEditor GetEditor(QuestionModel model)
	{
		var editor = new CommanEditor
		{
			ClassId = model.Id.ToString(),
			Text = model.SelectedAnswer ?? string.Empty,
			TextColor = AppColorResources.blackTextColor.ToColor(),
			FontSize = fontSize14,
			IsBorder = false,
			AutoSize = EditorAutoSizeOption.TextChanges
		};

		editor.Completed -= EditorCompleted;
		editor.Unfocused -= EditorUnfocused;

		editor.Completed += EditorCompleted;
		editor.Unfocused += EditorUnfocused;
		return editor;
	}

	void QuestionsRadioValueSelected(object sender, RadioSelectedEventArgs e)
	{
		try
		{
			var parameter = viewModel.QuestionList.FirstOrDefault(x => x.Id.ToString() == (sender as RadioControl).ClassId);
			parameter.SelectedAnswer = e.Name;

			SetQuestionUI();
		}
		catch (Exception ex)
		{
			ex.LogException();
		}
	}

	private void QuestionsCheckboxSelected(object sender, EventArgs e)
	{
		try
		{
			var parameter = viewModel.QuestionList.FirstOrDefault(x => x.Id.ToString() == (sender as QuestionCheckbox).ClassId);
			parameter.SelectedAnswer = (sender as QuestionCheckbox).IsSelected ? "true" : "false";

			SetQuestionUI();
		}
		catch (Exception ex)
		{
			ex.LogException();
		}
	}

	void EditorUnfocused(object sender, FocusEventArgs e)
	{
		try
		{
			EditorCompleted(sender, EventArgs.Empty);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	void EditorCompleted(object sender, EventArgs e)
	{
		try
		{
			var editor = sender as CommanEditor;
			var parameter = viewModel.QuestionList.FirstOrDefault(x => x.Id.ToString() == editor.ClassId);

			if (parameter != null)
			{
				parameter.SelectedAnswer = editor.Text;
			}
		}
		catch (Exception ex)
		{
			ex.LogException();
		}
	}

	private void PinchToZoomContainer_ScaleChanged(object sender, ScaleChangeEventArgs e)
	{
		try
		{
			// scrollView.IsEnabled = e.Scale <= 1;
		}
		catch (Exception ex)
		{
			ex.LogException();
		}
    }

	private async void Button_Clicked(object sender, EventArgs e)
	{
		try
		{
			var customerBottomLabelWithImages = new List<BottomLabelWithImage>
			{
				new BottomLabelWithImage()
				{
					Image = "ic_add",
					Title = "Add",
				},
				new BottomLabelWithImage()
				{
					Image = "ic_edit",
					Title = "Edit",
				},
				new BottomLabelWithImage()
				{
					Image = "ic_delete",
					Title = "Delete",
				},
			};

			var popupPage = new SelectBottomLabelWithImagePopup(customerBottomLabelWithImages);
			popupPage.OnBottomLabelWithImageSelect += (page, item) =>
            {
                try
                {
                    if (item == null)
                    {
                        return;
                    }

					if (item.Title == "Add")
                    {
						ToastPopup.Instance.ShowMessage("Add");
                    }
					else if (item.Title == "Edit")
					{
						ToastPopup.Instance.ShowMessage("Edit");
					}
					else if (item.Title == "Delete")
					{
						ToastPopup.Instance.ShowMessage("Delete");
					}
                }
                catch (Exception ex)
                {
                    ex.LogException();
                }
            };
			await NavigationServices.OpenPopupPage(popupPage);
		}
		catch (Exception ex)
		{
			ex.LogException();
		}
    }
}