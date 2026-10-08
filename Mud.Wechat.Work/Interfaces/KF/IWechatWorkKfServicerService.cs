// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Kf;

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「微信客服」模块接待人员管理域公共 SDK
/// （接待人员添加 / 删除 / 列表查询）。
/// <para>
/// 官方对三类应用开放完全一致的 3 个端点，全部收敛声明于本接口；应用类型子接口均为零差异端点空标记：
/// 自建应用见 <see cref="IWechatWorkInternalKfServicerService"/>，
/// 第三方应用见 <see cref="IWechatWorkThirdPartyKfServicerService"/>，
/// 服务商代开发见 <see cref="IWechatWorkProviderKfServicerService"/>。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 三类应用消费的令牌路由键均为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>）：
/// 企业自建为应用自身 access_token；第三方 / 代开发为授权企业级 access_token（scope = authCorpId），
/// 由多应用基座按当前应用上下文路由——企业级令牌须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。
/// </para>
/// <para>
/// 权限口径：自建应用须配置到「微信客服-可调用接口的应用」中；
/// 第三方 / 代开发应用须具有「微信客服-&gt;管理账号、分配会话和收发消息」权限（添加 / 删除端点）
/// 或「微信客服-&gt;获取基础信息」权限（列表端点），且须有该客服账号的管理权限。
/// </para>
/// <para>
/// 官方约束：添加 / 删除接待人员的 userid_list 与 department_id_list 至少填一个；
/// 每个客服账号最多可添加 2000 个接待人员、20 个接待部门；批量操作为逐条返回结果，单条失败不整体报错；
/// 第三方 / 代开发应用的 userid 为密文 userid（即 open_userid）；
/// 接待人员应在应用的可见范围内；只能通过 API 管理企业指定的客服账号；
/// 自 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口（存量企业暂不受影响）。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkKfServicerService
{
    /// <summary>
    /// 添加接待人员
    /// <para>向客服账号添加接待人员（成员或部门）；userid_list 与 department_id_list 至少填一个，
    /// 每个客服账号最多可添加 2000 个接待人员、20 个接待部门。</para>
    /// <para>第三方 / 代开发应用填密文 userid（即 open_userid）；批量操作为逐条返回结果，单条失败不整体报错。</para>
    /// </summary>
    /// <param name="request">添加请求体（<see cref="AddKfServicerRequest"/>：open_kfid / userid_list / department_id_list）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>逐条操作结果列表（result_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94646"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94646"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96418"/></para>
    /// </remarks>
    [Post("/cgi-bin/kf/servicer/add")]
    Task<AddKfServicerResponse> AddServicerAsync(
        [Body] AddKfServicerRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除接待人员
    /// <para>从客服账号删除接待人员；userid_list 与 department_id_list 至少填一个，
    /// 超出单次上限（成员 100 个 / 部门 100 个）需分批调用。</para>
    /// <para>第三方 / 代开发应用填密文 userid（即 open_userid）；批量操作为逐条返回结果，单条失败不整体报错。</para>
    /// </summary>
    /// <param name="request">删除请求体（<see cref="DeleteKfServicerRequest"/>：open_kfid / userid_list / department_id_list）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>逐条操作结果列表（result_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94647"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94647"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96419"/></para>
    /// </remarks>
    [Post("/cgi-bin/kf/servicer/del")]
    Task<DeleteKfServicerResponse> DeleteServicerAsync(
        [Body] DeleteKfServicerRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取接待人员列表
    /// <para>获取指定客服账号的接待人员列表（含按成员与按部门接待的条目及接待状态）。</para>
    /// </summary>
    /// <param name="openKfId">客服账号 ID（官方必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>接待人员列表（servicer_list，含 userid / 接待状态 / 停止接待子类型 / 部门 id）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94645"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94645"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96420"/></para>
    /// </remarks>
    [Get("/cgi-bin/kf/servicer/list")]
    Task<GetKfServicerListResponse> GetServicerListAsync(
        [Query("open_kfid")] string openKfId,
        CancellationToken cancellationToken = default);
}
