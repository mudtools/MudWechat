// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions.Authentication;

/// <summary>
/// 第三方/服务商套件 suite_access_token 令牌管理器（路由键 <see cref="WechatTokenTypes.SuiteAccessToken"/>）。
/// </summary>
/// <remarks>
/// 租户语义：suite_access_token 是「一个套件全部分身」的共享凭据，实现
/// <see cref="ISharedTokenManager"/> 后租户绑定守卫默认豁免。
/// 刷新依赖 <see cref="TokenManager.IWechatSuiteTicketProvider"/> 供应的最新 suite_ticket
/// （由微信每 10 分钟推送、回调包写入仓储）。
/// </remarks>
public interface IWechatSuiteTokenManager : ITokenManager
{
}
