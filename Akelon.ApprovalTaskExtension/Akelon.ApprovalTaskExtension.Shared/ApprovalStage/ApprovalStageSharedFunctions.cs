using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Akelon.ApprovalTaskExtension.ApprovalStage;

namespace Akelon.ApprovalTaskExtension.Shared
{
  partial class ApprovalStageFunctions
  {
    /// <summary>
    /// Получить список ролей, доступных для этого этапа.
    /// </summary>
    /// <returns>Список ролей.</returns>
    public override List<Nullable<Enumeration>> GetPossibleRoles()
    {
      var baseRoles = base.GetPossibleRoles();
      baseRoles.Add(MatrixCore.ApprovalRole.Type.MatrixPerformer);
      
      return baseRoles;
    }
  }
}