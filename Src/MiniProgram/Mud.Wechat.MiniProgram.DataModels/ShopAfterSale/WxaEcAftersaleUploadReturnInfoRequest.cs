// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopAfterSale;

/// <summary>
/// 上传退货物流请求体（<c>POST /shop/ecaftersale/uploadreturninfo</c>）。
/// </summary>
/// <remarks>
/// <para><b>官方请求示例缺逗号（照录）</b>：<c>"aftersale_id": 123</c> 后缺分隔逗号；参数表四支仍清晰：</para>
/// <para><c>openid</c> / <c>delivery_id</c> / <c>waybill_id</c> 必填，<c>delivery_name</c> 选填。</para>
/// <para><b>敏感值</b>：<c>openid</c> 与运单号属个人信息，禁入日志、遥测与异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAfterSale")]
public class WxaEcAftersaleUploadReturnInfoRequest
{
    /// <summary>商家自定义售后 ID（与 <c>aftersale_id</c> 二选一）。</summary>
    [JsonPropertyName("out_aftersale_id")]
    public string? OutAftersaleId { get; set; }

    /// <summary>微信侧售后单号（与 <c>out_aftersale_id</c> 二选一）。</summary>
    [JsonPropertyName("aftersale_id")]
    public long? AftersaleId { get; set; }

    /// <summary>用户 <c>openid</c>（必填，敏感）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>快递公司 ID（必填）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>快递单号（必填，敏感）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>快递公司名称（选填）。</summary>
    [JsonPropertyName("delivery_name")]
    public string? DeliveryName { get; set; }
}
