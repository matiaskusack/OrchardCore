using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentManagement.Metadata.Models;
using OrchardCore.ContentTypes.Editors;
using OrchardCore.DisplayManagement.Views;

namespace OrchardCore.ContentFields.Settings
{
    public class DateTimeFieldSettingsDriver
        : ContentPartFieldDefinitionDisplayDriver<DateTimeField>
    {
        public override IDisplayResult Edit(ContentPartFieldDefinition model)
        {
            return base.Edit(model);
        }

        public override Task<IDisplayResult> UpdateAsync(ContentPartFieldDefinition model, UpdatePartFieldEditorContext context)
        {
            return base.UpdateAsync(model, context);
        }
    }
}
