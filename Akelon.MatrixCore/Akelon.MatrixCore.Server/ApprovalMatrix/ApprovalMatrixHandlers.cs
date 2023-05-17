using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Akelon.MatrixCore.ApprovalMatrix;

namespace Akelon.MatrixCore
{
  partial class ApprovalMatrixCategoriesCategoryPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> CategoriesCategoryFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (!_root.DocumentKinds.Any())
        return query.Where(t => false);
      
      var kinds = _root.DocumentKinds.Select(k => k.DocumentKind).ToList();
      return query.Where(t => !t.DocumentKinds.Any() || t.DocumentKinds.Any(k => kinds.Contains(k.DocumentKind)));
    }
  }

  partial class ApprovalMatrixDepartmentsDepartmentPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> DepartmentsDepartmentFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (!_root.BusinessUnits.Any())
        return query;
      
      var units = _root.BusinessUnits.Select(u => u.BusinessUnit).ToList();
      return query.Where(d => units.Contains(d.BusinessUnit));
    }
  }

  partial class ApprovalMatrixJobTitlesJobTitlePropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> JobTitlesJobTitleFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (!_root.Departments.Any())
        return query;
      
      var departments = _root.Departments.Select(d => d.Department).ToList();
      return query.Where(t => t.Department == null || departments.Contains(t.Department));
    }
  }

  partial class ApprovalMatrixMembersMemberPropertyFilteringServerHandler<T>
  {
    public virtual IQueryable<T> MembersMemberFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      var members = _root.Members.Where(m => !Equals(_obj.Member, m.Member) && m.Member != null).Select(m => m.Member).ToList();
      return query.Where(m => !members.Contains(m));
    }
  }

  partial class ApprovalMatrixServerHandlers
  {
    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      // Очистка пустых строк.
      _obj.Members.Where(m => m.Member == null)
        .ToList()
        .ForEach(m => _obj.Members.Remove(m));
    }
  }
}