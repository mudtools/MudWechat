// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Meeting;

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「会议」模块预约会议基础管理域公共 SDK（创建预约会议 + 修改预约会议 + 取消预约会议 + 获取成员会议 ID 列表）。
/// <para>
/// 官方对三类应用开放一致的 4 个端点，收敛声明于本接口；应用类型子接口承载官方开放面差异端点：
/// 企业自建应用见 <see cref="IWechatWorkInternalMeetingService"/> 与
/// 第三方应用见 <see cref="IWechatWorkThirdPartyMeetingService"/>（均额外开放「获取会议详情」），
/// 服务商代开发见 <see cref="IWechatWorkProviderMeetingService"/>（零差异端点空标记，
/// 官方代开发章节未开放「获取会议详情」）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 令牌路由键说明：三类应用消费的令牌路由键均为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>）——
/// 自建应用消费应用自身 access_token；第三方/服务商代开发消费授权企业级 access_token（scope = authCorpId），
/// 须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。
/// 官方权限口径：发起人和企业内部参与人必须在应用可见范围内；自建应用需配置在「可调用接口的应用」列表中；
/// 第三方应用必须指定 cal_id（会议所属日历须为 access_token 对应应用创建的日历）。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkMeetingService
{
    /// <summary>
    /// 创建预约会议
    /// <para>创建一个预约会议，返回会议 ID（可用于调用「进入会议」接口，通过小程序和 JS-SDK 提供入会入口）。</para>
    /// <para>官方限制：会议开始时间需大于当前时间；持续时长最小 300 秒、最大 86399 秒；会议标题最多 40 字节或 20 个 utf8 字符；
    /// 参会人数上限由管理员可预约人数决定——普通企业最多 100 人，付费企业由所购在线会议室/高级账号容量决定、最多 300 人，
    /// 超过 300 人需调用「更新会议受邀成员列表」接口；周期性会议每天/每个工作日/每周最多重复 200 次，每两周/每月最多 50 次；
    /// 仅购买了会议高级功能的企业可指定主持人（包含创建者 userid 会被自动过滤）；第三方应用必须指定 cal_id。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="CreateMeetingRequest"/>：admin_userid / title / meeting_start / meeting_duration / description / location / agentid / invitees / guests / cal_id / settings / reminders）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>会议 ID（meetingid）、包含无效会议账号的参会人 userid 列表（excess_users，仅购买会议专业版企业且部分参会人无有效会议账号时返回）、会议号（meeting_code）与入会链接（meeting_link，后两者仅预约会议高级管理文档页声明）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99104"/></para>
    /// <para><b>企业自建应用·预约会议高级管理</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98148"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93706"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97454"/></para>
    /// </remarks>
    [Post("/cgi-bin/meeting/create")]
    Task<CreateMeetingResponse> CreateMeetingAsync(
        [Body] CreateMeetingRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改预约会议
    /// <para>修改一个预约会议的信息。</para>
    /// <para>官方限制：仅允许修改当前应用创建的、处于预约状态下的会议；
    /// 修改开始时间时必须同时指定持续时长，修改持续时长时必须同时指定开始时间（两字段须成对修改）；
    /// 若会议为指定创建者的老会议，修改时不允许指定应用身份创建的新日历。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="UpdateMeetingRequest"/>：meetingid / title / meeting_start / meeting_duration / description / location / remind_time / agentid / invitees / cal_id / settings / reminders）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>包含无效会议账号的参会人 userid 列表（excess_users，仅购买会议专业版企业且部分参会人无有效会议账号时返回；无业务负载时仅 errcode / errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99047"/></para>
    /// <para><b>企业自建应用·预约会议高级管理</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98154"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93710"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97455"/></para>
    /// </remarks>
    [Post("/cgi-bin/meeting/update")]
    Task<UpdateMeetingResponse> UpdateMeetingAsync(
        [Body] UpdateMeetingRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 取消预约会议
    /// <para>取消一个指定的预约会议。</para>
    /// <para>官方限制：仅允许取消当前应用创建的会议；仅允许取消处于预约状态下的会议。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="CancelMeetingRequest"/>：meetingid / sub_meetingid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99048"/></para>
    /// <para><b>企业自建应用·预约会议高级管理</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98153"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93709"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97456"/></para>
    /// </remarks>
    [Post("/cgi-bin/meeting/cancel")]
    Task<WechatWorkResponse> CancelMeetingAsync(
        [Body] CancelMeetingRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取成员会议 ID 列表
    /// <para>获取指定成员指定时间内的会议 ID 列表。</para>
    /// <para>官方限制：只能拉取该应用创建的会议 ID；begin_time 与 end_time 时间跨度不超过 180 天
    /// （两者都没填时默认 end_time 为当前时间）；limit 默认值和最大值均为 100。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetUserMeetingIdListRequest"/>：userid / cursor / limit / begin_time / end_time）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>分页游标（next_cursor，未返回或为空字符串表示数据已取完）与会议 ID 列表（meetingid_list，可能为空）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99050"/></para>
    /// <para><b>企业自建应用·预约会议高级管理</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98714"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93707"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97457"/></para>
    /// </remarks>
    [Post("/cgi-bin/meeting/get_user_meetingid")]
    Task<GetUserMeetingIdListResponse> GetUserMeetingIdListAsync(
        [Body] GetUserMeetingIdListRequest request,
        CancellationToken cancellationToken = default);
}
