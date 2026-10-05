// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「日程」模块待办域公共 SDK（获取待办详情 + 更新待办状态）。
/// <para>
/// 官方仅向企业自建应用开放本域全部 2 个端点：第三方应用开发与服务商代开发均无对应 API。
/// 按官方开放面收敛为「零端点父接口 + 唯一自建子接口承载端点」形态
/// （对齐紧急通知域 / 身份验证二次验证族）：
/// 端点全部声明于 <see cref="IWechatWorkInternalScheduleTodoService"/>，
/// 继承链上不得出现代开发 / 第三方子接口（能力漂移守卫）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>，
/// Query 注入 <c>access_token</c>）。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkScheduleTodoService
{
}
