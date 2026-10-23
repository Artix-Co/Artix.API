namespace Artix.API.Core.Contract.Primitives.Authorization;

public static class AdminPermission
{
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
}
