using System.Windows.Input;

namespace Foscamun.Views
{
    /// <summary>
    /// A page of the session flow (Home, Roll call, Session, Voting, Result, Final result).
    /// The sidebar shows the previous page, this one and the next one.
    /// </summary>
    public interface IFlowPage
    {
        /// <summary>String resource key of this page's name.</summary>
        string TitleKey { get; }

        /// <summary>Link to the previous page, or null if there is none.</summary>
        FlowLink? Previous { get; }

        /// <summary>Link to the next page, or null if there is none.</summary>
        FlowLink? Next { get; }
    }

    /// <summary>
    /// Sidebar link to another page of the flow.
    /// </summary>
    /// <param name="LabelKey">String resource key of the destination's name.</param>
    /// <param name="Command">Navigates there; null shows the link dimmed, for a page that comes by itself.</param>
    public record FlowLink(string LabelKey, ICommand? Command)
    {
        public const string HomeKey = "HomeBtn";

        /// <summary>The destination is Home: the link shows the house instead of an arrow.</summary>
        public bool IsHome => LabelKey == HomeKey;
    }
}
