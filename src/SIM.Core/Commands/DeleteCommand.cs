namespace SIM.Core.Commands
{
  using System.Collections.Generic;
  using Sitecore.Diagnostics.Base;
  using SIM.Core.Common;
  using SIM.Instances;
  using SIM.Pipelines;
  using SIM.Pipelines.Delete;

  public class DeleteCommand : AbstractMultiInstanceActionCommand
  {
    protected override void DoExecute(IReadOnlyList<Instance> instances, CommandResult result)
    {
      Assert.ArgumentNotNull(instances, nameof(instances));
      Assert.ArgumentNotNull(result, nameof(result));
                                       
      var profile = Profile.Read();
      var connectionString = profile.GetValidConnectionString();

      PipelineManager.Initialize(XmlDocumentEx.LoadXml(PipelinesConfig.Contents).DocumentElement);

      var fail = false;
      foreach (var instance in instances)
      {
        Assert.IsNotNull(instance, nameof(instance));

        var deleteArgs = new DeleteArgs(instance, connectionString);
        var controller = new AggregatePipelineController();
        PipelineManager.StartPipeline("delete", deleteArgs, controller, false);

        fail = fail || !string.IsNullOrEmpty(controller.Message);
      }

      result.Success = !fail;
      result.Message = "Done.";    
    }
  }
}