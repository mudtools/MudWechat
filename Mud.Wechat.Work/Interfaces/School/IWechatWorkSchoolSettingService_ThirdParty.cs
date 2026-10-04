// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「家校沟通」模块家校管理配置域第三方应用 SDK。
/// <para>
/// 官方仅向自建与第三方应用开放本域 3 个端点，全部收敛于
/// <see cref="IWechatWorkSchoolSettingService"/>；本接口不新增端点，仅作为
/// 第三方应用的类型化契约入口存在（形态对齐 <see cref="IWechatWorkThirdPartyBatchService"/> 空标记）。
/// </para>
/// <para>自建应用见 <see cref="IWechatWorkInternalSchoolSettingService"/>；
/// 官方未向服务商代开发开放本域，不设代开发子接口。</para>
/// </summary>
/// <remarks>
/// <para>
/// 消费授权企业级 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>，scope = authCorpId），
/// 由多应用基座按当前应用上下文路由——企业级令牌须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。
/// 官方权限口径：第三方应用须具有「家校沟通」使用和编辑权限。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "School",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkSchoolSettingService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkThirdPartySchoolSettingService : IWechatWorkSchoolSettingService
{
}
