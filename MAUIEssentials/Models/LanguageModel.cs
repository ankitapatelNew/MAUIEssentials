namespace MAUIEssentials.Models
{
    public class LanguageModel
	{
		public string? Name { get; set; }
		public string? Code { get; set; }
		public string? Description { get; set; }
		public AppLanguage? Language { get; set; }
		public FlowDirection? FlowDirection { get; set; }
	}

	public enum AppLanguage
	{
		English,
		Arabic
	}
}
