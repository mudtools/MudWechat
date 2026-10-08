// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.CorpGroup.ChainContacts;

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「上下游通讯录管理」域公共 SDK（获取上下游列表、通讯录分组、分组下企业列表、企业信息）。
/// <para>
/// 官方对企业自建应用与服务商代开发开放了完全一致的 4 个读取端点（第三方应用无对应文档，不设子接口），
/// 因此全部公共端点声明于本公共父接口；自建与代开发应用类型子接口
/// （<see cref="IWechatWorkInternalCorpGroupContactsService"/> / <see cref="IWechatWorkProviderCorpGroupContactsService"/>）
/// 分别承载应用类型差异端点 / 作为类型化契约入口。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 落位与形态对齐 <see cref="IWechatWorkCorpGroupService"/>：令牌路由键为
/// <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>，即上游企业应用凭证），
/// 由多应用基座按当前应用上下文（AppKey + scope）路由——代开发调用前须经
/// <c>IWechatAppContextSwitcher</c> 切换到目标授权企业作用域。
/// </para>
/// <para>
/// 权限口径：自建/代开发应用仅返回（或可指定）应用可见范围内的上下游 / 分组 / 企业；
/// 「上下游-可调用接口的应用」调用时返回全部。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkCorpGroupContactsService
{
    /// <summary>
    /// 获取上下游列表
    /// <para>获取企业所在的上下游列表；仅返回应用可见范围内的上下游列表（「上下游-可调用接口的应用」返回全部）。</para>
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>企业上下游列表（chain_id + chain_name）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95820"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96876"/></para>
    /// </remarks>
    [Get("/cgi-bin/corpgroup/corp/get_chain_list")]
    Task<GetChainListResponse> GetChainListAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取上下游通讯录分组
    /// <para>获取企业上下游通讯录分组详情；不填 groupid 返回全部分组，填写返回指定分组。</para>
    /// </summary>
    /// <param name="request">分组查询请求体（<see cref="GetChainGroupRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>分组列表数据（group_name / parentid / order）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95820"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96876"/></para>
    /// </remarks>
    [Post("/cgi-bin/corpgroup/corp/get_chain_group")]
    Task<GetChainGroupResponse> GetChainGroupAsync(
        [Body] GetChainGroupRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取企业上下游通讯录分组下的企业详情列表
    /// <para>获取上下游通讯录某个分组下的企业列表（含未加入企业，需 need_pending 开启）。</para>
    /// <para>如需获取某分组及其子分组的所有企业详情，需先获取该分组的所有子分组，再逐层递归获取子分组下的企业。</para>
    /// </summary>
    /// <param name="request">分组企业列表请求体（<see cref="GetChainCorpInfoListRequest"/>；limit &gt; 0 开启分页）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>企业详情列表（has_more 分页标志 + next_cursor 游标 + group_corps）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95820"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96876"/></para>
    /// </remarks>
    [Post("/cgi-bin/corpgroup/corp/get_chain_corpinfo_list")]
    Task<GetChainCorpInfoListResponse> GetChainCorpInfoListAsync(
        [Body] GetChainCorpInfoListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取企业上下游通讯录下的企业信息
    /// <para>获取上下游通讯录中某个企业的自定义 id 和所属分组的分组 id。</para>
    /// </summary>
    /// <param name="request">企业信息请求体（<see cref="GetChainCorpInfoRequest"/>；corpid 与 pending_corpid 至少填一个，同时填时 corpid 生效）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>企业信息（corp_name / qualification_status / custom_id / groupid / is_joined）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95820"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96876"/></para>
    /// </remarks>
    [Post("/cgi-bin/corpgroup/corp/get_chain_corpinfo")]
    Task<GetChainCorpInfoResponse> GetChainCorpInfoAsync(
        [Body] GetChainCorpInfoRequest request,
        CancellationToken cancellationToken = default);
}
