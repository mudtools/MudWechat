// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.MerchantManage;

/// <summary>
/// 商户管理记录查询响应（<c>GET /v3/mch-manage/mch-manage-records/{manage_record_id}</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方页面</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014940383">4014940383</see>
/// （更新于 2025.07.18，支持商户【普通服务商】）。
/// </para>
/// <para>
/// <b>关键字段形态</b>：<c>submit_data</c> 官方类型 <c>string(4000000)</c> —— JSON 序列化后的字符串，
/// <b>不是</b>内层对象；SDK 保持 <c>string?</c> 承载，宿主自行反序列化。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantManage")]
public class MerchantManageRecordQueryResponse : WechatPayResponse
{
    /// <summary>商户管理记录 ID（<c>manage_record_id</c>，必填 integer）。</summary>
    [JsonPropertyName("manage_record_id")]
    public long? ManageRecordId { get; set; }

    /// <summary>子商户号（<c>sub_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>管理记录状态（<c>manage_record_state</c>，必填 string），取值见 <see cref="MerchantManageRecordStates"/>。</summary>
    [JsonPropertyName("manage_record_state")]
    public string? ManageRecordState { get; set; }

    /// <summary>资料内容（<c>submit_data</c>，选填 string(4000000)）：JSON 序列化后的字符串，非对象。</summary>
    [JsonPropertyName("submit_data")]
    public string? SubmitData { get; set; }

    /// <summary>驳回原因（<c>reject_reason</c>，选填 string(1000)）：仅审核驳回时有值。</summary>
    [JsonPropertyName("reject_reason")]
    public string? RejectReason { get; set; }
}

/// <summary>
/// 按 sub_mchid 查询管理记录响应（<c>GET /v3/mch-manage/mch-manage-records/sub-mchid/{sub_mchid}</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方页面</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014940381">4014940381</see>
/// （更新于 2025.07.18，支持商户【普通服务商】）。
/// </para>
/// <para>
/// <b>与单条查询的差异</b>：本端点返回数组形态（可能多条记录），且带分页参数 <c>limit</c>（必填，上限 10）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantManage")]
public class MerchantManageRecordsBySubMchIdResponse : WechatPayResponse
{
    /// <summary>管理记录列表（<c>data</c>，必填 array）。</summary>
    [JsonPropertyName("data")]
    public MerchantManageRecordItem[]? Data { get; set; }

    /// <summary>总数（<c>total_count</c>，必填 integer）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }
}

/// <summary>
/// 管理记录项（嵌套在数组响应中）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MerchantManage")]
public class MerchantManageRecordItem
{
    /// <summary>商户管理记录 ID（<c>manage_record_id</c>，必填 integer）。</summary>
    [JsonPropertyName("manage_record_id")]
    public long? ManageRecordId { get; set; }

    /// <summary>管理记录状态（<c>manage_record_state</c>，必填 string）。</summary>
    [JsonPropertyName("manage_record_state")]
    public string? ManageRecordState { get; set; }

    /// <summary>资料内容（<c>submit_data</c>，选填 string(4000000)）：JSON 序列化后的字符串。</summary>
    [JsonPropertyName("submit_data")]
    public string? SubmitData { get; set; }

    /// <summary>驳回原因（<c>reject_reason</c>，选填 string(1000)）。</summary>
    [JsonPropertyName("reject_reason")]
    public string? RejectReason { get; set; }
}

/// <summary>
/// 提交管理记录资料请求（<c>POST /v3/mch-manage/mch-manage-records/{manage_record_id}/mch-manage-submissions</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方页面</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014940417">4014940417</see>
/// （更新于 2025.07.18，支持商户【普通服务商】）。
/// </para>
/// <para>
/// <b>成功应答</b>：204 No Content（无包体）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantManage")]
public class MerchantManageSubmissionRequest
{
    /// <summary>子商户号（<c>sub_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>资料内容（<c>submit_data</c>，必填 string(4000000)）：JSON 序列化后的字符串。</summary>
    [JsonPropertyName("submit_data")]
    public string? SubmitData { get; set; }
}

/// <summary>
/// 上传管理记录文件元数据（multipart/form-data 的 <c>meta</c> 字段）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方页面</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4014940425">4014940425</see>
/// （更新于 2025.07.18，支持商户【普通服务商】）。
/// </para>
/// <para>
/// <b>表单字段名</b>：<c>submission_file</c>（文件）+ <c>meta</c>（JSON 对象）。
/// 与媒体上传的 <c>file</c> + <c>meta</c> 不同，此处为独立命名空间。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantManage")]
public class MerchantManageFileMeta
{
    /// <summary>文件名（<c>file_name</c>，必填 string(128)）。</summary>
    [JsonPropertyName("file_name")]
    public string? FileName { get; set; }

    /// <summary>文件摘要（<c>file_digest</c>，必填 string(64)）：SHA-256 值。</summary>
    [JsonPropertyName("file_digest")]
    public string? FileDigest { get; set; }

    /// <summary>资料项 ID（<c>item_id</c>，必填 string(64)）。</summary>
    [JsonPropertyName("item_id")]
    public string? ItemId { get; set; }

    /// <summary>商户管理记录 ID（<c>manage_record_id</c>，选填 integer）：部分场景需要。</summary>
    [JsonPropertyName("manage_record_id")]
    public long? ManageRecordId { get; set; }

    /// <summary>子商户号（<c>sub_mchid</c>，选填 string(32)）：部分场景需要。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }
}
