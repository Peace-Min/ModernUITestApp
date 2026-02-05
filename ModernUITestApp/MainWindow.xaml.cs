using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ModernUITestApp.ViewModels;

namespace ModernUITestApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void Canvas_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            // 1. Identify what was clicked (Hit Testing)
            var canvas = sender as Canvas;
            var hitResult = VisualTreeHelper.HitTest(canvas, e.GetPosition(canvas));

            if (hitResult?.VisualHit is DependencyObject visualHit)
            {
                // Traverse up to find the ContentPresenter (which corresponds to our AnnotationViewModel)
                var contentPresenter = FindAncestor<ContentPresenter>(visualHit);
                if (contentPresenter != null && contentPresenter.DataContext is AnnotationViewModel annotation)
                {
                    // 2. Get click position relative to Window (for Absolute Popup Placement)
                    Point clickPoint = e.GetPosition(this);

                    // 3. DIRECT CONTROL: Show Menu via Code Behind
                    EditorOverlay.ShowMenu(annotation, clickPoint.X, clickPoint.Y);

                    e.Handled = true;
                }
            }
        }

        private static T FindAncestor<T>(DependencyObject current) where T : DependencyObject
        {
            while (current != null)
            {
                if (current is T typed) return typed;
                current = VisualTreeHelper.GetParent(current);
            }
            return null;
        }
    }
}
