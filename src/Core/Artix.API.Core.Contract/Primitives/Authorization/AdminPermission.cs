namespace Artix.API.Core.Contract.Primitives.Authorization;

public static class AdminPermission
{
    public const string MuseumScopeClaim = "museum_scope";
    public const string PortalRead = "admin.portal.read";
    public const string MuseumRead = "admin.museum.read";
    public const string MuseumWrite = "admin.museum.write";
    public const string MuseumDelete = "admin.museum.delete";
    public const string ObjectRead = "admin.object.read";
    public const string ObjectWrite = "admin.object.write";
    public const string ObjectDelete = "admin.object.delete";
    public const string ContentApprove = "admin.content.approve";
    public const string ContentPublish = "admin.content.publish";
    public const string FinanceRead = "admin.finance.read";
    public const string FinanceManage = "admin.finance.manage";
    public const string TechnicalRead = "admin.technical.read";
    public const string TechnicalManage = "admin.technical.manage";
    public const string DigitalAssetRead = "admin.digital_asset.read";
    public const string DigitalAssetManage = "admin.digital_asset.manage";
    public const string GamificationRead = "admin.gamification.read";
    public const string GamificationManage = "admin.gamification.manage";
    public const string AnalyticsRead = "admin.analytics.read";
    public const string SupportRead = "admin.support.read";
    public const string SupportManage = "admin.support.manage";
    public const string SensitiveOperations = "admin.sensitive.operations";
    public const string EmergencyAccess = "admin.emergency.access";
}
