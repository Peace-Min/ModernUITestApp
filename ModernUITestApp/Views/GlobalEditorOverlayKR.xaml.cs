using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ModernUITestApp.ViewModels;

namespace ModernUITestApp.Views
{
    public partial class GlobalEditorOverlayKR : UserControl
    {
        // TARGET STATE (The "Model" we are editing)
        private AnnotationViewModel _currentTarget;
        private string _originalText;

        public GlobalEditorOverlayKR()
        {
            InitializeComponent();
        }

        // --- PUBLIC API ---

        public void ShowMenu(AnnotationViewModel target, double x, double y)
        {
            _currentTarget = target;

            // Close Editor if open
            EditorPopup.IsOpen = false;

            // Position and Open Menu
            MenuPopup.HorizontalOffset = x;
            MenuPopup.VerticalOffset = y;
            MenuPopup.IsOpen = true;
        }

        public void ShowEditor(AnnotationViewModel target)
        {
            _currentTarget = target;
            if (_currentTarget == null) return;

            // 1. Close Menu if open
            MenuPopup.IsOpen = false;

            // 2. Setup Editor
            _originalText = _currentTarget.Text;
            EditorTextBox.Text = _originalText;
            EditorTextBox.Focus();
            EditorTextBox.SelectAll();

            // 3. Position Editor (Simple default position or relative to Menu)
            // If MenuPopup was just open, use its last known position, otherwise center or use default
            EditorPopup.HorizontalOffset = MenuPopup.HorizontalOffset;
            EditorPopup.VerticalOffset = MenuPopup.VerticalOffset;

            // 4. Open
            EditorPopup.IsOpen = true;
        }

        public void HideAll()
        {
            MenuPopup.IsOpen = false;
            EditorPopup.IsOpen = false;
        }

        // --- INTERNAL HANDLERS ---

        private void OnEditClicked(object sender, RoutedEventArgs e)
        {
            if (_currentTarget == null) return;

            // 1. Close Menu
            MenuPopup.IsOpen = false;

            // 2. Setup Editor
            _originalText = _currentTarget.Text;
            EditorTextBox.Text = _originalText;
            EditorTextBox.Focus();
            EditorTextBox.SelectAll();

            // 3. Position Editor
            EditorPopup.HorizontalOffset = MenuPopup.HorizontalOffset;
            EditorPopup.VerticalOffset = MenuPopup.VerticalOffset;

            // 4. Open
            EditorPopup.IsOpen = true;
        }

        private void OnDeleteClicked(object sender, RoutedEventArgs e)
        {
            if (_currentTarget != null)
            {
                if (_currentTarget.DeleteCommand?.CanExecute(null) == true)
                {
                    _currentTarget.DeleteCommand.Execute(null);
                }
            }
            MenuPopup.IsOpen = false;
        }

        private void EditorBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                {
                    return; // New Line
                }
                else
                {
                    // Commit
                    if (_currentTarget != null)
                    {
                        _currentTarget.Text = EditorTextBox.Text;
                    }
                    EditorPopup.IsOpen = false;
                    e.Handled = true;
                }
            }
            else if (e.Key == Key.Escape)
            {
                // Cancel
                EditorPopup.IsOpen = false;
                e.Handled = true;
            }
        }
    }
}
