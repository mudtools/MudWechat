// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.ExternalContact.ContactWay;

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「客户联系」模块「联系我」与客户入群方式域公共 SDK
/// （「联系我」方式管理 + 临时会话 + 客户群「加入群聊」方式管理）。
/// <para>
/// 官方对三类应用开放完全一致的 10 个端点，全部收敛声明于本接口；应用类型子接口均为零差异端点空标记：
/// 自建应用见 <see cref="IWechatWorkInternalExternalContactContactWayService"/>，
/// 第三方应用见 <see cref="IWechatWorkThirdPartyExternalContactContactWayService"/>，
/// 服务商代开发见 <see cref="IWechatWorkProviderExternalContactContactWayService"/>。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 三类应用消费的令牌路由键均为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>）：
/// 企业自建为应用自身 access_token；第三方 / 代开发为授权企业级 access_token（scope = authCorpId），
/// 由多应用基座按当前应用上下文路由——企业级令牌须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。
/// </para>
/// <para>
/// 权限口径：「联系我」端点要求自建应用配置到「客户联系 可调用接口的应用」中，
/// 第三方 / 代开发应用须具有「配置『联系我』二维码」权限（营销获客应用需将客户建联方式配置为「联系我」二维码）；
/// 「加入群聊」端点要求第三方 / 代开发应用具有「企业客户权限-&gt;客户群-配置『加入群聊』二维码」权限
/// 且已购买「加入群聊」高级接口。传入的成员与部门须在应用可见范围内，
/// 应用仅能获取和管理由本应用创建的「联系我」方式与「加入群聊」二维码 / 小程序组件。
/// </para>
/// <para>
/// 官方约束：临时会话二维码仅医疗行业企业可创建（仅支持单人、每日最多 10 万个）；
/// 每个企业 API 配置「联系我」上限 50 万个（临时会话不计入），每个联系方式最多 100 个使用成员。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkExternalContactContactWayService
{
    /// <summary>
    /// 配置客户联系「联系我」方式
    /// <para>创建「联系我」方式配置（单人 / 多人，小程序中联系或二维码联系），返回配置 id 与二维码链接。</para>
    /// <para>每个联系方式最多配置 100 个使用成员；临时会话模式仅支持单人且仅医疗行业企业可创建；
    /// 每个企业 API 配置上限 50 万个（临时会话不计入数量）。</para>
    /// </summary>
    /// <param name="request">配置请求体（<see cref="AddContactWayRequest"/>：type / scene / user / party / conclusions 等）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>联系方式的配置 id（config_id）与联系我二维码链接（qr_code，仅二维码场景返回）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92228"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95724"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96348"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/add_contact_way")]
    Task<AddContactWayResponse> AddContactWayAsync(
        [Body] AddContactWayRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取企业已配置的「联系我」方式
    /// <para>通过配置 id 获取「联系我」方式的完整配置详情（含使用成员 / 部门、结束语等）。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetContactWayRequest"/>：config_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>「联系我」方式配置详情（contact_way）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92228"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95724"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96348"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/get_contact_way")]
    Task<GetContactWayResponse> GetContactWayAsync(
        [Body] GetContactWayRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取企业已配置的「联系我」列表
    /// <para>按创建时间范围分页获取企业配置的「联系我」id 列表，详情须逐条调用「获取『联系我』方式」。</para>
    /// <para>该接口不包含临时会话模式的「联系我」，且仅可查询 2021-07-10 之后创建的记录。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="ListContactWayRequest"/>：start_time / end_time / cursor / limit）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>「联系我」配置 id 列表（contact_way[]，元素仅含 config_id）与下一页游标（next_cursor）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92228"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95724"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96348"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/list_contact_way")]
    Task<ListContactWayResponse> GetContactWayListAsync(
        [Body] ListContactWayRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新企业已配置的「联系我」方式
    /// <para>更新指定配置 id 的「联系我」方式；传入字段将覆盖原配置。</para>
    /// <para>已失效的临时会话不允许编辑；user / party 须在应用可见范围内；
    /// mark_source 只能由创建该「联系我」的应用更新。</para>
    /// </summary>
    /// <param name="request">更新请求体（<see cref="UpdateContactWayRequest"/>：config_id + 覆盖字段）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>更新结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92228"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95724"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96348"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/update_contact_way")]
    Task<WechatWorkResponse> UpdateContactWayAsync(
        [Body] UpdateContactWayRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除企业已配置的「联系我」方式
    /// <para>删除指定配置 id 的「联系我」方式。</para>
    /// </summary>
    /// <param name="request">删除请求体（<see cref="DeleteContactWayRequest"/>：config_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>删除结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92228"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95724"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96348"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/del_contact_way")]
    Task<WechatWorkResponse> DeleteContactWayAsync(
        [Body] DeleteContactWayRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 结束临时会话
    /// <para>结束指定成员与指定客户之间的临时会话。</para>
    /// <para>成员与客户之间必须存在有效的临时会话才可结束；
    /// 通过其他方式添加的外部联系人无法通过此接口关闭会话。</para>
    /// </summary>
    /// <param name="request">结束请求体（<see cref="CloseTempChatRequest"/>：userid / external_userid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>结束结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92228"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95724"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96348"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/close_temp_chat")]
    Task<WechatWorkResponse> CloseTempChatAsync(
        [Body] CloseTempChatRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 配置客户群进群方式
    /// <para>为客户群配置「加入群聊」方式（群的小程序插件或群的二维码插件），返回配置 id。</para>
    /// <para>应用仅能获取和管理由本应用创建的「加入群聊」二维码 / 小程序组件；
    /// 第三方 / 代开发应用须已购买「加入群聊」高级接口。</para>
    /// </summary>
    /// <param name="request">配置请求体（<see cref="AddGroupChatJoinWayRequest"/>：scene / chat_id_list / auto_create_room 等）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>进群方式的配置 id（config_id）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92229"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99546"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99547"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/groupchat/add_join_way")]
    Task<AddGroupChatJoinWayResponse> AddGroupChatJoinWayAsync(
        [Body] AddGroupChatJoinWayRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取客户群进群方式配置
    /// <para>通过配置 id 获取客户群「加入群聊」方式的配置详情（含二维码 / 小程序插件链接）。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetGroupChatJoinWayRequest"/>：config_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>进群方式配置详情（join_way）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92229"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99546"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99547"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/groupchat/get_join_way")]
    Task<GetGroupChatJoinWayResponse> GetGroupChatJoinWayAsync(
        [Body] GetGroupChatJoinWayRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新客户群进群方式配置
    /// <para>更新指定配置 id 的客户群「加入群聊」方式；该接口采用覆盖方式更新，传入字段将整体覆盖原配置。</para>
    /// <para>mark_source 只能由创建此二维码的应用更新。</para>
    /// </summary>
    /// <param name="request">更新请求体（<see cref="UpdateGroupChatJoinWayRequest"/>：config_id / scene / chat_id_list 等）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>更新结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92229"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99546"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99547"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/groupchat/update_join_way")]
    Task<WechatWorkResponse> UpdateGroupChatJoinWayAsync(
        [Body] UpdateGroupChatJoinWayRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除客户群进群方式配置
    /// <para>删除指定配置 id 的客户群「加入群聊」方式。</para>
    /// </summary>
    /// <param name="request">删除请求体（<see cref="DeleteGroupChatJoinWayRequest"/>：config_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>删除结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92229"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99546"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99547"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/groupchat/del_join_way")]
    Task<WechatWorkResponse> DeleteGroupChatJoinWayAsync(
        [Body] DeleteGroupChatJoinWayRequest request,
        CancellationToken cancellationToken = default);
}
