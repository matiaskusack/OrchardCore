using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using OrchardCore.ContentFields.Fields;
using OrchardCore.Indexing;

namespace OrchardCore.ContentFields.Indexing
{
    public class DateTimeFieldIndexHandler
        : ContentFieldIndexHandler<DateTimeField>
    {
        public override Task BuildIndexAsync(DateTimeField field, BuildFieldIndexContext context)
        {
            var settings = context.ContentPartFieldDefinition.Settings.ToObject<DateTimeField>();
            var options = context.Settings.ToOptions();
            
            context.DocumentIndex.Entries.Add(context.Key, new DocumentIndex.DocumentIndexEntry(field.Value, DocumentIndex.Types.DateTime, options));
            return Task.CompletedTask;
        }
    }
}
