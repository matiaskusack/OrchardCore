using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Fluid;
using Fluid.Ast;
using OrchardCore.DisplayManagement.Liquid.Ast;
using OrchardCore.DisplayManagement.Shapes;

namespace OrchardCore.DisplayManagement.Liquid.Tags
{
    public class RemoveItemTag : ExpressionArgumentsTag
    {
        public override async Task<Completion> WriteToAsync(TextWriter writer, TextEncoder encoder, TemplateContext context, Expression expression, FilterArgument[] args)
        {
            var objectValue = (await expression.EvaluateAsync(context)).ToObjectValue();

            if (objectValue is Shape shape && shape.Items != null)
            {
                var arguments = (FilterArguments)(await new ArgumentsExpression(args).EvaluateAsync(context)).ToObjectValue();
                shape.Remove(arguments["item"].Or(arguments.At(0)).ToStringValue());
            }

            return Completion.Normal;
        }
    }
}