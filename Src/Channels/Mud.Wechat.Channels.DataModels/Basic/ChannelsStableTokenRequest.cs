// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Basic;

/// <summary>
/// 获取稳定版接口调用凭据请求体（<c>POST /cgi-bin/stable_token</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方注意：本接口<b>仅支持 POST</b>；与 <c>getAccessToken</c> 的凭据<b>完全隔离</b>；
/// 调用频率 1 万次/分、50 万次/天；<b>平台在普通模式下提前 5 分钟更新 access_token</b>。
/// </para>
/// <para>
/// <b>本 SDK 恒传 <c>force_refresh = false</c></b>：本地缓存失效后调用普通模式即可取回平台当前有效
/// token（平台提前 5 分钟轮换，本地提前刷新阈值更早拦截）。官方「强制刷新模式每天限 20 次且需间隔 30 秒」
/// 属配额约束；本 SDK 不提供强刷开关（无消费场景，避免烧配额）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Basic")]
public class ChannelsStableTokenRequest
{
    /// <summary>凭证类型，固定填写 <c>client_credential</c>。</summary>
    [JsonPropertyName("grant_type")]
    public string GrantType { get; set; } = "client_credential";

    /// <summary>账号的唯一凭证，即 AppID（小店 AppID，wx 开头但与公众号/小程序不互通）。</summary>
    [JsonPropertyName("appid")]
    public string AppId { get; set; } = string.Empty;

    /// <summary>唯一凭证密钥，即 AppSecret。</summary>
    [JsonPropertyName("secret")]
    public string Secret { get; set; } = string.Empty;

    /// <summary>
    /// 是否强制刷新（默认 <c>false</c>）。
    /// </summary>
    /// <remarks>
    /// 可空：本 SDK 恒传 <c>false</c>（显式下发），不传则官方默认亦为 false；
    /// 保留可空形态以便将来按需显式下发（当前无消费场景）。
    /// </remarks>
    [JsonPropertyName("force_refresh")]
    public bool? ForceRefresh { get; set; }
}