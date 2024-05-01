namespace VENTUS.Interaction.TransformTool.Annotation
{
    public interface IAnnotation
    {
        public void Initialize(AnnotationController annotationController);
        public void OnValuesChanged();
        public void Dispose();
    }
}
