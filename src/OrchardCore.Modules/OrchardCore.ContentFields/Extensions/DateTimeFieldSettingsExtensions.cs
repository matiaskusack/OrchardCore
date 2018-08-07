using System;
using System.Collections.Generic;
using System.Text;
using OrchardCore.ContentFields.Settings;

namespace OrchardCore.ContentFields.Extensions
{
    public static class DateTimeFieldSettingsExtensions
    {
        /// <summary>
        /// Returns the format string in order to render an HTML5
        /// value for an input of type datetime.
        /// </summary>
        /// <param name="settings">Field settings</param>
        /// <returns>A format string witch can be used to format a DateTime value.</returns>
        public static string GetDateTimeFormat(this DateTimeFieldSettings settings)
            => settings.WithTime ? "yyyy-MM-dd\\THH:mm" : "yyyy-MM-dd";

        /// <summary>
        /// Converts a datetime value to an HTML5 value 
        /// for an input of type datetime using settings provided.
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="settings">Field settings</param>
        /// <returns>A string that can be used to format a DateTime value.</returns>
        public static string ToHtmlValueString(this DateTime value, DateTimeFieldSettings settings)
            => value.ToString(settings.GetDateTimeFormat());
    }
}
