// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.ExternalContact.GroupChat;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「客户联系」模块客户群管理域公共 SDK
/// （获取客户群列表 / 获取客户群详情 / 客户群 opengid 转换）。
/// <para>
/// 官方对三类应用开放完全一致的 3 个端点，全部收敛声明于本接口；应用类型子接口均为零差异端点空标记：
/// 自建应用见 <see cref="IWechatWorkInternalExternalContactGroupChatService"/>，
/// 第三方应用见 <see cref="IWechatWorkThirdPartyExternalContactGroupChatService"/>，
/// 服务商代开发见 <see cref="IWechatWorkProviderExternalContactGroupChatService"/>。
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
/// 第三方 / 代开发应用须具有「企业客户权限-&gt;客户基础信息」权限；
/// 群主必须在应用的可见范围内。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkExternalContactGroupChatService
{
    /// <summary>
    /// 获取客户群列表
    /// <para>获取配置过客户群管理的客户群列表，支持按客户群跟进状态与群主过滤。</para>
    /// <para>不指定群主过滤将拉取应用可见范围内全部群主数据（可见范围人数超过 1000 人会报错 81017）；
    /// 群主为离职成员时必须指定群主过滤才可拉取对应数据；
    /// 旧版 offset + limit 分页将废弃，应使用 cursor + limit。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetGroupChatListRequest"/>：status_filter / owner_filter / cursor / limit）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>客户群列表（group_chat_list[]）与下一页游标（next_cursor）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93414"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92120"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93414"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/groupchat/list")]
    Task<GetGroupChatListResponse> GetGroupChatListAsync(
        [Body] GetGroupChatListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取客户群详情
    /// <para>通过客户群 ID 获取群详情，包括群名、群成员列表、群成员入群时间、入群方式。</para>
    /// <para>如发生群信息变动会立即收到群变更事件，但部分信息为异步处理，
    /// 可能需要等一段时间调此接口才能得到最新结果。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetGroupChatDetailRequest"/>：chat_id / need_name）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>客户群详情（group_chat）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92707"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92122"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92707"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/groupchat/get")]
    Task<GetGroupChatDetailResponse> GetGroupChatDetailAsync(
        [Body] GetGroupChatDetailRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 客户群 opengid 转换
    /// <para>用户在微信里的客户群里打开小程序时，某些场景下可以获取到群的 opengid；
    /// 若该群是企业微信客户群，可调用此接口将 opengid 转换为客户群 chat_id。</para>
    /// <para>仅支持企业服务人员创建的客户群，且仅可转换出自己企业下的客户群 chat_id。</para>
    /// </summary>
    /// <param name="request">转换请求体（<see cref="OpenGidToChatIdRequest"/>：opengid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>客户群 ID（chat_id，可用于调用「获取客户群详情」接口）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94828"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94822"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94828"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/opengid_to_chatid")]
    Task<OpenGidToChatIdResponse> OpenGidToChatIdAsync(
        [Body] OpenGidToChatIdRequest request,
        CancellationToken cancellationToken = default);
}
