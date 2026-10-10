// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Identity;

/// <summary>
/// code2Session（小程序登录凭证校验）响应体（<c>/cgi-bin/miniprogram/jscode2session</c>，
/// 企业自建应用与服务商代开发共用）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Identity")]
public class Code2SessionResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置用户所属企业的 corpid。
    /// </summary>
    [JsonPropertyName("corpid")]
    public string? Corpid { get; set; }

    /// <summary>
    /// 获取或设置用户在企业内的 UserID，对应管理端的账号，企业内唯一。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 官方注意事项：第三方小程序（代开发模板下的小程序）此处返回<b>加密的 userid</b>。
    /// </para>
    /// </remarks>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>
    /// 获取或设置会话密钥；是对用户数据进行加密签名的密钥。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 官方安全约束：为了应用自身的数据安全，开发者服务器不应该把会话密钥下发到小程序，
    /// 也不应该对外提供这个密钥。
    /// </para>
    /// </remarks>
    [JsonPropertyName("session_key")]
    public string? SessionKey { get; set; }
}
