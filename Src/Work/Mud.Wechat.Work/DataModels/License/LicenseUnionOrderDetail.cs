// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.License;

/// <summary>
/// 接口许可多企业订单详情（<c>order</c>，<c>/cgi-bin/license/get_union_order</c>，官方 OrderInfo）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "License")]
public class LicenseUnionOrderDetail
{
    /// <summary>获取或设置订单号。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>获取或设置订单类型：<c>8</c>-多企业新购订单。</summary>
    [JsonPropertyName("order_type")]
    public int? OrderType { get; set; }

    /// <summary>
    /// 获取或设置订单状态：<c>0</c>-待支付 / <c>1</c>-已支付 / <c>2</c>-已取消（未支付，订单已关闭）/
    /// <c>3</c>-未支付，订单已过期。
    /// </summary>
    [JsonPropertyName("order_status")]
    public int? OrderStatus { get; set; }

    /// <summary>获取或设置订单金额（单位分）。</summary>
    [JsonPropertyName("price")]
    public long? Price { get; set; }

    /// <summary>获取或设置创建时间（unix 时间戳）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>获取或设置支付时间（unix 时间戳）。</summary>
    [JsonPropertyName("pay_time")]
    public long? PayTime { get; set; }
}
