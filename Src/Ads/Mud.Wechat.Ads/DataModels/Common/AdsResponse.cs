// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Ads.DataModels.Common;

/// <summary>
/// 腾讯广告 Marketing API v3.0 统一应答信封（<c>{code, message, message_cn, data}</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>与微信系信封的关键差异</b>：v3.0 的错误码字段名是 <c>code</c>（整数）而非 <c>errcode</c>，
/// 且同时返回 <c>message</c>（英文）与 <c>message_cn</c>（中文）两支描述。公用层判错契约
/// <see cref="IWechatApiResponse"/> 只约束「判错面」，故本类把 <c>code</c> 映射到
/// <see cref="ErrorCode"/>、把 <c>message</c> 映射到 <see cref="ErrorMessage"/>，
/// <c>message_cn</c> 作为 v3.0 独有字段单独暴露（<b>不得</b>用它顶替 <see cref="ErrorMessage"/>：
/// 官方 <c>message</c> 才是与错误码对齐的权威描述，<c>message_cn</c> 在多数应答里为空串）。
/// </para>
/// <para>
/// <b>成功判定</b>：<c>code == 0</c>。官方应答示例恒带 <c>"code": 0, "message": "", "message_cn": ""</c>
/// （2026-10-10 逐页核验：<c>oauth/token</c> / <c>oauth/refresh_token</c> 两页应答示例均如此），
/// 与支付线「成功响应整字段缺省」的形态不同 ⇒ 判定用整数比较而非空串判定。
/// </para>
/// <para>
/// <b>HTTP 状态码</b>：本线全部接口带 <c>[AllowAnyStatusCode]</c>，业务失败形态一律落到本信封再判错，
/// 理由见 <c>Mud.Wechat.Ads</c> 侧接口 remarks 与守卫 ADS-B2。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Common")]
public class AdsResponse : IWechatApiResponse
{
    /// <summary>官方错误码（<c>code</c>，整数；<c>0</c> 表示成功）。</summary>
    [JsonPropertyName("code")]
    public virtual int Code { get; set; }

    /// <summary>官方错误描述（<c>message</c>，英文；成功时官方返回空串）。</summary>
    [JsonPropertyName("message")]
    public virtual string? Message { get; set; }

    /// <summary>官方错误描述的中文分支（<c>message_cn</c>；成功时官方返回空串，多数失败场景亦为空串）。</summary>
    [JsonPropertyName("message_cn")]
    public virtual string? MessageCn { get; set; }

    /// <inheritdoc />
    [JsonIgnore]
    public int ErrorCode => Code;

    /// <inheritdoc />
    [JsonIgnore]
    public string? ErrorMessage => Message;

    /// <inheritdoc />
    [JsonIgnore]
    public bool IsSuccess => Code == 0;
}

/// <summary>
/// 带业务载荷的 v3.0 应答信封（<c>data</c> 为对象/数组的端点用本形态）。
/// </summary>
/// <typeparam name="TData">官方 <c>data</c> 字段的形态（逐端点显式声明，不用 <c>object</c> 兜底）。</typeparam>
/// <remarks>
/// <para>
/// <b>为何必须有这一支</b>：v3.0 的业务载荷<b>一律</b>包在 <c>data</c> 里（不像公众号把字段摊平在根上），
/// 若不建泛型基底就得为 35 个端点各写一遍 <c>code/message/message_cn/data</c> 四字段。
/// </para>
/// <para>
/// <b>AOT</b>：源生成上下文登记的是<b>闭合</b>类型（如 <c>AdsAdvertiserGetResponse</c>），
/// 开放泛型 <c>AdsResponse&lt;T&gt;</c> 本身<b>不</b>标 <c>[HttpJsonSerializable]</c> ——
/// 标了会被脚手架登记为 <c>typeof(AdsResponse&lt;&gt;)</c>，而 STJ 源生成对开放泛型不产出元数据
/// （<c>SYSLIB1030</c>，脚手架同时打 <c>AOT002</c> 警告），是一处死登记；与企微线
/// <c>WechatChatbotResponse&lt;&gt;</c> 未登记同一处置，不构成缺陷。
/// <b>由此得出一条建模硬约束</b>：端点应答<b>必须</b>声明自己的闭合类型（派生自本泛型），
/// <b>不得</b>把 <c>AdsResponse&lt;XxxData&gt;</c> 直接作为方法返回类型 —— 后者在 Native AOT 下无元数据。
/// </para>
/// </remarks>
public class AdsResponse<TData> : AdsResponse
{
    /// <summary>业务载荷（官方 <c>data</c>）。部分端点在失败时不返回该字段 ⇒ 可空。</summary>
    [JsonPropertyName("data")]
    public TData? Data { get; set; }
}

/// <summary>
/// 普通分页元信息（v3.0 的 <c>page_info</c>，请求侧 <c>pagination_mode = NORMAL</c> 时返回）。
/// </summary>
/// <remarks>
/// <para>
/// 字段名照官方原文（<c>total_number</c> / <c>total_page</c> 不得驼峰化）。
/// 四字段形态取自 <c>advertiser/get</c>（2026-10-10 逐页核验），其中 <c>page</c> 官方类型标注为
/// <c>number</c>（其余三项标 <c>integer</c>）—— 官方标注不统一，本类型统一按 <see cref="long"/> 承载。
/// </para>
/// <para>
/// <b>本公共类型只覆盖 <c>page_info</c></b>：<c>cursor_page_info</c> 的字段集<b>逐页不同</b>
/// （<c>advertiser/get</c> 为 <c>{page_size, total_number, has_more, cursor}</c>、<c>cursor</c> 为 <c>integer</c>），
/// 故<b>按域分建</b>、不收敛成公共一支 —— 收敛会把某一页的字段名当成全页事实，是最难察觉的一类契约错误。
/// 第一支见 <c>Advertiser/AdsAdvertiserCursorPageInfo</c>，其余各域核验后各建各的。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Common")]
public class AdsPageInfo
{
    /// <summary>当前页码（<c>page</c>）。</summary>
    [JsonPropertyName("page")]
    public long? Page { get; set; }

    /// <summary>每页条数（<c>page_size</c>）。</summary>
    [JsonPropertyName("page_size")]
    public long? PageSize { get; set; }

    /// <summary>总条数（<c>total_number</c>）。</summary>
    [JsonPropertyName("total_number")]
    public long? TotalNumber { get; set; }

    /// <summary>总页数（<c>total_page</c>）。</summary>
    [JsonPropertyName("total_page")]
    public long? TotalPage { get; set; }
}

/// <summary>
/// 批量操作族的逐条结果（v3.0 的 <c>data.list[]</c> 元素公共部分）。
/// </summary>
/// <remarks>
/// <para>
/// 官方形态（2026-10-10 核验 <c>advertiser/update_daily_budget</c>）：批量接口返回
/// <c>data.list[]</c>，<b>每个元素自带 code/message/message_cn</b>
/// —— 即信封字段在批量结果里<b>再次</b>出现，与外层信封是两个判定面：外层 <c>code</c> 只表示
/// 「请求被受理」，<b>逐条</b>成败看元素 <c>code</c>。故判错必须两层都做，见守卫 ADS-B2。
/// </para>
/// <para>派生类补各族的主键字段（如 <c>account_id</c>），字段名照官方原文。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Common")]
public class AdsBatchResultItem
{
    /// <summary>该条操作的结果码（<c>code</c>，<c>0</c> 表示该条成功）。</summary>
    [JsonPropertyName("code")]
    public int? Code { get; set; }

    /// <summary>该条操作的描述（<c>message</c>）。</summary>
    [JsonPropertyName("message")]
    public string? Message { get; set; }

    /// <summary>该条操作的中文描述（<c>message_cn</c>）。</summary>
    [JsonPropertyName("message_cn")]
    public string? MessageCn { get; set; }
}

/// <summary>
/// 过滤条件（v3.0 各 <c>*/get</c> 接口的 <c>filtering</c> 元素，字段名照官方原文）。
/// </summary>
/// <remarks>
/// <para>
/// 官方把 <c>filtering</c> 定义为「数组，元素为 <c>{field, operator, values}</c> 结构」，
/// 但<b>每个端点可用的 <c>field</c> 与 <c>operator</c> 组合各不相同</b>（且逐字段列出），
/// 故本类型只承载「形状」，可选值一律写进各端点接口的 XML remarks 并由守卫 ADS-B2 锁定，
/// <b>不做</b>成本地枚举（官方枚举集会随端点变化，做成公共枚举必然失真）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Common")]
public class AdsFiltering
{
    /// <summary>过滤字段名（<c>field</c>，必填；取值域见各端点 remarks）。</summary>
    [JsonPropertyName("field")]
    public string? Field { get; set; }

    /// <summary>操作符（<c>operator</c>，必填；如 EQUALS / CONTAINS / IN，取值域见各端点 remarks）。</summary>
    [JsonPropertyName("operator")]
    public string? Operator { get; set; }

    /// <summary>过滤值列表（<c>values</c>，必填；长度与字符上限见各端点 remarks）。</summary>
    [JsonPropertyName("values")]
    public List<string>? Values { get; set; }
}
