// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Gov;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「政民沟通」模块巡查上报族企业自建应用 SDK：
/// 官方仅向自建应用开放本族 6 个端点（官方权限表对代开发 / 第三方均标注「暂不支持」），全部声明于本接口
/// （形态对齐 <see cref="IWechatWorkInternalIdentityTfaService"/>：零端点父接口 + 唯一自建子接口承载端点）。
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>）。官方权限口径：
/// 自建应用须配置到「巡查上报 - 可调用接口的应用」中；获取个人数据统计时成员必须在应用可见范围内。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Gov",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkGovPatrolService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.InternalAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalGovPatrolService : IWechatWorkGovPatrolService
{
    /// <summary>
    /// 获取配置的网格及网格负责人
    /// <para>获取政民沟通「巡查上报」已配置的网格及各网格管理员列表。</para>
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>网格列表（grid_list：grid_id / grid_name / grid_admin 管理员 userId 列表）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93531"/></para>
    /// <para>官方权限：自建应用须配置到「巡查上报 - 可调用接口的应用」中；代开发应用暂不支持；第三方应用暂不支持。</para>
    /// </remarks>
    [Get("/cgi-bin/report/patrol/get_grid_info")]
    Task<GovPatrolGetGridInfoResponse> GetGridInfoAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取单位巡查上报数据统计
    /// <para>获取企业或指定网格维度的巡查上报数据统计概况。</para>
    /// <para>官方业务限制：grid_id 不传的话获取整个企业的概况。</para>
    /// </summary>
    /// <param name="request">统计请求体（<see cref="GovPatrolGetCorpStatusRequest"/>：grid_id 可选）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>单位统计（待分配 / 办理中 / 今日上报 / 今日办结 / 累计上报 / 累计办结）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93532"/></para>
    /// <para>官方权限：自建应用须配置到「巡查上报 - 可调用接口的应用」中；代开发应用暂不支持；第三方应用暂不支持。</para>
    /// </remarks>
    [Post("/cgi-bin/report/patrol/get_corp_status")]
    Task<GovPatrolGetCorpStatusResponse> GetCorpStatusAsync(
        [Body] GovPatrolGetCorpStatusRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取个人巡查上报数据统计
    /// <para>获取指定成员的巡查上报数据统计。</para>
    /// <para>官方业务限制：成员必须在应用可见范围内。</para>
    /// </summary>
    /// <param name="request">统计请求体（<see cref="GovPatrolGetUserStatusRequest"/>：userid 官方必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>个人统计（办理中 / 今日上报 / 今日办结）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93533"/></para>
    /// <para>官方权限：自建应用须配置到「巡查上报 - 可调用接口的应用」中；代开发应用暂不支持；第三方应用暂不支持。</para>
    /// </remarks>
    [Post("/cgi-bin/report/patrol/get_user_status")]
    Task<GovPatrolGetUserStatusResponse> GetUserStatusAsync(
        [Body] GovPatrolGetUserStatusRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取上报事件分类统计
    /// <para>按事件分类维度获取巡查上报的累计上报与累计办结统计。</para>
    /// <para>官方业务限制：category_id 不传此字段，能拉取到所有一级分类的数据；
    /// 传一级分类的 category_id 能拉取到该一级分类下的所有二级分类的数据。</para>
    /// </summary>
    /// <param name="request">统计请求体（<see cref="GovPatrolGetCategoryStatisticRequest"/>：category_id 可选）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>分类统计列表（dashboard_list：分类 ID / 名称 / 等级 / 累计上报 / 累计办结 / 分类类型）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93534"/></para>
    /// <para>官方权限：自建应用须配置到「巡查上报 - 可调用接口的应用」中；代开发应用暂不支持；第三方应用暂不支持。</para>
    /// </remarks>
    [Post("/cgi-bin/report/patrol/category_statistic")]
    Task<GovPatrolGetCategoryStatisticResponse> GetCategoryStatisticAsync(
        [Body] GovPatrolGetCategoryStatisticRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取巡查上报事件列表
    /// <para>分页获取巡查上报工单列表（含发生地点、当前处理人与历史流程）。</para>
    /// <para>官方业务限制：cursor 首次查询为空、查询条件有变更需置空；
    /// limit 不填默认 20 条、最大 50；响应 next_cursor 为空字符串代表是最后一页。</para>
    /// </summary>
    /// <param name="request">列表请求体（<see cref="GovPatrolGetOrderListRequest"/>：begin_create_time /
    /// begin_modify_time 时间戳筛选 + cursor/limit 分页）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>工单列表（order_list）与翻页凭据（next_cursor）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93536"/></para>
    /// <para>官方权限：自建应用须配置到「巡查上报 - 可调用接口的应用」中；代开发应用暂不支持；第三方应用暂不支持。</para>
    /// </remarks>
    [Post("/cgi-bin/report/patrol/get_order_list")]
    Task<GovPatrolGetOrderListResponse> GetOrderListAsync(
        [Body] GovPatrolGetOrderListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取巡查上报的事件详情信息
    /// <para>按工单 id 获取巡查上报单个工单的详情（含发生地点、当前处理人与历史流程）。</para>
    /// <para>官方业务限制：order_id 不为空的话，其他参数无效。</para>
    /// </summary>
    /// <param name="request">详情请求体（<see cref="GovPatrolGetOrderInfoRequest"/>：order_id 官方必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>工单详情（order_info，结构与事件列表元素一致）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93535"/></para>
    /// <para>官方权限：自建应用须配置到「巡查上报 - 可调用接口的应用」中；代开发应用暂不支持；第三方应用暂不支持。</para>
    /// </remarks>
    [Post("/cgi-bin/report/patrol/get_order_info")]
    Task<GovPatrolGetOrderInfoResponse> GetOrderInfoAsync(
        [Body] GovPatrolGetOrderInfoRequest request,
        CancellationToken cancellationToken = default);
}
