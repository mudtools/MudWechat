// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Auth;

/// <summary>
/// 登录凭证校验应答（<c>GET /sns/jscode2session</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<c>user-login/api_code2session.html</c>（2026-10-09 逐字段核验）。
/// </para>
/// <para>
/// <b>红线 MP-X7</b>：<see cref="SessionKey"/> 是用户<b>会话密钥</b>——
/// <b>不得</b>入日志 / 遥测 / 异常消息，<b>不得</b>下发到前端。本 DTO 刻意不重写 <c>ToString</c>
/// 以免被「顺手」用于日志。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Auth")]
public class WxaCode2SessionResponse : WxaResponse
{
    /// <summary>用户唯一标识（<c>openid</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>
    /// 会话密钥（<c>session_key</c>）。<b>敏感：不得入日志、不得下发前端</b>（MP-X7）。
    /// </summary>
    [JsonPropertyName("session_key")]
    public string? SessionKey { get; set; }

    /// <summary>用户在开放平台的唯一标识（<c>unionid</c>；未绑定开放平台或未关注同主体公众号时缺省）。</summary>
    [JsonPropertyName("unionid")]
    public string? UnionId { get; set; }
}
