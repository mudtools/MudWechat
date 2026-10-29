// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

/// <summary>
/// 隐私接口项（<c>get_privacy_interface</c> 应答 <c>interface_list[]</c>）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/privacy-api-management/getPrivacyInterface.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MiniProgramManage")]
public class OpenPlatformMpPrivacyInterfaceItem
{
    /// <summary>隐私接口名称（官方 <c>api_name</c>）。</summary>
    [JsonPropertyName("api_name")]
    public string? ApiName { get; set; }

    /// <summary>接口中文名称（官方 <c>api_ch_name</c>）。</summary>
    [JsonPropertyName("api_ch_name")]
    public string? ApiChName { get; set; }

    /// <summary>接口说明（官方 <c>api_desc</c>）。</summary>
    [JsonPropertyName("api_desc")]
    public string? ApiDesc { get; set; }

    /// <summary>申请时间（官方 <c>apply_time</c>，number 时间戳）。</summary>
    [JsonPropertyName("apply_time")]
    public long? ApplyTime { get; set; }

    /// <summary>接口状态（官方 <c>status</c>，number）。</summary>
    [JsonPropertyName("status")]
    public long? Status { get; set; }

    /// <summary>审核单 id（官方 <c>audit_id</c>，number）。</summary>
    [JsonPropertyName("audit_id")]
    public long? AuditId { get; set; }

    /// <summary>审核失败原因（官方 <c>fail_reason</c>）。</summary>
    [JsonPropertyName("fail_reason")]
    public string? FailReason { get; set; }

    /// <summary>接口文档链接（官方 <c>api_link</c>）。</summary>
    [JsonPropertyName("api_link")]
    public string? ApiLink { get; set; }

    /// <summary>接口分组名称（官方 <c>group_name</c>）。</summary>
    [JsonPropertyName("group_name")]
    public string? GroupName { get; set; }
}
