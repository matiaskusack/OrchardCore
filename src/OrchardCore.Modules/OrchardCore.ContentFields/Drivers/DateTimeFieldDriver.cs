using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentManagement.Display.Models;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Views;
using OrchardCore.Settings;

namespace OrchardCore.ContentFields.Drivers
{
    public class DateTimeFieldDriver
         : ContentFieldDisplayDriver<DateTimeField>
    {
        SiteSettings _siteSettings;
        public IStringLocalizer T { get; set; }

        public DateTimeFieldDriver(SiteSettings siteSettings, IStringLocalizer<DateTimeFieldDriver> localizer)
        {
            _siteSettings = siteSettings;
            T = localizer;
        }

        public override IDisplayResult Display(DateTimeField field, BuildFieldDisplayContext context)
        {
            // TODO Matías: show display shape
            return base.Display(field, context);
        }

        public override IDisplayResult Edit(DateTimeField field, BuildFieldEditorContext context)
        {
            // TODO Matías: show edit shape
            return base.Edit(field, context);
        }

        public override Task<IDisplayResult> UpdateAsync(DateTimeField field, IUpdateModel updater, UpdateFieldEditorContext context)
        {
            // TODO Matías: update
            return base.UpdateAsync(field, updater, context);
        }
    }
}
