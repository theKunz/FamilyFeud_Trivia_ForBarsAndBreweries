﻿using FamilyFeud.CustomEventArgs;
using FamilyFeud.DataObjects;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FamilyFeud.Controls
{
  /// <summary>
  /// Interaction logic for QuestionBuilder.xaml
  /// </summary>
  public partial class QuestionBuilder : Window, INotifyPropertyChanged
  {
    private bool mIsNormalQuestion;
    public event PropertyChangedEventHandler PropertyChanged;
    public event EventHandler<EventArgs<Round>> QuestionComplete;
    public event EventHandler<EventArgs<BonusQuestion>> BonusQuestionComplete;
  
    public QuestionBuilder()
    {
      PropertyChanged += PropertyChanged_RenameAnswerField;
      InitializeComponent();
      mIsNormalQuestion = true;
    }
  
    public QuestionBuilder(bool isBonus)
    {
      PropertyChanged += PropertyChanged_RenameAnswerField;
      InitializeComponent();
      mIsNormalQuestion = isBonus;
    }

    private void PropertyChanged_RenameAnswerField(object sender, PropertyChangedEventArgs args)
    {
      if(args.PropertyName.Equals(nameof(IsNormalQuestion)))
      {
        if(IsNormalQuestion)
        {
          labelAnswer1.Text = "Answer 1:";
        }
        else
        {
          labelAnswer1.Text = "Answer:";
        }
      }
    }

    private bool CheckNonEmptyStackPanelTextBoxes(StackPanel sp)
    {
      bool isNonEmpty = true;

      foreach(object o in sp.Children)
      {
        TextBox tb = o as TextBox;
        if(tb != null)
        {
          isNonEmpty &= !string.IsNullOrWhiteSpace(tb.Text);
        }
      }

      return isNonEmpty;
    }
    
    public bool IsNormalQuestion
    {
      get
      {
        return mIsNormalQuestion;
      }
      set
      {
        if(value != mIsNormalQuestion)
        {
          mIsNormalQuestion = value;
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsNormalQuestion)));
        }
      }
    }

    public bool CanSave
    {
      get
      {
        return CheckNonEmptyStackPanelTextBoxes(spQuestion) &&
               CheckNonEmptyStackPanelTextBoxes(spAnswer1);
      }
    }

    private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs eventArgs)
    {
      eventArgs.Handled = !uint.TryParse((sender as TextBox).Text + eventArgs.Text, out _);
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
      this.Close();
    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
      if(mIsNormalQuestion)
      {
        Round round = new Round();
        round.Question = new Question(tbQuestion.Text);
        round.Answers = new ObservableCollection<Answer>();
        
        if (CheckNonEmptyStackPanelTextBoxes(spAnswer1)) { round.Answers.Add(new Answer(tbAnswer1.Text, uint.Parse(tbAnswer1Points.Text))); }
        if (CheckNonEmptyStackPanelTextBoxes(spAnswer2)) { round.Answers.Add(new Answer(tbAnswer2.Text, uint.Parse(tbAnswer2Points.Text))); }
        if (CheckNonEmptyStackPanelTextBoxes(spAnswer3)) { round.Answers.Add(new Answer(tbAnswer3.Text, uint.Parse(tbAnswer3Points.Text))); }
        if (CheckNonEmptyStackPanelTextBoxes(spAnswer4)) { round.Answers.Add(new Answer(tbAnswer4.Text, uint.Parse(tbAnswer4Points.Text))); }
        if (CheckNonEmptyStackPanelTextBoxes(spAnswer5)) { round.Answers.Add(new Answer(tbAnswer5.Text, uint.Parse(tbAnswer5Points.Text))); }
        if (CheckNonEmptyStackPanelTextBoxes(spAnswer6)) { round.Answers.Add(new Answer(tbAnswer6.Text, uint.Parse(tbAnswer6Points.Text))); }
        if (CheckNonEmptyStackPanelTextBoxes(spAnswer7)) { round.Answers.Add(new Answer(tbAnswer7.Text, uint.Parse(tbAnswer7Points.Text))); }
        if (CheckNonEmptyStackPanelTextBoxes(spAnswer8)) { round.Answers.Add(new Answer(tbAnswer8.Text, uint.Parse(tbAnswer8Points.Text))); }

        round.Answers.OrderByDescending(a => a.PointValue);

        QuestionComplete?.Invoke(this, new EventArgs<Round>(round));
      }
      else
      {
        BonusQuestion bonusQuestion = new BonusQuestion()
        {
          Question = new Question(tbQuestion.Text),
          Answer = new Answer(tbAnswer1.Text, uint.Parse(tbAnswer1Points.Text))
        };

        BonusQuestionComplete?.Invoke(this, new EventArgs<BonusQuestion>(bonusQuestion));
      }

      this.Close();
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs args)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanSave)));
    }
  }
}
