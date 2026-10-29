// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 售后管理（Aftersale）域「纠纷单」DTO（aftersale/getcomplaintorder、aftersale/addcomplaintmaterial、
// aftersale/addcomplaintproof、aftersale/syncworkorder）+ 纠纷单状态常量。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Aftersale。

namespace Mud.Wechat.Channels.DataModels.Aftersale;

/// <summary>工单条目（官方 <c>syncworkorder.work_order_info.items</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsWorkOrderItem
{
    /// <summary>获取或设置工单 ID（官方 <c>work_order_id</c>，必填）。</summary>
    [JsonPropertyName("work_order_id")]
    public string WorkOrderId { get; set; } = string.Empty;

    /// <summary>获取或设置工单状态（官方 <c>status</c>，必填）：1 创建工单；2 处理工单；3 工单完结。</summary>
    [JsonPropertyName("status")]
    public int Status { get; set; }

    /// <summary>获取或设置描述（官方 <c>desc</c>，必填）。</summary>
    [JsonPropertyName("desc")]
    public string Desc { get; set; } = string.Empty;

    /// <summary>获取或设置更新时间（官方 <c>update_time</c>，必填）。</summary>
    [JsonPropertyName("update_time")]
    public long UpdateTime { get; set; }

    /// <summary>获取或设置终态结果类型（官方 <c>result_type</c>，必填）：0 无结果（非终态）；1 同意退款；2 拒绝退款（仅终态有效）。</summary>
    [JsonPropertyName("result_type")]
    public int ResultType { get; set; }

    /// <summary>获取或设置退款金额（官方 <c>refund_amount</c>，单位分，终态且同意退款时有效）。</summary>
    [JsonPropertyName("refund_amount")]
    public long? RefundAmount { get; set; }

    /// <summary>获取或设置图片 / 视频素材列表（官方 <c>media_list</c>）。</summary>
    [JsonPropertyName("media_list")]
    public List<ChannelsWorkOrderMedia>? MediaList { get; set; }
}
