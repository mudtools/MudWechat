// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions.Authentication;

/// <summary>
/// 服务商 provider_access_token 令牌管理器（路由键 <see cref="WechatTokenTypes.ProviderAccessToken"/>）。
/// </summary>
/// <remarks>
/// 租户语义：provider_access_token 是「一个服务商全部分身」的共享凭据，凭据
/// （corpid + provider_secret）无租户属性。实现 <see cref="ISharedTokenManager"/>
/// （Mud.HttpUtils v2.0.9）后，基类租户绑定守卫默认豁免，多应用上下文（AppKey）
/// 可复用同一管理器而不触发租户绑定守卫。
/// </remarks>
public interface IWechatProviderTokenManager : ITokenManager
{
}
