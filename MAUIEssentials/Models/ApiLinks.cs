using Newtonsoft.Json;

namespace MAUIEssentials.Models
{
    public class ApiLinks
	{
		[JsonProperty(PropertyName = "_links")]
		public List<ApiLinkResults>? Links { get; set; }
	}

	public class ApiLinkResults
	{
		[JsonProperty(PropertyName = "href")]
		public string? Href { get; set; }

		[JsonProperty(PropertyName = "rel")]
		public string? Rel { get; set; }

		[JsonProperty(PropertyName = "method")]
		public string? Method { get; set; }

		[JsonProperty(PropertyName = "args")]
		public List<ApiLinkParameters>? Parameters { get; set; }

        [JsonProperty(PropertyName = "grouped")]
        public bool Grouped { get; set; }

        public string? Label { get; set; }
		public string? Templated { get; set; }
		public string? ResponseType { get; set; }
		public string? Message { get; set; }
	}

	public class ApiLinkParameters
	{
		[JsonProperty(PropertyName = "name")]
		public string? Name { get; set; }

		[JsonProperty(PropertyName = "type")]
		public string? Type { get; set; }

		[JsonProperty(PropertyName = "mandatory")]
		public bool Mandatory { get; set; }

		[JsonProperty(PropertyName = "user_input")]
		public bool UserInput { get; set; }

		[JsonProperty(PropertyName = "in_body")]
		public bool InBody { get; set; }

		[JsonProperty(PropertyName = "value")]
		public object? ParameterValue { get; set; }
	}

	public class DownloadResponse
	{
		public string? FileName { get; set; }
		public byte[]? FileData { get; set; }
	}

	public class UnauthorizeApiError
	{
		public int StatusCode { get; set; }
		public string? Message { get; set; }
	}

	public class RefreshApiError
	{
		public List<string>? Errors { get; set; }
	}

	public class ApiError
	{
		public bool Success { get; set; }
		public string? Message { get; set; }
		public List<string>? Errors { get; set; }
	}

	public class SuccessResultResponse : ApiError
	{
		public string? Result { get; set; }
	}
}
