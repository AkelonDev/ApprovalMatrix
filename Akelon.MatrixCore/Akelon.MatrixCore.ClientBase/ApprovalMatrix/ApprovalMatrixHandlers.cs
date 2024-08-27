using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Akelon.MatrixCore.ApprovalMatrix;

namespace Akelon.MatrixCore
{
  partial class ApprovalMatrixClientHandlers
  {

    public override void Closing(Sungero.Presentation.FormClosingEventArgs e)
    {
      if (e.Params.Contains(Constants.ApprovalMatrix.KindIdsWithGroupParam))
        e.Params.Remove(Constants.ApprovalMatrix.KindIdsWithGroupParam);
    }

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      List<long> kindIdsWithGroup;
      e.Params.TryGetValue(Constants.ApprovalMatrix.KindIdsWithGroupParam, out kindIdsWithGroup);
      
      if (kindIdsWithGroup == null)
      {
        kindIdsWithGroup = Functions.ApprovalMatrix.Remote.GetKindIdsWithGroup();
        e.Params.AddOrUpdate(Constants.ApprovalMatrix.KindIdsWithGroupParam, kindIdsWithGroup);
      }
      var properties = _obj.State.Properties;
      properties.Categories.IsVisible = _obj.DocumentKinds.Any(r => kindIdsWithGroup.Contains(r.DocumentKind.Id));
      properties.ApprovalRole.IsRequired = _obj.ForNoCode != true;
      properties.ApprovalRole.IsVisible = _obj.ForNoCode != true;
    }
  }
}