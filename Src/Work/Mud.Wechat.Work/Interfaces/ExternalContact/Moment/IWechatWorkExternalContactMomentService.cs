// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.ExternalContact.Moment;

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「客户联系」模块客户朋友圈域公共 SDK
/// （发表任务管理 + 发表记录数据获取 + 朋友圈规则组管理）。
/// <para>
/// 官方对三类应用开放完全一致的 14 个端点，全部收敛声明于本接口；应用类型子接口均为零差异端点空标记：
/// 自建应用见 <see cref="IWechatWorkInternalExternalContactMomentService"/>，
/// 第三方应用见 <see cref="IWechatWorkThirdPartyExternalContactMomentService"/>，
/// 服务商代开发见 <see cref="IWechatWorkProviderExternalContactMomentService"/>。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 三类应用消费的令牌路由键均为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>）：
/// 企业自建为应用自身 access_token；第三方 / 代开发为授权企业级 access_token（scope = authCorpId），
/// 由多应用基座按当前应用上下文路由——企业级令牌须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。
/// </para>
/// <para>
/// 权限口径：发表 / 停止端点要求自建应用配置到「可调用应用」列表中，第三方 / 代开发应用须有企业授权
/// 「客户朋友圈下发表到成员客户的朋友圈」权限；数据获取端点须有「获取企业全部的发表记录」权限；
/// 规则组端点要求自建应用配置到「客户联系 可调用接口的应用」中，第三方 / 代开发应用须具有
/// 「管理客户朋友圈规则组」权限，且应用仅能获取和管理由本应用创建的规则组。
/// </para>
/// <para>
/// 官方约束：企业每个月允许通过 API 创建的朋友圈次数上限 10 万次、每分钟最多创建 10 次；
/// 自建应用调用只返回应用可见范围内用户的发送情况。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkExternalContactMomentService
{
    /// <summary>
    /// 创建发表朋友圈任务
    /// <para>创建企业发表到客户朋友圈的异步任务，返回任务 id（jobid，24 小时有效），
    /// 任务执行结果通过「获取任务创建结果」查询。</para>
    /// <para>文本与附件不能同时为空；附件最多 9 个图片、或 1 个视频、或 1 个链接（三选一，不可混用）；
    /// 企业每月允许通过 API 创建朋友圈 10 万次、每分钟最多创建 10 次。</para>
    /// </summary>
    /// <param name="request">创建任务请求体（<see cref="AddMomentTaskRequest"/>：visible_range / text / attachments）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>异步任务 id（jobid）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95094"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95095"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96351"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/add_moment_task")]
    Task<AddMomentTaskResponse> AddMomentTaskAsync(
        [Body] AddMomentTaskRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取任务创建结果
    /// <para>查询「创建发表朋友圈任务」的异步任务状态与处理结果（只能查询已经提交过的历史任务）。</para>
    /// <para>status = 3（创建完成）后 result 才有效，含朋友圈 id 与不合法的执行者 / 客户列表。</para>
    /// </summary>
    /// <param name="jobid">异步任务 id（由「创建发表朋友圈任务」返回，最大 64 字节，24 小时有效）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>任务状态（status / type）与详细处理结果（result）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95094"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95095"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96351"/></para>
    /// </remarks>
    [Get("/cgi-bin/externalcontact/get_moment_task_result")]
    Task<GetMomentTaskResultResponse> GetMomentTaskResultAsync(
        [Query("jobid")] string jobid,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 停止发表企业朋友圈
    /// <para>停止尚未发送的企业朋友圈发表任务；<b>无法撤回已经发表到客户朋友圈的信息</b>。</para>
    /// </summary>
    /// <param name="request">停止请求体（<see cref="CancelMomentTaskRequest"/>：moment_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>停止结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97612"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97616"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97615"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/cancel_moment_task")]
    Task<WechatWorkResponse> CancelMomentTaskAsync(
        [Body] CancelMomentTaskRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取企业全部的发表列表
    /// <para>按时间范围分页获取企业全部的朋友圈发表记录（含企业发表与个人发表）。</para>
    /// <para>起止时间间隔不能超过 30 天，仅取 (start_time, end_time) 范围内的数据；
    /// 已删除的朋友圈不会通过 API 返回。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetMomentListRequest"/>：start_time / end_time / creator / filter_type / cursor / limit）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>朋友圈发表记录列表（moment_list）与下一页游标（next_cursor）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93333"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93443"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96352"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/get_moment_list")]
    Task<GetMomentListResponse> GetMomentListAsync(
        [Body] GetMomentListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取客户朋友圈企业发表的列表
    /// <para>获取企业发表的朋友圈的成员执行列表与各自发表状态（仅支持企业发表的朋友圈 id）。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetMomentPublishListRequest"/>：moment_id / cursor / limit）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>发表任务列表（task_list：userid 与 publish_status）与下一页游标（next_cursor）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93333"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93443"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96352"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/get_moment_task")]
    Task<GetMomentPublishListResponse> GetMomentPublishListAsync(
        [Body] GetMomentPublishListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取客户朋友圈发表时选择的可见范围
    /// <para>获取指定成员发表的朋友圈所选可见范围内的客户列表。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetMomentCustomerListRequest"/>：moment_id / userid / cursor / limit）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>可见客户列表（customer_list）与下一页游标（next_cursor）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93333"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93443"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96352"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/get_moment_customer_list")]
    Task<GetMomentCustomerListResponse> GetMomentCustomerListAsync(
        [Body] GetMomentCustomerListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取客户朋友圈发表后的可见客户列表
    /// <para>获取指定成员发表的朋友圈实际发送成功的客户列表。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetMomentSendResultRequest"/>：moment_id / userid / cursor / limit）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>发送成功的客户列表（customer_list）与下一页游标（next_cursor）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93333"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93443"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96352"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/get_moment_send_result")]
    Task<GetMomentSendResultResponse> GetMomentSendResultAsync(
        [Body] GetMomentSendResultRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取客户朋友圈的互动数据
    /// <para>获取指定成员发表的朋友圈的评论与点赞列表（元素为客户或企业成员，二者不会同时出现）。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetMomentCommentsRequest"/>：moment_id / userid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>评论列表（comment_list）与点赞列表（like_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93333"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93443"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96352"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/get_moment_comments")]
    Task<GetMomentCommentsResponse> GetMomentCommentsAsync(
        [Body] GetMomentCommentsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取客户朋友圈规则组列表
    /// <para>获取企业配置的所有客户朋友圈规则组 id 列表；cursor + limit 分页（limit 默认 / 上限均为 1000）。</para>
    /// <para>应用仅能获取和管理由本应用创建的规则组。</para>
    /// </summary>
    /// <param name="request">分页请求体（<see cref="GetMomentStrategyListRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>朋友圈规则组 id 列表（strategy）与下一页游标（next_cursor）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94890"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99541"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99545"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/moment_strategy/list")]
    Task<GetMomentStrategyListResponse> GetMomentStrategyListAsync(
        [Body] GetMomentStrategyListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取客户朋友圈规则组详情
    /// <para>获取某个客户朋友圈规则组的详细信息（含权限配置 privilege）。</para>
    /// <para>应用仅能获取和管理由本应用创建的规则组。</para>
    /// </summary>
    /// <param name="request">详情请求体（<see cref="GetMomentStrategyDetailRequest"/>：strategy_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>朋友圈规则组详情（strategy）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94890"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99541"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99545"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/moment_strategy/get")]
    Task<GetMomentStrategyDetailResponse> GetMomentStrategyDetailAsync(
        [Body] GetMomentStrategyDetailRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取客户朋友圈规则组管理范围
    /// <para>获取某个客户朋友圈规则组管理的成员和部门列表；cursor + limit 分页（limit 默认 / 上限均为 1000）。</para>
    /// </summary>
    /// <param name="request">范围请求体（<see cref="GetMomentStrategyRangeRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>管理范围节点列表（range）与下一页游标（next_cursor）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94890"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99541"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99545"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/moment_strategy/get_range")]
    Task<GetMomentStrategyRangeResponse> GetMomentStrategyRangeAsync(
        [Body] GetMomentStrategyRangeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建新的客户朋友圈规则组
    /// <para>创建一个新的客户朋友圈规则组。</para>
    /// <para><b>危险操作约束</b>：该接口仅支持串行调用，请勿并发创建规则组；
    /// 管理员列表不可包含超级管理员，每个规则组最多 20 个负责人；
    /// 若创建具有父规则组的规则组，其管理范围必须是父规则组的子集且完全继承父规则组的权限配置（privilege 将被忽略）；
    /// 管理组最大层级 5 层，每个管理组的管理范围内最多支持 3000 个节点。</para>
    /// </summary>
    /// <param name="request">创建请求体（<see cref="CreateMomentStrategyRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>新建规则组的规则组 id（strategy_id）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94890"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99541"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99545"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/moment_strategy/create")]
    Task<CreateMomentStrategyResponse> CreateMomentStrategyAsync(
        [Body] CreateMomentStrategyRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 编辑客户朋友圈规则组及其管理范围
    /// <para>编辑客户朋友圈规则组的基本信息和修改管理范围；传入字段不填则不修改，有值则整体覆盖。</para>
    /// <para><b>危险操作约束</b>：该接口仅支持串行调用，请勿并发修改规则组；
    /// 编辑含父规则组的规则组时管理范围须为父规则组的子集且完全继承其权限配置（privilege 将被忽略）。</para>
    /// </summary>
    /// <param name="request">编辑请求体（<see cref="UpdateMomentStrategyRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>编辑结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94890"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99541"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99545"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/moment_strategy/edit")]
    Task<WechatWorkResponse> UpdateMomentStrategyAsync(
        [Body] UpdateMomentStrategyRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除客户朋友圈规则组
    /// <para>删除某个客户朋友圈规则组。应用仅能删除由本应用创建的规则组。</para>
    /// </summary>
    /// <param name="request">删除请求体（<see cref="DeleteMomentStrategyRequest"/>：strategy_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>删除结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94890"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99541"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99545"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/moment_strategy/del")]
    Task<WechatWorkResponse> DeleteMomentStrategyAsync(
        [Body] DeleteMomentStrategyRequest request,
        CancellationToken cancellationToken = default);
}
