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

        private void OnTestMenuClick(object sender, RoutedEventArgs e)
        {
            // 사용 예제: ContextmenuEditorKR 인스턴스 생성 및 사용
            var menu = new ModernUITestApp.Views.ContextmenuEditorKR();

            // 메뉴 항목 추가
            var editItem = new MenuItem { Header = "편집 (Edit)" };
            editItem.Click += (s, args) =>
            {
                // 정말 단순하게 텍스트만 수정하고 결과를 받아오는 예제
                string currentText = "편집할 텍스트 (Click to Edit)";

                menu.ShowEditor(currentText, (newText) =>
                {
                    // 여기서 수정된 텍스트를 처리 (예: DB 저장, UI 갱신 등)
                    MessageBox.Show($"수정된 내용: {newText}");
                });
            };
            menu.Items.Add(editItem);
            menu.Items.Add(new MenuItem { Header = "삭제 (Delete)" });
            menu.Items.Add(new Separator());

            var exitItem = new MenuItem { Header = "닫기 (Close)" };
            exitItem.Click += (s, args) => menu.IsOpen = false;
            menu.Items.Add(exitItem);

            // 위치 설정 및 표시
            menu.PlacementTarget = sender as UIElement;
            menu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            menu.IsOpen = true;
        }
    }
}
