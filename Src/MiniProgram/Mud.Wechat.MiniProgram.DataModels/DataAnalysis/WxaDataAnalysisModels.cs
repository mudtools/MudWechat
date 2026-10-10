// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和义务，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.DataAnalysis;

/// <summary>
/// 数据分析日期区间请求体（<c>begin_date</c> / <c>end_date</c>；9 端点共用同形）。
/// </summary>
/// <remarks>
/// <para>格式 <c>yyyymmdd</c>；<b>上界恒为昨日</b>。跨度按端点族不同（1 天 / 对应周期 / 0·6·29 天），
/// 见各端点 remarks —— SDK <b>不做本地校验</b>，越界由官方错误码表达。</para>
/// <para>保持 <see cref="string"/> 而非日期类型：官方为定长压缩格式（<c>20170313</c>），
/// 转成 <c>DateTime</c> 再格式化只会引入文化/时区漂移风险。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "DataAnalysis")]
public class WxaDateRangeRequest
{
    /// <summary>开始日期（<c>begin_date</c>，必填，格式 <c>yyyymmdd</c>）。</summary>
    [JsonPropertyName("begin_date")]
    public string? BeginDate { get; set; }

    /// <summary>结束日期（<c>end_date</c>，必填，格式 <c>yyyymmdd</c>；最大值 ≤ 昨日）。</summary>
    [JsonPropertyName("end_date")]
    public string? EndDate { get; set; }
}

/// <summary>访问趋势应答（日 / 周 / 月三端点共用；<b>顶层无 <c>ref_date</c></b>，仅 <c>list</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "DataAnalysis")]
public class WxaVisitTrendResponse : WxaResponse
{
    /// <summary>趋势数据列表（<c>list</c>），见 <see cref="WxaVisitTrendItem"/>。</summary>
    [JsonPropertyName("list")]
    public List<WxaVisitTrendItem>? List { get; set; }
}

/// <summary>趋势数据条目（<c>list[]</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "DataAnalysis")]
public class WxaVisitTrendItem
{
    /// <summary>日期（<c>ref_date</c>，格式 <c>yyyymmdd</c>）。</summary>
    [JsonPropertyName("ref_date")]
    public string? RefDate { get; set; }

    /// <summary>打开次数（<c>session_cnt</c>）。</summary>
    [JsonPropertyName("session_cnt")]
    public long? SessionCount { get; set; }

    /// <summary>访问次数（<c>visit_pv</c>）。</summary>
    [JsonPropertyName("visit_pv")]
    public long? VisitPv { get; set; }

    /// <summary>访问人数（<c>visit_uv</c>）。</summary>
    [JsonPropertyName("visit_uv")]
    public long? VisitUv { get; set; }

    /// <summary>新用户数（<c>visit_uv_new</c>）。</summary>
    [JsonPropertyName("visit_uv_new")]
    public long? VisitUvNew { get; set; }

    /// <summary>人均停留时长（<c>stay_time_uv</c>，<b>浮点</b>，单位秒）。</summary>
    [JsonPropertyName("stay_time_uv")]
    public double? StayTimeUv { get; set; }

    /// <summary>次均停留时长（<c>stay_time_session</c>，<b>浮点</b>，单位秒）。</summary>
    [JsonPropertyName("stay_time_session")]
    public double? StayTimeSession { get; set; }

    /// <summary>平均访问深度（<c>visit_depth</c>，<b>浮点</b>）。</summary>
    [JsonPropertyName("visit_depth")]
    public double? VisitDepth { get; set; }
}

/// <summary>访问留存应答（日 / 周 / 月三端点共用；<b>顶层带 <c>ref_date</c> + 两个数组</b>）。</summary>
[HttpJsonSerializable(SerializerClassName = "DataAnalysis")]
public class WxaRetainResponse : WxaResponse
{
    /// <summary>日期（<c>ref_date</c>，格式 <c>yyyymmdd</c>）。</summary>
    [JsonPropertyName("ref_date")]
    public string? RefDate { get; set; }

    /// <summary>新增用户留存（<c>visit_uv_new</c>，数组），见 <see cref="WxaKeyValueItem"/>。</summary>
    [JsonPropertyName("visit_uv_new")]
    public List<WxaKeyValueItem>? VisitUvNew { get; set; }

    /// <summary>活跃用户留存（<c>visit_uv</c>，数组），见 <see cref="WxaKeyValueItem"/>。</summary>
    [JsonPropertyName("visit_uv")]
    public List<WxaKeyValueItem>? VisitUv { get; set; }
}

/// <summary>
/// 键值条目（<c>{key,value}</c>）：留存数组元素与访问分布 <c>item_list</c> 元素<b>同形共用</b>。
/// </summary>
/// <remarks>
/// <b><c>key</c> 语义随宿主端点不同</b>：留存端点为留存天数（<c>0</c> 当天、<c>1</c> 一天后……
/// 取值 <c>0,1,2,3,4,5,6,7,14,30</c>）；访问分布端点为各 <c>index</c> 下的场景 id。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "DataAnalysis")]
public class WxaKeyValueItem
{
    /// <summary>标识（<c>key</c>）。</summary>
    [JsonPropertyName("key")]
    public long? Key { get; set; }

    /// <summary>数值（<c>value</c>）：留存端点为用户数，分布端点为该场景访问 pv。</summary>
    [JsonPropertyName("value")]
    public long? Value { get; set; }
}

/// <summary>用户画像应答（<c>ref_date</c> 为<b>区间串</b> + 新/活跃两组画像）。</summary>
[HttpJsonSerializable(SerializerClassName = "DataAnalysis")]
public class WxaUserPortraitResponse : WxaResponse
{
    /// <summary>时间范围（<c>ref_date</c>，如 <c>"20170611-20170617"</c>，<b>非单日</b>）。</summary>
    [JsonPropertyName("ref_date")]
    public string? RefDate { get; set; }

    /// <summary>新用户画像（<c>visit_uv_new</c>），见 <see cref="WxaPortrait"/>。</summary>
    [JsonPropertyName("visit_uv_new")]
    public WxaPortrait? VisitUvNew { get; set; }

    /// <summary>活跃用户画像（<c>visit_uv</c>），见 <see cref="WxaPortrait"/>。</summary>
    [JsonPropertyName("visit_uv")]
    public WxaPortrait? VisitUv { get; set; }
}

/// <summary>用户画像（省 / 市 / 性别 / 平台 / 终端 / 年龄六维）。</summary>
[HttpJsonSerializable(SerializerClassName = "DataAnalysis")]
public class WxaPortrait
{
    /// <summary>省份分布（<c>province</c>）。</summary>
    [JsonPropertyName("province")]
    public List<WxaPortraitItem>? Province { get; set; }

    /// <summary>城市分布（<c>city</c>）。</summary>
    [JsonPropertyName("city")]
    public List<WxaPortraitItem>? City { get; set; }

    /// <summary>性别分布（<c>genders</c>）。</summary>
    [JsonPropertyName("genders")]
    public List<WxaPortraitItem>? Genders { get; set; }

    /// <summary>平台分布（<c>platforms</c>，Android / iOS 等）。</summary>
    [JsonPropertyName("platforms")]
    public List<WxaPortraitItem>? Platforms { get; set; }

    /// <summary>终端分布（<c>devices</c>，iPhone / android / 其他）。</summary>
    [JsonPropertyName("devices")]
    public List<WxaPortraitItem>? Devices { get; set; }

    /// <summary>年龄区间分布（<c>ages</c>）。</summary>
    [JsonPropertyName("ages")]
    public List<WxaPortraitItem>? Ages { get; set; }
}

/// <summary>画像条目（<c>{id,name,value}</c>）。</summary>
/// <remarks>
/// <see cref="Id"/> 为可空：官方字段表对六类数组统一列 <c>id</c>，但示例中 <c>devices</c> 元素缺 <c>id</c>
/// （官方不一致，照录且<b>不本地补齐</b>）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "DataAnalysis")]
public class WxaPortraitItem
{
    /// <summary>属性值 id（<c>id</c>；官方示例中部分维度缺省）。</summary>
    [JsonPropertyName("id")]
    public long? Id { get; set; }

    /// <summary>属性值名称（<c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>该属性值的访问 uv（<c>value</c>）。</summary>
    [JsonPropertyName("value")]
    public long? Value { get; set; }
}

/// <summary>访问分布应答（<c>ref_date</c> + 按 <c>index</c> 分组的 <c>list</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "DataAnalysis")]
public class WxaVisitDistributionResponse : WxaResponse
{
    /// <summary>日期（<c>ref_date</c>，格式 <c>yyyymmdd</c>）。</summary>
    [JsonPropertyName("ref_date")]
    public string? RefDate { get; set; }

    /// <summary>分布数据列表（<c>list</c>），见 <see cref="WxaVisitDistributionItem"/>。</summary>
    [JsonPropertyName("list")]
    public List<WxaVisitDistributionItem>? List { get; set; }
}

/// <summary>分布数据条目（<c>list[]</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "DataAnalysis")]
public class WxaVisitDistributionItem
{
    /// <summary>
    /// 分布类型（<c>index</c>）：<c>access_source_session_cnt</c>（访问来源）/
    /// <c>access_staytime_info</c>（访问时长）/ <c>access_depth_info</c>（访问深度）。
    /// </summary>
    [JsonPropertyName("index")]
    public string? Index { get; set; }

    /// <summary>分布明细（<c>item_list</c>，<c>{key,value}</c> 数组），见 <see cref="WxaKeyValueItem"/>。</summary>
    [JsonPropertyName("item_list")]
    public List<WxaKeyValueItem>? ItemList { get; set; }
}

/// <summary>访问页面应答（<c>ref_date</c> + <c>list</c>，<b>只含 top 200</b>）。</summary>
[HttpJsonSerializable(SerializerClassName = "DataAnalysis")]
public class WxaVisitPageResponse : WxaResponse
{
    /// <summary>日期（<c>ref_date</c>，格式 <c>yyyymmdd</c>）。</summary>
    [JsonPropertyName("ref_date")]
    public string? RefDate { get; set; }

    /// <summary>页面明细列表（<c>list</c>，按 <c>page_visit_pv</c> 排序的 top 200），见 <see cref="WxaVisitPageItem"/>。</summary>
    [JsonPropertyName("list")]
    public List<WxaVisitPageItem>? List { get; set; }
}

/// <summary>页面明细条目（<c>list[]</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "DataAnalysis")]
public class WxaVisitPageItem
{
    /// <summary>页面路径（<c>page_path</c>）。</summary>
    [JsonPropertyName("page_path")]
    public string? PagePath { get; set; }

    /// <summary>访问次数（<c>page_visit_pv</c>）。</summary>
    [JsonPropertyName("page_visit_pv")]
    public long? PageVisitPv { get; set; }

    /// <summary>访问人数（<c>page_visit_uv</c>）。</summary>
    [JsonPropertyName("page_visit_uv")]
    public long? PageVisitUv { get; set; }

    /// <summary>次均停留时长（<c>page_staytime_pv</c>，<b>浮点</b>）。</summary>
    [JsonPropertyName("page_staytime_pv")]
    public double? PageStayTimePv { get; set; }

    /// <summary>进入页次数（<c>entrypage_pv</c>）。</summary>
    [JsonPropertyName("entrypage_pv")]
    public long? EntryPagePv { get; set; }

    /// <summary>退出页次数（<c>exitpage_pv</c>）。</summary>
    [JsonPropertyName("exitpage_pv")]
    public long? ExitPagePv { get; set; }

    /// <summary>转发次数（<c>page_share_pv</c>）。</summary>
    [JsonPropertyName("page_share_pv")]
    public long? PageSharePv { get; set; }

    /// <summary>转发人数（<c>page_share_uv</c>）。</summary>
    [JsonPropertyName("page_share_uv")]
    public long? PageShareUv { get; set; }
}

/// <summary>用户访问小程序数据概况应答（<c>POST /datacube/getweanalysisappiddailysummarytrend</c>）。</summary>
/// <remarks>
/// <para>官方文档：<c>data-analysis/others/api_getdailysummary.html</c>。</para>
/// <para><b>与「日趋势」差异（勿混淆）</b>：趋势返回打开 / 访问 / 停留等<b>过程指标</b>；概况返回
/// 累计用户数 / 转发次数 / 转发人数等<b>结果指标</b>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "DataAnalysis")]
public class WxaDailySummaryResponse : WxaResponse
{
    /// <summary>概况数据列表（<c>list</c>），见 <see cref="WxaDailySummaryItem"/>。</summary>
    [JsonPropertyName("list")]
    public List<WxaDailySummaryItem>? List { get; set; }
}

/// <summary>概况数据条目（<c>list[]</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "DataAnalysis")]
public class WxaDailySummaryItem
{
    /// <summary>日期（<c>ref_date</c>，格式 <c>yyyymmdd</c>）。</summary>
    [JsonPropertyName("ref_date")]
    public string? RefDate { get; set; }

    /// <summary>累计用户数（<c>visit_total</c>）。</summary>
    [JsonPropertyName("visit_total")]
    public long? VisitTotal { get; set; }

    /// <summary>转发次数（<c>share_pv</c>）。</summary>
    [JsonPropertyName("share_pv")]
    public long? SharePv { get; set; }

    /// <summary>转发人数（<c>share_uv</c>）。</summary>
    [JsonPropertyName("share_uv")]
    public long? ShareUv { get; set; }
}

/// <summary>获取小程序性能数据请求体（<c>POST /wxa/business/performance/boot</c>）。</summary>
/// <remarks>
/// <para>官方文档：<c>data-analysis/others/api_getperformancedata.html</c>。</para>
/// <para>
/// <b>时间粒度（官方原文）</b>：<c>time</c> 为开始时间，格式 <c>yyyymmddhh</c>，<b>精确到小时</b>
/// （与 datacube 各端点的 <c>yyyymmdd</c> 粒度不同，勿混用）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "DataAnalysis")]
public class WxaPerformanceBootRequest
{
    /// <summary>开始时间（<c>time</c>，必填；格式 <c>yyyymmddhh</c>，精确到小时）。</summary>
    [JsonPropertyName("time")]
    public string? Time { get; set; }

    /// <summary>模块类型（<c>module</c>，必填）：<c>1</c> 启动性能 / <c>2</c> 运行性能。</summary>
    [JsonPropertyName("module")]
    public long? Module { get; set; }

    /// <summary>模块扩展参数（<c>extinfo</c>，选填；标准 JSON 序列化字符串，具体字段以官方页面为准）。</summary>
    [JsonPropertyName("extinfo")]
    public string? ExtInfo { get; set; }
}

/// <summary>获取小程序性能数据应答（<c>data</c>，<c>{key,value}</c> 数组）。</summary>
[HttpJsonSerializable(SerializerClassName = "DataAnalysis")]
public class WxaPerformanceBootResponse : WxaResponse
{
    /// <summary>性能指标数组（<c>data</c>），见 <see cref="WxaPerformanceDataItem"/>。</summary>
    [JsonPropertyName("data")]
    public List<WxaPerformanceDataItem>? Data { get; set; }
}

/// <summary>性能指标条目（<c>data[]</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "DataAnalysis")]
public class WxaPerformanceDataItem
{
    /// <summary>指标名（<c>key</c>；如启动性能的 <c>app_start_elapsed_time</c>）。</summary>
    [JsonPropertyName("key")]
    public string? Key { get; set; }

    /// <summary>指标值（<c>value</c>，<b>数值型</b>；单位以官方口径为准）。</summary>
    [JsonPropertyName("value")]
    public double? Value { get; set; }
}
