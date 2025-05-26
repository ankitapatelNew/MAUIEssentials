using MAUIEssentials.AppCode.Helpers;

namespace MAUIEssentials.AppCode.DependencyServices
{
    public interface ICommonUtils
    {
        bool IsIphoneX();
        long SystemClockERealtime();
        void StatusbarColor(Color color);
        Task<bool> EnableLocation();
        bool IsLocationEnabled();
        Task<LocalBioMetricOption> BioMetricAuthAvailability();
        void CloseApplication();
        Size MeasureTextSize(string text, double width, double fontSize, string fontName = null);
        string GetDocumentDirectoryPath();
        string SaveDocument(string folderName, string name, string base64File);
        string SaveDocument(string folderName, string name, byte[] fileArray);
        void OpenDocument(string filePath);
        Task<Size> GetFileSize(byte[] fileData);
        byte[] ResizeImage(byte[] imageData, float width, float height, string extension = "");
        bool IsIphone18OrAbove();
        Size GetImageSize(string fileName);
        void StatusbarColor(Color startColor, Color endColor, GradientOrientation orientation);
        string GetDeviceId();
        Task<bool> CheckNotificationPermission();
        string GetExportsFolder();
    }

    public interface ILocationHelper
	{
		Task<Location> GetLastLocation();
	}

	public interface ICheckMap
	{
		bool IsAppleMap();
		bool IsGoogleMap();
		void OpenGoogleMap(double latitude, double longitude);
		void OpenGoogleMap(Location source, Location destination);
	}

	public interface IListenToSmsRetriever
	{
		void ListenToSmsRetriever();
	}

	public interface INativeHttpHandler
	{
		HttpMessageHandler GetHandler();
	}
}