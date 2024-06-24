using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Metadata;

namespace Avalonia.Platform
{
    public interface IScreenImpl : IEquatable<IScreenImpl>
    {
        string? DisplayName { get; }
        ScreenOrientation CurrentOrientation { get; }
        double Scaling { get; }
        PixelRect Bounds { get; }
        PixelRect WorkingArea { get; }
        bool IsPrimary { get; }
        IPlatformHandle? PlatformHandle { get; }
        IDisposable SubscribeOnChanges(IObserver<bool> observer);
    }
    
    [Unstable]
    public interface IScreensImpl
    {
        /// <summary>
        /// Gets the total number of screens available on the device.
        /// </summary>
        int ScreenCount { get; }

        /// <summary>
        /// Gets the list of all screens available on the device.
        /// </summary>
        IReadOnlyList<IScreenImpl> AllScreens { get; }

        Action? Changed { get; set; }
        
        /// <inheritdoc cref="Avalonia.Controls.Screens.ScreenFromWindow(IWindowBaseImpl)"/>
        IScreenImpl? ScreenFromWindow(IWindowBaseImpl window);

        /// <inheritdoc cref="Avalonia.Controls.Screens.ScreenFromTopLevel(Avalonia.Controls.TopLevel)"/>
        IScreenImpl? ScreenFromTopLevel(ITopLevelImpl topLevel);

        /// <inheritdoc cref="Avalonia.Controls.Screens.ScreenFromPoint"/>
        IScreenImpl? ScreenFromPoint(PixelPoint point);

        /// <inheritdoc cref="Avalonia.Controls.Screens.ScreenFromBounds"/>
        IScreenImpl? ScreenFromRect(PixelRect rect);

        /// <inheritdoc cref="Avalonia.Controls.Screens.RequestScreenDetails"/>
        Task<bool> RequestScreenDetails();
    }
}
