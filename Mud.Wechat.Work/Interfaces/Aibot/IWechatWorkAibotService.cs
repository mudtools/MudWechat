// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「智能机器人」模块接口族公共 SDK（官方「服务端API → 消息接收与发送 → 智能机器人」）。
/// <para>
/// 官方文档树把智能机器人整体置于「<b>企业自建应用开发</b>」分类下，全部 9 篇文档正文<b>零提及</b>
/// 第三方应用 / 服务商代开发；机器人的创建与凭证（API 模式下的回调 URL/Token/EncodingAESKey，
/// 或长连接的 BotID/Secret）<b>均在企业微信管理后台配置</b>，
/// 故本域不设 <c>_ThirdParty</c> / <c>_Provider</c> 子接口
/// （形态对齐 <see cref="IWechatWorkSmartSheetGroupChatService"/>：零端点父接口 + 唯一自建子接口承载端点）。
/// </para>
/// <para>
/// 端点全部声明于 <see cref="IWechatWorkInternalAibotService"/>；继承链上不得出现代开发 / 第三方子接口。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// <b>令牌面：本域端点不使用任何令牌链路</b>（与 <c>access_token</c> / <c>suite_access_token</c> /
/// <c>provider_access_token</c> 三条链路均无关）。官方 101138 主动回复接口以 URL 上的
/// <c>response_code</c> 为一次性凭据（每个 <c>response_url</c> 仅调用一次、有效期 1 小时），
/// 故本域父 / 子接口<b>均不声明 <c>[Token]</c></b> —— 这是「无令牌端点」的第二个既存例外
/// （第一个为 <see cref="IWechatWorkProviderAuthenticationUrl"/>：令牌经显式 Query 参数直传）。
/// 父接口不进入令牌归属域守卫 TO1 的枚举面，子接口的例外登记见 <c>WechatTokenOwnerContractGuards</c>。
/// </para>
/// <para>
/// <b>服务商的唯一交集</b>：<c>AccountId</c> 域的
/// <see cref="IWechatWorkAccountIdBotService"/>（官方 96516 / 97106，以 <c>provider_access_token</c> 鉴权）
/// 用于把「企业主体下的加密 userid」转成「服务商主体下的 open_userid」——与本域能力线正交，无需新令牌链。
/// </para>
/// <para>
/// 官方凭证脱敏：<c>response_code</c> 为<b>一次性 URL 凭据</b>（1 小时窗口），
/// 其参数名不在组件 <c>SensitiveUrlRedactor</c> 脱敏词表内（该词表为精确匹配），
/// 已在方案文档中登记为已知接受风险（泄露窗口有限且一次性），不在 SDK 内抢占组件脱敏面。
/// </para>
/// </remarks>
[HttpClientApi(IsAbstract = true)]
public interface IWechatWorkAibotService
{
}
