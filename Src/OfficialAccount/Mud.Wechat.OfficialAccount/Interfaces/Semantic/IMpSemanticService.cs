// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount;

/// <summary>
/// 微信公众号 / 服务号「语义理解（智能对话旧接口）」域 SDK
/// （<c>/semantic/semproxy/search</c> 单端点——按服务类型对自然语言做结构化理解）。
/// </summary>
/// <remarks>
/// <para>
/// <b>停维警示</b>：本域是微信「智能对话」旧接口，官方长期未迭代；新项目应改用微信智能对话平台。
/// SDK 按官方原样承载——<see cref="MpSemanticResult.Details"/> 以原始 JSON 透出
/// （官方 details 结构随服务类型有二十余种形态，逐形态建模会引入开放多态 DTO，
/// 与本仓「不做运行时多态」红线冲突，裁决见 <see cref="MpSemanticResult"/>）。
/// </para>
/// <para>
/// MUD005 已知接受风险：微信公众号官方契约强制令牌走 Query 参数（<c>access_token</c>），
/// 无法改用 Header；URL 遥测已由组件 <c>SensitiveUrlRedactor</c> 与 <c>MpException</c> 构造期脱敏处置。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Semantic", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IMpSemanticService
{
    /// <summary>
    /// 语义理解检索。官方文档：<see href="https://developers.weixin.qq.com/doc/offiaccount/Intelligent_Interface/Natural_Language_Processing.html"/>
    /// （官方接口英文名 <c>semproxySearch</c>）。
    /// </summary>
    /// <param name="request">检索请求（<c>query</c> + <c>type</c> 必填；位置上下文可选）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>语义结果（intent / query / type + 原样 details，解析见 <see cref="MpSemanticResult"/>）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体；<c>type</c> 取值见官方服务分类表（weather / travel / music 等）。</para>
    /// <para>官方错误码：<c>60010</c> / <c>60011</c> / <c>60015</c>（本页错误码表照录）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/semantic/semproxy/search")]
    Task<MpSemanticSearchResponse> SearchAsync(
        [Body] MpSemanticSearchRequest request,
        CancellationToken cancellationToken = default);
}
