// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.ExternalContact.InterceptRule;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「客户联系」模块聊天敏感词（敏感词规则管理）域公共 SDK
/// （新建 / 列表 / 详情 / 修改 / 删除敏感词规则）。
/// <para>
/// 官方对三类应用开放完全一致的 5 个端点，全部收敛声明于本接口；应用类型子接口均为零差异端点空标记：
/// 自建应用见 <see cref="IWechatWorkInternalExternalContactInterceptRuleService"/>，
/// 第三方应用见 <see cref="IWechatWorkThirdPartyExternalContactInterceptRuleService"/>，
/// 服务商代开发见 <see cref="IWechatWorkProviderExternalContactInterceptRuleService"/>。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 三类应用消费的令牌路由键均为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>）：
/// 企业自建为应用自身 access_token；第三方 / 代开发为授权企业级 access_token（scope = authCorpId），
/// 由多应用基座按当前应用上下文路由——企业级令牌须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。
/// </para>
/// <para>
/// 权限口径：自建应用须配置到「客户联系 可调用接口的应用」中，第三方 / 代开发应用须具有
/// 「管理敏感词」权限；企业敏感词规则条数上限为 100 个；
/// 列表可获取企业所有规则，详情仅返回应用可见范围内的成员和部门，修改 / 删除仅可操作自己创建的规则。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkExternalContactInterceptRuleService
{
    /// <summary>
    /// 新建敏感词规则
    /// <para>创建一个聊天敏感词规则（敏感词列表 + 语义规则 + 拦截方式 + 适用范围）。</para>
    /// <para>规则名称 1 ~ 20 个 UTF-8 字符；敏感词列表不超过 300 个（每个词 1 ~ 32 个 UTF-8 字符）；
    /// 适用范围的成员与部门不可同时为空；企业敏感词规则条数上限 100 个。</para>
    /// </summary>
    /// <param name="request">创建请求体（<see cref="AddInterceptRuleRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>规则 id（rule_id）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95097"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95130"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96346"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/add_intercept_rule")]
    Task<AddInterceptRuleResponse> AddInterceptRuleAsync(
        [Body] AddInterceptRuleRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取敏感词规则列表
    /// <para>获取企业配置的全部聊天敏感词规则摘要（rule_id / rule_name / create_time）。</para>
    /// <para>官方契约本端点为 GET 且无业务参数。</para>
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>规则摘要列表（rule_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95097"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95130"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96346"/></para>
    /// </remarks>
    [Get("/cgi-bin/externalcontact/get_intercept_rule_list")]
    Task<GetInterceptRuleListResponse> GetInterceptRuleListAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取敏感词规则详情
    /// <para>通过规则 id 获取敏感词规则完整配置；使用范围字段只返回应用可见范围内的成员和部门。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetInterceptRuleRequest"/>：rule_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>规则详情（rule）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95097"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95130"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96346"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/get_intercept_rule")]
    Task<GetInterceptRuleResponse> GetInterceptRuleAsync(
        [Body] GetInterceptRuleRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改敏感词规则
    /// <para>修改敏感词规则；除 rule_id 外仅需更新的字段才填，使用范围通过 add / remove 两侧增删。</para>
    /// <para>应用只可修改自己创建的规则；extra_rule 的语义规则列表传空表示清除全部语义规则。</para>
    /// </summary>
    /// <param name="request">修改请求体（<see cref="UpdateInterceptRuleRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>修改结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95097"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95130"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96346"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/update_intercept_rule")]
    Task<WechatWorkResponse> UpdateInterceptRuleAsync(
        [Body] UpdateInterceptRuleRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除敏感词规则
    /// <para>删除指定敏感词规则。应用只可删除自己创建的规则。</para>
    /// </summary>
    /// <param name="request">删除请求体（<see cref="DeleteInterceptRuleRequest"/>：rule_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>删除结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95097"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95130"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96346"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/del_intercept_rule")]
    Task<WechatWorkResponse> DeleteInterceptRuleAsync(
        [Body] DeleteInterceptRuleRequest request,
        CancellationToken cancellationToken = default);
}
