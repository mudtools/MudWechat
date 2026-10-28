// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.School;

/// <summary>
/// 获取访问用户身份响应体（第三方应用，<c>/cgi-bin/service/getuserinfo3rd</c>，
/// 家校沟通-网页授权登录）。
/// <para>
/// 官方形态：按用户身份三选一返回 ——
/// 用户属于某个企业返回 <see cref="CorpId"/> + <see cref="UserId"/>（+ <see cref="DeviceId"/>）；
/// 用户为学校家长返回 <see cref="CorpId"/>（兼容旧版，与 <see cref="Parents"/> 第一个元素相同）+
/// <see cref="ExternalUserid"/>（兼容旧版，建议用 <see cref="Parents"/> 字段）+ <see cref="Parents"/>；
/// 用户不属于任何企业返回 <see cref="OpenId"/>。
/// </para>
/// <para>
/// 官方业务限制：code 最大 512 字节、每次授权的 code 不同、只能使用一次、
/// 5 分钟未被使用自动过期；跳转域名须完全匹配应用的可信域名，否则返回 50001。
/// 官方响应字段名为 PascalCase 的 <c>CorpId</c> / <c>UserId</c> / <c>DeviceId</c> / <c>OpenId</c>，照抄不纠正。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "School")]
public class SchoolAuthThirdPartyUserInfoResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置用户所属企业的 corpid（官方字段名即 PascalCase 的 <c>CorpId</c>；
    /// 用户属于某个企业或为学校家长时返回，家长分支为兼容旧版字段，与 <see cref="Parents"/> 第一个元素相同）。
    /// </summary>
    [JsonPropertyName("CorpId")]
    public string? CorpId { get; set; }

    /// <summary>
    /// 获取或设置用户在企业内的 UserID（官方字段名即 PascalCase 的 <c>UserId</c>；
    /// 用户属于某个企业时返回）。
    /// </summary>
    [JsonPropertyName("UserId")]
    public string? UserId { get; set; }

    /// <summary>
    /// 获取或设置手机设备号（官方字段名即 PascalCase 的 <c>DeviceId</c>；
    /// 由企业微信在安装时随机生成，删除重装会改变，升级不受影响）。
    /// </summary>
    [JsonPropertyName("DeviceId")]
    public string? DeviceId { get; set; }

    /// <summary>
    /// 获取或设置非企业成员的标识（对当前服务商唯一；官方字段名即 PascalCase 的 <c>OpenId</c>；
    /// 用户不属于任何企业时返回）。
    /// </summary>
    [JsonPropertyName("OpenId")]
    public string? OpenId { get; set; }

    /// <summary>
    /// 获取或设置家长的外部联系人 id（家长分支返回的兼容旧版字段，
    /// 与 <see cref="Parents"/> 第一个元素相同，建议使用 <see cref="Parents"/> 字段）。
    /// </summary>
    [JsonPropertyName("external_userid")]
    public string? ExternalUserid { get; set; }

    /// <summary>
    /// 获取或设置家长列表（用户为学校家长时返回；同一家长微信可在多个学校各有一条记录）。
    /// </summary>
    [JsonPropertyName("parents")]
    public List<SchoolAuthThirdPartyParentItem>? Parents { get; set; }
}
