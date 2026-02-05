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

        // --- EXPORT DROPDOWN LOGIC ---

        // --- EXPORT DROPDOWN LOGIC ---

        private void OnExportBtnClick(object sender, RoutedEventArgs e)
        {
            // Toggle Popup
            ExportPopup.IsOpen = !ExportPopup.IsOpen;
        }

        private void OnExportCurrentClick(object sender, RoutedEventArgs e)
        {
            ExportPopup.IsOpen = false;
            string path = PathText.Text;
            MessageBox.Show($"'{path}' 경로에\n현재 테이블을 저장했습니다.", "Export Current", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OnExportAllClick(object sender, RoutedEventArgs e)
        {
            ExportPopup.IsOpen = false;
            string path = PathText.Text;
            MessageBox.Show($"'{path}' 경로에\n전체 데이터를 저장했습니다.", "Export All", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OnBrowsePathClick(object sender, RoutedEventArgs e)
        {
            // Simulate Folder Browser Dialog
            // In a real app: use System.Windows.Forms.FolderBrowserDialog or Microsoft.Win32.OpenFileDialog
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "저장할 폴더를 선택하세요 (파일 선택으로 대체)",
                CheckFileExists = false,
                FileName = "Folder Selection",
                ValidateNames = false
            };

            if (dialog.ShowDialog() == true)
            {
                // Just take the directory of what they picked/typed
                string folder = System.IO.Path.GetDirectoryName(dialog.FileName);
                if (string.IsNullOrEmpty(folder)) folder = "C:\\NewPath";
                PathText.Text = folder;
            }
        }
    }
}
