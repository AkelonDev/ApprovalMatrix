using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Akelon.ApprovalTaskExtension.ApprovalStage;

namespace Akelon.ApprovalTaskExtension.Server
{
  partial class ApprovalStageFunctions
  {
    /// <summary>
    /// Получить исполнителей этапа без раскрытия групп и ролей.
    /// </summary>
    /// <param name="task">Задача.</param>
    /// <param name="additionalApprovers">Доп.согласующие.</param>
    /// <returns>Исполнители.</returns>
    [Remote(IsPure = true), Public]
    public override List<IRecipient> GetStageRecipients(Sungero.Docflow.IApprovalTask task, List<IRecipient> additionalApprovers)
    {
      var recipients =  base.GetStageRecipients(task, additionalApprovers);
      
      _obj.ApprovalRoles.Select(x => MatrixCore.ApprovalRoles.As(x.ApprovalRole))
        .Where(x => x != null).ToList()
        .ForEach(role =>
                 {
                   recipients.AddRange(MatrixCore.PublicFunctions.ApprovalRole.GetRolePerformers(role, task));
                 });

      return recipients;
    }
  }
}