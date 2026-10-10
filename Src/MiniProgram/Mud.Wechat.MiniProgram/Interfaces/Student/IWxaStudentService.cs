// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「学生身份」域 SDK（1 端点：快速获取学生身份）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>（小程序服务端 API → 学生身份，2026-10-10 依据官方清单核验）：
/// <c>student/api_quickcheckstudentidentity.html</c>。
/// </para>
/// <para>
/// <b>业务形态（官方原文）</b>：小程序接入<b>学生验证</b>能力后，用户在授权弹窗同意后即可快速获取
/// 学生身份（免证件号填写流程）；服务端凭 <c>wx_studentcheck_code</c> 校验并返回身份结果。
/// </para>
/// <para>
/// <b>令牌与鉴权</b>：走应用级 <c>access_token</c>（Query）。官方为 <b>POST</b>。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Student", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaStudentService
{
    /// <summary>
    /// 快速获取学生身份。官方文档：<c>student/api_quickcheckstudentidentity.html</c>。
    /// </summary>
    /// <param name="request">查询请求（<c>openid</c> / <c>wx_studentcheck_code</c> 必填），见 <see cref="DataModels.Student.WxaQuickCheckStudentIdentityRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>学生身份结果（<c>bind_status</c> / <c>is_student</c>），见 <see cref="DataModels.Student.WxaQuickCheckStudentIdentityResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/intp/quickcheckstudentidentity</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para><b>Code 时效（官方原文）</b>：<c>wx_studentcheck_code</c> 为小程序端学生验证组件的授权查询 Code，
    /// <b>一次性有效</b>；成功获取后如再查询须重新拉起授权。</para>
    /// </remarks>
    [Post("/intp/quickcheckstudentidentity")]
    Task<DataModels.Student.WxaQuickCheckStudentIdentityResponse> QuickCheckStudentIdentityAsync(
        [Body] DataModels.Student.WxaQuickCheckStudentIdentityRequest request,
        CancellationToken cancellationToken = default);
}