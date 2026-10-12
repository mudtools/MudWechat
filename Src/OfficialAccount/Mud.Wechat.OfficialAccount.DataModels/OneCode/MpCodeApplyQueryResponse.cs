// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.OneCode;

/// <summary>
/// 查询二维码申请单（<c>POST /intp/marketcode/applycodequery</c>）响应。
/// </summary>
/// <remarks>
/// <para>
/// 官方字段表（平级）：<c>status</c>（申请单状态）/ <c>application_id</c> / <c>isv_application_id</c> /
/// <c>code_generate_list</c>（二维码信息数组）/ <c>create_time</c> / <c>update_time</c>。
/// </para>
/// <para>
/// 官方文档缺陷（照录）：<c>status</c> <b>无枚举表</b>（仅返回示例出现 <c>FINISH</c>，SDK 只登记该示例值，
/// 见 <c>MpCodeApplyStatuses</c>）；<c>create_time</c>/<c>update_time</c> 未说明单位与格式。
/// </para>
/// <para>官方错误码：<c>40001</c>（本页错误码表仅此行，照录）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OneCode")]
public class MpCodeApplyQueryResponse : MpResponse
{
    /// <summary>获取或设置申请单状态（官方 <c>status</c>；官方未给枚举表，示例为 <c>FINISH</c>）。</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>获取或设置申请单号（官方 <c>application_id</c>）。</summary>
    [JsonPropertyName("application_id")]
    public long? ApplicationId { get; set; }

    /// <summary>获取或设置外部单号（官方 <c>isv_application_id</c>）。</summary>
    [JsonPropertyName("isv_application_id")]
    public string? IsvApplicationId { get; set; }

    /// <summary>获取或设置二维码信息（官方 <c>code_generate_list</c>；码段起止，供下载与激活使用）。</summary>
    [JsonPropertyName("code_generate_list")]
    public List<MpCodeRange>? CodeGenerateList { get; set; }

    /// <summary>获取或设置创建时间（官方 <c>create_time</c>；官方未说明单位与格式）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>获取或设置更新时间（官方 <c>update_time</c>；官方未说明单位与格式）。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }
}
