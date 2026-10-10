// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「用工关系」域 SDK（2 端点：推送用工消息 + 解绑用工关系）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>（小程序服务端 API → 用工关系，2026-10-10 依据官方清单核验）：
/// <c>laboruse/api_sendemployeerelationmsg.html</c>、<c>laboruse/api_unbinduserb2cauthinfo.html</c>。
/// </para>
/// <para>
/// <b>业务形态（官方原文）</b>：用工关系面向「用工单位 + 劳动者」的小程序用工协作场景；
/// 企业可向已建立用工关联的用户推送消息，或按需解绑用户的 B2C 授权信息。
/// </para>
/// <para>
/// <b>令牌与鉴权</b>：全部走应用级 <c>access_token</c>（Query）。两端点官方均为 <b>POST</b>。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "LaborUse", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaLaborUseService
{
    /// <summary>
    /// 推送用工消息。官方文档：<c>laboruse/api_sendemployeerelationmsg.html</c>。
    /// </summary>
    /// <param name="request">推送请求（<c>template_id</c> / <c>touser</c> 必填），见 <see cref="DataModels.LaborUse.WxaSendEmployeeRelationMessageRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>标准应答（<c>errcode</c> / <c>errmsg</c>），见 <see cref="DataModels.WxaResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/cgi-bin/message/wxopen/employeerelationmsg/send</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para><b>推送语义（官方原文）</b>：仅可向<b>已建立用工关联</b>的用户推送；
    /// <c>data</c> 为键值对 JSON 字符串（模板填充数据）。</para>
    /// </remarks>
    [Post("/cgi-bin/message/wxopen/employeerelationmsg/send")]
    Task<DataModels.WxaResponse> SendEmployeeRelationMessageAsync(
        [Body] DataModels.LaborUse.WxaSendEmployeeRelationMessageRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 解绑用工关系（批量解绑用户的 B2C 授权信息）。官方文档：<c>laboruse/api_unbinduserb2cauthinfo.html</c>。
    /// </summary>
    /// <param name="request">解绑请求（<c>openid_list</c> 必填），见 <see cref="DataModels.LaborUse.WxaUnbindUserB2CAuthInfoRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>标准应答（<c>errcode</c> / <c>errmsg</c>），见 <see cref="DataModels.WxaResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/wxa/business/unbinduserb2cauthinfo</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para><b>批量语义（官方原文）</b>：<c>openid_list</c> 为待解绑用户列表（非空，单次上限以官方页面为准）；解绑后无法撤回。</para>
    /// </remarks>
    [Post("/wxa/business/unbinduserb2cauthinfo")]
    Task<DataModels.WxaResponse> UnbindUserB2CAuthInfoAsync(
        [Body] DataModels.LaborUse.WxaUnbindUserB2CAuthInfoRequest request,
        CancellationToken cancellationToken = default);
}