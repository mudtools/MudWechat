// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Meeting;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「会议」模块会议室连接器（MRA）管理域企业自建应用 SDK（承载本域全部 4 端点）。
/// <para>
/// 官方仅向企业自建应用开放本域端点（第三方应用开发与服务商代开发章节均无对应 API），
/// 端点全部声明于本接口；继承链上不得出现代开发 / 第三方子接口（能力漂移守卫），
/// 父接口见 <see cref="IWechatWorkMeetingMraService"/>。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>，Query 注入 <c>access_token</c>）。
/// 官方权限口径：自建应用需配置在「可调用接口的应用」列表中；仅允许获取/修改/操作该应用创建的会议的数据。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Meeting",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkMeetingMraService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.InternalAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalMeetingMraService : IWechatWorkMeetingMraService
{
    /// <summary>
    /// 获取 MRA 状态信息
    /// <para>对 API 创建的会议获取指定 MRA 设备的当前状态信息，包括名称、静音状态、视频状态、举手状态、默认布局等。</para>
    /// <para>官方限制：仅允许获取该应用创建的会议的数据。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="QueryMraStatusRequest"/>：meetingid / tmp_openid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>MRA 设备状态（tmp_openid / instance_id / user_role / webinar_member_role / ip / name / audio_state /
    /// video_state / screen_shared_state / default_layout / raise_hands_state）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98786"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许获取该应用创建的会议的数据。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/mra/query_status")]
    Task<QueryMraStatusResponse> QueryMraStatusAsync(
        [Body] QueryMraStatusRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 切换 MRA 默认布局
    /// <para>会议中对 MRA 的默认布局进行设置。</para>
    /// <para>官方限制：仅允许修改该应用创建的会议的数据；
    /// 如果当前 MRA 已显示会议自定义布局或个性布局或焦点视频，则不支持进行默认布局设置。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="SetMraDefaultLayoutRequest"/>：meetingid / default_layout / default_novideo_user / mra）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98787"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许修改该应用创建的会议的数据。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/mra/set_default_layout")]
    Task<WechatWorkResponse> SetMraDefaultLayoutAsync(
        [Body] SetMraDefaultLayoutRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置 MRA 举手或手放下
    /// <para>API 创建的会议中对 MRA 进行举手和手放下操作。</para>
    /// <para>官方限制：仅允许修改该应用创建的会议的数据。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="SetMraRaiseHandRequest"/>：meetingid / raise_hand / mra）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98788"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许修改该应用创建的会议的数据。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/mra/set_raise_hand")]
    Task<WechatWorkResponse> SetMraRaiseHandAsync(
        [Body] SetMraRaiseHandRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 挂断 MRA 呼叫
    /// <para>会议中对 MRA 的呼叫进行挂断操作。</para>
    /// <para>官方限制：仅允许操作该应用创建的会议。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="HangupMraRequest"/>：meetingid / mra）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98789"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许操作该应用创建的会议。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/mra/hangup")]
    Task<WechatWorkResponse> HangupMraAsync(
        [Body] HangupMraRequest request,
        CancellationToken cancellationToken = default);
}
