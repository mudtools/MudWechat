// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.QrCodeLink;

/// <summary>查询 URL Scheme 应答（<c>scheme_info</c> 或 <c>quota_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "QrCodeLink")]
public class WxaQuerySchemeResponse : WxaResponse
{
    /// <summary>scheme 配置（<c>scheme_info</c>；<c>query_type = 1</c> 时缺省），见 <see cref="WxaSchemeInfo"/>。</summary>
    [JsonPropertyName("scheme_info")]
    public WxaSchemeInfo? SchemeInfo { get; set; }

    /// <summary>额度配置（<c>quota_info</c>；<c>query_type = 1</c> 时返回），见 <see cref="WxaQuotaInfo"/>。</summary>
    [JsonPropertyName("quota_info")]
    public WxaQuotaInfo? QuotaInfo { get; set; }
}
