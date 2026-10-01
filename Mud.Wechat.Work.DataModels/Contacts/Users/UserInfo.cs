// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Users;

/// <summary>
/// 企业微信成员对象：读取成员（<c>/cgi-bin/user/get</c>）的响应体，同时作为获取部门成员详情（<c>/cgi-bin/user/list</c>）中 <c>userlist</c> 的元素。
/// </summary>
/// <remarks>
/// <para>
/// 各应用类型可获取的字段不同（详见官方文档逐字段说明）：第三方应用调用读取成员时 <c>userid</c> 字段返回
/// <c>open_userid</c>，且仅通讯录应用可获取 name/mobile/position 等敏感字段；代开发自建应用需管理员授权；
/// 自 2022-06-20 起新创建的自建与代开发应用不再返回头像、性别、手机、邮箱、企业邮箱、员工个人二维码、地址，
/// 需经 oauth2 手工授权获取。
/// </para>
/// <para>
/// 官方文档对 <c>gender</c> 的参数表标注为整数，但请求/响应示例均按字符串传输（如 <c>"1"</c>），
/// 故本模型以字符串承载以兼容两种形态。
/// </para>
/// </remarks>
public class UserInfo : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置成员 UserID（对应管理端账号，企业内唯一，不区分大小写，1~64 字节）；第三方应用调用时返回 open_userid。
    /// </summary>
    [JsonPropertyName("userid")]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置成员全局唯一标识 open_userid（同一服务商下不同应用获取同一成员的值相同，最长 64 字节）；仅第三方应用可获取。
    /// </summary>
    [JsonPropertyName("open_userid")]
    public string? OpenUserId { get; set; }

    /// <summary>
    /// 获取或设置成员名称（第三方不可获取，返回 userid 代替；代开发需管理员授权）。
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// 获取或设置手机号码（需管理员与成员授权；第三方仅通讯录应用可获取；上游企业不可获取下游企业成员该字段）。
    /// </summary>
    [JsonPropertyName("mobile")]
    public string? Mobile { get; set; }

    /// <summary>
    /// 获取或设置所属部门 ID 列表（仅返回应用有查看权限的部门；成员授权模式下固定返回根部门 1）。
    /// </summary>
    [JsonPropertyName("department")]
    public List<int>? Department { get; set; }

    /// <summary>
    /// 获取或设置部门内排序值（与 Department 数量一致，数值越大越靠前，官方范围 [0, 2^32)，故以 64 位整数承载）；成员授权模式下不返回。
    /// </summary>
    [JsonPropertyName("order")]
    public List<long>? Order { get; set; }

    /// <summary>
    /// 获取或设置职务（代开发需管理员授权；第三方仅通讯录应用可获取）。
    /// </summary>
    [JsonPropertyName("position")]
    public string? Position { get; set; }

    /// <summary>
    /// 获取或设置性别（官方示例按字符串传输："0" 未定义，"1" 男，"2" 女；不可获取时返回 "0"）。
    /// </summary>
    [JsonPropertyName("gender")]
    public string? Gender { get; set; }

    /// <summary>
    /// 获取或设置邮箱（需授权；第三方仅通讯录应用可获取）。
    /// </summary>
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    /// <summary>
    /// 获取或设置企业邮箱（需授权；第三方仅通讯录应用可获取；代开发自建应用不返回）。
    /// </summary>
    [JsonPropertyName("biz_mail")]
    public string? BizMail { get; set; }

    /// <summary>
    /// 获取或设置在所在部门内是否为部门负责人（0 否；1 是；数量须与 Department 一致）。
    /// </summary>
    [JsonPropertyName("is_leader_in_dept")]
    public List<int>? IsLeaderInDept { get; set; }

    /// <summary>
    /// 获取或设置直属上级 UserID 列表（最多 1 个，仅返回应用可见范围内的直属上级）。
    /// </summary>
    [JsonPropertyName("direct_leader")]
    public List<string>? DirectLeader { get; set; }

    /// <summary>
    /// 获取或设置头像 URL。
    /// </summary>
    [JsonPropertyName("avatar")]
    public string? Avatar { get; set; }

    /// <summary>
    /// 获取或设置头像缩略图 URL。
    /// </summary>
    [JsonPropertyName("thumb_avatar")]
    public string? ThumbAvatar { get; set; }

    /// <summary>
    /// 获取或设置座机（代开发需管理员授权；第三方仅通讯录应用可获取）。
    /// </summary>
    [JsonPropertyName("telephone")]
    public string? Telephone { get; set; }

    /// <summary>
    /// 获取或设置别名（第三方仅通讯录应用可获取）。
    /// </summary>
    [JsonPropertyName("alias")]
    public string? Alias { get; set; }

    /// <summary>
    /// 获取或设置地址（需授权）。
    /// </summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }

    /// <summary>
    /// 获取或设置英文名（仅官方响应示例携带，官方参数表未单列说明）。
    /// </summary>
    [JsonPropertyName("english_name")]
    public string? EnglishName { get; set; }

    /// <summary>
    /// 获取或设置主部门 ID（仅当应用对主部门有查看权限时返回）。
    /// </summary>
    [JsonPropertyName("main_department")]
    public int? MainDepartment { get; set; }

    /// <summary>
    /// 获取或设置扩展属性（须先在企业 WEB 管理端添加对应字段）。
    /// </summary>
    [JsonPropertyName("extattr")]
    public UserExtAttr? ExtAttr { get; set; }

    /// <summary>
    /// 获取或设置激活状态（1 已激活；2 已禁用；4 未激活；5 退出企业）。
    /// </summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>
    /// 获取或设置员工个人二维码 URL（扫码可添加为外部联系人）。
    /// </summary>
    [JsonPropertyName("qr_code")]
    public string? QrCode { get; set; }

    /// <summary>
    /// 获取或设置成员对外属性。
    /// </summary>
    [JsonPropertyName("external_profile")]
    public UserExternalProfile? ExternalProfile { get; set; }

    /// <summary>
    /// 获取或设置对外职务（设置了该值则以此作为对外展示的职务，否则以 Position 展示）。
    /// </summary>
    [JsonPropertyName("external_position")]
    public string? ExternalPosition { get; set; }
}
