// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Meeting;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「会议」模块电话入会（PSTN）管理域企业自建应用 SDK（承载本域全部 3 端点）。
/// <para>
/// 官方仅向企业自建应用开放本域端点（第三方应用开发与服务商代开发章节均无对应 API），
/// 端点全部声明于本接口；继承链上不得出现代开发 / 第三方子接口（能力漂移守卫），
/// 父接口见 <see cref="IWechatWorkMeetingPstnService"/>。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>，Query 注入 <c>access_token</c>）。
/// 官方权限口径：自建应用需配置在「可调用接口的应用」列表中；仅允许操作/获取该应用创建的会议的数据。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Meeting",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkMeetingPstnService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.InternalAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalMeetingPstnService : IWechatWorkMeetingPstnService
{
    /// <summary>
    /// 批量外呼
    /// <para>创建批量电话入会呼叫（电话入会支持成员通过手机或座机拨打对应号码进入会议，也可在会中输入电话号码呼叫其他成员加入）。</para>
    /// <para>官方限制：支持在会议未开始、会中外呼；每次调用支持批量外呼 50 路；支持境外电话号及分机号的外呼；Webinar 暂不支持外呼。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="PstnBatchCalloutRequest"/>：meetingid / phone_numbers）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成功外呼的电话号码对象数组（phone_numbers：area / phone / extension_number / status）、
    /// 不合法的外呼电话号码对象数组（invalid_phone_numbers）；拨打后立刻返回呼叫状态，可通过查询接口和回调事件获取。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98823"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许操作该应用创建的会议。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/phone/callout")]
    Task<PstnBatchCalloutResponse> BatchCalloutAsync(
        [Body] PstnBatchCalloutRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取会议的外呼状态
    /// <para>获取外呼电话入会的数据。</para>
    /// <para>官方限制：无论从客户端、API 还是 Web 端发起的外呼均可获取，不针对呼叫人做数据隔离；
    /// 需要分页，分页最大条数 100；Webinar 暂不支持外呼。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="PstnGetCalloutStatusRequest"/>：meetingid / cursor / limit）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>电话号码对象数组（phone_numbers：area / phone / extension_number / status / tmp_openid）、
    /// 分页游标（next_cursor）与是否还有待拉取的列表（has_more）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98824"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许获取该应用创建的会议的数据。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/phone/get_callout_status")]
    Task<PstnGetCalloutStatusResponse> GetCalloutStatusAsync(
        [Body] PstnGetCalloutStatusRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取电话入会的成员 ID
    /// <para>获取电话入会的成员 ID，以 tmp_openid 返回（PSTN 的识别标识，成员拨号进入后在实时参会列表等 API 及回调事件中可以获取）。</para>
    /// <para>官方限制：仅允许获取该应用创建的会议的数据；phone_numbers 上限 20 个；
    /// 由于可能出现一个座机号同时拨入的情况，同一个座机号可能对应多个 tmp_openid；仅当 PSTN 接通后返回；
    /// 支持查询会议下 PSTN 的历史 tmp_openid 列表。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="PstnGetTmpOpenidRequest"/>：meetingid / phone_numbers）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>返回的 tmp_openid 对象数组（tmp_openid_list：area / phone / extension_number / tmp_openid）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98825"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许获取该应用创建的会议的数据。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/phone/get_tmp_openid")]
    Task<PstnGetTmpOpenidResponse> GetTmpOpenidAsync(
        [Body] PstnGetTmpOpenidRequest request,
        CancellationToken cancellationToken = default);
}
