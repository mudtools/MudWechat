// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.ExternalContact.Tag;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「客户联系」模块客户标签管理域公共 SDK（企业标签库 / 企业客户标签管理 / 客户打标签 / 规则组标签管理）。
/// <para>
/// 官方对三类应用开放完全一致的 9 个端点，全部收敛声明于本接口；应用类型子接口均为零差异端点空标记：
/// 自建应用见 <see cref="IWechatWorkInternalExternalContactTagService"/>，
/// 第三方应用见 <see cref="IWechatWorkThirdPartyExternalContactTagService"/>，
/// 服务商代开发见 <see cref="IWechatWorkProviderExternalContactTagService"/>。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 三类应用消费的令牌路由键均为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>）：
/// 企业自建为应用自身 access_token；第三方 / 代开发为授权企业级 access_token（scope = authCorpId），
/// 由多应用基座按当前应用上下文路由——企业级令牌须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。
/// </para>
/// <para>
/// 权限口径：自建应用须配置到「客户联系 可调用接口的应用」中；第三方 / 代开发应用对标签库读取须具有
/// 「客户基础信息」权限、对企业客户标签的管理（添加 / 编辑 / 删除）须具有「管理企业客户标签」权限、
/// 对规则组标签管理须具有「管理客户联系规则组」权限。应用仅能编辑和删除本应用创建的标签，
/// 仅能获取和管理由本应用创建的规则组标签；规则组标签仅可被该规则组管理范围内的企业成员使用。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkExternalContactTagService
{
    /// <summary>
    /// 获取企业标签库
    /// <para>企业和第三方服务商可通过此接口获取企业客户标签；每个企业最多可配置 10000 个企业标签。</para>
    /// <para>标签 id 与标签组 id 均为空时返回所有标签；同时传递时官方忽略标签 id，仅以标签组 id 作为过滤条件。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetCorpTagListRequest"/>：tag_id 与 group_id 均可空）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>标签组列表（tag_group，每组含组下标签 tag[]）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92696"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92696"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96320"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/get_corp_tag_list")]
    Task<GetCorpTagListResponse> GetCorpTagListAsync(
        [Body] GetCorpTagListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 添加企业客户标签
    /// <para>填写标签组 id 表示在指定标签组下添加标签（此时标签组名称与次序值被忽略）；
    /// 未填写则按标签组名称新建标签组（同名标签组会复用已存在的组，不支持创建空标签组）。
    /// 组内标签不可同名，同名标签只会创建一个。</para>
    /// </summary>
    /// <param name="request">添加请求体（<see cref="AddCorpTagRequest"/>：tag 列表官方必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>添加 / 复用的标签组（tag_group，含新建标签信息）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92696"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92696"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96320"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/add_corp_tag")]
    Task<AddCorpTagResponse> AddCorpTagAsync(
        [Body] AddCorpTagRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 编辑企业客户标签
    /// <para>仅可修改标签 / 标签组的名称与次序值；修改后的标签组不能和已有的标签组重名，
    /// 标签也不能和同一标签组下的其他标签重名。应用仅能编辑本应用创建的标签。</para>
    /// </summary>
    /// <param name="request">编辑请求体（<see cref="EditCorpTagRequest"/>：id 官方必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>编辑结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92696"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92696"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96320"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/edit_corp_tag")]
    Task<WechatWorkResponse> EditCorpTagAsync(
        [Body] EditCorpTagRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除企业客户标签
    /// <para>标签 id 与标签组 id 不可同时为空；标签组内的所有标签被删除后，标签组自动删除。
    /// 应用仅能删除本应用创建的标签。</para>
    /// </summary>
    /// <param name="request">删除请求体（<see cref="DelCorpTagRequest"/>：tag_id 与 group_id 均可空但不可同时为空）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>删除结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92696"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92696"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96320"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/del_corp_tag")]
    Task<WechatWorkResponse> DelCorpTagAsync(
        [Body] DelCorpTagRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 编辑客户企业标签（打标签）
    /// <para>为指定成员添加的外部联系人标记或移除企业客户标签；要标记与要移除的标签列表不可同时为空，
    /// 且需确保 external_userid 是 userid 的外部联系人。
    /// 每个成员对同一个客户最多可添加 3000 个由企业统一配置的标签。</para>
    /// <para>应用只能编辑可见范围内的成员添加的企业客户标签；若要使用某个客户联系规则组下的企业客户标签，
    /// 则规则组必须由同一个应用创建并且相关成员在规则组的管理范围内。</para>
    /// </summary>
    /// <param name="request">打标签请求体（<see cref="MarkTagRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>编辑结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92697"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92697"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96322"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/mark_tag")]
    Task<WechatWorkResponse> MarkTagAsync(
        [Body] MarkTagRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取指定规则组下的企业客户标签
    /// <para>标签 id 与标签组 id 均为空时返回所有标签；同时传递时官方忽略标签 id，仅以标签组 id 作为过滤条件。
    /// 应用仅能获取由本应用创建的规则组标签；自 2023 年 12 月 1 日起不再支持通过系统应用 secret 调用。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetStrategyTagListRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>标签组列表（tag_group，每组携带所属规则组 id strategy_id）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99542"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99542"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99544"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/get_strategy_tag_list")]
    Task<GetStrategyTagListResponse> GetStrategyTagListAsync(
        [Body] GetStrategyTagListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 为指定规则组创建企业客户标签
    /// <para>仅可在一级规则组下添加标签；每个企业标签 + 规则组标签合计上限 10000 个；
    /// 填写标签组 id 表示在指定标签组下添加标签（此时标签组名称与次序值被忽略），
    /// 未填写则按标签组名称新建标签组（不支持创建空标签组）。
    /// 应用仅能管理由本应用创建的规则组标签。</para>
    /// </summary>
    /// <param name="request">创建请求体（<see cref="AddStrategyTagRequest"/>：strategy_id 与 tag 列表官方必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>新建 / 复用的标签组（tag_group，含新建标签信息）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99542"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99542"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99544"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/add_strategy_tag")]
    Task<AddStrategyTagResponse> AddStrategyTagAsync(
        [Body] AddStrategyTagRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 编辑指定规则组下的企业客户标签
    /// <para>仅可修改标签 / 标签组的名称与次序值，不可重新指定标签 / 标签组所属规则组；
    /// 修改后的标签组不能和已有的标签组重名，标签也不能和同一标签组下的其他标签重名。</para>
    /// </summary>
    /// <param name="request">编辑请求体（<see cref="EditStrategyTagRequest"/>：id 官方必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>编辑结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99542"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99542"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99544"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/edit_strategy_tag")]
    Task<WechatWorkResponse> EditStrategyTagAsync(
        [Body] EditStrategyTagRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除指定规则组下的企业客户标签
    /// <para>标签 id 与标签组 id 不可同时为空；标签组内的所有标签被删除后，标签组自动删除。
    /// 应用仅能删除由本应用创建的规则组标签。</para>
    /// </summary>
    /// <param name="request">删除请求体（<see cref="DelStrategyTagRequest"/>：tag_id 与 group_id 均可空但不可同时为空）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>删除结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99542"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99542"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99544"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/del_strategy_tag")]
    Task<WechatWorkResponse> DelStrategyTagAsync(
        [Body] DelStrategyTagRequest request,
        CancellationToken cancellationToken = default);
}
