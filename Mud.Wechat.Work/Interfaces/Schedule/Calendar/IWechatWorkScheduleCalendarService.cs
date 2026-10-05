// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Schedule;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「日程」模块管理日历域公共 SDK（创建日历 + 更新日历 + 获取日历详情 + 删除日历）。
/// <para>
/// 官方对三类应用开放一致的 4 个端点，全部收敛声明于本接口；应用类型子接口均为零差异端点空标记：
/// 自建应用见 <see cref="IWechatWorkInternalScheduleCalendarService"/>，
/// 服务商代开发见 <see cref="IWechatWorkProviderScheduleCalendarService"/>，
/// 第三方应用见 <see cref="IWechatWorkThirdPartyScheduleCalendarService"/>。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 令牌路由键说明：三类应用消费的令牌路由键均为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>）——
/// 自建应用消费应用自身 access_token；第三方/服务商代开发消费授权企业级 access_token（scope = authCorpId），
/// 须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。
/// </para>
/// <para>
/// 官方约束：日历管理员须在通知范围成员中，最多 3 人；日历通知范围成员最多 2000 人；
/// 公开范围成员最多 1000 个、部门最多 100 个；每人最多可创建或订阅 100 个公共日历；每个企业最多可创建 20 个全员日历。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkScheduleCalendarService
{
    /// <summary>
    /// 创建日历
    /// <para>在日历中创建一个日历对象（返回日历 id <c>cal_id</c>，后续管理日历/管理日程接口均以该 id 定位日历）。</para>
    /// <para>官方限制：每个人最多可创建或订阅 100 个公共日历；每个企业最多可创建 20 个全员日历；
    /// 全员日历也是公共日历的一种，需指定 public_range，且不支持指定颜色、默认日历、只读权限；
    /// <c>is_public</c> 与 <c>is_corp_calendar</c> 属性不可更新；<c>set_as_default</c> 第三方应用不支持使用。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="AddScheduleCalendarRequest"/>：calendar 日历信息（admins / set_as_default / summary / color / description / is_public / public_range / is_corp_calendar / shares） / agentid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>日历 ID 与无效的输入内容（cal_id / fail_result.shares：errcode / errmsg / userid）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93647"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93702"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96823"/></para>
    /// <para>官方契约陷阱：创建日历路由为 <c>/cgi-bin/oa/calendar/add</c> 而非 create；创建成功时响应仍可能携带 fail_result.shares（无效的通知范围成员列表，逐成员返回 errcode/errmsg/userid），调用方须同时检查 fail_result。</para>
    /// </remarks>
    [Post("/cgi-bin/oa/calendar/add")]
    Task<AddScheduleCalendarResponse> AddCalendarAsync(
        [Body] AddScheduleCalendarRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新日历
    /// <para>修改指定日历的信息。</para>
    /// <para>官方限制：<b>更新操作是覆盖式，而不是增量式</b>；日历管理员最多指定 3 人；通知范围成员最多 2000 人；
    /// 公开范围成员最多 1000 个、部门最多 100 个；<c>is_public</c> 与 <c>is_corp_calendar</c> 属性不可更新；
    /// 可通过 <c>skip_public_range</c> 指定是否不更新可订阅范围（默认为 0，会更新可订阅范围）。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="UpdateScheduleCalendarRequest"/>：skip_public_range / calendar 日历信息（cal_id / admins / summary / color / description / public_range / shares））。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>无效的输入内容（fail_result.shares：errcode / errmsg / userid；无业务负载时仅 errcode / errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97716"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97783"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97758"/></para>
    /// </remarks>
    [Post("/cgi-bin/oa/calendar/update")]
    Task<UpdateScheduleCalendarResponse> UpdateCalendarAsync(
        [Body] UpdateScheduleCalendarRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取日历详情
    /// <para>获取应用在企业内创建的日历信息。</para>
    /// <para>官方限制：<c>cal_id_list</c> 一次最多可获取 1000 条。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetScheduleCalendarRequest"/>：cal_id_list 日历 ID 列表）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>日历列表（calendar_list：cal_id / admins / summary / color / description / shares / is_public / public_range / is_corp_calendar）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97717"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97784"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97759"/></para>
    /// <para>官方契约陷阱：响应的日历管理员字段在官方参数表作 <c>admins</c>，而三类应用文档页的返回示例均作 <c>adminis</c>，本模型以示例为准承载 <c>adminis</c>（照抄勿「顺手修正」）。</para>
    /// </remarks>
    [Post("/cgi-bin/oa/calendar/get")]
    Task<GetScheduleCalendarResponse> GetCalendarAsync(
        [Body] GetScheduleCalendarRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除日历
    /// <para>删除指定日历。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="DelScheduleCalendarRequest"/>：cal_id 日历 ID）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97718"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97785"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97760"/></para>
    /// </remarks>
    [Post("/cgi-bin/oa/calendar/del")]
    Task<WechatWorkResponse> DelCalendarAsync(
        [Body] DelScheduleCalendarRequest request,
        CancellationToken cancellationToken = default);
}
