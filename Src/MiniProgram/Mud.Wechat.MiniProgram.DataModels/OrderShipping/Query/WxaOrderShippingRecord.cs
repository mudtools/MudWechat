// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.OrderShipping;

/// <summary>
/// 「物流单（查询侧）」，<c>shipping.shipping_list</c> 数组元素。
/// </summary>
/// <remarks>
/// <para><b>与上传侧元素字段名不同</b>：本类型商品描述为 <c>goods_desc</c>（上传侧为 <c>item_desc</c>），
/// 且多一个 <c>upload_time</c>（<c>number</c> 时间戳；上传侧的 <c>upload_time</c> 是 RFC 3339 <c>string</c>
/// 且位于请求体顶层）⇒ 守卫 OS4 锁定两侧不可互换。</para>
/// <para><c>express_company</c> 语义为「同城配送公司名<b>或</b>物流公司编码」（查询侧原文比上传侧多前者）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_getorder.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OrderShipping")]
public class WxaOrderShippingRecord
{
    /// <summary>tracking_no（官方类型 <c>string</c>）：物流单号。</summary>
    [JsonPropertyName("tracking_no")]
    public string? TrackingNo { get; set; }

    /// <summary>express_company（官方类型 <c>string</c>）：同城配送公司名或物流公司编码，快递公司ID，参见「获取运力 id 列表 get_delivery_list」。</summary>
    [JsonPropertyName("express_company")]
    public string? ExpressCompany { get; set; }

    /// <summary>goods_desc（官方类型 <c>string</c>）：使用上传物流信息 API 录入的该物流信息的商品描述。</summary>
    [JsonPropertyName("goods_desc")]
    public string? GoodsDesc { get; set; }

    /// <summary>upload_time（官方类型 <c>number</c>）：该物流信息的上传时间，时间戳形式。</summary>
    [JsonPropertyName("upload_time")]
    public long? UploadTime { get; set; }

    /// <summary>contact（官方类型 <c>object</c>）：联系方式（寄件人 / 收件人掩码号码，字段面与上传侧相同）。</summary>
    [JsonPropertyName("contact")]
    public WxaOrderContact? Contact { get; set; }
}
