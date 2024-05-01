namespace VENTUS.Interaction.TransformTool.UI.View.PreparedText
{
    public class SimplePreparedTextSpawner : BasePreparedTextSpawner
    {
        protected override void OnDestroy()
        {
            base.OnDestroy();

            Unfocus();
        }
    }
}
