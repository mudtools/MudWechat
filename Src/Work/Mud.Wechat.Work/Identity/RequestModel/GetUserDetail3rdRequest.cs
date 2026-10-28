// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Identity;

/// <summary>
/// 获取访问用户敏感信息请求体（第三方，<c>/cgi-bin/service/auth/getuserdetail3rd</c>）。
/// <para><see cref="UserTicket"/> 为官方必填：来自「获取访问用户身份（第三方）」接口、
/// 授权 scope 为 snsapi_privateinfo 且成员在授权应用可见范围内时返回的成员票据。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Identity")]
public class GetUserDetail3rdRequest
{
    /// <summary>
    /// 获取或设置成员票据（官方必填，最大 512 字节，由第三方换取身份接口返回）。
    /// </summary>
    [JsonPropertyName("user_ticket")]
    public string? UserTicket { get; set; }
}
