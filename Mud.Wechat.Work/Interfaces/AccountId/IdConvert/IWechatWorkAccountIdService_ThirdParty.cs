// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「账号ID」域「ID 转换」接口族第三方应用 SDK（类型化契约入口）。
/// <para>
/// 官方对第三方应用与代开发开放完全一致的 9 个端点，全部声明于公共父接口
/// <see cref="IWechatWorkAccountIdService"/>，本接口为空标记；
/// 「群 ID 升级」差异端点仅代开发应用持有（见 <see cref="IWechatWorkProviderAccountIdService"/>）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "AccountId",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkAccountIdService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkThirdPartyAccountIdService : IWechatWorkAccountIdService
{
}
