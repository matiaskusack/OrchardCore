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

namespace OrchardCore.ContentFields.Drivers
{
    public class DateTimeFieldDriver
         : ContentFieldDisplayDriver<DateTimeField>
    {
        ISiteService _siteSettings;
        public IStringLocalizer T { get; set; }

        System.Globalization.CultureInfo _cultureInfo = null;
        public System.Globalization.CultureInfo Culture
        {
            get
            {
                if (_cultureInfo == null)
                {
                    var culture = _siteSettings.GetSiteSettingsAsync().GetAwaiter().GetResult().Culture;
                    // Site settings provides a Null culture. So we use invariant
                    if (string.IsNullOrEmpty(culture))
                        return System.Globalization.CultureInfo.InvariantCulture;
                    _cultureInfo = new System.Globalization.CultureInfo(culture);
                }
                return _cultureInfo;
            }
        }

        public DateTimeFieldDriver(ISiteService siteService, IStringLocalizer<DateTimeFieldDriver> localizer)
        {
            _siteSettings = siteService;
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
                model.Value = Format(context.IsNew ? DateTime.Now : field.Value, settings);

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

            if (!DateTime.TryParseExact(viewModel.Value, GetFormat(settings), Culture, System.Globalization.DateTimeStyles.None, out value))
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

        private string Format(DateTime? dateTime, DateTimeFieldSettings settings)
        {
            if (!dateTime.HasValue)
                return string.Empty;
            var format = Culture.DateTimeFormat.ShortDatePattern + (settings.WithTime ? $" {Culture.DateTimeFormat.ShortTimePattern}" : string.Empty);
            return dateTime.Value.ToString(format);
        }

        private string GetFormat(DateTimeFieldSettings settings)
        {
            return settings.WithTime ? Culture.DateTimeFormat.FullDateTimePattern : Culture.DateTimeFormat.ShortDatePattern;
        }
    }
}
