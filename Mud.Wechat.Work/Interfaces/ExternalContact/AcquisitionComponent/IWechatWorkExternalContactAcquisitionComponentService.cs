// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「客户联系」模块「获客助手组件」域公共 SDK。
/// <para>
/// 获客助手组件是服务商以<b>第三方应用（套件）</b>形态承接企业获客的官方组件，官方仅向第三方应用开放
/// （企业自建应用与服务商代开发均无对应功能），因此本父接口没有公共端点，亦不设自建 / 代开发子接口；
/// 全部 6 个端点声明于 <see cref="IWechatWorkThirdPartyExternalContactAcquisitionComponentService"/>
/// （形态对齐 <see cref="IWechatWorkExternalContactServedContactService"/> 的单应用类型收敛模式）。
/// </para>
/// <para>
/// 注意：代支付流水查询（<c>/cgi-bin/service/customer_acquisition/get_bill_list</c>）官方契约以
/// <c>suite_access_token</c> 鉴权，与本族端点的企业级 <c>access_token</c> 分属不同令牌路由键，
/// 独立声明于 <see cref="IWechatWorkExternalContactAcquisitionComponentBillService"/> 接口族。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 落位与形态对齐 <see cref="IWechatWorkExternalContactServedContactService"/>：令牌路由键为
/// <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>），由多应用基座按当前应用上下文
/// （AppKey + scope）路由；企业级令牌一企一份，宿主须以授权企业 scope 获取。
/// </para>
/// <para>
/// 获客助手事件通知（官方文档 99485：组件链接加粉、多次收消息、余额与代付消耗等事件）经企业微信<b>回调推送</b>
/// 至服务商的指令回调 URL，由 <c>Mud.Wechat.Work.Callback</c> 包的事件管线承载，不在本接口族声明 HTTP 端点；
/// 本域 <c>get_chat_info</c> 的 <c>chat_key</c> 即来自成员多次收消息事件的回调内容（回调后 30 分钟内有效）。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkExternalContactAcquisitionComponentService
{
}
