using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AiCard
{
    public static class SysRole
    {
        #region 系统权限


        public const string RoleManageRead = "RoleManageRead";
        public const string RoleManageCreate = "RoleManageCreate";
        public const string RoleManageEdit = "RoleManageEdit";
        public const string RoleManageDelete = "RoleManageDelete";


        public const string UserManageRead = "SystemUserManageRead";
        public const string UserManageCreate = "SystemUserManageCreate";
        public const string UserManageEdit = "SystemUserManageEdit";
        public const string UserManageDelete = "SystemUserManageDelete";

        public const string EnterpriseManageRead = "EnterpriseManageRead";
        public const string EnterpriseManageCreate = "EnterpriseManageCreate";
        public const string EnterpriseManageEdit = "EnterpriseManageEdit";
        public const string EnterpriseManageDelete = "EnterpriseManageDelete";
        public const string EnterpriseManageDeploy = "EnterpriseManageDeploy";//配置绑定企业微信
        public const string EnterpriseManageCogradient = "EnterpriseManageCogradient";//同步企业微信用户


        public const string CardManageRead = "CardManageRead";
        public const string CardManageCreate = "CardManageCreate";
        public const string CardManageEdit = "CardManageEdit";
        public const string CardMangeDelete = "CardMangeDelete";

        public const string ProductKindManageRead = "ProductKindManageRead";
        public const string ProductKindManageCreate = "ProductKindManageCreate";
        public const string ProductKindManageEdit = "ProductKindManageEdit";
        public const string ProductKindMangeDelete = "ProductKindMangeDelete";

        public const string ProductManageRead = "ProductManageRead";
        public const string ProductManageCreate = "ProductManageCreate";
        public const string ProductManageEdit = "ProductManageEdit";
        public const string ProductMangeDelete = "ProductMangeDelete";
        #endregion

        #region 企业权限
        public const string EUserManageRead = "EUserManageRead";
        public const string EUserManageCreate = "EUserManageCreate";
        public const string EUserManageEdit = "EUserManageEdit";
        public const string EUserManageDelete = "EUserManageDelete";

        public const string ERoleManageRead = "ERoleManageRead";
        public const string ERoleManageCreate = "ERoleManageCreate";
        public const string ERoleManageEdit = "ERoleManageEdit";
        public const string ERoleManageDelete = "ERoleManageDelete";

        public const string EEnterpriseManageRead = "EEnterpriseManageRead";
        public const string EEnterpriseManageEdit = "EEnterpriseManageEdit";
        public const string EEnterpriseManageDeploy = "EEnterpriseManageDeploy";//配置绑定企业微信
        public const string EEnterpriseManageCogradient = "EEnterpriseManageCogradient";//同步企业微信用户

        public const string ECardManageRead = "ECardManageRead";
        public const string ECardManageCreate = "ECardManageCreate";
        public const string ECardManageEdit = "ECardManageEdit";
        public const string ECardManageDelete = "ECardManageDelete";

        public const string EProductKindManageRead = "EProductKindManageRead";
        public const string EProductKindManageCreate = "EProductKindManageCreate";
        public const string EProductKindManageEdit = "EProductKindManageEdit";
        public const string EProductKindMangeDelete = "EProductKindMangeDelete";

        public const string EProductManageRead = "EProductManageRead";
        public const string EProductManageCreate = "EProductManageCreate";
        public const string EProductManageEdit = "EProductManageEdit";
        public const string EProductMangeDelete = "EProductMangeDelete";

        public const string EHomePageModularsManageRead = "EHomePageModularsManageRead";
        public const string EHomePageModularsManageEdit = "EHomePageModularsManageEdit";
        public const string EHomePageModularsManageDelete = "EHomePageModularsManageDelete";

        #endregion

    }
}