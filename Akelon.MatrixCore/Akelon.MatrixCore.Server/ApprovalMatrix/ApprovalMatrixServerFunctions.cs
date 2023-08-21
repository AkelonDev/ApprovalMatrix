using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Akelon.MatrixCore.ApprovalMatrix;

namespace Akelon.MatrixCore.Server
{
  partial class ApprovalMatrixFunctions
  {
    /// <summary>
    /// Получить ИД видов документов, у которых есть категории.
    /// </summary>
    /// <returns>Список ИД видов документов.</returns>
    [Remote]
    public static List<long> GetKindIdsWithGroup()
    {
      return Sungero.Docflow.DocumentGroupBases.GetAll().SelectMany(g => g.DocumentKinds.Select(r => r.DocumentKind.Id)).Distinct().ToList();
    }
  }
}