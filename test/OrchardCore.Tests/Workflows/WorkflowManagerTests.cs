using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Linq;
using OrchardCore.Scripting;
using OrchardCore.Scripting.JavaScript;
using OrchardCore.Tests.Workflows.Activities;
using OrchardCore.Workflows.Activities;
using OrchardCore.Workflows.Models;
using OrchardCore.Workflows.Services;
using Xunit;

namespace OrchardCore.Tests.Workflows
{
    public class WorkflowManagerTests
    {
        [Fact]
        public async Task CanExecuteSimpleWorkflow()
        {
            var localizer = new Mock<IStringLocalizer>();

            var stringBuilder = new StringBuilder();
            var output = new StringWriter(stringBuilder);
            var addTask = new AddTask(localizer.Object);
            var writeLineTask = new WriteLineTask(localizer.Object, output);
            var workflowDefinition = new WorkflowDefinitionRecord
            {
                Id = 1,
                Activities = new List<ActivityRecord>
                {
                    new ActivityRecord { Id = 1, IsStart = true, Name = addTask.Name, Properties = JObject.FromObject( new
                    {
                        A = new WorkflowExpression<double>("js: workflow().Input[\"A\"]"),
                        B = new WorkflowExpression<double>("js: input(\"B\")"),
                    }) },
                    new ActivityRecord { Id = 2, Name = writeLineTask.Name, Properties = JObject.FromObject( new { Text = new WorkflowExpression<string>("js: result().toString()") }) }
                },
                Transitions = new List<TransitionRecord>
                {
                    new TransitionRecord{ SourceActivityId = 1, SourceOutcomeName = "Done", DestinationActivityId = 2 }
                }
            };

            var workflowManager = CreateWorkflowManager(new IActivity[] { addTask, writeLineTask }, workflowDefinition);
            var a = 10d;
            var b = 22d;
            var expectedResult = (a + b).ToString() + "\r\n";

            var workflowContext = await workflowManager.StartWorkflowAsync(workflowDefinition, new { A = a, B = b });
            var actualResult = stringBuilder.ToString();

            Assert.Equal(expectedResult, actualResult);
        }

        private WorkflowManager CreateWorkflowManager(IEnumerable<IActivity> activities, WorkflowDefinitionRecord workflowDefinition)
        {
            var serviceCollection = new ServiceCollection();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var javaScriptEngine = new JavaScriptEngine(memoryCache, new Mock<IStringLocalizer<JavaScriptEngine>>().Object);
            var globalMethodProviders = new IGlobalMethodProvider[0];
            var scriptingManager = new DefaultScriptingManager(new[] { javaScriptEngine }, globalMethodProviders, serviceProvider);
            var activityLibrary = new Mock<IActivityLibrary>();
            var workflowDefinitionRepository = new Mock<IWorkflowDefinitionRepository>();
            var workflowInstanceRepository = new Mock<IWorkflowInstanceRepository>();
            var workflowContextHandlers = new IWorkflowContextHandler[0];
            var workflowManagerLogger = new Mock<ILogger<WorkflowManager>>();
            var workflowContextLogger = new Mock<ILogger<WorkflowContext>>();
            var missingActivityLogger = new Mock<ILogger<MissingActivity>>();
            var missingActivityLocalizer = new Mock<IStringLocalizer<MissingActivity>>();
            var workflowManager = new WorkflowManager(
                activityLibrary.Object,
                workflowDefinitionRepository.Object,
                workflowInstanceRepository.Object,
                scriptingManager,
                workflowContextHandlers,
                workflowManagerLogger.Object,
                workflowContextLogger.Object,
                missingActivityLogger.Object,
                missingActivityLocalizer.Object);

            foreach (var activity in activities)
            {
                activityLibrary.Setup(x => x.InstantiateActivity(activity.Name)).Returns(activity);
            }

            workflowDefinitionRepository.Setup(x => x.GetWorkflowDefinitionAsync(workflowDefinition.Id)).Returns(Task.FromResult(workflowDefinition));

            return workflowManager;
        }
    }
}