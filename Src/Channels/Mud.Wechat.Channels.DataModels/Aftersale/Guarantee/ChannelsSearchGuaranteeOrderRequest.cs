// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 售后管理（Aftersale）域「保障单」DTO（aftersale/getguaranteeorder、aftersale/searchguaranteeorder、
// aftersale/merchantacceptguarantee、aftersale/merchantmodifyguarantee、aftersale/merchantproofguarantee、
// aftersale/merchantrefuseguarantee）+ 保障单状态常量。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Aftersale。

namespace Mud.Wechat.Channels.DataModels.Aftersale;

/// <summary>获取保障单列表（<c>aftersale/searchguaranteeorder</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsSearchGuaranteeOrderRequest
{
    /// <summary>获取或设置保障单号（官方 <c>guarantee_order_id_list</c>）。</summary>
    [JsonPropertyName("guarantee_order_id_list")]
    public List<long>? GuaranteeOrderIdList { get; set; }

    /// <summary>获取或设置订单号（官方 <c>order_id_list</c>）。</summary>
    [JsonPropertyName("order_id_list")]
    public List<long>? OrderIdList { get; set; }

    /// <summary>获取或设置想要获取的保障单类型（官方 <c>type</c>）：0 全部；1 假一赔三；2 坏损包退。</summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>获取或设置保障单申请的开始时间戳（官方 <c>begin_time</c>）。</summary>
    [JsonPropertyName("begin_time")]
    public long? BeginTime { get; set; }

    /// <summary>获取或设置保障单申请的结束时间戳（官方 <c>end_time</c>）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>获取或设置保障单当前状态筛选（官方 <c>status_list</c>，枚举值见 <see cref="ChannelsGuaranteeStatuses"/>）。</summary>
    [JsonPropertyName("status_list")]
    public List<string>? StatusList { get; set; }

    /// <summary>获取或设置列表起始下标（官方 <c>offset</c>，默认从 0 开始）。</summary>
    [JsonPropertyName("offset")]
    public int? Offset { get; set; }

    /// <summary>获取或设置需要的保障单列表条数（官方 <c>limit</c>，必填）。</summary>
    [JsonPropertyName("limit")]
    public int Limit { get; set; }
}
