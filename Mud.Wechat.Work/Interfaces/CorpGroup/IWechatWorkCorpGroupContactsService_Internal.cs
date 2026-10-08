// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.CorpGroup.ChainContacts;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「上下游通讯录管理」域企业自建应用 SDK：除继承自 <see cref="IWechatWorkCorpGroupContactsService"/>
/// 的公共读取端点外，本接口提供自建应用的导入 / 移除 / 查询端点（批量导入上下游联系人、获取导入任务结果、
/// 移除企业、查询成员自定义 id、获取下级企业加入的上下游）。
/// <para>服务商代开发见 <see cref="IWechatWorkProviderCorpGroupContactsService"/>；第三方应用官方无对应文档，不设子接口。</para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>，即上游企业应用凭证）。
/// 官方权限口径：导入 / 移除仅已验证的企业可调用，自建应用须配置到「上下游-可调用接口的应用」中；
/// 自 2023-12-01 起不再支持通过系统应用 secret 调用接口。导入任务只允许串行调用且同时只能存在一个。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "CorpGroup",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkCorpGroupContactsService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.InternalAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalCorpGroupContactsService : IWechatWorkCorpGroupContactsService
{
    /// <summary>
    /// 批量导入上下游联系人
    /// <para>提交批量导入上下游联系人任务：经微信服务通知邀请下级企业加入上下游，支持「仅导入的企业可加入」选项。</para>
    /// <para>导入限制：单次 ≤ 1000 个企业、单个企业 ≤ 200 人、单次 ≤ 2000 人、每天 ≤ 20000 人；
    /// 只允许串行调用且同时只能存在一个导入任务（含管理后台提交的任务）。仅已验证的企业可调用。</para>
    /// </summary>
    /// <param name="request">导入请求体（<see cref="ImportChainContactsRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>导入任务受理结果（jobid；经获取导入任务结果接口查询进度）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95821"/></para>
    /// </remarks>
    [Post("/cgi-bin/corpgroup/import_chain_contact")]
    Task<ImportChainContactsResponse> ImportChainContactsAsync(
        [Body] ImportChainContactsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取导入任务结果
    /// <para>查询批量导入上下游联系人任务的执行状态与逐企业 / 逐联系人失败明细；只能查询已提交过的历史任务。</para>
    /// </summary>
    /// <param name="jobId">异步任务 id（提交导入任务时返回，最大长度 64 字节）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>任务状态（status：1 开始 / 2 进行中 / 3 完成）与导入结果（import_status + fail_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95823"/></para>
    /// </remarks>
    [Get("/cgi-bin/corpgroup/getresult")]
    Task<GetChainImportResultResponse> GetChainImportResultAsync(
        [Query("jobid")] string jobId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 移除企业
    /// <para>上级/上游企业通过该接口移除下游企业；仅已验证的企业可调用，并发限制 1。</para>
    /// </summary>
    /// <param name="request">移除企业请求体（<see cref="RemoveChainCorpRequest"/>；corpid 与 pending_corpid 至少填一个，都填时 corpid 生效）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>移除结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95822"/></para>
    /// </remarks>
    [Post("/cgi-bin/corpgroup/corp/remove_corp")]
    Task<WechatWorkResponse> RemoveChainCorpAsync(
        [Body] RemoveChainCorpRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询成员自定义 id
    /// <para>查询上下游通讯录中指定企业成员的自定义 id（批量导入时填写的 user_custom_id）。</para>
    /// </summary>
    /// <param name="request">成员自定义 id 查询请求体（<see cref="GetChainUserCustomIdRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成员自定义 id（user_custom_id）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97441"/></para>
    /// </remarks>
    [Post("/cgi-bin/corpgroup/corp/get_chain_user_custom_id")]
    Task<GetChainUserCustomIdResponse> GetChainUserCustomIdAsync(
        [Body] GetChainUserCustomIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取下级企业加入的上下游
    /// <para>查询指定下级企业所在的上下游列表；仅可指定应用可见范围内的企业。</para>
    /// </summary>
    /// <param name="request">下级企业所在上下游查询请求体（<see cref="GetCorpSharedChainListRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>该企业所在的上下游列表（chain_id + chain_name）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97442"/></para>
    /// </remarks>
    [Post("/cgi-bin/corpgroup/get_corp_shared_chain_list")]
    Task<GetCorpSharedChainListResponse> GetCorpSharedChainListAsync(
        [Body] GetCorpSharedChainListRequest request,
        CancellationToken cancellationToken = default);
}
