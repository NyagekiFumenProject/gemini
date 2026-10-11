namespace Gemini.Modules.Toolbox.Services
{
    /// <summary>
    /// Optional hook: converts a toolbox item's raw description (usually a resource key)
    /// into display text. Hosts that localize the toolbox export an implementation;
    /// without one the raw value is shown as-is.
    /// </summary>
    public interface IToolboxItemLocalizer
    {
        string LocalizeDescription(string description);
    }
}
