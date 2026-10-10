// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 收集表的统计信息查询响应体（<c>/cgi-bin/wedoc/get_form_statistic</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方业务限制：<c>submit_users</c> / <c>unfill_users</c> 仅在 <c>req_type</c> 为 <c>2</c> / <c>3</c> 时返回；
/// 未提交列表仅当收集表限制了提交范围时才有结果；
/// 匿名填写时 <c>userid</c>、<c>user_name</c>、<c>tmp_external_userid</c> 均不返回。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class GetWedocFormStatisticResponse : WechatWorkResponse
{
    /// <summary>获取或设置已填写次数（官方 <c>fill_cnt</c>）。</summary>
    [JsonPropertyName("fill_cnt")]
    public ulong? FillCnt { get; set; }

    /// <summary>获取或设置已填写人数（官方 <c>fill_user_cnt</c>）。</summary>
    [JsonPropertyName("fill_user_cnt")]
    public ulong? FillUserCnt { get; set; }

    /// <summary>获取或设置未填写人数（官方 <c>unfill_user_cnt</c>）。</summary>
    [JsonPropertyName("unfill_user_cnt")]
    public ulong? UnfillUserCnt { get; set; }

    /// <summary>获取或设置已填写人列表（官方 <c>submit_users</c>），<c>req_type</c> 为 <c>2</c> 时返回。</summary>
    [JsonPropertyName("submit_users")]
    public List<WedocFormSubmitUser>? SubmitUsers { get; set; }

    /// <summary>获取或设置未填写人列表（官方 <c>unfill_users</c>），<c>req_type</c> 为 <c>3</c> 时返回。</summary>
    [JsonPropertyName("unfill_users")]
    public List<WedocFormUnfillUser>? UnfillUsers { get; set; }

    /// <summary>获取或设置是否还有更多数据（官方 <c>has_more</c>）。</summary>
    [JsonPropertyName("has_more")]
    public bool? HasMore { get; set; }

    /// <summary>获取或设置上次分页拉取返回的游标（官方 <c>cursor</c>），作为下一次请求的 <c>cursor</c> 继续分页。</summary>
    [JsonPropertyName("cursor")]
    public ulong? Cursor { get; set; }
}
