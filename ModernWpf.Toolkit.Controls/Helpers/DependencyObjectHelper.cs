using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace ModernWpf.Toolkit.Controls.Helpers
{
    public static class DependencyObjectHelper
    {
        private static Dictionary<long, (DependencyPropertyDescriptor Descriptor, EventHandler Handler)> descriptors = new Dictionary<long, (DependencyPropertyDescriptor Descriptor, EventHandler Handler)>();

        public static long RegisterPropertyChangedCallback(this DependencyObject dependencyObject, DependencyProperty dependencyProperty, PropertyChangedCallback callback)
        {
            long token = new Random().Next(0, int.MaxValue);
            var descriptor = DependencyPropertyDescriptor.FromProperty(dependencyProperty, dependencyObject.GetType());

            EventHandler handler = (s, e) =>
            {
                callback.Invoke(dependencyObject, new DependencyPropertyChangedEventArgs());
            };

            while (descriptors.ContainsKey(token))
            {
                token += 1;
            }

            descriptors.Add(token, (descriptor, handler));

            descriptor.AddValueChanged(dependencyObject, handler);
            return token;
        }

        public static void UnregisterPropertyChangedCallback(this DependencyObject dependencyObject, DependencyProperty dependencyProperty, long token)
        {
            if (descriptors.TryGetValue(token, out (DependencyPropertyDescriptor Descriptor, EventHandler Handler) value))
            {
                value.Descriptor.RemoveValueChanged(dependencyObject, value.Handler);
                descriptors.Remove(token);
            }
        }
    }
}
