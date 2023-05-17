using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Akelon.MatrixCore.ApprovalRole;

namespace Akelon.MatrixCore.Server
{
  partial class ApprovalRoleFunctions
  {
    /// <summary>
    /// Вычислить исполнителей роли.
    /// </summary>
    /// <param name="task">Задача согласования по регламенту.</param>
    /// <returns>Список исполнителей.</returns>
    [Public]
    public override List<Sungero.Company.IEmployee> GetRolePerformers(Sungero.Docflow.IApprovalTask task)
    {
      var document = task.DocumentGroup.OfficialDocuments.FirstOrDefault();
      
      var recipients = Functions.Module.GetMatrixApprovalRoleRecipients(document, _obj.Type);
      return Sungero.Company.PublicFunctions.Module.GetEmployeesFromRecipients(recipients);
    }
  }
}