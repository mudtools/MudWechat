// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Student;

/// <summary>快速获取学生身份请求体（<c>POST /intp/quickcheckstudentidentity</c>）。</summary>
/// <remarks>
/// <para>官方文档：<c>student/api_quickcheckstudentidentity.html</c>。</para>
/// <para>
/// 业务场景（官方原文）：小程序接入<b>学生验证</b>能力后，用户在授权弹窗同意后，
/// 即可<b>快速获取学生身份</b>（免填证件号流程）。<c>wx_studentcheck_code</c> 为
/// 用户授权查询 Code（由小程序端学生验证组件返回，一次性有效）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Student")]
public class WxaQuickCheckStudentIdentityRequest
{
    /// <summary>用户唯一标识（<c>openid</c>，必填）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>用户授权查询 Code（<c>wx_studentcheck_code</c>，必填；一次性有效）。</summary>
    [JsonPropertyName("wx_studentcheck_code")]
    public string? CheckCode { get; set; }
}
