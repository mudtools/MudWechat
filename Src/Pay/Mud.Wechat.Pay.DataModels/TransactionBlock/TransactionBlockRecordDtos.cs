// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.TransactionBlock;

/// <summary>查询子商户交易拦截记录详情响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014940435"/></summary>
[HttpJsonSerializable(SerializerClassName = "TransactionBlock")]
public class TransactionBlockRecordInfoResponse : WechatPayResponse
{
    /// <summary>【交易拦截记录ID】 拦截记录查询成功时返回，交易拦截记录的主键，唯一定义此资源的标识（官方字段路径 <c>block_record_id</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("block_record_id")]
    public string? BlockRecordId { get; set; }

    /// <summary>【拦截时间】 拦截记录查询成功时返回，遵循rfc3339标准格式：yyyy-MM-DDThh:mm:ss+TIMEZONE（官方字段路径 <c>block_time</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("block_time")]
    public string? BlockTime { get; set; }

    /// <summary>【拦截原因】 拦截记录查询成功时返回，描述交易被拦截的原因（官方字段路径 <c>block_reason</c>，选填，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("block_reason")]
    public string? BlockReason { get; set; }

    /// <summary>【拦截笔数】 拦截记录查询成功时返回，交易拦截的笔数。可选取值：`LESS_THAN_TWENTY` 拦截笔数小于20 / `LESS_THAN_ONE_HUNDRED` 拦截笔数小于100 / `LESS_THAN_ONE_THOUSAND` 拦截笔数小于1000 / `OVER_ONE_THOUSAND` 拦截笔数大于等于1000（官方字段路径 <c>number_of_block</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("number_of_block")]
    public string? NumberOfBlock { get; set; }

    /// <summary>【解除管控方式】 拦截记录查询成功时返回，解除管控的方法。可选取值：`SUBMIT_INFORMATION` 按要求提交资料，审核通过后解脱（官方字段路径 <c>recover_way</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("recover_way")]
    public string? RecoverWay { get; set; }

    /// <summary>【要求提交的资料】 拦截记录查询成功时返回，要求提交的资料及其校验规则，JSON格式，详见 资料说明文档（官方字段路径 <c>requested_item_info</c>，选填，类型 <c>string(4000000)</c>）。</summary>
    [JsonPropertyName("requested_item_info")]
    public string? RequestedItemInfo { get; set; }

}

/// <summary>分页查询子商户名下的交易拦截记录响应·【交易拦截记录详情列表】 交易拦截记录的详情列表，查询结果不为空时返回。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014940432"/></summary>
[HttpJsonSerializable(SerializerClassName = "TransactionBlock")]
public class TransactionBlockRecordListResponseDataItem
{
    /// <summary>【交易拦截记录ID】 拦截记录查询成功时返回，交易拦截记录的主键，唯一定义此资源的标识（官方字段路径 <c>data.block_record_id</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("block_record_id")]
    public string? BlockRecordId { get; set; }

    /// <summary>【拦截时间】 拦截记录查询成功时返回，遵循rfc3339标准格式：yyyy-MM-DDThh:mm:ss+TIMEZONE（官方字段路径 <c>data.block_time</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("block_time")]
    public string? BlockTime { get; set; }

    /// <summary>【拦截原因】 拦截记录查询成功时返回，描述交易被拦截的原因（官方字段路径 <c>data.block_reason</c>，选填，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("block_reason")]
    public string? BlockReason { get; set; }

    /// <summary>【拦截笔数】 拦截记录查询成功时返回，交易拦截的笔数。可选取值：`LESS_THAN_TWENTY` 拦截笔数小于20 / `LESS_THAN_ONE_HUNDRED` 拦截笔数小于100 / `LESS_THAN_ONE_THOUSAND` 拦截笔数小于1000 / `OVER_ONE_THOUSAND` 拦截笔数大于等于1000（官方字段路径 <c>data.number_of_block</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("number_of_block")]
    public string? NumberOfBlock { get; set; }

    /// <summary>【解除管控方式】 拦截记录查询成功时返回，解除管控的方法。可选取值：`SUBMIT_INFORMATION` 按要求提交资料，审核通过后解脱（官方字段路径 <c>data.recover_way</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("recover_way")]
    public string? RecoverWay { get; set; }

    /// <summary>【要求提交的资料】 拦截记录查询成功时返回，要求提交的资料及其校验规则，JSON格式，详见 资料说明文档（官方字段路径 <c>data.requested_item_info</c>，选填，类型 <c>string(4000000)</c>）。</summary>
    [JsonPropertyName("requested_item_info")]
    public string? RequestedItemInfo { get; set; }

}

/// <summary>分页查询子商户名下的交易拦截记录响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014940432"/></summary>
[HttpJsonSerializable(SerializerClassName = "TransactionBlock")]
public class TransactionBlockRecordListResponse : WechatPayResponse
{
    /// <summary>【交易拦截记录详情列表】 交易拦截记录的详情列表，查询结果不为空时返回（官方字段路径 <c>data</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("data")]
    public List<TransactionBlockRecordListResponseDataItem>? Data { get; set; }

    /// <summary>【请求资源起始位置】 该次请求资源的起始位置，查询结果不为空时返回（官方字段路径 <c>offset</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("offset")]
    public long? Offset { get; set; }

    /// <summary>【最大资源条数】 该次请求可返回的最大资源条数，查询结果不为空时返回（官方字段路径 <c>limit</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("limit")]
    public long? Limit { get; set; }

    /// <summary>【资源总条数】 可选返回，资源总条数。当offset=0或者当前查询为空时返回总条数（官方字段路径 <c>total_count</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

}
