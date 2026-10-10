// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json;

namespace Mud.Wechat.MiniProgram.DataModels.Operation;

/// <summary>论坛（feedback）列表条目（<c>list[]</c>）。</summary>
/// <remarks>字段以官方页面为准；SDK 不做本地校验。</remarks>
[HttpJsonSerializable(SerializerClassName = "Operation")]
public class WxaFeedbackItem
{
    /// <summary>反馈记录 ID（<c>record_id</c>）。</summary>
    [JsonPropertyName("record_id")]
    public long? RecordId { get; set; }

    /// <summary>反馈用户 openid（<c>openid</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>反馈时间（<c>create_time</c>，Unix 秒级时间戳）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>反馈内容（<c>content</c>）。</summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    /// <summary>反馈类型（<c>type</c>）：<c>1</c> 无法打开 / <c>2</c> 一直闪退 / <c>3</c> 一直卡死 / <c>4</c> 其他问题（数值以官方页面为准）。</summary>
    [JsonPropertyName("type")]
    public long? Type { get; set; }

    /// <summary>反馈图片媒体 ID 列表（<c>media_ids</c>）。</summary>
    [JsonPropertyName("media_ids")]
    public List<string>? MediaIds { get; set; }
}

/// <summary>获取用户反馈列表请求体（<c>POST /wxaapi/feedback/list</c>）。</summary>
/// <remarks>官方文档：<c>operation/api_getfeedback.html</c>。<c>type</c> 选填（不传则查全部）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Operation")]
public class WxaFeedbackListRequest
{
    /// <summary>起始位置（<c>page</c>，从 <c>0</c> 开始，必填）。</summary>
    [JsonPropertyName("page")]
    public long? Page { get; set; }

    /// <summary>返回条数（<c>limit</c>，必填；上限以官方页面为准）。</summary>
    [JsonPropertyName("limit")]
    public long? Limit { get; set; }

    /// <summary>反馈类型（<c>type</c>，选填）：<c>1</c> 无法打开 / <c>2</c> 一直闪退 / <c>3</c> 一直卡死 / <c>4</c> 其他问题。</summary>
    [JsonPropertyName("type")]
    public long? Type { get; set; }
}

/// <summary>获取用户反馈列表应答（<c>total</c> + <c>list</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Operation")]
public class WxaFeedbackListResponse : WxaResponse
{
    /// <summary>反馈总数（<c>total</c>）。</summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>反馈列表（<c>list</c>），见 <see cref="WxaFeedbackItem"/>。</summary>
    [JsonPropertyName("list")]
    public List<WxaFeedbackItem>? List { get; set; }
}

/// <summary>查询实时日志请求体（<c>POST /wxaapi/userlog/userlog_search</c>）。</summary>
/// <remarks>
/// <para>官方文档：<c>operation/api_realtimelogsearch.html</c>。</para>
/// <para><c>date</c> / <c>begintime</c> / <c>endtime</c> / <c>start</c> / <c>limit</c> 必填；
/// <c>traceId</c>、<c>uin</c> 选填（按需过滤）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Operation")]
public class WxaUserLogSearchRequest
{
    /// <summary>日期（<c>date</c>，必填；格式 <c>yyyyMMdd</c>）。</summary>
    [JsonPropertyName("date")]
    public string? Date { get; set; }

    /// <summary>起始时间（<c>begintime</c>，必填；格式 <c>HH:mm:ss</c>）。</summary>
    [JsonPropertyName("begintime")]
    public string? BeginTime { get; set; }

    /// <summary>结束时间（<c>endtime</c>，必填；格式 <c>HH:mm:ss</c>）。</summary>
    [JsonPropertyName("endtime")]
    public string? EndTime { get; set; }

    /// <summary>起始偏移（<c>start</c>，必填）。</summary>
    [JsonPropertyName("start")]
    public long? Start { get; set; }

    /// <summary>查询条数（<c>limit</c>，必填；上限以官方页面为准）。</summary>
    [JsonPropertyName("limit")]
    public long? Limit { get; set; }

    /// <summary>链路 ID（<c>traceId</c>，选填）。</summary>
    [JsonPropertyName("traceId")]
    public string? TraceId { get; set; }

    /// <summary>用户标识（<c>uin</c>，选填）。</summary>
    [JsonPropertyName("uin")]
    public string? Uin { get; set; }
}

/// <summary>查询实时日志应答（<c>numbers</c> + <c>datas</c>）。</summary>
/// <remarks>官方文档：<c>operation/api_realtimelogsearch.html</c>；<c>datas</c> 为原始日志行数组。</remarks>
[HttpJsonSerializable(SerializerClassName = "Operation")]
public class WxaUserLogSearchResponse : WxaResponse
{
    /// <summary>本次返回的数据条数（<c>numbers</c>）。</summary>
    [JsonPropertyName("numbers")]
    public long? Numbers { get; set; }

    /// <summary>日志数据（<c>datas</c>，原始日志行数组）。</summary>
    [JsonPropertyName("datas")]
    public List<string>? Datas { get; set; }
}

/// <summary>获取性能 / 访问来源 / 客户端版本通用请求体（<c>date</c> + <c>module</c>）。</summary>
/// <remarks>
/// <para>官方文档：<c>operation/api_getperformance.html</c>、<c>api_getscenelist.html</c>、<c>api_getversionlist.html</c>。</para>
/// <para><c>module</c> 取值：<c>all</c> / <c>js</c> / <c>network</c> / <c>run</c>（以官方页面为准）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Operation")]
public class WxaLogDateModuleRequest
{
    /// <summary>日期（<c>date</c>，必填；格式 <c>yyyyMMdd</c>）。</summary>
    [JsonPropertyName("date")]
    public string? Date { get; set; }

    /// <summary>模块（<c>module</c>，必填）：<c>all</c> / <c>js</c> / <c>network</c> / <c>run</c>。</summary>
    [JsonPropertyName("module")]
    public string? Module { get; set; }
}

/// <summary>
/// 运维日志类应答基底（<c>data</c> 为<b>逐项键值对</b>列表透传）。
/// </summary>
/// <remarks>
/// <para>承载性能（<c>get_performance</c>）、访问来源（<c>get_scene</c>）、客户端版本（<c>get_client_version</c>）
/// 三类端点的 <c>data</c> 列表 —— 官方各「项」字段随版本演进且互不一致，SDK 不猜测字段名，
/// 以原始键值对透传（元素即官方数据项，键名照官方原文）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Operation")]
public class WxaLogDataListResponse : WxaResponse
{
    /// <summary>数据项列表（<c>data</c>），每项为官方原始键值对。</summary>
    [JsonPropertyName("data")]
    public List<Dictionary<string, JsonElement>>? Data { get; set; }
}

/// <summary>查询 JS 错误详情请求体（<c>POST /wxaapi/log/jserr_detail</c>）。</summary>
/// <remarks>官方文档：<c>operation/api_getjserrdetail.html</c>；<c>errmsg_key</c> 为错误信息关键字（必填）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Operation")]
public class WxaJsErrDetailRequest
{
    /// <summary>日期（<c>date</c>，必填；格式 <c>yyyyMMdd</c>）。</summary>
    [JsonPropertyName("date")]
    public string? Date { get; set; }

    /// <summary>错误信息关键字（<c>errmsg_key</c>，必填）。</summary>
    [JsonPropertyName("errmsg_key")]
    public string? ErrmsgKey { get; set; }
}

/// <summary>查询 JS 错误列表请求体（<c>POST /wxaapi/log/jserr_list</c>）。</summary>
/// <remarks>官方文档：<c>operation/api_getjserrlist.html</c>；<c>errmsg_key</c> 选填（不传则查全量）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Operation")]
public class WxaJsErrListRequest
{
    /// <summary>日期（<c>date</c>，必填；格式 <c>yyyyMMdd</c>）。</summary>
    [JsonPropertyName("date")]
    public string? Date { get; set; }

    /// <summary>错误信息关键字（<c>errmsg_key</c>，选填）。</summary>
    [JsonPropertyName("errmsg_key")]
    public string? ErrmsgKey { get; set; }
}

/// <summary>JS 错误详情 / 列表应答基底（<c>data</c> 原始透传）。</summary>
/// <remarks>官方 <c>jserr_detail</c> / <c>jserr_list</c> 的 <c>data</c> 结构随版本演进，SDK 不猜测字段名、以原始键值对透传。</remarks>
[HttpJsonSerializable(SerializerClassName = "Operation")]
public class WxaJsErrDataResponse : WxaResponse
{
    /// <summary>错误数据（<c>data</c>，官方原始键值对）。</summary>
    [JsonPropertyName("data")]
    public Dictionary<string, JsonElement>? Data { get; set; }
}

/// <summary>获取分阶段发布详情应答（<c>POST /wxa/getgrayreleaseplan</c>）。</summary>
/// <remarks>
/// 官方文档：<c>operation/api_getgrayreleaseplan.html</c>。
/// <see cref="GrayReleasePlan"/> 为官方 <c>gray_release_plan</c> 对象（含灰度状态、时间窗、灰度比例等），
/// 字段随版本演进以原始键值对透传。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Operation")]
public class WxaGrayReleasePlanResponse : WxaResponse
{
    /// <summary>分阶段发布计划（<c>gray_release_plan</c>，官方原始键值对）。</summary>
    [JsonPropertyName("gray_release_plan")]
    public Dictionary<string, JsonElement>? GrayReleasePlan { get; set; }
}

/// <summary>查询域名配置应答（<c>POST /wxa/getwxadevinfo</c>）。</summary>
/// <remarks>官方文档：<c>operation/api_getdomaininfo.html</c>；返回四类网络请求域名白名单配置。</remarks>
[HttpJsonSerializable(SerializerClassName = "Operation")]
public class WxaDevInfoResponse : WxaResponse
{
    /// <summary>request 合法域名配置（<c>request_domain</c>），见 <see cref="WxaDomainGroup"/>。</summary>
    [JsonPropertyName("request_domain")]
    public WxaDomainGroup? RequestDomain { get; set; }
}

/// <summary>网络请求域名分组（<c>request_domain</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Operation")]
public class WxaDomainGroup
{
    /// <summary>request 合法域名（<c>request</c>）。</summary>
    [JsonPropertyName("request")]
    public List<string>? Request { get; set; }

    /// <summary>socket 合法域名（<c>ws</c>）。</summary>
    [JsonPropertyName("ws")]
    public List<string>? Ws { get; set; }

    /// <summary>uploadFile 合法域名（<c>upload</c>）。</summary>
    [JsonPropertyName("upload")]
    public List<string>? Upload { get; set; }

    /// <summary>downloadFile 合法域名（<c>download</c>）。</summary>
    [JsonPropertyName("download")]
    public List<string>? Download { get; set; }
}

/// <summary>获取用户反馈图片请求体（<c>POST /cgi-bin/media/getfeedbackmedia</c>）。</summary>
/// <remarks>官方文档：<c>operation/api_getfeedbackmedia.html</c>；成功响应为<b>图片二进制流</b>（走独立图片通道，见 <c>IWxaFeedbackMediaService</c>）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Operation")]
public class WxaFeedbackMediaRequest
{
    /// <summary>反馈图片的媒体 ID（<c>media_id</c>，必填；取自 <see cref="WxaFeedbackItem.MediaIds"/>）。</summary>
    [JsonPropertyName("media_id")]
    public string? MediaId { get; set; }
}