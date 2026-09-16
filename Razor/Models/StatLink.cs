namespace Razor.Models
{
    public class StatLink
    {
        public string Label { get; set; } = string.Empty;
        public int Count { get; set; }
        public string? Page { get; set; }
        public string? RouteStatus { get; set; }
        public string? RouteCategory { get; set; }
        public StatLinkType Type { get; set; } = StatLinkType.Default;
        public string? Icon { get; set; }
    }
}
