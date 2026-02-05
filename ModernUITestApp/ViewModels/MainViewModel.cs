using System.Collections.ObjectModel;
using ModernUITestApp.Core;

namespace ModernUITestApp.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public ObservableCollection<AnnotationViewModel> Annotations { get; } = new ObservableCollection<AnnotationViewModel>();

        public MainViewModel()
        {
            // Initial Data
            AddAnnotation(new AnnotationViewModel("Right Click Me!", 100, 100));
            AddAnnotation(new AnnotationViewModel("Another Node", 300, 200));
        }

        private void AddAnnotation(AnnotationViewModel annotation)
        {
            // Subscribe to delete request to remove from collection
            annotation.DeleteRequest += OnAnnotationDeleteRequest;
            Annotations.Add(annotation);
        }

        private void OnAnnotationDeleteRequest(AnnotationViewModel annotation)
        {
            annotation.DeleteRequest -= OnAnnotationDeleteRequest;
            Annotations.Remove(annotation);
        }
    }
}
