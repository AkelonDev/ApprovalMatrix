using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Akelon.MatrixCore.ApprovalMatrix;

namespace Akelon.MatrixCore
{
  partial class ApprovalMatrixSharedHandlers
  {

    public virtual void ForNoCodeChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      _obj.ApprovalRole = null;
    }

    public virtual void DocumentKindsChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      _obj.Categories.Clear();
    }

    public virtual void BusinessUnitsChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      _obj.Departments.Clear();
    }

    public virtual void DepartmentsChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      _obj.JobTitles.Clear();
    }
  }
}