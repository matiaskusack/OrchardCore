using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentFields.Settings;
using OrchardCore.ContentFields.ViewModels;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentManagement.Display.Models;
using OrchardCore.ContentManagement.Metadata.Models;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Views;
using OrchardCore.Settings;
using OrchardCore.ContentFields.Extensions;

namespace OrchardCore.ContentFields.Drivers
{
    public class DateTimeFieldDriver
         : ContentFieldDisplayDriver<DateTimeField>
    {
        public IStringLocalizer T { get; set; }

        public DateTimeFieldDriver(IStringLocalizer<DateTimeFieldDriver> localizer)
        {
            T = localizer;
        }

        public override IDisplayResult Display(DateTimeField field, BuildFieldDisplayContext context)
        {
            return Shape<DisplayDateTimeFieldViewModel>("DateTimeField", model =>
            {
                model.Field = field;
                model.Part = context.ContentPart;
                model.PartFieldDefinition = context.PartFieldDefinition;
            }).Location("Content").Location("SummaryAdmin", "");
        }

        public override IDisplayResult Edit(DateTimeField field, BuildFieldEditorContext context)
        {
            return Shape<EditDateTimeFieldViewModel>("DateTimeField_Edit", model =>
            {
                var settings = context.PartFieldDefinition.Settings.ToObject<DateTimeFieldSettings>();
                var dateValue = context.IsNew ? DateTime.Now : field.Value;
                model.Value = dateValue.HasValue ? dateValue.Value.ToHtmlValueString(settings) : string.Empty;

                model.Field = field;
                model.Part = context.ContentPart;
                model.PartFieldDefinition = context.PartFieldDefinition;
            });
        }

        public override async Task<IDisplayResult> UpdateAsync(DateTimeField field, IUpdateModel updater, UpdateFieldEditorContext context)
        {
            var viewModel = new EditDateTimeFieldViewModel();

            bool modelUpdated = await updater.TryUpdateModelAsync(viewModel, Prefix, f => f.Value);

            if (!modelUpdated)
                return Edit(field, context);

            DateTime value;
            field.Value = null;
            var settings = context.PartFieldDefinition.Settings.ToObject<DateTimeFieldSettings>();

            if (string.IsNullOrWhiteSpace(viewModel.Value) && settings.Required)
            {
                updater.ModelState.AddModelError(Prefix, T["The {0} field is required.", context.PartFieldDefinition.DisplayName()]);
                return Edit(field, context);
            }
            if (!DateTime.TryParse(viewModel.Value, out value))
            {
                updater.ModelState.AddModelError(Prefix, T["{0} is an invalid date.", context.PartFieldDefinition.DisplayName()]);
                return Edit(field, context);
            }

            field.Value = value;
            if (settings.Minimum.HasValue && value < settings.Minimum.Value)
                updater.ModelState.AddModelError(Prefix, T["The value must be greater than {0}.", settings.Minimum.Value]);

            if (settings.Maximum.HasValue && value > settings.Maximum.Value)
                updater.ModelState.AddModelError(Prefix, T["The value must be less than {0}.", settings.Maximum.Value]);

            return Edit(field, context);
        }
    }
}
