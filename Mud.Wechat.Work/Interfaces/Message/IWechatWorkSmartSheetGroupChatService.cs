// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「消息推送」模块智能表格自动化创建的群聊域公共 SDK
/// （获取群聊列表 / 获取群聊会话 / 修改群聊会话）。
/// <para>
/// 官方仅向<b>企业自建应用</b>开放本域 3 个端点（wedoc/smartsheet/groupchat/list、get、update），
/// 第三方应用与服务商代开发均无对应文档，不设子接口，因此本父接口没有公共端点；
/// 全部 3 个端点声明于 <see cref="IWechatWorkInternalSmartSheetGroupChatService"/>
/// （唯一的应用类型子接口，落位形态对齐 <see cref="IWechatWorkInternalAppChatService"/>）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 落位与形态对齐 <see cref="IWechatWorkAppChatService"/>：令牌路由键为
/// <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>，应用自身凭证），
/// 由多应用基座按当前应用上下文（AppKey + scope）路由。
/// </para>
/// <para>
/// 权限口径：仅自建应用可调用，且需配置到文档「可调用应用」列表中的应用，
/// 应用的可见范围需包含根部门；操作的智能表格须为当前应用创建。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkSmartSheetGroupChatService
{
}
