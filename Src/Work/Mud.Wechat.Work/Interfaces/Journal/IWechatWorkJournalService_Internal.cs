// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Journal;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「汇报」（日志）模块企业自建应用 SDK（汇报记录号列表、汇报记录详情、汇报统计数据）。
/// <para>
/// 官方仅向自建应用开放本域 3 个端点（「汇报」文档树只在自建应用开发根下存在页面），
/// 声明于本接口（形态对齐 <see cref="IWechatWorkInternalDialService"/>）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>）。
/// </para>
/// <para>
/// 官方权限（三页同口径）：调用的应用需配置到「汇报 - 可调用接口的应用」中；<b>代开发应用与第三方应用官方均标注「暂不支持」</b>
/// ⇒ 本域只有自建子接口。注：从 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口，存量企业暂不受影响。
/// </para>
/// <para>
/// 官方频率口径：93393 在正文里独立给出「调用频率不可超过 600 次/分」，并按「一次拉取调用最多拉取 100 个汇报记录」
/// 限制单页规模（已写入该端点文档）；93394 与 93395 两页<b>未</b>给出独立的数值型频率限制，
/// 走官方「附录 - 访问频率限制」的默认阈值（每企业调用单个 cgi/api 不可超过 1 万次/分、15 万次/小时，
/// <see href="https://developer.work.weixin.qq.com/document/path/90312"/>）；
/// 该两端点<b>不</b>写「接口调用频率限制为 N 次/分钟」——该空缺是官方正文口径，不得凭第三方 SDK 注释或记忆补数。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Journal",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkJournalService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.InternalAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalJournalService : IWechatWorkJournalService
{
    /// <summary>
    /// 获取汇报记录号列表
    /// <para>按时间范围翻页拉取汇报记录 UUID 列表（可按筛选条件收窄），
    /// 取得的 UUID 用于调用「获取汇报记录详情」。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="JournalRecordListRequest"/>：starttime / endtime / cursor / limit 官方均为必填，filters 可选）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>汇报 UUID 列表（journaluuid_list）、翻页结束标识（endflag）与下一页标记（next_cursor）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93393"/></para>
    /// <para>官方权限：调用的应用需配置到「汇报 - 可调用接口的应用」中；代开发应用与第三方应用官方均标注「暂不支持」。
    /// 注：从 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口，存量企业暂不受影响。</para>
    /// <para>官方限制：<b>一次拉取调用最多拉取 100 个汇报记录</b>，可以通过多次拉取的方式来满足需求，
    /// 但<b>调用频率不可超过 600 次/分</b>（本域唯一给出独立数值频率的端点，官方正文口径）。</para>
    /// <para>官方业务限制：本接口为 POST；<c>cursor</c> 官方为<b>必填</b>且「首次请求传 0，非首次请求携带上一次请求返回的 next_cursor」，
    /// <c>limit</c>（拉取条数）与 <c>starttime</c> / <c>endtime</c> 同为必填，且「开始时间和结束时间间隔不能超过一个月」；
    /// <c>filters</c> 可选，其 key 官方枚举为 creator（指定汇报记录提单人）/ department（指定提单人所在部门）/ template_id（指定模板），
    /// key 与 value 各不多于 256 字节；<c>endflag</c> 官方口径为「0 代表还有数据，1 代表已无数据」，
    /// 官方以数字 0/1 承载该标识，DTO 原样以 <c>int</c> 承载；取得的 <c>journaluuid_list</c> 用于调用「获取汇报记录详情」；
    /// 汇报表单 id 的获取路径（官方正文）：管理后台 - 汇报应用 - 某个汇报的内容设置页 - 点击「汇报名称」。</para>
    /// <para>官方错误码（本域三页共用）：301065 无汇报应用数据拉取权限 / 301066 请求参数错误 / 301067 接口内部失败。</para>
    /// </remarks>
    [Post("/cgi-bin/oa/journal/get_record_list")]
    Task<JournalRecordListResponse> GetJournalRecordListAsync(
        [Body] JournalRecordListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取汇报记录详情
    /// <para>按汇报 UUID 取单条汇报的表单信息、提交人、接收人与已读接收人、汇报内容以及评论列表。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="JournalRecordDetailRequest"/>：journaluuid，官方必填且不多于 256 字节）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>汇报详情（info：journal_uuid / template_id / template_name / submitter / receivers / readed_receivers / apply_data / sys_journal_data / comments / report_time）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93394"/></para>
    /// <para>官方权限：调用的应用需配置到「汇报 - 可调用接口的应用」中；代开发应用与第三方应用官方均标注「暂不支持」。</para>
    /// <para>本页面未给出独立的数值型频率限制，走官方「附录 - 访问频率限制」的默认阈值（见本接口 <c>remarks</c> 汇总口径）。</para>
    /// <para>官方业务限制：本接口为 POST，入参 <c>journaluuid</c> 取自「获取汇报记录号列表」；
    /// 汇报内容 <c>apply_data.contents</c> 按控件（control）+ 控件 ID（id）+ 多语言名称（title，配置多语言时含中英文）+ 控件值（value）
    /// 的形态返回，官方 <c>control</c> 取值表为 Text / Textarea / Number / Money / Date / Selector / Contact / Tips / File /
    /// Table / DateRange / Location / Formula / SchoolContact / Doc / WedriveFile；控件值各字段仅在对应控件类型被使用时出现，
    /// 官方附录（附1~附17）另有若干细节口径：Tips 控件不显示在详情中故 value 为空；Date 的 <c>s_timestamp</c> 官方为<b>字符串</b>类型，
    /// 且 <c>type</c> 为 minute 时该值是「从 0 点的偏移秒数」而非时间戳；Selector 的 <c>options</c> <b>仅包含申请人所选择的选项</b>，
    /// 要了解全部选项须改用「获取审批模板详情」接口，其 <c>key</c> 对 2022-08-25 之后新创建的企业不再支持关联审批模板；
    /// File 的 <c>file_id</c> 即 media_id，需另走「获取临时素材」下载；Location 的 <c>time</c> 官方标注「已废弃」；
    /// 企业微信 4.0.20 版本后不会再有新增的文档控件（Doc）数据；「汇报」模板是一个<b>特殊模板</b>，其表单不在 apply_data 中返回，
    /// 而落在 <c>info.sys_journal_data</c>（富文本，官方以 <c>SysJournalData</c> 承载，<c>apply_data</c> 与它二者按模板形态取用）；
    /// 评论的 <c>commentid</c> / <c>tocommentid</c> 官方线格式为数字（uint64），DTO 以 <c>long</c> 承载。</para>
    /// <para>官方错误码（本域三页共用）：301065 无汇报应用数据拉取权限 / 301066 请求参数错误 / 301067 接口内部失败。</para>
    /// </remarks>
    [Post("/cgi-bin/oa/journal/get_record_detail")]
    Task<JournalRecordDetailResponse> GetJournalRecordDetailAsync(
        [Body] JournalRecordDetailRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取汇报统计数据
    /// <para>按时间范围（可选按表单）取汇报的统计信息：表单与周期时间、汇报人员范围与接收人范围、
    /// 已汇报列表与未汇报列表。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="JournalStatListRequest"/>：starttime / endtime / template_id，官方三参数均为必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>统计列表（stat_list，元素含 template_id / report_range / white_range / receivers / report_list / unreport_list 等）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93395"/></para>
    /// <para>官方权限：调用的应用需配置到「汇报 - 可调用接口的应用」中；代开发应用与第三方应用官方均标注「暂不支持」。</para>
    /// <para>官方业务限制：本接口为 POST；<c>template_id</c> 为<b>必填</b>（不多于 256 字节），即本接口按<b>单一汇报表单</b>取统计汇总，
    /// 官方三参数（starttime / endtime / template_id）无可选参数；<c>endtime</c> 与 <c>starttime</c> 的时间区间官方限「最大长度为一年」；
    /// 官方正文口径「只能拉取到<b>已经汇总</b>的统计数据，对于尚未完成汇总的周期不会返回」；
    /// 表单 id 的获取路径（官方正文）：管理后台 - 汇报应用 - 某个汇报的内容设置页 - 点击「汇报名称」。</para>
    /// <para>官方响应口径：<c>report_type</c> 为数值枚举（2 按日汇报 / 3 按周汇报 / 4 按月汇报）；
    /// <c>receivers.leader_list.level</c> 官方为「上级级别从 1 开始」；统计侧的部门与标签以 <c>open_partyid</c> / <c>open_tagid</c>
    /// 回传（官方拼写，非 partyid / tagid）；已汇报条目的 <c>flag</c> 官方以数字承载迟交标识（1 迟交 / 0 非迟交），
    /// DTO 原样以 <c>int?</c> 承载；<c>unreport_list</c> 与 <c>report_list</c> 官方为同一 <c>user</c> + <c>itemlist</c> 形态，
    /// 未汇报条目的 <c>journaluuid</c> 官方示例回传空串。</para>
    /// <para>本页面未给出独立的数值型频率限制，走官方「附录 - 访问频率限制」的默认阈值（见本接口 <c>remarks</c> 汇总口径）。</para>
    /// <para>官方错误码（本域三页共用）：301065 无汇报应用数据拉取权限 / 301066 请求参数错误 / 301067 接口内部失败。</para>
    /// </remarks>
    [Post("/cgi-bin/oa/journal/get_stat_list")]
    Task<JournalStatListResponse> GetJournalStatListAsync(
        [Body] JournalStatListRequest request,
        CancellationToken cancellationToken = default);
}
