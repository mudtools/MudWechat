// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.MerchantOperationManage;

/// <summary>
/// 查询商户管控情况响应（<c>GET /v3/mch-operation-manage/merchant-limitations/sub-mchid/{sub_mchid}</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方页面</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012803072">4012803072</see>
/// （更新于 2025.07.18，支持商户【普通服务商】）。
/// </para>
/// <para>
/// <b>同前缀零端点守卫</b>：同前缀 <c>/v3/mch-operation-manage/enhanced-due-diligence/</c> 下另有三支尽调族端点，
/// 但「支持商户」为【平台商户】⇒ 本包零声明（守卫 MOM-B2），正对照取本端点证明扫描机制生效。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantOperationManage")]
public class MerchantLimitationsQueryResponse : WechatPayResponse
{
    /// <summary>子商户号（<c>sub_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>管控措施列表（<c>limitation_list</c>，必填 array）。</summary>
    [JsonPropertyName("limitation_list")]
    public MerchantLimitationItem[]? LimitationList { get; set; }

    /// <summary>总数（<c>total_count</c>，必填 integer）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }
}

/// <summary>
/// 管控措施项（嵌套在数组中）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MerchantOperationManage")]
public class MerchantLimitationItem
{
    /// <summary>管控措施类型（<c>limitation_type</c>，必填 string），取值见 <see cref="MerchantLimitationTypes"/>。</summary>
    [JsonPropertyName("limitation_type")]
    public string? LimitationType { get; set; }

    /// <summary>管控原因（<c>limitation_reason</c>，选填 string(500)）。</summary>
    [JsonPropertyName("limitation_reason")]
    public string? LimitationReason { get; set; }

    /// <summary>管控开始时间（<c>start_time</c>，选填 string，rfc3339）。</summary>
    [JsonPropertyName("start_time")]
    public string? StartTime { get; set; }

    /// <summary>管控结束时间（<c>end_time</c>，选填 string，rfc3339；为空表示长期管控）。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }
}
