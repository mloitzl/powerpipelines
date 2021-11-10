namespace blogdeployments.api.Infrastructure
{
    public static class AppRole
    {
        public const string AppRoleConfigManage = "AppRole.Config.Manage";
    }

    /// <summary>
    /// Wrapper class the contain all the authorization policies available in this application.
    /// </summary>
    public static class AuthorizationPolicies
    {
        public const string ConfigManageRequired = "ConfigManageRequired";
    }
}