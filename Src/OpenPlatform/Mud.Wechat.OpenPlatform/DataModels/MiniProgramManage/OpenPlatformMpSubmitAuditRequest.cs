// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

/// <summary>
/// 「提交审核」（<c>submit_audit</c>）请求。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/code-management/submitAudit.html"/>。
/// </summary>
/// <remarks>
/// <para>官方约束：<b>串行提交</b>（上一次审核未结束不得再次提交），<c>item_list</c> <b>最多 5 个</b>页面。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MiniProgramManage")]
public class OpenPlatformMpSubmitAuditRequest
{
    /// <summary>审核页面列表（官方必填 <c>item_list</c>，最多 5 项）。</summary>
    [JsonPropertyName("item_list")]
    public List<OpenPlatformMpAuditItem>? ItemList { get; set; }

    /// <summary>反馈信息（官方选填 <c>feedback_info</c>，string）。</summary>
    [JsonPropertyName("feedback_info")]
    public string? FeedbackInfo { get; set; }

    /// <summary>反馈截图 <c>mediaid</c>（官方选填 <c>feedback_stuff</c>，string）。</summary>
    [JsonPropertyName("feedback_stuff")]
    public string? FeedbackStuff { get; set; }

    /// <summary>版本描述（官方选填 <c>version_desc</c>，string）。</summary>
    [JsonPropertyName("version_desc")]
    public string? VersionDesc { get; set; }

    /// <summary>预览信息（官方选填 <c>preview_info</c>）。</summary>
    [JsonPropertyName("preview_info")]
    public OpenPlatformMpAuditPreviewInfo? PreviewInfo { get; set; }

    /// <summary>用户生成内容声明（官方选填 <c>ugc_declare</c>）。</summary>
    [JsonPropertyName("ugc_declare")]
    public OpenPlatformMpAuditUgcDeclare? UgcDeclare { get; set; }

    /// <summary>是否未使用隐私接口（官方选填 <c>privacy_api_not_use</c>，boolean）。</summary>
    [JsonPropertyName("privacy_api_not_use")]
    public bool? PrivacyApiNotUse { get; set; }

    /// <summary>订单路径（官方选填 <c>order_path</c>，string）。</summary>
    [JsonPropertyName("order_path")]
    public string? OrderPath { get; set; }
}
