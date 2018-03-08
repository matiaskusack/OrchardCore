using System;
using System.Collections.Generic;
using System.Text;

namespace OrchardCore.ContentFields.Settings
{
    public class DateTimeFieldSettings
    {
        public string Hint { get; set; }
        public bool Required { get; set; }

        public bool WithTime { get; set; }

        public DateTime? Minimum { get; set; }
        public DateTime? Maximum { get; set; }
    }
}
