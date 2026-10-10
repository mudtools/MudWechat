// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.CorpGroup.Rules;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「上下游规则」（对接规则）域企业自建应用 SDK：官方仅向自建应用开放本域端点
/// （获取对接规则 id 列表 / 删除对接规则 / 获取对接规则详情 / 新增对接规则 / 更新对接规则），全部声明于本接口。
/// <para>第三方应用与服务商代开发官方无对应文档，不设子接口。</para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>）。官方权限口径：
/// <b>仅适用于上下游中创建空间的主企业</b>调用，自建应用须配置到「上下游-可调用接口的应用」中；
/// 操作的规则对应的企业成员和部门都需要在应用的可见范围内；
/// 新增和更新对接规则的接口<b>每天最多调用 1000 次</b>（自 2023-12-01 起不再支持系统应用 secret 调用）。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "CorpGroup",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkCorpGroupRulesService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.InternalAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalCorpGroupRulesService : IWechatWorkCorpGroupRulesService
{
    /// <summary>
    /// 获取对接规则 id 列表
    /// <para>获取企业上下游规则 id 列表；仅适用于上下游中创建空间的主企业调用。</para>
    /// </summary>
    /// <param name="request">规则 id 列表请求体（<see cref="ListChainRuleIdsRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>上下游关系规则的 id 列表（rule_ids）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95631"/></para>
    /// </remarks>
    [Post("/cgi-bin/corpgroup/rule/list_ids")]
    Task<ListChainRuleIdsResponse> ListChainRuleIdsAsync(
        [Body] ListChainRuleIdsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除对接规则
    /// <para>删除企业上下游规则；操作的规则对应的企业成员和部门都需要在应用的可见范围内。</para>
    /// </summary>
    /// <param name="request">删除规则请求体（<see cref="DeleteChainRuleRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>删除结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95632"/></para>
    /// </remarks>
    [Post("/cgi-bin/corpgroup/rule/delete_rule")]
    Task<WechatWorkResponse> DeleteChainRuleAsync(
        [Body] DeleteChainRuleRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取对接规则详情
    /// <para>获取企业上下游规则详情（上游对接人范围 owner_corp_range + 下游企业范围 member_corp_range）。</para>
    /// </summary>
    /// <param name="request">规则详情请求体（<see cref="GetChainRuleInfoRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>上下游关系规则的详情（rule_info）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95633"/></para>
    /// </remarks>
    [Post("/cgi-bin/corpgroup/rule/get_rule_info")]
    Task<GetChainRuleInfoResponse> GetChainRuleInfoAsync(
        [Body] GetChainRuleInfoRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 新增对接规则
    /// <para>新增一条上下游对接规则；每天最多调用 1000 次（与更新规则共用额度）。</para>
    /// </summary>
    /// <param name="request">新增规则请求体（<see cref="AddChainRuleRequest"/>；对接人范围与下游企业范围各自的两个列表均须至少填一个）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>新建规则结果（rule_id）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95634"/></para>
    /// </remarks>
    [Post("/cgi-bin/corpgroup/rule/add_rule")]
    Task<AddChainRuleResponse> AddChainRuleAsync(
        [Body] AddChainRuleRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新对接规则
    /// <para>修改一条上下游对接规则；每天最多调用 1000 次（与新增规则共用额度）。</para>
    /// </summary>
    /// <param name="request">更新规则请求体（<see cref="ModifyChainRuleRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>更新结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95635"/></para>
    /// </remarks>
    [Post("/cgi-bin/corpgroup/rule/modify_rule")]
    Task<WechatWorkResponse> ModifyChainRuleAsync(
        [Body] ModifyChainRuleRequest request,
        CancellationToken cancellationToken = default);
}
