// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.ExternalContact.Customer;

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「客户联系」模块客户管理域公共 SDK（客户列表 / 客户详情 / 批量详情 / 备注信息 / 客户联系规则组管理）。
/// <para>
/// 三类应用公共面端点声明于本接口；能力差异端点声明于应用类型子接口：
/// 自建应用见 <see cref="IWechatWorkInternalExternalContactCustomerService"/>（零差异端点，空标记），
/// 第三方应用见 <see cref="IWechatWorkThirdPartyExternalContactCustomerService"/>
/// （unionid 与 external_userid 的关联、代开发 external_userid 转换），
/// 服务商代开发见 <see cref="IWechatWorkProviderExternalContactCustomerService"/>（零差异端点，空标记）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 三类应用消费的令牌路由键均为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>）：
/// 企业自建为应用自身 access_token；第三方 / 代开发为授权企业级 access_token（scope = authCorpId），
/// 由多应用基座按当前应用上下文路由——企业级令牌须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。
/// </para>
/// <para>
/// 权限口径：客户读取 / 备注端点要求自建应用配置到「客户联系 可调用接口的应用」中，
/// 第三方 / 代开发应用须具有「客户基础信息」权限；规则组端点要求具有「管理客户联系规则组」权限，
/// 且应用仅能获取和管理由本应用创建的规则组。
/// 敏感字段官方口径见 <see cref="ExternalContactInfo"/> / <see cref="CustomerFollowUser"/> 注释
/// （avatar、gender、unionid、remark_mobiles 第三方 / 代开发不可获取等）。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkExternalContactCustomerService
{
    /// <summary>
    /// 获取客户列表
    /// <para>获取指定成员添加的客户列表（客户指配置了客户联系功能的成员所添加的外部联系人，无分页）。</para>
    /// <para>应用只能获取到可见范围内的配置了客户联系功能的成员；「营销获客」应用只能获取到该应用带来的客户。</para>
    /// </summary>
    /// <param name="userid">企业成员的 userid。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>外部联系人的 userid 列表（external_userid）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92113"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92264"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96314"/></para>
    /// </remarks>
    [Get("/cgi-bin/externalcontact/list")]
    Task<GetCustomerListResponse> GetCustomerListAsync(
        [Query("userid")] string userid,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取客户详情
    /// <para>根据外部联系人的 userid 拉取客户详情（含客户基本信息与全部跟进人信息）。</para>
    /// <para>当客户在企业内的跟进人超过 500 人时需要使用 <paramref name="cursor"/> 参数进行分页获取。</para>
    /// </summary>
    /// <param name="externalUserid">外部联系人的 userid（注意不是企业成员的账号）。</param>
    /// <param name="cursor">上次请求返回的 next_cursor（跟进人超过 500 人时分页获取）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>客户基本信息（external_contact）与跟进人列表（follow_user）及下一页游标（next_cursor）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92114"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92265"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96315"/></para>
    /// </remarks>
    [Get("/cgi-bin/externalcontact/get")]
    Task<GetCustomerDetailResponse> GetCustomerDetailAsync(
        [Query("external_userid")] string externalUserid,
        [Query("cursor")] string? cursor = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量获取客户详情
    /// <para>获取指定成员（最多 100 个）添加的客户信息列表；cursor + limit 分页（limit 最大 100，默认 50）。</para>
    /// <para>若请求中所有 userid 都无有效互通许可，接口直接报错 701008；部分无许可时返回成功并在
    /// fail_info.unlicensed_userid_list 列出。</para>
    /// </summary>
    /// <param name="request">批量查询请求体（<see cref="BatchGetCustomerDetailsRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>客户信息列表（external_contact_list）与下一页游标（next_cursor）及失败信息（fail_info）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92994"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93010"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96316"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/batch/get_by_user")]
    Task<BatchGetCustomerDetailsResponse> BatchGetCustomerDetailsAsync(
        [Body] BatchGetCustomerDetailsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改客户备注信息
    /// <para>修改指定用户添加的客户的备注信息。应用仅可编辑可见范围内的成员添加的企业客户备注信息。</para>
    /// <para>各可填字段不可同时为空；填写 remark_mobiles 将整体覆盖旧备注手机号（清除全部传空字符串）。</para>
    /// </summary>
    /// <param name="request">备注请求体（<see cref="UpdateCustomerRemarkRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>修改结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92115"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92694"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96317"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/remark")]
    Task<WechatWorkResponse> UpdateCustomerRemarkAsync(
        [Body] UpdateCustomerRemarkRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取规则组列表
    /// <para>获取企业配置的所有客户规则组 id 列表；cursor + limit 分页（limit 默认 / 上限均为 1000）。</para>
    /// <para>应用仅能获取和管理由本应用创建的规则组。</para>
    /// </summary>
    /// <param name="request">分页请求体（<see cref="GetCustomerStrategyListRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>规则组 id 列表（strategy）与下一页游标（next_cursor）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94883"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99540"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99543"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/customer_strategy/list")]
    Task<GetCustomerStrategyListResponse> GetCustomerStrategyListAsync(
        [Body] GetCustomerStrategyListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取规则组详情
    /// <para>获取某个客户规则组的详细信息（含权限配置 privilege）。</para>
    /// <para>应用仅能获取和管理由本应用创建的规则组。</para>
    /// </summary>
    /// <param name="request">详情请求体（<see cref="GetCustomerStrategyDetailRequest"/>：strategy_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>规则组详情（strategy）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94883"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99540"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99543"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/customer_strategy/get")]
    Task<GetCustomerStrategyDetailResponse> GetCustomerStrategyDetailAsync(
        [Body] GetCustomerStrategyDetailRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取规则组管理范围
    /// <para>获取某个客户规则组管理的成员和部门列表；cursor + limit 分页（limit 默认 / 上限均为 1000）。</para>
    /// </summary>
    /// <param name="request">范围请求体（<see cref="GetCustomerStrategyRangeRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>管理范围节点列表（range）与下一页游标（next_cursor）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94883"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99540"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99543"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/customer_strategy/get_range")]
    Task<GetCustomerStrategyRangeResponse> GetCustomerStrategyRangeAsync(
        [Body] GetCustomerStrategyRangeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建新的规则组
    /// <para>创建一个新的客户规则组。</para>
    /// <para><b>危险操作约束</b>：该接口仅支持串行调用，请勿并发创建规则组；单次最多可配置 20 个管理员和
    /// 100 个管理节点；管理组最大层级 5 层；每个管理组的管理范围内最多支持 3000 个节点。
    /// 若创建具有父规则组的规则组，其管理范围必须是父规则组的子集且完全继承父规则组的权限配置（privilege 将被忽略）。</para>
    /// </summary>
    /// <param name="request">创建请求体（<see cref="CreateCustomerStrategyRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>新建规则组的规则组 id（strategy_id）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94883"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99540"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99543"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/customer_strategy/create")]
    Task<CreateCustomerStrategyResponse> CreateCustomerStrategyAsync(
        [Body] CreateCustomerStrategyRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 编辑规则组及其管理范围
    /// <para>编辑规则组的基本信息和修改客户规则组管理范围。</para>
    /// <para><b>危险操作约束</b>：该接口仅支持串行调用，请勿并发修改规则组；单次最多可配置 20 个管理员和
    /// 100 个管理节点；admin_list / privilege 传值即整体覆盖旧配置。</para>
    /// </summary>
    /// <param name="request">编辑请求体（<see cref="UpdateCustomerStrategyRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>编辑结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94883"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99540"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99543"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/customer_strategy/edit")]
    Task<WechatWorkResponse> UpdateCustomerStrategyAsync(
        [Body] UpdateCustomerStrategyRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除规则组
    /// <para>删除某个规则组。应用仅能删除由本应用创建的规则组。</para>
    /// </summary>
    /// <param name="request">删除请求体（<see cref="DeleteCustomerStrategyRequest"/>：strategy_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>删除结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94883"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99540"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99543"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/customer_strategy/del")]
    Task<WechatWorkResponse> DeleteCustomerStrategyAsync(
        [Body] DeleteCustomerStrategyRequest request,
        CancellationToken cancellationToken = default);
}
