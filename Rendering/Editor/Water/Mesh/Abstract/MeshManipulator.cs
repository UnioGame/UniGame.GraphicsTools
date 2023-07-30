namespace UniGame.Rendering.Editor.Water.Mesh.Abstract
{
    using Runtime.Water;

    public abstract class MeshManipulator
    {
        public virtual bool IsEnabled              { get; protected set; } = true;
        public virtual bool CanChangeEnabledStatus { get; }                = false;

        public virtual void DrawInspector()
        {
        }

        public abstract int Handle(WaterMesh waterMesh, int selectedVertex);

        public void Disable()
        {
            IsEnabled = false;
        }
    }
}