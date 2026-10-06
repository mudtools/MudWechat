// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Basic;

/// <summary>
/// 获取接口调用凭据响应（<c>getAccessToken</c> 与 <c>getStableAccessToken</c> 共用，响应形态一致）。
/// </summary>
/// <remarks>
/// <para>
/// <b>成功响应不带 <c>errcode</c></b>：仅返回 <c>access_token</c> + <c>expires_in</c>；
/// 失败时才带 <c>errcode</c> / <c>errmsg</c>（继承 <see cref="MpResponse"/>）。
/// </para>
/// <para>
/// <c>expires_in</c> 语义：普通模式（<c>force_refresh=false</c>）复用旧 token 时返回的是
/// <b>剩余有效秒数</b>（官方示例 345），而非固定 7200 —— 令牌管理器必须按「本次返回的剩余时长」写入缓存。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Basic")]
public class MpGetTokenResponse : MpResponse
{
    /// <summary>获取到的凭证。</summary>
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; set; }

    /// <summary>凭证有效时间（单位：秒，7200 秒之内的值；复用旧 token 时为剩余秒数）。</summary>
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
}
