// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>预览面单模板请求体（<c>POST /cgi-bin/express/delivery/template/preview</c>）。</summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-provider/api_previewtemplate"/>。</para>
/// <para>服务商自定义面单模板时调用，用于预览渲染效果；<c>waybill_template</c> 为需 Base64 编码的 HTML 模板内容，<c>waybill_data</c> 为下单事件返回的 WaybillData。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressPreviewTemplateRequest
{
    /// <summary>运单 ID（<c>waybill_id</c>，必填）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>面单 HTML 模板内容（<c>waybill_template</c>，必填；需经 Base64 编码）。</summary>
    [JsonPropertyName("waybill_template")]
    public string? WaybillTemplate { get; set; }

    /// <summary>面单数据（<c>waybill_data</c>，必填；详情参考下单事件返回值中的 WaybillData）。</summary>
    [JsonPropertyName("waybill_data")]
    public List<WxaExpressWaybillDataItem>? WaybillData { get; set; }

    /// <summary>商户下单数据（<c>custom</c>，必填；格式为商家寄件 <c>order/add</c> 的请求体），见 <see cref="WxaExpressAddOrderRequest"/>。</summary>
    [JsonPropertyName("custom")]
    public WxaExpressAddOrderRequest? CustomData { get; set; }
}
