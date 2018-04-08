namespace DSerfozo.RpcBindings.Contract.Analyze
{
    public class AnalyzeOptions
    {
        public bool ExtractPropertyValues { get; set; }

        public bool AnalyzeProperties { get; set; } = true;

        public string Name { get; set; }
    }
}
