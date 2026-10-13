// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.MeetingRoom;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「会议室管理」模块企业自建应用 SDK（添加会议室 / 查询会议室列表 / 编辑会议室 / 删除会议室）。
/// <para>
/// 官方「会议室」文档树只在企业自建应用开发根下存在页面，本域 4 个端点全部声明于本接口
/// （形态对齐 <see cref="IWechatWorkInternalDialService"/> 与 <see cref="IWechatWorkInternalGovPatrolService"/>）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>）。
/// </para>
/// <para>
/// 官方权限说明（93619「权限说明」一节原文）：自建应用须配置到「应用管理 - 会议室 - 可调用接口的应用」中，
/// 代开发应用与第三方应用均为「暂不支持」——故本族只有唯一的自建子接口。
/// 官方注：自 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口（存量企业暂不受影响）。
/// </para>
/// <para>
/// 官方频率：本族页面均无独立的数值型频率限制，走官方「附录 - 访问频率限制」的默认阈值
/// （每企业调用单个 cgi/api 不可超过 1 万次/分、15 万次/小时；每 IP 不可超过 2 万次/分、60 万次/小时），
/// <see href="https://developer.work.weixin.qq.com/document/path/90312"/>。
/// </para>
/// <para>
/// 官方附录：会议室支持的设备 ID 为 1-电视 / 2-电话 / 3-投影 / 4-白板 / 5-视频。
/// </para>
/// <para>
/// 与「视频会议」域的硬件会议室（<c>/cgi-bin/meeting/rooms/*</c>）分属官方两棵章节树，勿混淆。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "MeetingRoom",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkMeetingRoomService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.InternalAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalMeetingRoomService : IWechatWorkMeetingRoomService
{
    /// <summary>
    /// 添加会议室
    /// <para>创建一个实体会议室（名称、容纳人数、位置、使用范围与支持的设备）。</para>
    /// <para>官方限制：name 最多 30 个字符，capacity 为可容纳人数；range.user_list 最多指定 1000 个成员、
    /// range.department_list 最多指定 1000 个部门，<b>不填 range 则默认为全公司可用</b>；
    /// 若要设置位置信息，必须同时填写 city、building 与 floor 三个参数。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="MeetingRoomAddRequest"/>：name / capacity 为必填项，city / building / floor / coordinate / range / equipment 可缺省）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>新建会议室的 ID（官方 meetingroom_id，后续编辑、删除与预定均以该值定位）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93619"/></para>
    /// <para>官方权限：自建应用须配置到「应用管理 - 会议室 - 可调用接口的应用」中；代开发与第三方应用官方标注为「暂不支持」。</para>
    /// <para>官方业务限制：第三方应用与服务商代开发应用文档树无本端点页面（均不开放）；
    /// 官方以 range 的 user_list / department_list 表达会议室使用范围（成员 UserId 与部门 ID），
    /// coordinate 为所在建筑经纬度（官方建议经腾讯地图坐标拾取器获取）、equipment 为支持的设备 ID 列表。</para>
    /// </remarks>
    [Post("/cgi-bin/oa/meetingroom/add")]
    Task<MeetingRoomAddResponse> AddMeetingRoomAsync(
        [Body] MeetingRoomAddRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询会议室列表
    /// <para>按城市、楼宇、楼层与设备 ID 过滤查询会议室列表。</para>
    /// <para>官方限制：使用位置信息过滤时须保证其上一级已填写（如用楼宇过滤必须同时填写城市）；
    /// 官方无分页参数，请求体即四个过滤项。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="MeetingRoomListRequest"/>：city / building / floor / equipment 均为可选过滤项）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>会议室列表（官方 meetingroom_list，元素结构见 <see cref="MeetingRoomInfo"/>，比添加侧多回传 need_approval）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93619"/></para>
    /// <para>官方权限：自建应用须配置到「应用管理 - 会议室 - 可调用接口的应用」中；代开发与第三方应用官方标注为「暂不支持」。</para>
    /// <para>官方业务限制：第三方应用与服务商代开发应用文档树无本端点页面（均不开放）；
    /// city / building / floor / equipment 四个过滤项均可缺省；
    /// 列表元素比添加侧多回传 need_approval（0-无需审批 / 1-需要审批）；
    /// 官方注明 range 的 user_list / department_list <b>仅会议室系统应用查询时返回</b>。</para>
    /// </remarks>
    [Post("/cgi-bin/oa/meetingroom/list")]
    Task<MeetingRoomListResponse> GetMeetingRoomListAsync(
        [Body] MeetingRoomListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 编辑会议室
    /// <para>按会议室 ID 修改其名称、容纳人数、位置、使用范围与支持的设备。</para>
    /// <para>官方限制：<b>会议室使用范围（range）为覆盖操作而非增量添加</b>，
    /// user_list / department_list 各最多 1000 项且填写后即覆盖整个使用范围；
    /// 修改位置信息须同时输入城市、楼宇与楼层三个参数，<b>已生成的建筑暂不支持修改经纬度</b>；
    /// 除 meetingroom_id 外其余参数官方均标为可选（不传即不改）。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="MeetingRoomEditRequest"/>：meetingroom_id 必填，其余字段为可改项）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>官方成功响应仅有 errcode / errmsg，无业务字段。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93619"/></para>
    /// <para>官方权限：自建应用须配置到「应用管理 - 会议室 - 可调用接口的应用」中；代开发与第三方应用官方标注为「暂不支持」。</para>
    /// <para>官方业务限制：第三方应用与服务商代开发应用文档树无本端点页面（均不开放）；
    /// 官方响应体不含业务字段，故返回 <see cref="WechatWorkResponse"/>，不另建响应 DTO。</para>
    /// </remarks>
    [Post("/cgi-bin/oa/meetingroom/edit")]
    Task<WechatWorkResponse> EditMeetingRoomAsync(
        [Body] MeetingRoomEditRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除会议室
    /// <para>按会议室 ID 删除会议室。</para>
    /// <para>官方限制：请求侧仅 meetingroom_id 一个业务参数（官方标为必填），无其他约束。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="MeetingRoomDeleteRequest"/>：仅 meetingroom_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>官方成功响应仅有 errcode / errmsg，无业务字段。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93619"/></para>
    /// <para>官方权限：自建应用须配置到「应用管理 - 会议室 - 可调用接口的应用」中；代开发与第三方应用官方标注为「暂不支持」。</para>
    /// <para>官方业务限制：第三方应用与服务商代开发应用文档树无本端点页面（均不开放）；
    /// 官方响应体不含业务字段，故返回 <see cref="WechatWorkResponse"/>，不另建响应 DTO。</para>
    /// </remarks>
    [Post("/cgi-bin/oa/meetingroom/del")]
    Task<WechatWorkResponse> DeleteMeetingRoomAsync(
        [Body] MeetingRoomDeleteRequest request,
        CancellationToken cancellationToken = default);
}
