using System;
using System.Collections.Generic;
using System.Text;
using OrchardCore.ContentManagement;

namespace OrchardCore.ContentFields.Fields
{
    public class DateTimeField
        : ContentField
    {
        public DateTime? Value { get; set; }
    }
}
