using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WoTMapWPF
{
    public class AutoCompleteTextBox : TextBox
    {
        private string currentInput = string.Empty;
        private string currentSuggestion = string.Empty;
        public static readonly DependencyProperty SuggestionValuesProperty = DependencyProperty.Register(
            "SuggestionValues", typeof(ICollection<string>), typeof(AutoCompleteTextBox),
            new FrameworkPropertyMetadata(default(ICollection)));

        public ICollection<string> SuggestionValues
        {
            get => (ICollection<string>)GetValue(SuggestionValuesProperty);
            set => SetValue(SuggestionValuesProperty, value);
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);
            string input = Text;
            if(SuggestionValues != null && input.Length > currentInput.Length && input != currentSuggestion)
            {
                currentSuggestion = SuggestionValues.FirstOrDefault(s => s.StartsWith(input));
                if(currentSuggestion != null)
                {
                    int selectionStart = input.Length;
                    int selectionLength = currentSuggestion.Length - input.Length;
                    Text = currentSuggestion;
                    Select(selectionStart, selectionLength);
                }
            }
            currentInput = input;
        }
    }
}
