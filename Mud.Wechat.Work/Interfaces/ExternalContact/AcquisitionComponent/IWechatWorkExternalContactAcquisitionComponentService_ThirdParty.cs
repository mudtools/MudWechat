// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.ExternalContact.AcquisitionComponent;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「客户联系」模块「获客助手组件」域第三方应用 SDK：
/// 官方仅向第三方应用开放（企业自建应用与服务商代开发均无对应功能），全部 6 个端点声明于本接口。
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>，企业级令牌一企一份，
/// 宿主须以授权企业 scope 获取）。官方权限口径：获客助手组件可调用，组件版仅可获取 / 查询
/// <b>授权给获客助手组件</b>的链接。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "ExternalContact",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkExternalContactAcquisitionComponentService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.CorpAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkThirdPartyExternalContactAcquisitionComponentService : IWechatWorkExternalContactAcquisitionComponentService
{
    /// <summary>
    /// 获取组件授权信息
    /// <para>服务商可获取企业获客助手组件的授权信息：付费模式（扣企业 / 扣服务商）、
    /// 代付单价（pay_mode=1 时返回）、链接授权模式与授权的应用列表。</para>
    /// <para>官方契约本端点为 POST 且无请求体参数。</para>
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>组件授权信息（pay_mode / price / link_auth_mode / auth_apps）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99610"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/customer_acquisition/get_comp_auth_info")]
    Task<GetAcquisitionComponentAuthInfoResponse> GetAcquisitionComponentAuthInfoAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获客链接管理 - 获取获客链接列表
    /// <para>分页获取授权给获客助手组件的获客链接 id 列表；limit 最大不超过 100，默认值 100。</para>
    /// <para>组件版仅可获取<b>授权给获客助手组件</b>的链接（不含企业其它链接）。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetAcquisitionComponentLinkListRequest"/>：limit / cursor）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>获客链接 id 列表（link_id_list）与分页游标（next_cursor）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99484"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/customer_acquisition/list_link")]
    Task<GetAcquisitionComponentLinkListResponse> GetAcquisitionComponentLinkListAsync(
        [Body] GetAcquisitionComponentLinkListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获客链接管理 - 获取获客链接详情
    /// <para>按 link_id 查询授权给获客助手组件的获客链接；组件版响应仅含链接名称（link_name）与
    /// 访问地址（url），不含自建版的配置字段。</para>
    /// <para>组件版链接 url 形如 <c>https://work.weixin.qq.com/ca/xxxxxx?comp_scene=xxxxxx</c>，
    /// 需携带 <c>comp_scene</c> 参数方计入组件数据。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetAcquisitionComponentLinkDetailRequest"/>：link_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>获客链接信息（link：link_name / url）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99484"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/customer_acquisition/get")]
    Task<GetAcquisitionComponentLinkDetailResponse> GetAcquisitionComponentLinkDetailAsync(
        [Body] GetAcquisitionComponentLinkDetailRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询链接使用详情
    /// <para>按天统计获客链接的点击客户数与新增客户数；统计最小粒度为天（开始日期与结束日期闭区间），
    /// 查询日期跨度不超过 30 天，仅支持 180 天内的数据查询。</para>
    /// <para>组件口径：获客链接需携带 <c>comp_scene</c> 参数方可计入组件数据。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetAcquisitionComponentLinkStatisticRequest"/>：link_id / start_time / end_time）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>统计数据（click_link_customer_cnt / new_customer_cnt）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99483"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/customer_acquisition/statistic")]
    Task<GetAcquisitionComponentLinkStatisticResponse> GetAcquisitionComponentLinkStatisticAsync(
        [Body] GetAcquisitionComponentLinkStatisticRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 生成代支付 key
    /// <para>为获客链接生成一次性代支付 key（14 天内有效）；使用方式为将 <c>once_key</c> 参数拼接到
    /// <c>comp_scene</c> 链接尾部。key_num 默认 100，最大不超过 1000。</para>
    /// <para>官方口径：需企业授权为「服务商代支付模式」（pay_mode=1）方可调用。</para>
    /// </summary>
    /// <param name="request">生成请求体（<see cref="CreateAcquisitionComponentOnceKeyRequest"/>：link_id / key_num）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>代支付 key 列表（keys）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99603"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/customer_acquisition/create_once_key")]
    Task<CreateAcquisitionComponentOnceKeyResponse> CreateAcquisitionComponentOnceKeyAsync(
        [Body] CreateAcquisitionComponentOnceKeyRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取获客链接使用成员接受消息数据
    /// <para>获取成员收消息情况（消息次数、客户来源链接等）；chat_key 来自成员多次收消息事件的回调内容，
    /// 回调后 30 分钟内有效。</para>
    /// <para>组件版响应<b>不返回</b>顶层 userid / external_userid（与自建 / 第三方直连版收消息详情的差异点），
    /// 仅返回聚合会话信息 chat_info。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetAcquisitionComponentChatInfoRequest"/>：chat_key）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>会话详情（chat_info：recv_msg_cnt / link_id / state）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100135"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/customer_acquisition/get_chat_info")]
    Task<GetAcquisitionComponentChatInfoResponse> GetAcquisitionComponentChatInfoAsync(
        [Body] GetAcquisitionComponentChatInfoRequest request,
        CancellationToken cancellationToken = default);
}
