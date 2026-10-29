// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.MarketingPayGiftActivity;

/// <summary>查询活动商品响应·【结果集】 商品信息列表(sku信息列表)。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012466448"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPayGiftActivity")]
public class PayGiftActivityGoodsResponseDataItem
{
    /// <summary>【指定商品】 指定商品（商品sku）（官方字段路径 <c>data.goods_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("goods_id")]
    public string? GoodsId { get; set; }

    /// <summary>【创建时间】 创建时间（官方字段路径 <c>data.create_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>【更新时间】 更新时间（官方字段路径 <c>data.update_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("update_time")]
    public string? UpdateTime { get; set; }

}

/// <summary>查询活动商品响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012466448"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPayGiftActivity")]
public class PayGiftActivityGoodsResponse : WechatPayResponse
{
    /// <summary>【结果集】 商品信息列表(sku信息列表)（官方字段路径 <c>data</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("data")]
    public List<PayGiftActivityGoodsResponseDataItem>? Data { get; set; }

    /// <summary>【总数量】（官方字段路径 <c>total_count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>【分页页码】 分页页码（官方字段路径 <c>offset</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("offset")]
    public long? Offset { get; set; }

    /// <summary>【分页大小】 分页大小（官方字段路径 <c>limit</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("limit")]
    public long? Limit { get; set; }

    /// <summary>【活动ID】 活动ID（官方字段路径 <c>activity_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("activity_id")]
    public string? ActivityId { get; set; }

}
