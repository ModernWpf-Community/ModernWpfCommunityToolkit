using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace ModernWpf.Toolkit.UI.Extensions
{
    public static class DependencyObjectExtensions
    {
        private static Dictionary<long, (DependencyPropertyDescriptor Descriptor, EventHandler Handler)> descriptors = new Dictionary<long, (DependencyPropertyDescriptor Descriptor, EventHandler Handler)>();

        public static long RegisterPropertyChangedCallback(this DependencyObject dependencyObject, DependencyProperty dependencyProperty, PropertyChangedCallback callback)
        {
            long token = new Random().Next(0, int.MaxValue);
            var descriptor = DependencyPropertyDescriptor.FromProperty(dependencyProperty, dependencyObject.GetType());

            var oldValue = dependencyObject.GetValue(dependencyProperty);
            EventHandler handler = (s, e) =>
            {
                var newValue = dependencyObject.GetValue(dependencyProperty);
                callback.Invoke(dependencyObject, new DependencyPropertyChangedEventArgs(dependencyProperty, oldValue, newValue));
                oldValue = newValue;
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
