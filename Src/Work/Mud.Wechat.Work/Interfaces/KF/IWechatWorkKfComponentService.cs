// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「微信客服」模块微信客服组件域公共 SDK。
/// <para>
/// 官方将本域端点收录于第三方应用开发文档的「微信客服组件」目录，仅由<b>微信客服组件应用</b>（套件形态）
/// 消费，因此本父接口没有公共端点，亦不设自建 / 代开发子接口；
/// 全部 3 个端点声明于 <see cref="IWechatWorkThirdPartyKfComponentService"/>
/// （形态对齐 <see cref="IWechatWorkExternalContactAcquisitionComponentService"/> 零端点父接口）。
/// </para>
/// <para>
/// 组件版「获取客服账号列表」「获取客服账号链接」与客服账号管理域<b>共用路由</b>
/// （<c>/cgi-bin/kf/account/list</c>、<c>/cgi-bin/kf/add_contact_way</c>），
/// 但组件应用仅可获取<b>企业已授权</b>的客服账号，且「获取客服账号列表」响应不返回 manage_privilege 字段。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 落位与形态对齐获客助手组件域：令牌路由键为 <see cref="WechatTokenTypes.AccessToken"/>
/// （Query 注入 <c>access_token</c>），由多应用基座按当前应用上下文（AppKey + scope）路由。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkKfComponentService
{
}
