namespace MAUIEssentials.AppCode.DependencyServices
{
    public interface IRequestPermission
	{
		Task<PermissionStatus> RequestAsync<T>(T permission);
	}
}
