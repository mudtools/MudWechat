// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.OpenApi;

/// <summary>重置 API 调用次数（<c>clear_quota</c>）请求体。</summary>
/// <remarks>
/// 官方契约（逐页核验 2026-10-07）：<b>POST</b> + 请求体 <c>{"appid":…}</c>
/// （<c>access_token</c> 在 Query——官方明确「不在 body 中」；access_token 在 Query 由 [Token] 管线注入）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OpenApi")]
public class MpClearQuotaRequest
{
    /// <summary>获取或设置要被清空的账号的 appid（官方 <c>appid</c>，必填）。</summary>
    [JsonPropertyName("appid")]
    public string AppId { get; set; } = string.Empty;
}
