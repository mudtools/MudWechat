// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2.DataModels.PayV2;

/// <summary>
/// APIv2 违规记录查询请求体（机构/银行服务商侧）。
/// </summary>
/// <remarks>
/// <para><b>本支请求面没有 <c>appid</c></b>（官方表就 9 行，主体标识只有 <c>mch_id</c>），
/// 是 APIv2 线里少见的「不带 appid」端点，照抄不得补。</para>
/// <para><b>时间区间 ≤ 30 天</b>、<b>每页 ≤ 50 条</b>：越界官方直接回失败而不是截断。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2ViolationRecordQueryRequest
{
    /// <summary>商户号：银行服务商、从业机构或渠道商的商户号。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }

    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }
    [JsonPropertyName("sign_type")] public string? SignType { get; set; }

    /// <summary>开始时间，格式 <c>yyyy-MM-dd HH:mm:ss</c>。</summary>
    [JsonPropertyName("begin_time")] public string? BeginTime { get; set; }

    /// <summary>结束时间，格式 <c>yyyy-MM-dd HH:mm:ss</c>；与开始时间的区间不超过 30 天。</summary>
    [JsonPropertyName("end_time")] public string? EndTime { get; set; }

    /// <summary>页码（官方类型为 String，不是 int）。</summary>
    [JsonPropertyName("page_index")] public string? PageIndex { get; set; }

    /// <summary>每页条数（官方类型为 String）：最大 50。</summary>
    [JsonPropertyName("page_size")] public string? PageSize { get; set; }

    /// <summary>记录数据格式：<c>csv</c>（文本，逗号分隔）或 <c>json</c>（<see cref="V2ViolationRecordQueryResponse.ViolationRecords"/> 为 JSON 文档）。</summary>
    [JsonPropertyName("violation_records_format")] public string? ViolationRecordsFormat { get; set; }

    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 违规记录查询应答。
/// </summary>
/// <remarks>
/// <para><b>官方形态是「XML 信封里装一段字符串」</b>：记录集合不是 XML 子树，而是
/// <c>violation_records</c> 这一个字段里的 JSON 或 CSV 文本，由请求的
/// <c>violation_records_format</c> 决定 ⇒ 本 SDK 原样交回字符串，不猜结构、不做二次解析。</para>
/// <para><b>只有一级门控</b>：官方把 <c>result_code</c> 与 <c>violation_records</c> 写在同一张
/// 由 <c>return_code</c> 把关的表里，没有「return_code 与 result_code 都为 SUCCESS」的第三块。</para>
/// <para><b>反直觉类型</b>：官方标注 <c>result_code</c>/<c>result_msg</c> 为 string(32)、
/// <c>violation_records</c> 为 string(1024)（示例 JSON 明显超过该长度），字段名与门控照抄官方。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2ViolationRecordQueryResponse
{
    [JsonPropertyName("return_code")] public string? ReturnCode { get; set; }
    [JsonPropertyName("return_msg")] public string? ReturnMsg { get; set; }

    /// <summary>处理结果：SUCCESS / FAIL（官方标注 string(32)）。</summary>
    [JsonPropertyName("result_code")] public string? ResultCode { get; set; }

    /// <summary>处理信息（官方字段名 <c>result_msg</c>，本支特有，别当作 <c>return_msg</c>）。</summary>
    [JsonPropertyName("result_msg")] public string? ResultMsg { get; set; }

    [JsonPropertyName("err_code")] public string? ErrCode { get; set; }
    [JsonPropertyName("err_code_des")] public string? ErrCodeDes { get; set; }

    /// <summary>
    /// 违规记录数据原文：JSON 形态为 <c>{"total_size":N,"violation_records":[…]}</c>，
    /// CSV 形态首行为总记录数、其后每行一条记录。<b>可能含公司名称与子商户号，不得进日志。</b>
    /// </summary>
    [JsonPropertyName("violation_records")] public string? ViolationRecords { get; set; }
}
