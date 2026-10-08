// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Kf;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「微信客服」模块微信客服组件域第三方应用 SDK：
/// 官方仅向微信客服组件应用（套件形态）开放本域端点，端点全部收敛声明于本接口。
/// </summary>
/// <remarks>
/// <para>
/// 消费授权企业级 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>，企业级令牌一企一份，
/// 宿主须以授权企业 scope 获取）。官方权限口径：
/// 「管理接入的微信客服」相应权限——数据统计须具有「管理来自该场景的客服咨询→获取客服数据统计」权限，
/// 并需在场景中配置获取到的带参客服账号链接。
/// </para>
/// <para>
/// 架构决策（单一所有者）：获取客服账号列表（<c>/cgi-bin/kf/account/list</c>）与获取客服账号链接
/// （<c>/cgi-bin/kf/add_contact_way</c>）曾在本接口重复声明——二者与「客服账号管理域」
/// <see cref="IWechatWorkKfAccountService"/> 为同路由、<b>同一批DTO</b>（无平行家族），
/// 现已收敛为客服账号管理域单一所有者声明。组件应用请注入
/// <see cref="IWechatWorkThirdPartyKfAccountService"/> 调用
/// <see cref="IWechatWorkKfAccountService.GetAccountListAsync"/> 与
/// <see cref="IWechatWorkKfAccountService.GetAccountContactWayAsync"/>
/// （组件形态下响应不返回 manage_privilege 字段，无需另一套契约面）。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Kf",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkKfComponentService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.CorpAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkThirdPartyKfComponentService : IWechatWorkKfComponentService
{
    /// <summary>
    /// 获取客服数据统计（组件版）
    /// <para>按天获取组件场景的客服统计数据（企业级口径，无 open_kfid 入参）；
    /// 组件应用须具有「管理来自该场景的客服咨询→获取客服数据统计」权限，
    /// 且需在场景中配置获取到的带参客服账号链接。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetKfComponentStatisticRequest"/>：start_time / end_time）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>逐日统计列表（statistic_list）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99367"/></para>
    /// </remarks>
    [Post("/cgi-bin/kf/get_statistic")]
    Task<GetKfComponentStatisticResponse> GetStatisticAsync(
        [Body] GetKfComponentStatisticRequest request,
        CancellationToken cancellationToken = default);
}
