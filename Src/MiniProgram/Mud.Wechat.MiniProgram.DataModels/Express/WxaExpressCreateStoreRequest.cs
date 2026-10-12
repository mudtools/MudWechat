// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>创建门店请求体（<c>POST /cgi-bin/express/intracity/createstore</c>）。</summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_createstore"/>。</para>
/// <para>
/// 自定义门店编号 <c>out_store_id</c> 需要唯一；创建后系统生成全局唯一门店编号 <c>wx_store_id</c>，
/// 后续创建运力订单需要该门店编号。<c>order_pattern</c>：1 价格优先，2 运力优先（默认价格优先）；
/// <c>order_pattern=2</c> 时 <c>service_trans_prefer</c> 必填。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressCreateStoreRequest
{
    /// <summary>自定义门店编号（<c>out_store_id</c>，必填；需唯一）。</summary>
    [JsonPropertyName("out_store_id")]
    public string? OutStoreId { get; set; }

    /// <summary>门店名称（<c>store_name</c>，必填）。</summary>
    [JsonPropertyName("store_name")]
    public string? StoreName { get; set; }

    /// <summary>运力偏好（<c>order_pattern</c>，可选；1 价格优先，2 运力优先，默认价格优先）。</summary>
    [JsonPropertyName("order_pattern")]
    public int? OrderPattern { get; set; }

    /// <summary>优先使用的运力 ID（<c>service_trans_prefer</c>，可选；<c>order_pattern=2</c> 时必填）。</summary>
    [JsonPropertyName("service_trans_prefer")]
    public string? ServiceTransIdPrefer { get; set; }

    /// <summary>门店地址信息（<c>address_info</c>，必填；务必传入正确的门店地址作为发货地址），见 <see cref="WxaExpressIntracityAddress"/>。</summary>
    [JsonPropertyName("address_info")]
    public WxaExpressIntracityAddress? AddressInfo { get; set; }
}
