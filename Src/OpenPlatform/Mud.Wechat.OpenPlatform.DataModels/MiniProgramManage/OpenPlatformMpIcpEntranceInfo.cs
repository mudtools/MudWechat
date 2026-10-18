// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

/// <summary>
/// ICP 备案入口信息（<c>get_icp_entrance_info</c> 应答 <c>info</c>）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/record/getIcpEntranceInfo.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MiniProgramManage")]
public class OpenPlatformMpIcpEntranceInfo
{
    /// <summary>备案状态（官方 <c>status</c>，number）。</summary>
    [JsonPropertyName("status")]
    public long? Status { get; set; }

    /// <summary>是否正在注销（官方 <c>is_canceling</c>，boolean）。</summary>
    [JsonPropertyName("is_canceling")]
    public bool? IsCanceling { get; set; }

    /// <summary>可执行的备案操作（官方 <c>available</c>，number）。</summary>
    [JsonPropertyName("available")]
    public long? Available { get; set; }

    /// <summary>短信核验状态（官方 <c>sms_verify_status</c>，number）。</summary>
    [JsonPropertyName("sms_verify_status")]
    public long? SmsVerifyStatus { get; set; }

    /// <summary>审核数据（官方 <c>audit_data</c>，array；官方未固定结构，原样承载）。</summary>
    [JsonPropertyName("audit_data")]
    public List<System.Text.Json.JsonElement>? AuditData { get; set; }
}
