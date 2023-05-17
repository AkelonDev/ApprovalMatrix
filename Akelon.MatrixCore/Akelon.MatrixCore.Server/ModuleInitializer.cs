using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace Akelon.MatrixCore.Server
{
  public partial class ModuleInitializer
  {

    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      // Создать роль согласования.
      CreateApprovalRole(MatrixCore.ApprovalRole.Type.MatrixPerformer);
      // Создать роль с правами к модулю.
      CreateModuleRole();
      // Выдать права на справочники.
      InitializationLogger.Debug("Init: Grant rights on databooks.");
      GrantRightsOnDatabooks();
      // Выдать права на специальные папки.
      InitializationLogger.Debug("Init: Grant rights on folders.");
      GrantRightsOnFolders();
    }
    
    /// <summary>
    /// Выдать права на специальные папки.
    /// </summary>
    public void GrantRightsOnFolders()
    {
      var moduleRole = Roles.GetAll(p => p.Sid == Constants.Module.ModuleRole).FirstOrDefault();
      
      ApprovalTaskExtensionUI.SpecialFolders.ApprovalMatrices.AccessRights.Grant(moduleRole, DefaultAccessRightsTypes.Read);
      ApprovalTaskExtensionUI.SpecialFolders.ApprovalMatrices.AccessRights.Save();
    }
    
    /// <summary>
    /// Выдача прав на справочники.
    /// </summary>
    public void GrantRightsOnDatabooks()
    {
      var moduleRole = Roles.GetAll(p => p.Sid == Constants.Module.ModuleRole).FirstOrDefault();
      var allUsers = Roles.AllUsers;
      
      ApprovalMatrices.AccessRights.Grant(moduleRole, DefaultAccessRightsTypes.FullAccess);
      ApprovalMatrices.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Read);
			ApprovalMatrices.AccessRights.Save();
			
			ApprovalRoles.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Read);
			ApprovalRoles.AccessRights.Save();
    }
    
    /// <summary>
    /// Создать роль модуля.
    /// </summary>
    public void CreateModuleRole()
    {
      // Пользователи модуля
      var roleGuid = Constants.Module.ModuleRole;
      
      var role = Sungero.CoreEntities.Roles.GetAll(p => p.Sid == roleGuid).FirstOrDefault();
      if (role != null)
        return;
      
      InitializationLogger.Debug("Init: Create module role.");
      var name = Akelon.MatrixCore.Resources.ModuleRoleName;
      role = Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(name, name, roleGuid);
    }
    
    /// <summary>
    /// Создать роль согласования.
    /// </summary>
    /// <param name="roleType">Тип роли.</param>
    public void CreateApprovalRole(Enumeration roleType)
    {
      CreateApprovalRole(roleType, ApprovalRoles.Info.Properties.Type.GetLocalizedValue(roleType));
    }
    
    /// <summary>
    /// Создать роль согласования.
    /// </summary>
    /// <param name="roleType">Тип роли.</param>
    /// <param name="description">Описание.</param>
    public void CreateApprovalRole(Enumeration roleType, string description)
    {
      InitializationLogger.DebugFormat("Init: approval role {0}", description);
      var role = ApprovalRoles.GetAll().Where(r => Equals(r.Type, roleType)).FirstOrDefault();

      // Проверить наличие роли.
      if (role == null)
      {
        role = ApprovalRoles.Create();
        role.Type = roleType;
      }

      role.Description = description;
      role.Save();
    }
  }
}
