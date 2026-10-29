// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.OpenApi;

/// <summary>
/// 使用 AppSecret 重置 API 调用次数（<c>clear_quota/v2</c>）请求体（<b>免令牌端点</b>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约（逐页核验 2026-10-07）：该接口<b>不消费 access_token</b>——官方注意事项原文
/// 「该接口通过 appsecret 调用，解决了 access_token 耗尽无法调用『重置 API 调用次数』的问题」。
/// </para>
/// <para>
/// <b>参数位置裁决（I4，官方文档矛盾照录）</b>：官方参数表把 <c>appid</c>/<c>appsecret</c> 列于
/// 「请求体 Request Payload」，但官方代码示例把两参数放在 Query String——两者矛盾。
/// SDK 按<b>参数表（请求体）</b>建模：<c>appsecret</c> 是唯一凭证密钥，进 URL 会扩大泄露面
/// （组件脱敏词表按精确匹配 Query 参数名设计，请求体形态天然不入 URL 日志）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OpenApi")]
public class MpClearQuotaV2Request
{
    /// <summary>获取或设置要被清空的账号的 appid（官方 <c>appid</c>，必填）。</summary>
    [JsonPropertyName("appid")]
    public string AppId { get; set; } = string.Empty;

    /// <summary>获取或设置唯一凭证密钥（官方 <c>appsecret</c>，必填；SDK 走请求体，不进 URL）。</summary>
    [JsonPropertyName("appsecret")]
    public string AppSecret { get; set; } = string.Empty;
}
