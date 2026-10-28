// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Invoice;

// ---------------------------------------------------------------- 上传发票 PDF（POST /card/invoice/platform/setpdf）

/// <summary>
/// 上传发票 PDF（<c>POST /card/invoice/platform/setpdf</c>）响应。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约：<b>POST multipart/form-data</b>（表单文件字段名 <c>pdf</c>），Query 携带 <c>access_token</c>。
/// </para>
/// <para>
/// <b>有效期语义（官方原文，勿弱化）</b>：<c>s_media_id</c> 在「将发票卡券插入用户卡包」时用于关联 PDF 与发票卡券，
/// <b>有效期 3 天</b>；上传成功的 PDF 若 3 天内未被关联到发票卡券并发送到用户卡包，<b>将被清理</b>
/// （3 天后仍要关联须<b>重新上传</b>）。
/// </para>
/// <para>
/// 官方文档缺陷（照录）：<c>s_media_id</c> 类型标为 <c>string</c> 但描述写「<b>64 位整数</b>」，
/// 而示例值仅 16 位 —— 三处不一致 ⇒ SDK 以 <b>string</b> 承载（不丢前导零 / 不受整数位宽限制）。
/// </para>
/// <para>官方错误码：<c>40001</c>（本页错误码表仅此行，照录）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceSetPdfResponse : MpResponse
{
    /// <summary>获取或设置 PDF 素材 id（官方 <c>s_media_id</c>；插卡时关联用，<b>有效期 3 天</b>）。</summary>
    [JsonPropertyName("s_media_id")]
    public string? SMediaId { get; set; }
}
