// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「高级功能（成员申请单）」域<b>自建应用</b> SDK（类型化契约入口）。
/// <para>官方 3 支端点（99880 / 99883 / 99885）仅对自建应用开放，声明于公共父接口
/// <see cref="IWechatWorkAdvancedFeatureService"/>，本接口为其自建应用形态的契约入口。</para>
/// </summary>
/// <remarks>
/// <para>
/// <b>本域唯一子接口（官方开放面锁定）</b>：三支端点的应用类型表均为
/// 「自建应用：配置到『高级功能 - 成员申请管理 - 通过 api 设置申请流程』中；
/// 代开发应用：暂不支持；第三方应用：暂不支持」⇒ 本域<b>不得</b>出现第三方 / 代开发子接口，
/// 由本域契约守卫（AF 系列）锁定零回潮。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// <para>
/// <b>高危写面提示</b>：<see cref="IWechatWorkAdvancedFeatureService.SetApprovalDetailAsync"/>
/// 为<b>覆盖式全量写入</b>（只传部分节点等于删除其余节点），调用方应置于自身幂等 / 审批保护之下。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "AdvancedFeature",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkAdvancedFeatureService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.InternalAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalAdvancedFeatureService : IWechatWorkAdvancedFeatureService
{
}
