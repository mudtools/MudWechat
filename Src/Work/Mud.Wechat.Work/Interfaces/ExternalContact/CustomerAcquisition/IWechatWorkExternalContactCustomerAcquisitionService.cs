// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.ExternalContact.CustomerAcquisition;

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「客户联系」模块获客助手域公共 SDK
/// （获客链接管理 + 获客客户列表 + 额度与使用统计 + 成员多次收消息详情）。
/// <para>
/// 官方对三类应用开放完全一致的 9 个端点，全部收敛声明于本接口；应用类型子接口均为零差异端点空标记：
/// 自建应用见 <see cref="IWechatWorkInternalExternalContactCustomerAcquisitionService"/>，
/// 第三方应用见 <see cref="IWechatWorkThirdPartyExternalContactCustomerAcquisitionService"/>，
/// 服务商代开发见 <see cref="IWechatWorkProviderExternalContactCustomerAcquisitionService"/>。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 三类应用消费的令牌路由键均为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>）：
/// 企业自建为应用自身 access_token；第三方 / 代开发为授权企业级 access_token（scope = authCorpId），
/// 由多应用基座按当前应用上下文路由——企业级令牌须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。
/// </para>
/// <para>
/// 权限口径：自建应用须使用配置到「可调用应用」列表中的 secret 获取的 access_token 调用；
/// 第三方 / 代开发应用须具有「企业客户权限-&gt;获客助手」权限；不支持客户联系系统应用调用；
/// 「营销获客」应用使用链接管理端点须具有「建联客户信息」权限。
/// </para>
/// <para>
/// 官方约束：获客链接使用范围的成员与部门不可同时为空，覆盖总人数不超过 500，
/// 且须在应用可见范围或客户可建联成员范围内；优先分配选项仅部分经营类目企业支持。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkExternalContactCustomerAcquisitionService
{
    /// <summary>
    /// 获取获客链接列表
    /// <para>分页获取企业创建的获客链接 id 列表，详情须逐条调用「获取获客链接详情」。</para>
    /// </summary>
    /// <param name="request">分页请求体（<see cref="GetAcquisitionLinkListRequest"/>：limit / cursor）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>获客链接 id 列表（link_id_list）与下一页游标（next_cursor）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97297"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97394"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97398"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/customer_acquisition/list_link")]
    Task<GetAcquisitionLinkListResponse> GetAcquisitionLinkListAsync(
        [Body] GetAcquisitionLinkListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取获客链接详情
    /// <para>通过获客链接 id 获取链接详情（含名称、url、使用范围与优先分配选项）。</para>
    /// </summary>
    /// <param name="request">详情请求体（<see cref="GetAcquisitionLinkDetailRequest"/>：link_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>获客链接详情（link）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97297"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97394"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97398"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/customer_acquisition/get")]
    Task<GetAcquisitionLinkDetailResponse> GetAcquisitionLinkAsync(
        [Body] GetAcquisitionLinkDetailRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建获客链接
    /// <para>创建一个获客链接，返回链接 id 与实际链接 url。</para>
    /// <para>使用范围的成员与部门不可同时为空，覆盖总人数不超过 500，
    /// 且须在应用可见范围或客户可建联成员范围内；优先分配选项仅部分经营类目企业支持。</para>
    /// </summary>
    /// <param name="request">创建请求体（<see cref="CreateAcquisitionLinkRequest"/>：link_name / range / priority_option 等）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>创建的获客链接信息（link：link_id / link_name / url / create_time）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97297"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97394"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97398"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/customer_acquisition/create_link")]
    Task<CreateAcquisitionLinkResponse> CreateAcquisitionLinkAsync(
        [Body] CreateAcquisitionLinkRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 编辑获客链接
    /// <para>编辑指定获客链接；使用范围与优先分配选项均为覆盖式更新。</para>
    /// <para>仅能编辑当前应用创建的获客链接；优先分配选项仅部分经营类目企业支持。</para>
    /// </summary>
    /// <param name="request">编辑请求体（<see cref="UpdateAcquisitionLinkRequest"/>：link_id + 覆盖字段）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>编辑结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97297"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97394"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97398"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/customer_acquisition/update_link")]
    Task<WechatWorkResponse> UpdateAcquisitionLinkAsync(
        [Body] UpdateAcquisitionLinkRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除获客链接
    /// <para>删除指定获客链接。仅能删除当前应用创建的获客链接。</para>
    /// </summary>
    /// <param name="request">删除请求体（<see cref="DeleteAcquisitionLinkRequest"/>：link_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>删除结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97297"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97394"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97398"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/customer_acquisition/delete_link")]
    Task<WechatWorkResponse> DeleteAcquisitionLinkAsync(
        [Body] DeleteAcquisitionLinkRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取由获客链接添加的客户列表
    /// <para>分页获取由指定获客链接添加的客户列表（含跟进人、会话状态与渠道参数 state）。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetAcquisitionCustomerListRequest"/>：link_id / limit / cursor）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>获客客户列表（customer_list）与下一页游标（next_cursor）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97298"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97395"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97399"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/customer_acquisition/customer")]
    Task<GetAcquisitionCustomerListResponse> GetAcquisitionCustomerListAsync(
        [Body] GetAcquisitionCustomerListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询获客助手剩余使用量
    /// <para>查询获客助手的历史累计使用量、剩余使用量与即将过期的额度明细。</para>
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>累计使用量（total）、剩余使用量（balance）与额度列表（quota_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97375"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97396"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97400"/></para>
    /// </remarks>
    [Get("/cgi-bin/externalcontact/customer_acquisition_quota")]
    Task<GetAcquisitionQuotaResponse> GetAcquisitionQuotaAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询获客链接使用详情
    /// <para>按天统计指定获客链接的点击链接客户数与新增客户数。</para>
    /// <para>统计范围的最小粒度为日（起止时间戳自动转换为所在日，闭区间）；
    /// 仅可查询最近 180 天内的使用记录，起止时间相差不可超过 30 天。</para>
    /// </summary>
    /// <param name="request">统计请求体（<see cref="GetAcquisitionLinkStatisticRequest"/>：link_id / start_time / end_time）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>点击链接客户数（click_link_customer_cnt）与新增客户数（new_customer_cnt）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97375"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97396"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97400"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/customer_acquisition/statistic")]
    Task<GetAcquisitionLinkStatisticResponse> GetAcquisitionLinkStatisticAsync(
        [Body] GetAcquisitionLinkStatisticRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取成员多次收消息详情
    /// <para>查询成员多次收消息情况（收消息次数、客户 id 与来源获客链接信息）。</para>
    /// <para>chat_key 来自「成员多次收消息事件」回调，回调后 30 分钟内有效，需及时调用。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetAcquisitionChatInfoRequest"/>：chat_key）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成员 userid、客户 external_userid 与会话详情（chat_info）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100130"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100134"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100133"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/customer_acquisition/get_chat_info")]
    Task<GetAcquisitionChatInfoResponse> GetAcquisitionChatInfoAsync(
        [Body] GetAcquisitionChatInfoRequest request,
        CancellationToken cancellationToken = default);
}
