namespace Foscamun.Views
{
    /// <summary>
    /// A page whose edits are lost if it is left without saving: the window asks
    /// before going Home or exiting.
    /// </summary>
    public interface IUnsavedChanges
    {
        bool HasUnsavedChanges { get; }
    }
}
