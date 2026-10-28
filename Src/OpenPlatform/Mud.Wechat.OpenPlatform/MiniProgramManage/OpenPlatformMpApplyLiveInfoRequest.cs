// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

/// <summary>
/// 申请开通小程序直播请求体（<c>POST /wxa/business/applyliveinfo</c>）。
/// </summary>
/// <remarks>
/// <para><b>单操作驱动</b>：官方仅定义 <c>action=apply</c> 一种取值，用以提交开通申请。</para>
/// <para><b>频率与维护窗口</b>：官方限制 100 次/天；错误码 <c>268489001</c> 表示系统维护中。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MiniProgramManage")]
public class OpenPlatformMpApplyLiveInfoRequest
{
    /// <summary>操作类型（必填），当前仅 <c>apply</c>。</summary>
    [JsonPropertyName("action")]
    public string? Action { get; set; }
}
