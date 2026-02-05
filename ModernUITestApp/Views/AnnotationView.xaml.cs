using System.Windows.Controls;
using System.Windows.Input;
using ModernUITestApp.ViewModels;

namespace ModernUITestApp.Views
{
    public partial class AnnotationView : UserControl
    {
        public AnnotationView()
        {
            InitializeComponent();
        }

        private void EditorBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (DataContext is AnnotationViewModel vm)
            {
                if (e.Key == Key.Enter)
                {
                    if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                    {
                        // Allow default behavior (New Line)
                        // No logic needed, simple return lets the TextBox handle it
                        return;
                    }
                    else
                    {
                        // Standard Enter -> Commit
                        if (vm.CommitEditCommand.CanExecute(null))
                        {
                            vm.CommitEditCommand.Execute(null);
                        }
                        e.Handled = true; // Prevent new line being added
                    }
                }
                else if (e.Key == Key.Escape)
                {
                    // Esc -> Cancel
                    if (vm.CancelEditCommand.CanExecute(null))
                    {
                        vm.CancelEditCommand.Execute(null);
                    }
                    e.Handled = true;
                }
            }
        }
    }
}
