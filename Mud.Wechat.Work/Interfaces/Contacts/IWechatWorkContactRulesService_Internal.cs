// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Contacts.ContactRules;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信通讯录「通讯录查看权限管理」（通讯录隐藏规则）域企业自建应用 SDK：官方仅向自建应用开放本域端点
/// （创建规则 / 读取规则列表 / 修改规则 / 删除规则），全部声明于本接口。
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>）。官方权限口径：
/// <b>仅通讯录同步应用</b>的 access_token 可调用本域端点（宿主须将通讯录同步助手的 secret 配置为对应应用的 AgentSecret）。
/// 创建 / 修改 / 删除共用频率限制：1 分钟 5 次、1 小时 10 次、1 天 60 次。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Contact",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkContactRulesService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.InternalAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalContactRulesService : IWechatWorkContactRulesService
{
    /// <summary>
    /// 创建规则
    /// <para>批量创建通讯录隐藏规则（隐藏部门/成员、限制查看外部门、限制查看所有人），一次最多 100 条。</para>
    /// <para>仅通讯录同步应用可调用；频率限制 1 分钟 5 次、1 小时 10 次、1 天 60 次（创建/修改/删除共用）。</para>
    /// </summary>
    /// <param name="request">创建规则请求体（<see cref="CreateContactRulesRequest"/>；每条规则的 RuleId 不填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>创建结果（rule_ids 与请求规则顺序对应）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101536"/></para>
    /// </remarks>
    [Post("/cgi-bin/contactrule/create")]
    Task<CreateContactRulesResponse> CreateContactRulesAsync(
        [Body] CreateContactRulesRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 读取规则列表
    /// <para>读取企业当前配置的全部通讯录隐藏规则。</para>
    /// <para>仅通讯录同步应用可调用；官方契约本端点为 POST 且无请求体参数。</para>
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>规则列表（rules；含规则类型、目标范围、白名单、排除名单与搜索/会话开关）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101537"/></para>
    /// </remarks>
    [Post("/cgi-bin/contactrule/list")]
    Task<GetContactRulesResponse> GetContactRulesAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改规则
    /// <para>批量修改通讯录隐藏规则，一次最多 100 条；每条规则必须携带 rule_id，且不能更新规则类型。</para>
    /// <para>仅通讯录同步应用可调用；频率限制 1 分钟 5 次、1 小时 10 次、1 天 60 次（创建/修改/删除共用）。</para>
    /// </summary>
    /// <param name="request">修改规则请求体（<see cref="UpdateContactRulesRequest"/>；每条规则的 RuleId 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>修改结果（errcode/errmsg；官方返回示例含修改后规则的 rules 回显，见 <see cref="UpdateContactRulesResponse"/>）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101539"/></para>
    /// </remarks>
    [Post("/cgi-bin/contactrule/update")]
    Task<UpdateContactRulesResponse> UpdateContactRulesAsync(
        [Body] UpdateContactRulesRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除规则
    /// <para>按 rule_id 批量删除通讯录隐藏规则，一次最多 100 个。</para>
    /// <para>仅通讯录同步应用可调用；频率限制 1 分钟 5 次、1 小时 10 次、1 天 60 次（创建/修改/删除共用）。</para>
    /// </summary>
    /// <param name="request">删除规则请求体（<see cref="DeleteContactRulesRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>删除结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101540"/></para>
    /// </remarks>
    [Post("/cgi-bin/contactrule/delete")]
    Task<WechatWorkResponse> DeleteContactRulesAsync(
        [Body] DeleteContactRulesRequest request,
        CancellationToken cancellationToken = default);
}
