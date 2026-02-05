using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ModernUITestApp.ViewModels;

namespace ModernUITestApp.Views
{
    public partial class GlobalEditorOverlay : UserControl
    {
        // TARGET STATE (The "Model" we are editing)
        private AnnotationViewModel _currentTarget;
        private string _originalText;

        public GlobalEditorOverlay()
        {
            InitializeComponent();
        }

        // --- PUBLIC API (Called by MainWindow or external logic) ---

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

        public void HideAll()
        {
            MenuPopup.IsOpen = false;
            EditorPopup.IsOpen = false;
        }

        // --- INTERNAL EVENT HANDLERS (Connected in XAML via Click/PreviewKeyDown) ---

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

            // 3. Position Editor (Same X/Y as Menu for simplicity, or offset)
            EditorPopup.HorizontalOffset = MenuPopup.HorizontalOffset;
            EditorPopup.VerticalOffset = MenuPopup.VerticalOffset;

            // 4. Open
            EditorPopup.IsOpen = true;
        }

        private void OnDeleteClicked(object sender, RoutedEventArgs e)
        {
            if (_currentTarget != null)
            {
                // In a real app, you might raise an event: OnDeleteRequested(_currentTarget)
                // For now, we assume _currentTarget has a Delete logic (or we just remove it via ViewModel which is harder here)
                // Let's assume we invoke the Command on the ViewModel directly for deletion
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
