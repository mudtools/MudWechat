// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Meeting;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「会议」模块录制管理域企业自建应用 SDK（承载本域全部 10 端点）。
/// <para>
/// 官方仅向企业自建应用开放本域端点（第三方应用开发与服务商代开发章节均无对应 API），
/// 端点全部声明于本接口；继承链上不得出现代开发 / 第三方子接口（能力漂移守卫），
/// 父接口见 <see cref="IWechatWorkMeetingRecordingService"/>。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>，Query 注入 <c>access_token</c>）。
/// 官方权限口径：自建应用需配置在「可调用接口的应用」列表中；仅允许获取/修改/删除该应用创建的会议的数据；
/// 成员相关查询仅返回在应用可见范围内的成员。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Meeting",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkMeetingRecordingService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.InternalAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalMeetingRecordingService : IWechatWorkMeetingRecordingService
{
    /// <summary>
    /// 获取会议录制列表
    /// <para>获取云录制记录，根据成员 ID、会议 ID、会议 code 进行查询，支持根据时间区间分页获取；
    /// 如果使用成员 ID 进行查询，可以获取到该用户作为会议创建者的会议录制列表。</para>
    /// <para>官方限制：仅允许获取该应用创建的会议的数据；仅返回在应用可见范围内的成员；
    /// meetingid / meeting_code / userid 三者选填其中一项；查询时间区间跨度不允许超过 31 天；
    /// limit 默认 10、最大 20，且必须与首次调用获得 cursor 时传入的 limit 一致。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="ListMeetingRecordsRequest"/>：meetingid / meeting_code / userid / start_time / end_time / cursor / limit）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>是否还有未拉取的会议录制列表（has_more）、分页游标（next_cursor）与会议录制列表（record_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98192"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许获取该应用创建的会议的数据；仅返回在应用可见范围内的成员。</para>
    /// <para>官方文档陷阱：官方响应示例将录制列表字段误写为 <c>record_meetings</c>、录制文件列表字段误写为 <c>record_files</c>，
    /// 参数表为 <c>record_list</c> 与 <c>record_file_list</c>，本 SDK 以参数表为准。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/record/list")]
    Task<ListMeetingRecordsResponse> ListMeetingRecordsAsync(
        [Body] ListMeetingRecordsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取录制文件访问统计
    /// <para>获取会议录制 ID 对应的访问数据，按照天维度返回。</para>
    /// <para>官方限制：仅允许获取该应用创建的会议的数据；默认展示最近 31 天的数据，时间区间不允许超过 31 天。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetRecordStatisticsRequest"/>：meeting_record_id / start_time / end_time）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>统计结果列表（summaries：date / view_count / download_count）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98209"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许获取该应用创建的会议的数据。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/record/get_statistics")]
    Task<GetRecordStatisticsResponse> GetRecordStatisticsAsync(
        [Body] GetRecordStatisticsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改会议录制共享设置
    /// <para>根据会议录制 ID 修改共享等配置，支持修改共享权限、共享密码、共享有效期等信息。</para>
    /// <para>官方限制：仅允许修改该应用创建的会议的数据。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="UpdateRecordSharingConfigRequest"/>：meeting_record_id / meetingid / sharing_config）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98208"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许修改该应用创建的会议的数据。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/record/update_sharing_config")]
    Task<WechatWorkResponse> UpdateRecordSharingConfigAsync(
        [Body] UpdateRecordSharingConfigRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除会议录制
    /// <para>删除会议的所有录制文件，该接口会删除会议录制 ID 里对应的所有云录制文件。</para>
    /// <para>官方限制：仅允许删除该应用创建的会议的数据。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="DeleteMeetingRecordRequest"/>：meeting_record_id / meetingid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98206"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许删除该应用创建的会议的数据。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/record/delete")]
    Task<WechatWorkResponse> DeleteMeetingRecordAsync(
        [Body] DeleteMeetingRecordRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除单个录制文件
    /// <para>删除单个录制文件，该接口支持从会议中删除指定的某个录制文件。</para>
    /// <para>官方限制：仅允许删除该应用创建的会议的数据。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="DeleteRecordFileRequest"/>：record_file_id / meetingid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98207"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许删除该应用创建的会议的数据。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/record/delete_file")]
    Task<WechatWorkResponse> DeleteRecordFileAsync(
        [Body] DeleteRecordFileRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取单个录制文件详情
    /// <para>获取单个云录制的详情信息，包括录制文件和会议纪要，并可获取播放地址和下载地址。</para>
    /// <para>官方限制：仅允许获取该应用创建的会议的数据。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetRecordFileRequest"/>：record_file_id / meetingid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>录制文件详情（record_file_id / meetingid / meeting_code / view_address / download_address / download_address_file_type /
    /// audio_address / audio_address_file_type / meeting_summary / ai_meeting_transcripts / record_name / start_time / end_time / meeting_record_name）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98205"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许获取该应用创建的会议的数据。</para>
    /// <para>官方文档陷阱：start_time/end_time 参数表标注 int64、示例为字符串形态；meeting_summary 参数表标注 object[]、示例为单个对象，
    /// 本 SDK 均以参数表为准。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/record/get_file")]
    Task<GetRecordFileResponse> GetRecordFileAsync(
        [Body] GetRecordFileRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取会议录制地址
    /// <para>获取会议录制地址，可获取会议云录制的播放地址和下载地址。</para>
    /// <para>官方限制：仅允许获取该应用创建的会议的数据。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetRecordFileListRequest"/>：meeting_record_id / meetingid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>会议录制信息（meeting_record_id / meetingid / meeting_code / title）与录制文件列表（record_files）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98196"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许获取该应用创建的会议的数据。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/record/get_file_list")]
    Task<GetRecordFileListResponse> GetRecordFileListAsync(
        [Body] GetRecordFileListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取录制转写段落信息
    /// <para>获取云录制转写的段落信息（段落总数、段落 ID）。</para>
    /// <para>官方限制：仅允许获取该应用创建的会议的数据；本端点无分页参数。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetRecordTranscriptParagraphsRequest"/>：record_file_id / meetingid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>声纹识别状态（audio_detect）与段落列表（paragraphs：pid / start_time / end_time）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98212"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许获取该应用创建的会议的数据。</para>
    /// <para>官方文档陷阱：端点说明称返回「段落总数」，但响应参数表未定义段落总数字段（示例中亦无 total / paragraph_count），
    /// 段落数量须由调用方按 <c>paragraphs</c> 数组长度统计。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/record/transcript/get_paragraph_list")]
    Task<GetRecordTranscriptParagraphsResponse> GetRecordTranscriptParagraphsAsync(
        [Body] GetRecordTranscriptParagraphsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取录制转写详情
    /// <para>获取云录制转写的详情，包含时间戳、文本等内容。</para>
    /// <para>官方限制：仅允许获取该应用创建的会议的数据；仅返回在应用可见范围内的成员；
    /// <c>pid</c> 为查询的起始段落 ID（查询范围<b>含 pid 本身</b>，不传默认从 0 开始）、<c>limit</c> 为最多查询的段落数
    /// （不传默认查询全量）；传入 pid / limit 后若数据未能一次全部返回则 has_more 为 true，
    /// 继续查询须以本次响应中最后一条段落的 pid 作为下一次请求的起始 pid。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetRecordTranscriptDetailRequest"/>：record_file_id / meetingid / pid / limit）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>是否还有更多数据（has_more）与录制转写详情（transcripts：paragraphs / keywords / audio_detect）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98211"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许获取该应用创建的会议的数据；仅返回在应用可见范围内的成员。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/record/transcript/get_detail")]
    Task<GetRecordTranscriptDetailResponse> GetRecordTranscriptDetailAsync(
        [Body] GetRecordTranscriptDetailRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 搜索录制转写
    /// <para>根据指定内容搜索录制转写。</para>
    /// <para>官方限制：仅允许获取该应用创建的会议的数据。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="SearchRecordTranscriptRequest"/>：record_file_id / meetingid / text）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>搜索命中列表（hits：pid / sid / offset / length）与搜索时间轴列表（timelines：pid / sid / start_time）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98213"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许获取该应用创建的会议的数据。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/record/transcript/search")]
    Task<SearchRecordTranscriptResponse> SearchRecordTranscriptAsync(
        [Body] SearchRecordTranscriptRequest request,
        CancellationToken cancellationToken = default);
}
