using System;
using System.ComponentModel;
using Avalonia.Diagnostics;
using Avalonia.Metadata;

namespace Avalonia.Platform
{
    /// <summary>
    /// Describes the orientation of a screen.
    /// </summary>
    public enum ScreenOrientation
    {
        /// <summary>
        /// No screen orientation is specified.
        /// </summary>
        None,

        /// <summary>
        /// Specifies that the monitor is oriented in landscape mode where the width of the screen viewing area is greater than the height.
        /// </summary>
        Landscape = 1,

        /// <summary>
        /// Specifies that the monitor rotated 90 degrees in the clockwise direction to orient the screen in portrait mode
        /// where the height of the screen viewing area is greater than the width.
        /// </summary>
        Portrait = 2,

        /// <summary>
        /// Specifies that the monitor rotated another 90 degrees in the clockwise direction (to equal 180 degrees) to orient the screen in landscape mode
        /// where the width of the screen viewing area is greater than the height.
        /// This landscape mode is flipped 180 degrees from the Landscape mode.
        /// </summary>
        LandscapeFlipped = 4,

        /// <summary>
        /// Specifies that the monitor rotated another 90 degrees in the clockwise direction (to equal 270 degrees) to orient the screen in portrait mode
        /// where the height of the screen viewing area is greater than the width. This portrait mode is flipped 180 degrees from the Portrait mode.
        /// </summary>
        PortraitFlipped = 8
    }

    /// <summary>
    /// Represents a single display screen.
    /// </summary>
    public class Screen : IEquatable<Screen>
    {
        private readonly IScreenImpl _impl;

        public string? DisplayName => _impl.DisplayName;

        /// <summary>
        /// Gets the current orientation of a screen.
        /// </summary>
        public ScreenOrientation CurrentOrientation => _impl.CurrentOrientation;

        /// <summary>
        /// Gets the scaling factor applied to the screen by the operating system.
        /// </summary>
        /// <remarks>
        /// Multiply this value by 100 to get a percentage.
        /// Both X and Y scaling factors are assumed uniform.
        /// </remarks>
        public double Scaling => _impl.Scaling;

        /// <inheritdoc cref="Scaling"/>
        [Obsolete("Use the Scaling property instead."), EditorBrowsable(EditorBrowsableState.Never)]
        public double PixelDensity => Scaling;

        /// <summary>
        /// Gets the overall pixel-size of the screen.
        /// </summary>
        /// <remarks>
        /// This generally is the raw pixel counts in both the X and Y direction.
        /// </remarks>
        public PixelRect Bounds => _impl.Bounds;

        /// <summary>
        /// Gets the actual working-area pixel-size of the screen.
        /// </summary>
        /// <remarks>
        /// This area may be smaller than <see href="Bounds"/> to account for notches and
        /// other block-out areas such as taskbars etc.
        /// </remarks>
        public PixelRect WorkingArea => _impl.WorkingArea;

        /// <summary>
        /// Gets a value indicating whether the screen is the primary one.
        /// </summary>
        public bool IsPrimary => _impl.IsPrimary;

        /// <inheritdoc cref="IsPrimary"/>
        [Obsolete("Use the IsPrimary property instead."), EditorBrowsable(EditorBrowsableState.Never)]
        public bool Primary => IsPrimary;

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler? Changed;

        [PrivateApi]
        public Screen(IScreenImpl impl)
        {
            _impl = impl;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Screen"/> class.
        /// </summary>
        /// <param name="scaling">The scaling factor applied to the screen by the operating system.</param>
        /// <param name="bounds">The overall pixel-size of the screen.</param>
        /// <param name="workingArea">The actual working-area pixel-size of the screen.</param>
        /// <param name="isPrimary">Whether the screen is the primary one.</param>
        [Unstable(ObsoletionMessages.MayBeRemovedInAvalonia12)]
        public Screen(double scaling, PixelRect bounds, PixelRect workingArea, bool isPrimary)
            : this(new ManagedScreen(null, scaling, bounds, workingArea, isPrimary))
        {
        }

        /// <summary>
        /// Tries to get the platform handle for the Screen.
        /// </summary>
        /// <returns>
        /// An <see cref="IPlatformHandle"/> describing the screen handle, or null if the handle
        /// could not be retrieved.
        /// </returns>
        public IPlatformHandle? TryGetPlatformHandle() => _impl.PlatformHandle;

        /// <inheritdoc/>
        public bool Equals(Screen? other)
        {
            return other?._impl.Equals(_impl) ?? false;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return obj is Screen screen && screen.Equals(this);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return _impl.GetHashCode();
        }

        /// An immutable screen implementation for backwards compatibility. May be removed in the future. 
        private sealed record ManagedScreen(
            string? DisplayName, double Scaling, PixelRect Bounds, PixelRect WorkingArea, bool IsPrimary)
            : IScreenImpl
        {
            public ScreenOrientation CurrentOrientation => ScreenOrientation.None;
            public IPlatformHandle? PlatformHandle => null;
            public bool Equals(IScreenImpl? other) => Equals((object?)other);
        }
    }
}
