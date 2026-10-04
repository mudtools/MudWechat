// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Kf;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「微信客服」模块微信客服组件域第三方应用 SDK：
/// 官方仅向微信客服组件应用（套件形态）开放本域端点，全部 3 个端点声明于本接口。
/// </summary>
/// <remarks>
/// <para>
/// 消费授权企业级 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>，企业级令牌一企一份，
/// 宿主须以授权企业 scope 获取）。官方权限口径：
/// 「管理接入的微信客服」相应权限——账号信息与链接须具有「获取企业授权接入的客服账号→客服账号信息与链接」权限
/// （且仅可获取企业已授权的客服账号）；数据统计须具有「管理来自该场景的客服咨询→获取客服数据统计」权限，
/// 并需在场景中配置获取到的带参客服账号链接。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Kf",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkKfComponentService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkThirdPartyKfComponentService : IWechatWorkKfComponentService
{
    /// <summary>
    /// 获取客服账号列表（组件版）
    /// <para>分页获取企业<b>已授权</b>给微信客服组件的客服账号列表；分页形态为 offset + limit，
    /// 当返回的账号数量小于 limit 时表示已无更多数据，应终止获取。</para>
    /// <para>与客服账号管理域共用路由 <c>/cgi-bin/kf/account/list</c>；
    /// 组件应用调用时响应不返回 manage_privilege 字段。</para>
    /// </summary>
    /// <param name="request">分页请求体（<see cref="GetKfAccountListRequest"/>：offset / limit）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>客服账号信息列表（account_list）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99368"/></para>
    /// </remarks>
    [Post("/cgi-bin/kf/account/list")]
    Task<GetKfAccountListResponse> GetAccountListAsync(
        [Body] GetKfAccountListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取客服账号链接（组件版）
    /// <para>获取企业已授权客服账号的咨询链接；scene 非空时返回链接可拼接
    /// <c>scene_param=SCENE_PARAM</c> 参数使用，用户进入会话事件会原样返回该值。</para>
    /// <para>与客服账号管理域共用路由 <c>/cgi-bin/kf/add_contact_way</c>；
    /// 返回的客服链接不能修改或复制参数到其他链接使用，否则进入会话事件参数校验不通过，导致无法回调。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetKfAccountContactWayRequest"/>：open_kfid / scene）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>客服链接（url）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99400"/></para>
    /// </remarks>
    [Post("/cgi-bin/kf/add_contact_way")]
    Task<GetKfAccountContactWayResponse> GetAccountContactWayAsync(
        [Body] GetKfAccountContactWayRequest request,
        CancellationToken cancellationToken = default);

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
