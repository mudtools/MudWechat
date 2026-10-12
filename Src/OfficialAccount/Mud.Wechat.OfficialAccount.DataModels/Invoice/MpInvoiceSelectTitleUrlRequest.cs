// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Invoice;

/// <summary>
/// 获取商户专属抬头链接（<c>POST /card/invoice/biz/getselecttitleurl</c>）请求体。
/// </summary>
/// <remarks>
/// 官方契约：<c>attach</c>（附加字段，用户提交发票时会发给商户）/ <c>biz_name</c>（将商户名称显示给用户看）；
/// 官方原文「需将链接转为二维码展示」。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceSelectTitleUrlRequest
{
    /// <summary>获取或设置附加字段（官方 <c>attach</c>；用户提交发票时会发给商户）。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>获取或设置商户名称（官方 <c>biz_name</c>；将商户名称显示给用户看）。</summary>
    [JsonPropertyName("biz_name")]
    public string? BizName { get; set; }
}
