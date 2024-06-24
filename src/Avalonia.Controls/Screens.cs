using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Metadata;
using Avalonia.Platform;

namespace Avalonia.Controls
{
    /// <summary>
    /// Represents all screens available on a device.
    /// </summary>
    public class Screens
    {
        private readonly IScreensImpl _iScreenImpl;

        /// <summary>
        /// Gets the total number of screens available on the device.
        /// </summary>
        public int ScreenCount => _iScreenImpl?.ScreenCount ?? 0;

        /// <summary>
        /// Gets the list of all screens available on the device.
        /// </summary>
        public IReadOnlyList<Screen> All => _iScreenImpl?.AllScreens ?? Array.Empty<Screen>();

        /// <summary>
        /// Gets the primary screen on the device.
        /// </summary>
        public Screen? Primary => All.FirstOrDefault(x => x.IsPrimary);

        /// <summary>
        /// Event raised when any screen was added or removed.
        /// </summary>
        public event EventHandler? Changed;

        /// <summary>
        /// Initializes a new instance of the <see cref="Screens"/> class.
        /// </summary>
        [PrivateApi]
        public Screens(IScreensImpl iScreenImpl)
        {
            _iScreenImpl = iScreenImpl;
        }

        /// <summary>
        /// Retrieves a Screen for the display that contains the rectangle.
        /// </summary>
        /// <param name="bounds">Bounds that specifies the area for which to retrieve the display.</param>
        /// <returns>The <see cref="Screen"/>.</returns>
        public Screen? ScreenFromBounds(PixelRect bounds)
        {
            return _iScreenImpl.ScreenFromRect(bounds);
        }

        /// <summary>
        /// Retrieves a Screen for the display that contains the specified <see cref="WindowBase"/>.
        /// </summary>
        /// <param name="window">The window for which to retrieve the Screen.</param>
        /// <exception cref="ObjectDisposedException">Window platform implementation was already disposed.</exception>
        /// <returns>The <see cref="Screen"/>.</returns>
        public Screen? ScreenFromWindow(WindowBase window)
        {
            if (window.PlatformImpl is null)
            {
                throw new ObjectDisposedException("Window platform implementation was already disposed.");
            }

            return _iScreenImpl.ScreenFromWindow(window.PlatformImpl);
        }

        /// <summary>
        /// Retrieves a Screen for the display that contains the specified <see cref="TopLevel"/>.
        /// </summary>
        /// <param name="topLevel">The top level for which to retrieve the Screen.</param>
        /// <exception cref="ObjectDisposedException">TopLevel platform implementation was already disposed.</exception>
        /// <returns>The <see cref="Screen"/>.</returns>
        public Screen? ScreenFromTopLevel(TopLevel topLevel)
        {
            if (topLevel.PlatformImpl is null)
            {
                throw new ObjectDisposedException("Window platform implementation was already disposed.");
            }

            return _iScreenImpl.ScreenFromTopLevel(topLevel.PlatformImpl);
        }

        /// <summary>
        /// Retrieves a Screen for the display that contains the specified <see cref="IWindowBaseImpl"/>.
        /// </summary>
        /// <param name="window">The window impl for which to retrieve the Screen.</param>
        /// <returns>The <see cref="Screen"/>.</returns>
        [Obsolete("Use ScreenFromWindow(WindowBase) overload.", true), EditorBrowsable(EditorBrowsableState.Never)]
        public Screen? ScreenFromWindow(IWindowBaseImpl window)
        {
            return _iScreenImpl.ScreenFromWindow(window);
        }

        /// <summary>
        /// Retrieves a Screen for the display that contains the specified point.
        /// </summary>
        /// <param name="point">A Point that specifies the location for which to retrieve a Screen.</param>
        /// <returns>The <see cref="Screen"/>.</returns>
        public Screen? ScreenFromPoint(PixelPoint point)
        {
            return _iScreenImpl.ScreenFromPoint(point);
        }

        /// <summary>
        /// Retrieves a Screen for the display that contains the specified <see cref="Visual"/>.
        /// </summary>
        /// <param name="visual">A Visual for which to retrieve a Screen.</param>
        /// <returns>The <see cref="Screen"/>.</returns>
        public Screen? ScreenFromVisual(Visual visual)
        {
            var tl = visual.PointToScreen(visual.Bounds.TopLeft);
            var br = visual.PointToScreen(visual.Bounds.BottomRight);

            return ScreenFromBounds(new PixelRect(tl, br));
        }

        /// <summary>
        /// Asks underlying platform to provide detailed screen information.
        /// On some platforms it might include non-primary screens, as well as display names.
        /// </summary>
        /// <remarks>
        /// This method is async and might show a dialog to the user asking for a permission.
        /// </remarks>
        /// <returns>True, if detailed screen information was provided. False, if denied by the platform or user.</returns>
        public Task<bool> RequestScreenDetails() => _iScreenImpl.RequestScreenDetails();
    }
}
