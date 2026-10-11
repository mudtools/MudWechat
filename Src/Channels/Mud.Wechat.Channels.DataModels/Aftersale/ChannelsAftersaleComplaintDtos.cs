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

/// <summary>纠纷单状态（官方 <c>getcomplaintorder</c> 的 <c>status</c> 枚举）。</summary>
public static class ChannelsComplaintStatuses
{
    /// <summary>待商家处理纠纷。</summary>
    public const int WaitMerchantHandle = 100;

    /// <summary>待客服处理。</summary>
    public const int WaitCustomerServiceHandle = 101;

    /// <summary>取消客服介入。</summary>
    public const int CancelCustomerServiceIntervention = 102;

    /// <summary>客服处理中。</summary>
    public const int CustomerServiceHandling = 103;

    /// <summary>待用户补充凭证。</summary>
    public const int WaitUserProof = 104;

    /// <summary>用户已补充凭证。</summary>
    public const int UserProofAdded = 105;

    /// <summary>待商家补充凭证。</summary>
    public const int WaitMerchantProof = 106;

    /// <summary>商家已补充凭证。</summary>
    public const int MerchantProofAdded = 107;

    /// <summary>待双方补充凭证。</summary>
    public const int WaitBothProof = 108;

    /// <summary>双方补充凭证超时。</summary>
    public const int BothProofTimeout = 109;

    /// <summary>商家申诉中。</summary>
    public const int MerchantAppealing = 111;

    /// <summary>调解完成。</summary>
    public const int MediationDone = 112;

    /// <summary>待客服核实。</summary>
    public const int WaitCustomerServiceVerify = 113;

    /// <summary>重新退款中。</summary>
    public const int Refunding = 114;

    /// <summary>退款核实完成。</summary>
    public const int RefundVerifyDone = 115;

    /// <summary>调解关闭。</summary>
    public const int MediationClosed = 116;

    /// <summary>用户补充凭证超时。</summary>
    public const int UserProofTimeout = 305;

    /// <summary>商家补充凭证超时。</summary>
    public const int MerchantProofTimeout = 307;

    /// <summary>待商家上传工单。</summary>
    public const int WaitMerchantWorkOrder = 400;
}

/// <summary>获取纠纷单详情（<c>aftersale/getcomplaintorder</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsGetComplaintOrderRequest
{
    /// <summary>获取或设置纠纷单号（官方 <c>complaint_id</c>，必填）。</summary>
    [JsonPropertyName("complaint_id")]
    public string ComplaintId { get; set; } = string.Empty;
}

/// <summary>获取纠纷单详情（<c>aftersale/getcomplaintorder</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsGetComplaintOrderResponse : ChannelsResponse
{
    /// <summary>获取或设置售后单号（官方 <c>after_sale_order_id</c>）。</summary>
    [JsonPropertyName("after_sale_order_id")]
    public string? AfterSaleOrderId { get; set; }

    /// <summary>获取或设置订单号（官方 <c>order_id</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>获取或设置纠纷历史（官方 <c>history</c>）。</summary>
    [JsonPropertyName("history")]
    public List<ChannelsComplaintHistory>? History { get; set; }

    /// <summary>获取或设置纠纷单状态（官方 <c>status</c>，枚举值见 <see cref="ChannelsComplaintStatuses"/>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置虚拟号码信息（官方 <c>virtual_tel_num_info</c>，字段集与售后单共用）。</summary>
    [JsonPropertyName("virtual_tel_num_info")]
    public ChannelsVirtualTelNumInfo? VirtualTelNumInfo { get; set; }
}

/// <summary>纠纷历史（官方 <c>getcomplaintorder.history</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsComplaintHistory
{
    /// <summary>获取或设置历史操作类型（官方 <c>item_type</c>，枚举值见官方文档）。</summary>
    [JsonPropertyName("item_type")]
    public int? ItemType { get; set; }

    /// <summary>获取或设置操作时间（官方 <c>time</c>，Unix 秒级时间戳）。</summary>
    [JsonPropertyName("time")]
    public long? Time { get; set; }

    /// <summary>获取或设置相关文本内容（官方 <c>content</c>）。</summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    /// <summary>获取或设置相关图片 media_id 列表（官方 <c>media_id_list</c>，此字段已废弃，请用 <see cref="MediaInfos"/>）。</summary>
    [JsonPropertyName("media_id_list")]
    public List<string>? MediaIdList { get; set; }

    /// <summary>获取或设置售后类型（官方 <c>after_sale_type</c>）：1 仅退款；2 退货退款。</summary>
    [JsonPropertyName("after_sale_type")]
    public int? AfterSaleType { get; set; }

    /// <summary>获取或设置售后原因（官方 <c>after_sale_reason</c>，见 获取全量售后原因）。</summary>
    [JsonPropertyName("after_sale_reason")]
    public int? AfterSaleReason { get; set; }

    /// <summary>获取或设置多媒体信息列表（官方 <c>media_infos</c>）。</summary>
    [JsonPropertyName("media_infos")]
    public List<ChannelsMediaInfo>? MediaInfos { get; set; }
}

/// <summary>多媒体信息（官方 <c>media_infos</c> 数组元素，图片 / 视频二选一）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsMediaInfo
{
    /// <summary>获取或设置媒体类型（官方 <c>type</c>）：1 图片；2 视频。</summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>获取或设置图片信息（官方 <c>picture</c>，type 为 1 时有值）。</summary>
    [JsonPropertyName("picture")]
    public ChannelsMediaPicture? Picture { get; set; }

    /// <summary>获取或设置视频信息（官方 <c>video</c>，type 为 2 时有值）。</summary>
    [JsonPropertyName("video")]
    public ChannelsMediaVideo? Video { get; set; }
}

/// <summary>媒体图片信息（官方 <c>media_infos[].picture</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsMediaPicture
{
    /// <summary>获取或设置素材临时访问 URL（官方 <c>ticket_url</c>）。</summary>
    [JsonPropertyName("ticket_url")]
    public string? TicketUrl { get; set; }
}

/// <summary>媒体视频信息（官方 <c>media_infos[].video</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsMediaVideo
{
    /// <summary>获取或设置素材临时访问 URL（官方 <c>ticket_url</c>）。</summary>
    [JsonPropertyName("ticket_url")]
    public string? TicketUrl { get; set; }

    /// <summary>获取或设置视频时长（官方 <c>video_play_length</c>，秒，仅上传时有填时有值）。</summary>
    [JsonPropertyName("video_play_length")]
    public long? VideoPlayLength { get; set; }
}

/// <summary>纠纷单补充留言（<c>aftersale/addcomplaintmaterial</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsAddComplaintMaterialRequest
{
    /// <summary>获取或设置纠纷单号（官方 <c>complaint_id</c>，必填）。</summary>
    [JsonPropertyName("complaint_id")]
    public string ComplaintId { get; set; } = string.Empty;

    /// <summary>获取或设置留言内容（官方 <c>content</c>，必填，最多 500 字）。</summary>
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

    /// <summary>获取或设置图片 media_id 列表（官方 <c>media_id_list</c>，所有留言总图片数最多 20 张）。</summary>
    [JsonPropertyName("media_id_list")]
    public List<string>? MediaIdList { get; set; }
}

/// <summary>纠纷单举证（<c>aftersale/addcomplaintproof</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsAddComplaintProofRequest
{
    /// <summary>获取或设置纠纷单号（官方 <c>complaint_id</c>，必填）。</summary>
    [JsonPropertyName("complaint_id")]
    public string ComplaintId { get; set; } = string.Empty;

    /// <summary>获取或设置举证文字内容（官方 <c>content</c>，必填，最多 500 字）。</summary>
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

    /// <summary>获取或设置举证图片 media_id 列表（官方 <c>media_id_list</c>）。</summary>
    [JsonPropertyName("media_id_list")]
    public List<string>? MediaIdList { get; set; }
}

/// <summary>工单信息（官方 <c>syncworkorder.work_order_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsWorkOrderInfo
{
    /// <summary>获取或设置递增版本号（官方 <c>version</c>，必填，每次全量同步递增）。</summary>
    [JsonPropertyName("version")]
    public long Version { get; set; }

    /// <summary>获取或设置工单列表（官方 <c>items</c>，必填，全量替换）。</summary>
    [JsonPropertyName("items")]
    public List<ChannelsWorkOrderItem> Items { get; set; } = new();
}

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

/// <summary>工单素材（官方 <c>syncworkorder.items[].media_list</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsWorkOrderMedia
{
    /// <summary>获取或设置媒体类型（官方 <c>type</c>，必填，填 1 图片）。</summary>
    [JsonPropertyName("type")]
    public int Type { get; set; }

    /// <summary>获取或设置图片信息（官方 <c>picture</c>，必填）。</summary>
    [JsonPropertyName("picture")]
    public ChannelsWorkOrderMediaPicture? Picture { get; set; }
}

/// <summary>工单素材图片（官方 <c>media_list[].picture</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsWorkOrderMediaPicture
{
    /// <summary>
    /// 获取或设置临时媒体 ID（官方 <c>tmp_media_id</c>，必填，经 上传图片 resp_type 填 0 取 media_id）。
    /// </summary>
    [JsonPropertyName("tmp_media_id")]
    public string TmpMediaId { get; set; } = string.Empty;
}

/// <summary>工单同步（<c>aftersale/syncworkorder</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsSyncWorkOrderRequest
{
    /// <summary>获取或设置纠纷单号（官方 <c>complaint_id</c>，必填）。</summary>
    [JsonPropertyName("complaint_id")]
    public string ComplaintId { get; set; } = string.Empty;

    /// <summary>获取或设置工单信息（官方 <c>work_order_info</c>，必填）。</summary>
    [JsonPropertyName("work_order_info")]
    public ChannelsWorkOrderInfo WorkOrderInfo { get; set; } = new();
}
