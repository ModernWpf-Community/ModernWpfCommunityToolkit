using System;

namespace ModernWpf.Toolkit.UI.Controls
{
    public sealed class ImageResolvingDeferral
    {
        private readonly Action _handler;

        internal ImageResolvingDeferral(Action handler)
        {
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public void Complete()
        {
            _handler();
        }
    }
}
