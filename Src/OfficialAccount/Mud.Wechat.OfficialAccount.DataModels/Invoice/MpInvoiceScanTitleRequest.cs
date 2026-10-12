// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Invoice;

/// <summary>
/// 解析扫描的抬头二维码（<c>POST /card/invoice/scantitle</c>）请求体。
/// </summary>
/// <remarks>
/// 官方契约：<c>scan_text</c>（扫码获取的原始数据；示例值为
/// <c>https://mp.weixin.qq.com/intp/invoice/usertitlewxa?…</c> 形态的 URL）。
/// 官方文档缺陷（照录）：代码示例把字段名写成 <c>"scan_ text"</c>（中间多一空格），与参数表不一致。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceScanTitleRequest
{
    /// <summary>获取或设置扫码获取的原始数据（官方 <c>scan_text</c>，必填）。</summary>
    [JsonPropertyName("scan_text")]
    public string ScanText { get; set; } = string.Empty;
}
