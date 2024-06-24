using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Native.Interop;
using Avalonia.Platform;

namespace Avalonia.Native
{
    internal class ScreenImpl : IScreensImpl, IDisposable
    {
        private IAvnScreens _native;

        public ScreenImpl(IAvnScreens native)
        {
            _native = native;
        }

        public int ScreenCount => _native.ScreenCount;

        public IReadOnlyList<Screen> AllScreens
        {
            get
            {
                if (_native != null)
                {
                    var count = ScreenCount;
                    var result = new Screen[count];

                    for (int i = 0; i < count; i++)
                    {
                        var screen = _native.GetScreen(i);

                        result[i] = new Screen(
                            screen.Scaling,
                            screen.Bounds.ToAvaloniaPixelRect(),
                            screen.WorkingArea.ToAvaloniaPixelRect(),
                            screen.IsPrimary.FromComBool());
                    }

                    return result;
                }

                return Array.Empty<Screen>();
            }
        }

        public Action Changed { get; set; }

        public void Dispose ()
        {
            _native?.Dispose();
            _native = null;
        }

        public Screen ScreenFromTopLevel(ITopLevelImpl topLevel)
        {
            throw new NotImplementedException();
        }

        public Screen ScreenFromPoint(PixelPoint point)
        {
            return ScreenHelper.ScreenFromPoint(point, AllScreens);
        }

        public Screen ScreenFromRect(PixelRect rect)
        {
            return ScreenHelper.ScreenFromRect(rect, AllScreens);
        }

        public Task<bool> RequestScreenDetails() => Task.FromResult(true);

        public Screen ScreenFromWindow(IWindowBaseImpl window)
        {
            return ScreenHelper.ScreenFromWindow(window, AllScreens);
        }
    }
}
