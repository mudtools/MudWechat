// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「家校沟通」模块部门管理域企业自建应用 SDK。
/// <para>
/// 官方对三类应用开放完全一致的 5 个端点，全部收敛于 <see cref="IWechatWorkSchoolDepartmentService"/>；
/// 本接口不新增端点，仅作为自建应用的类型化契约入口存在
/// （形态对齐 <see cref="IWechatWorkInternalSchoolService"/> 空标记）。
/// </para>
/// <para>第三方应用见 <see cref="IWechatWorkThirdPartySchoolDepartmentService"/>；
/// 服务商代开发见 <see cref="IWechatWorkProviderSchoolDepartmentService"/>。</para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>）。官方权限口径：
/// 写入族端点须配置到「家校沟通-读取和编辑家校通讯录的应用」中；
/// 获取部门列表亦接受「家校沟通-读取家校通讯录的应用」或「家长可使用应用」列表。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "School",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkSchoolDepartmentService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalSchoolDepartmentService : IWechatWorkSchoolDepartmentService
{
}
