// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Ads.DataModels.Common;

namespace Mud.Wechat.Ads.DataModels.Reports;

/// <summary>
/// 异步报表任务查询（<c>GET /v3.0/async_reports/get</c>）的 <c>data</c> 载荷。
/// </summary>
/// <remarks>
/// 与同步报表族的 <c>AdsReportListData</c> <b>不同形</b>（那支的行是「随 <c>fields</c> 而变的透传字典」，
/// 本支的行是<b>固定</b>的任务信息结构）⇒ 两支各建各的，收敛成一支会让其中一页的 <c>list</c> 元素类型失真。
/// <c>page_info</c> 则复用公共的 <see cref="AdsPageInfo"/>（四字段形状逐字相同）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Reports")]
public class AdsAsyncReportListData
{
    /// <summary>任务列表（官方 <c>list</c>，<c>struct[]</c>）。</summary>
    [JsonPropertyName("list")]
    public List<AdsAsyncReportTaskInfo>? List { get; set; }

    /// <summary>分页元信息（官方 <c>page_info</c>）。</summary>
    [JsonPropertyName("page_info")]
    public AdsPageInfo? PageInfo { get; set; }
}

/// <summary>
/// 异步报表任务的列表元素（官方 <c>data.list[]</c> 元素，2026-10-10 整页核验）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方本页的 <c>result</c> 只有三层</b>：<c>{code, message, data{file_info_list[]{file_id, md5}}}</c>，
/// <b>没有</b> <c>message_cn</c>，也<b>没有</b>邻页 <c>async_tasks/get</c> 那四条深层分支
/// （<c>channel_package_info_list</c> / <c>union_channel_package_info_list</c> /
/// <c>review_element_prereview_result_list</c>）⇒ 本类型不多建一支官方未声明的字段，
/// 邻页那三支因层级证据仅为平面级而未建模（见留档 §11）。
/// </para>
/// <para>
/// <b>完成与否看本类型，不看请求过滤</b>：本页 <c>filtering.field</c> 官方只给
/// <c>{task_id, task_name}</c> 两支 —— <c>status</c> 是<b>应答</b>字段而非可过滤字段，
/// 所以「轮询到完成」的正确写法是 <c>filtering(field=task_id)</c> + 读回元素的 <see cref="Status"/>，
/// 而不是把 <c>TASK_STATUS_COMPLETED</c> 塞进过滤条件（留档 §7.8 早期小结把两者混写过，已更正）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Reports")]
public class AdsAsyncReportTaskInfo
{
    /// <summary>任务 id（官方 <c>task_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("task_id")]
    public long? TaskId { get; set; }

    /// <summary>任务名（官方 <c>task_name</c>，<c>string</c>，与创建时的 <c>task_name</c> 对应）。</summary>
    [JsonPropertyName("task_name")]
    public string? TaskName { get; set; }

    /// <summary>任务状态（官方 <c>status</c>，<c>enum</c>；完成态取值 <c>TASK_STATUS_COMPLETED</c>。
    /// 该枚举的全量取值集在邻页 <c>async_tasks/get</c> 的过滤规则里逐字核验，本页只声明为 <c>enum</c> ⇒
    /// 以 <see cref="string"/> 承载、不做本地枚举）。</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>创建时间（官方 <c>created_time</c>，<c>integer</c>；本页未写明单位与时区，照录不推断）。</summary>
    [JsonPropertyName("created_time")]
    public long? CreatedTime { get; set; }

    /// <summary>任务产出（官方 <c>result</c>，<c>struct</c>；任务未完成时官方可整字段缺省 ⇒ 可空）。</summary>
    [JsonPropertyName("result")]
    public AdsAsyncTaskResult? Result { get; set; }
}

/// <summary>
/// 异步任务产出信封（官方 <c>data.list[].result</c>，三层结构）。
/// </summary>
/// <remarks>
/// <b>这是「内层第二个判定面」，与外层 HTTP 信封不是一回事</b>：外层 <c>code == 0</c> 只表示
/// 「查询任务列表这个请求成功」，本层 <see cref="Code"/> 才是「这次报表生成任务本身成没成」。
/// 与批量族的逐条 <c>code</c> 同属「判错必须多层做」的一类（见 <c>AdsBatchResultItem</c>）。
/// 本层<b>不带</b> <c>message_cn</c>（官方本页应答表只有 <c>code</c> / <c>message</c> 两支描述）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Reports")]
public class AdsAsyncTaskResult
{
    /// <summary>任务结果码（官方 <c>code</c>，<c>integer</c>，<c>0</c> = 任务成功）。</summary>
    [JsonPropertyName("code")]
    public int? Code { get; set; }

    /// <summary>任务结果描述（官方 <c>message</c>，<c>string</c>）。</summary>
    [JsonPropertyName("message")]
    public string? Message { get; set; }

    /// <summary>任务产出数据（官方 <c>data</c>，<c>struct</c>）。</summary>
    [JsonPropertyName("data")]
    public AdsAsyncTaskResultData? Data { get; set; }
}

/// <summary>
/// 异步任务产出的数据层（官方 <c>result.data</c>，本页<b>只</b>声明 <c>file_info_list</c> 一支）。
/// </summary>
/// <remarks>
/// <c>async_tasks/get</c> 邻页在同一个 <c>result.data</c> 下还列了另外三条分支，但那些分支的
/// <b>层级证据仅为平面抽取级</b>（未做 DOM 层级核验）⇒ 本轮不并入本类型，避免把「平面读到的兄弟字段」
/// 挂错父结构（该类错误的后果是官方键静默绑不上，且名字完全合法、编译与序列化都不报错，见守卫 ADS-B2）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Reports")]
public class AdsAsyncTaskResultData
{
    /// <summary>生成文件列表（官方 <c>file_info_list</c>，<c>struct[]</c>）。</summary>
    [JsonPropertyName("file_info_list")]
    public List<AdsAsyncTaskFileInfo>? FileInfoList { get; set; }
}

/// <summary>
/// 异步报表文件的下载定位信息（官方 <c>file_info_list[]</c> 元素）。
/// </summary>
/// <remarks>
/// <b>两支字段是官方全集，且「文件名 / 文件格式」在整条异步链路上都不存在</b>：
/// 从 <c>async_reports/add</c> 到 <c>async_reports/get</c> 再到 <c>async_report_files/get</c>，
/// 三页参数表里没有任何 <c>file_name</c> / <c>format</c> / <c>extension</c> 字段，
/// 下载动作只吃 <c>task_id</c> + <c>file_id</c> ⇒ 落地文件的命名与格式是<b>调用方</b>的决定，
/// SDK 不代做猜测（不生成默认文件名、不解析扩展名）。
/// <c>md5</c> 是唯一的完整性字段：官方原文建议「先下载整个文件，然后再解析」，
/// 边下边解会因处理时间过长而超时。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Reports")]
public class AdsAsyncTaskFileInfo
{
    /// <summary>文件 id（官方 <c>file_id</c>，<c>integer</c>；与 <c>task_id</c> 一起交给
    /// <c>async_report_files/get</c> 才能下载）。</summary>
    [JsonPropertyName("file_id")]
    public long? FileId { get; set; }

    /// <summary>文件 MD5（官方 <c>md5</c>，<c>string</c>，完整性校验的唯一官方字段）。</summary>
    [JsonPropertyName("md5")]
    public string? Md5 { get; set; }
}

/// <summary><c>async_reports/get</c> 应答信封（闭合类型）。</summary>
[HttpJsonSerializable(SerializerClassName = "Reports")]
public class AdsAsyncReportGetResponse : AdsResponse<AdsAsyncReportListData>
{
}
