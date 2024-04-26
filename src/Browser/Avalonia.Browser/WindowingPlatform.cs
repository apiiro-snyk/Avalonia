using System;
using System.Reflection;
using System.Threading;
using Avalonia.Browser.Interop;
using Avalonia.Browser.Skia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Platform;
using Avalonia.Rendering;
using Avalonia.Threading;

namespace Avalonia.Browser;

internal class BrowserWindowingPlatform : IWindowingPlatform
{
    internal static readonly bool IsThreadingEnabled = DetectThreadSupport();
    
    static bool DetectThreadSupport()
    {
        var prop = typeof(System.Threading.Thread).GetProperty("IsThreadStartSupported",
            BindingFlags.Static | BindingFlags.NonPublic);
        if (prop != null && prop.GetValue(null) is bool value)
            return value;
        // No property is found, try to start a thread and get an exception in the face if threads aren't available
        try
        {
#pragma warning disable CA1416
            new Thread(() => { }).Start();
#pragma warning restore CA1416
            return true;
        }
        catch
        {
            return false;
        }

    }
    
    private static KeyboardDevice? s_keyboard;

    public IWindowImpl CreateWindow() => throw new NotSupportedException("Browser doesn't support windowing platform. In order to display a single-view content, set ISingleViewApplicationLifetime.MainView.");

    IWindowImpl IWindowingPlatform.CreateEmbeddableWindow()
    {
        throw new NotImplementedException("Browser doesn't support embeddable windowing platform.");
    }

    public ITrayIconImpl? CreateTrayIcon()
    {
        return null;
    }

    public static KeyboardDevice Keyboard => s_keyboard ??
        throw new InvalidOperationException("BrowserWindowingPlatform not registered.");

    public static void Register()
    {
        var instance = new BrowserWindowingPlatform();

        s_keyboard = new KeyboardDevice();
        AvaloniaLocator.CurrentMutable
            .Bind<IRuntimePlatform>().ToSingleton<BrowserRuntimePlatform>()
            .Bind<ICursorFactory>().ToSingleton<CssCursorFactory>()
            .Bind<IKeyboardDevice>().ToConstant(s_keyboard)
            .Bind<IPlatformSettings>().ToSingleton<BrowserPlatformSettings>()
            .Bind<IDispatcherImpl>().ToSingleton<BrowserDispatcherImpl>()
            .Bind<IRenderTimer>().ToConstant(ManualTriggerRenderTimer.Instance)
            .Bind<IWindowingPlatform>().ToConstant(instance)
            .Bind<IPlatformGraphics>().ToConstant(new BrowserSkiaGraphics())
            .Bind<IPlatformIconLoader>().ToSingleton<IconLoaderStub>()
            .Bind<PlatformHotkeyConfiguration>().ToSingleton<PlatformHotkeyConfiguration>()
            .Bind<IActivatableLifetime>().ToSingleton<BrowserActivatableLifetime>();

        if (AvaloniaLocator.Current.GetService<BrowserPlatformOptions>() is { } options
            && options.RegisterAvaloniaServiceWorker)
        {
            var swPath = AvaloniaModule.ResolveServiceWorkerPath();
            AvaloniaModule.RegisterServiceWorker(swPath, options.AvaloniaServiceWorkerScope);
        }
    }
}
