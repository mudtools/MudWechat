// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.School;

/// <summary>
/// 获取访问用户身份响应体（<c>/cgi-bin/auth/getuserinfo</c>，家校沟通-网页授权登录，
/// 企业自建应用与服务商代开发公共）。
/// <para>
/// 官方形态：按用户身份三选一返回 ——
/// 企业成员返回 <see cref="Userid"/>（如需用户详情可调用通讯录「读取成员」）；
/// 学校家长返回 <see cref="ExternalUserid"/> + <see cref="ParentUserid"/>
/// （局校互联场景下 parent_userid 格式为 CorpId/parent_userid）；
/// 非企业成员或学生家长返回 <see cref="Openid"/>。
/// </para>
/// <para>
/// 官方业务限制：code 最大 512 字节、每次授权的 code 不同、只能使用一次、
/// 5 分钟未被使用自动过期；跳转域名须完全匹配该 access_token 对应应用的可信域名，否则返回 50001。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "School")]
public class SchoolAuthUserInfoResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置成员 UserID（用户为企业成员时返回）。
    /// </summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>
    /// 获取或设置家长的外部联系人 id（用户为学校家长时返回）。
    /// </summary>
    [JsonPropertyName("external_userid")]
    public string? ExternalUserid { get; set; }

    /// <summary>
    /// 获取或设置家校通讯录里家长的 userid（用户为学校家长时返回；
    /// 局校互联场景下格式为 CorpId/parent_userid）。
    /// </summary>
    [JsonPropertyName("parent_userid")]
    public string? ParentUserid { get; set; }

    /// <summary>
    /// 获取或设置非企业成员的标识（对当前企业唯一；用户非企业成员或学生家长时返回）。
    /// </summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }
}
