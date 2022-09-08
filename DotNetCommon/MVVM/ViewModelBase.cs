using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using DotNetCommon.Extensions;

namespace DotNetCommon.MVVM
{
    public class ViewModelBase : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                OnPropertyChanged(propertyName);
            }
        }


        private bool _doValidate = false;
        private Dictionary<string, IEnumerable<string>> _errorsByPropertyName = new Dictionary<string, IEnumerable<string>>();


        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public bool HasErrors => _doValidate && _errorsByPropertyName.Any();


        public IEnumerable GetErrors(string propertyName)
        {
            if (_errorsByPropertyName.ContainsKey(propertyName))
            {
                return _errorsByPropertyName[propertyName];
            }
            else
            {
                return null;
            }
        }

        public bool IsValid()
        {
            _doValidate = true;

            foreach (var prop in this.GetType().GetAllPropertyNames()) 
            {
                Validate(prop);
            }
            return !HasErrors;
        }

        public void Validate([CallerMemberName] string propertyName = null)
        {
            if (_doValidate)
            {
                if (_errorsByPropertyName.ContainsKey(propertyName)) _errorsByPropertyName.Remove(propertyName);
                var errors = GetPropertyErrors(propertyName);
                if (errors.Any()) _errorsByPropertyName.Add(propertyName, errors);
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
                OnPropertyChanged(nameof(HasErrors));
            }
        }

        private IEnumerable<string> GetPropertyErrors(string propertyName)
        {
            if (String.IsNullOrWhiteSpace(propertyName)) return Enumerable.Empty<string>();
            ValidationContext context = new ValidationContext(this) { MemberName = propertyName };
            List<ValidationResult> results = new List<ValidationResult>();
            Validator.TryValidateProperty(this.GetValue(propertyName), context, results);
            return results.Select(x => x.ErrorMessage);
        }
    }
}