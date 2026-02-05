using System.Windows.Input;
using ModernUITestApp.Core;

namespace ModernUITestApp.ViewModels
{
    public class AnnotationViewModel : ViewModelBase
    {
        private string _text;
        private double _x;
        private double _y;
        private bool _isEditing;
        private string _backupText; // To restore on Cancel

        public AnnotationViewModel(string text, double x, double y)
        {
            _text = text;
            _x = x;
            _y = y;

            StartEditingCommand = new RelayCommand(_ => StartEditing());
            CommitEditCommand = new RelayCommand(_ => CommitEdit());
            CancelEditCommand = new RelayCommand(_ => CancelEdit());
            DeleteCommand = new RelayCommand(_ => DeleteRequest?.Invoke(this));
        }

        // Event to notify parent to remove this item
        public event System.Action<AnnotationViewModel> DeleteRequest;

        public string Text
        {
            get => _text;
            set => SetProperty(ref _text, value);
        }

        public double X
        {
            get => _x;
            set => SetProperty(ref _x, value);
        }

        public double Y
        {
            get => _y;
            set => SetProperty(ref _y, value);
        }

        public bool IsEditing
        {
            get => _isEditing;
            set => SetProperty(ref _isEditing, value);
        }

        public ICommand StartEditingCommand { get; }
        public ICommand CommitEditCommand { get; }
        public ICommand CancelEditCommand { get; }
        public ICommand DeleteCommand { get; }

        private void StartEditing()
        {
            _backupText = Text;
            IsEditing = true;
        }

        private void CommitEdit()
        {
            IsEditing = false;
        }

        private void CancelEdit()
        {
            Text = _backupText;
            IsEditing = false;
        }
    }
}
