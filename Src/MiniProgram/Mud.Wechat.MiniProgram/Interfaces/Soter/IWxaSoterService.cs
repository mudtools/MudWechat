// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「生物认证」域 SDK（1 端点：SOTER 生物认证秘钥签名验证）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>（小程序服务端 API → 生物认证，2026-10-10 依据官方清单核验）：
/// <c>soter/api_verifysignature.html</c>。
/// </para>
/// <para>
/// <b>业务形态（官方原文）</b>：本接口用于 SOTER 生物认证秘钥签名验证 —— 小程序前端经
/// <c>wx.startSoterAuthentication</c> 完成生物认证（指纹/人脸）后，服务端用本接口校验签名真实性。
/// </para>
/// <para>
/// <b>令牌与鉴权</b>：走应用级 <c>access_token</c>（Query）。官方为 <b>POST</b>。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Soter", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaSoterService
{
    /// <summary>
    /// 生物认证秘钥签名验证。官方文档：<c>soter/api_verifysignature.html</c>。
    /// </summary>
    /// <param name="request">验证请求（<c>openid</c> / <c>json_string</c> / <c>json_signature</c> 必填），见 <see cref="DataModels.Soter.WxaSoterVerifySignatureRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>验证结果（<c>is_ok</c>），见 <see cref="DataModels.Soter.WxaSoterVerifySignatureResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/cgi-bin/soter/verify_signature</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para>
    /// <b>数据来源（官方原文）</b>：<c>json_string</c> / <c>json_signature</c> 取自已通过
    /// <c>wx.startSoterAuthentication</c> 的成功回调（<c>resultJSON</c> / <c>resultJSONSignature</c>），
    /// 三者须同属一次认证流程。
    /// </para>
    /// </remarks>
    [Post("/cgi-bin/soter/verify_signature")]
    Task<DataModels.Soter.WxaSoterVerifySignatureResponse> VerifySignatureAsync(
        [Body] DataModels.Soter.WxaSoterVerifySignatureRequest request,
        CancellationToken cancellationToken = default);
}