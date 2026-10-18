// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 查询支付订单列表请求（<c>/shop/pay/getorderlist</c>，游标式分页）。
/// </summary>

[HttpJsonSerializable(SerializerClassName = "ShopOrder")]
public class WxaShopPayGetOrderListRequest
{
    /// <summary>分页过程中上次请求返回的索引值（选填，首页不传）。</summary>
    [JsonPropertyName("last_index")]
    public string? LastIndex { get; set; }

    /// <summary>每页订单数（<b>必填</b>，官方上限 <b>100</b>）。</summary>
    [JsonPropertyName("page_size")]
    public int? PageSize { get; set; }

    /// <summary>起始创建时间（<b>必填</b>），秒级时间戳；与 <c>end_create_time</c> <b>间隔不得超过 1 天</b>。</summary>
    [JsonPropertyName("begin_create_time")]
    public long? BeginCreateTime { get; set; }

    /// <summary>最终创建时间（<b>必填</b>），秒级时间戳；与 <c>begin_create_time</c> <b>间隔不得超过 1 天</b>。</summary>
    [JsonPropertyName("end_create_time")]
    public long? EndCreateTime { get; set; }

    /// <summary>商户号（选填；不填默认拉取小程序系统内所有订单；敏感，不得进日志）。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }
}
