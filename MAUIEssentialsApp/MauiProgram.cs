using CommunityToolkit.Maui;
using MAUIEssentials;
using MAUIEssentials.AppCode.Behaviors;
using MAUIEssentials.AppCode.Controls;
using MAUIEssentials.AppCode.DependencyServices;
using MAUIEssentials.AppCode.Helpers;
using MAUIEssentials.Effects;
using MAUIEssentials.Handlers;
using MAUIEssentialsApp.DepedencyServices;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Compatibility.Hosting;
using Plugin.Maui.Biometric;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace MAUIEssentialsApp;

public static class MauiProgram
{
	static IEnumerable<FontResource> Fonts
	{
		get
		{
			return new List<FontResource>(FontResource.Defaults)
			{
			};
		}
	}

	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit(options =>
			{
				options.SetShouldSuppressExceptionsInConverters(false);
				options.SetShouldSuppressExceptionsInBehaviors(false);
				options.SetShouldSuppressExceptionsInAnimations(false);
			})
			.UseSkiaSharp()
			.UseMauiCompatibility()
			.RegisterAppServices()
			.ConfigureHandlers()
			.ConfigureFonts(fonts =>
			{
				FontResource.Load(fonts, Fonts);
				foreach (var f in fonts)
				{
					Console.WriteLine($"Font: {f.Filename} / {f.Alias}");
				}
			})
			.UseMauiMaps()
			.ConfigureEffects(effects =>
			{
				effects.Add<TooltipEffect, PlatformTooltipEffect>();
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}

	private static MauiAppBuilder RegisterAppServices(this MauiAppBuilder builder)
	{
		try
		{
			builder.Services.AddSingleton<IBiometric>(BiometricAuthenticationService.Default);
			builder.Services.AddSingleton<IPageNavigationService, PageNavigationService>();

#if ANDROID
			DependencyService.Register<ICommonUtils, MAUIEssentials.Platforms.Android.DepedencyServices.CommonUtils>();
#elif IOS
			DependencyService.Register<ICommonUtils, MAUIEssentials.Platforms.iOS.DependencyServices.CommonUtils>();
#endif
		}
		catch (Exception ex)
		{
			ex.LogException();
		}

		return builder;
	}

	private static MauiAppBuilder ConfigureHandlers(this MauiAppBuilder builder)
	{
		builder.ConfigureMauiHandlers((handlers) =>
		{
#if ANDROID
			handlers.AddHandler(typeof(CommanEntry), typeof(MAUIEssentials.Platforms.Android.Handlers.CommanEntryHandler));
			handlers.AddHandler(typeof(CommanEditor), typeof(MAUIEssentials.Platforms.Android.Handlers.CommanEditorHandler));
			handlers.AddHandler(typeof(Shell), typeof(MAUIEssentials.Platforms.Android.Renderers.CustomShellRenderer));
			handlers.AddHandler(typeof(CustomListView), typeof(MAUIEssentials.Platforms.Android.Renderers.CustomListViewRenderer));
			handlers.AddHandler(typeof(DashedLine), typeof(MAUIEssentials.Platforms.Android.Renderers.CustomDashedLineRenderer));

			handlers.AddCompatibilityRenderer(typeof(CustomDatePicker), typeof(MAUIEssentials.Platforms.Android.Renderers.CustomDatePickerRenderer));
			handlers.AddCompatibilityRenderer(typeof(PickerView), typeof(MAUIEssentials.Platforms.Android.Renderers.CustomPickerViewRenderer));
#elif IOS
			handlers.AddHandler(typeof(CommanEntry), typeof(MAUIEssentials.Platforms.iOS.Handlers.CommanEntryHandler));
			handlers.AddHandler(typeof(CommanEditor), typeof(MAUIEssentials.Platforms.iOS.Handlers.CommanEditorHandler));
			handlers.AddHandler(typeof(CustomDatePicker), typeof(MAUIEssentials.Platforms.iOS.Handlers.CommanDatePickerHandler));
			handlers.AddHandler(typeof(Shell), typeof(MAUIEssentials.Platforms.iOS.Renderers.CustomShellRenderer));
			handlers.AddHandler(typeof(CustomListView), typeof(MAUIEssentials.Platforms.iOS.Renderers.CustomListViewRenderer));
			handlers.AddHandler(typeof(DashedLine), typeof(MAUIEssentials.Platforms.iOS.Renderers.CustomDashedLineRenderer));

			handlers.AddCompatibilityRenderer(typeof(PickerView), typeof(MAUIEssentials.Platforms.iOS.Renderers.CustomPickerViewRenderer));
#endif
		});

        return builder;
	}
}
