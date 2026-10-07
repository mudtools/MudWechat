// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Template;

/// <summary>
/// 发送模板消息（<c>message/template/send</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约（逐页核验 2026-10-07，页面 2025-11-26 更新版）：
/// <c>touser</c>/<c>template_id</c>/<c>data</c> 必填，<c>url</c>/<c>miniprogram</c>/<c>client_msg_id</c> 可选；
/// url 与 miniprogram 都不填则无跳转，都填优先跳小程序，客户端不支持跳小程序时跳 url。
/// </para>
/// <para>
/// <b>data 仅 value（逐页核验裁决）</b>：当前官方页 <c>data</c> 形态为
/// <c>{"key1": {"value": any}}</c>，<b>无 <c>color</c> 子字段</b>、无 #RRGGBB 约束、
/// 无单字段 200 字符限制表述（历史文档曾有，现页面已不可见——SDK 按核验事实建模 value-only）。
/// data 的 key 须以类型为前缀（如 <c>thing1.DATA</c>/<c>time2.DATA</c>），发送内容须与模板参数一致。
/// </para>
/// <para>
/// <b>client_msg_id 防重入</b>：官方原文「同一 openid + client_msg_id 只发一条，10 分钟有效」。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Template")]
public class MpSendTemplateMessageRequest
{
    /// <summary>获取或设置接收者（用户）的 openid（官方 <c>touser</c>，必填）。</summary>
    [JsonPropertyName("touser")]
    public string ToUser { get; set; } = string.Empty;

    /// <summary>获取或设置所需下发的模板 id（官方 <c>template_id</c>，必填；官方描述沿用「订阅模板 id」措辞，照录）。</summary>
    [JsonPropertyName("template_id")]
    public string TemplateId { get; set; } = string.Empty;

    /// <summary>获取或设置模板跳转链接（官方 <c>url</c>，可选；与 miniprogram 都不填则无跳转）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>获取或设置跳小程序信息（官方 <c>miniprogram</c>，可选）。</summary>
    [JsonPropertyName("miniprogram")]
    public MpTemplateMiniProgram? MiniProgram { get; set; }

    /// <summary>
    /// 获取或设置模板内容（官方 <c>data</c>，必填；key 为模板参数名——类型前缀形态如 <c>thing1.DATA</c>，
    /// 值为 <c>{"value": …}</c> 形态，经 <see cref="MpTemplateDataValue"/> 承载）。
    /// </summary>
    [JsonPropertyName("data")]
    public Dictionary<string, MpTemplateDataValue> Data { get; set; } = new Dictionary<string, MpTemplateDataValue>();

    /// <summary>获取或设置防重入 id（官方 <c>client_msg_id</c>，可选；同一 openid + client_msg_id 只发一条，10 分钟有效）。</summary>
    [JsonPropertyName("client_msg_id")]
    public string? ClientMsgId { get; set; }
}

/// <summary>模板消息跳小程序信息（官方 <c>miniprogram</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Template")]
public class MpTemplateMiniProgram
{
    /// <summary>获取或设置小程序 appid（官方 <c>appid</c>，可选）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>获取或设置小程序页面路径（官方 <c>pagepath</c>，可选）。</summary>
    [JsonPropertyName("pagepath")]
    public string? PagePath { get; set; }
}

/// <summary>模板消息 data 字段值（官方 <c>{"value": …}</c> 形态；逐页核验无 color 字段）。</summary>
[HttpJsonSerializable(SerializerClassName = "Template")]
public class MpTemplateDataValue
{
    /// <summary>获取或设置参数值（官方 <c>value</c>；官方类型标注 any、示例全为字符串）。</summary>
    [JsonPropertyName("value")]
    public string? Value { get; set; }
}

/// <summary>发送模板消息（<c>message/template/send</c>）响应。</summary>
/// <remarks>官方示例：<c>{"errcode":0,"errmsg":"ok","msgid":200228332}</c>；参数错误示例 errcode 47003。</remarks>
[HttpJsonSerializable(SerializerClassName = "Template")]
public class MpSendTemplateMessageResponse : MpResponse
{
    /// <summary>获取或设置消息 id（官方 <c>msgid</c>，小写无下划线，照抄官方）。</summary>
    [JsonPropertyName("msgid")]
    public long MsgId { get; set; }
}

/// <summary>设置所属行业（<c>template/api_set_industry</c>）请求体。</summary>
/// <remarks>
/// <para>
/// 官方契约（逐页核验 2026-10-07）：<b>仅 industry_id1/industry_id2 两个必填字段</b>
/// （行业代码 1~41 的字符串形态，官方示例 <c>"1"</c>/<c>"4"</c>；当前页面无 <c>industry_id3</c>，
/// 与旧版资料的 3 参数形态不同——以核验页面为准）。
/// </para>
/// <para>
/// <b>硬约束（官方原文）</b>：「每月可修改行业 1 次，账号仅可使用所属行业中相关的模板」；
/// 「修改行业后，你在原有行业中的模板将会被删除」。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Template")]
public class MpSetIndustryRequest
{
    /// <summary>获取或设置公众号模板消息所属行业编号（官方 <c>industry_id1</c>，主行业；代码 1~41）。</summary>
    [JsonPropertyName("industry_id1")]
    public string IndustryId1 { get; set; } = string.Empty;

    /// <summary>获取或设置公众号模板消息所属行业编号（官方 <c>industry_id2</c>，副行业；代码 1~41）。</summary>
    [JsonPropertyName("industry_id2")]
    public string IndustryId2 { get; set; } = string.Empty;
}

/// <summary>获取行业信息（<c>template/get_industry</c>）响应。</summary>
/// <remarks>
/// 官方契约（逐页核验 2026-10-07）：<b>GET</b>、无请求体；成功响应不含 errcode（缺省 0）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Template")]
public class MpGetIndustryResponse : MpResponse
{
    /// <summary>获取或设置账号设置的主营行业（官方 <c>primary_industry</c>）。</summary>
    [JsonPropertyName("primary_industry")]
    public MpIndustry? PrimaryIndustry { get; set; }

    /// <summary>获取或设置账号设置的副营行业（官方 <c>secondary_industry</c>）。</summary>
    [JsonPropertyName("secondary_industry")]
    public MpIndustry? SecondaryIndustry { get; set; }
}

/// <summary>行业信息（官方 <c>first_class</c>/<c>second_class</c> 两字段结构）。</summary>
/// <remarks>
/// <b>命名差异（勿互相「对齐」）</b>：本页副营行业键为 <c>secondary_industry</c>，
/// 而模板列表页的二级行业字段名为 <c>deputy_industry</c>——同域两处「二级」命名不一致（官方原文如此）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Template")]
public class MpIndustry
{
    /// <summary>获取或设置一级类目（官方 <c>first_class</c>）。</summary>
    [JsonPropertyName("first_class")]
    public string? FirstClass { get; set; }

    /// <summary>获取或设置二级类目（官方 <c>second_class</c>）。</summary>
    [JsonPropertyName("second_class")]
    public string? SecondClass { get; set; }
}

/// <summary>选用模板（<c>template/api_add_template</c>）请求体。</summary>
/// <remarks>
/// <para>
/// 官方契约（逐页核验 2026-10-07）：<c>template_id_short</c> + <c>keyword_name_list</c> 两字段；
/// 行业模板库编号为 TM**/OPENTMTM** 形态，<b>类目模板为纯数字 id</b>（示例 47123）——
/// 官方将两类形态合并在同一字段集中，无独立分支字段。
/// </para>
/// <para>
/// <b>官方文档矛盾（照录）</b>：<c>keyword_name_list</c> 标注必填，但仅行业模板库（TM** 形态）
/// 无需关键词——行业模板传关键词的行为页面未说明。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Template")]
public class MpAddTemplateRequest
{
    /// <summary>获取或设置模板库中模板的编号（官方 <c>template_id_short</c>，TM**/OPENTMTM**/纯数字形态）。</summary>
    [JsonPropertyName("template_id_short")]
    public string TemplateIdShort { get; set; } = string.Empty;

    /// <summary>获取或设置选用的类目模板关键词（官方 <c>keyword_name_list</c>，按顺序传入；空或不在模板库返回 40246）。</summary>
    [JsonPropertyName("keyword_name_list")]
    public List<string>? KeywordNameList { get; set; }
}

/// <summary>选用模板（<c>template/api_add_template</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Template")]
public class MpAddTemplateResponse : MpResponse
{
    /// <summary>获取或设置模板 ID（官方 <c>template_id</c>）。</summary>
    [JsonPropertyName("template_id")]
    public string? TemplateId { get; set; }
}

/// <summary>获取已选用模板列表（<c>template/get_all_private_template</c>）响应。</summary>
/// <remarks>
/// 官方契约（逐页核验 2026-10-07）：<b>GET</b>、无请求体（请求示例 <c>{}</c>）；
/// 响应元素二级行业字段名为 <c>deputy_industry</c>（非 secondary_industry，照抄官方）。
/// 官方指南页另有「每个账号可同时使用 25 个模板」规则（不在 API 页）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Template")]
public class MpGetAllPrivateTemplateResponse : MpResponse
{
    /// <summary>获取或设置已选用模板列表（官方 <c>template_list</c>）。</summary>
    [JsonPropertyName("template_list")]
    public List<MpPrivateTemplate>? TemplateList { get; set; }
}

/// <summary>已选用模板条目（官方 <c>template_list</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Template")]
public class MpPrivateTemplate
{
    /// <summary>获取或设置模板 ID（官方 <c>template_id</c>）。</summary>
    [JsonPropertyName("template_id")]
    public string? TemplateId { get; set; }

    /// <summary>获取或设置模板标题（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置模板所属行业的一级行业（官方 <c>primary_industry</c>）。</summary>
    [JsonPropertyName("primary_industry")]
    public string? PrimaryIndustry { get; set; }

    /// <summary>获取或设置模板所属行业的二级行业（官方 <c>deputy_industry</c>——注意与行业查询页的 secondary_industry 命名不同）。</summary>
    [JsonPropertyName("deputy_industry")]
    public string? DeputyIndustry { get; set; }

    /// <summary>获取或设置模板内容（官方 <c>content</c>，形如 <c>{{result.DATA}}</c> 的参数占位形态）。</summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    /// <summary>获取或设置模板示例（官方 <c>example</c>）。</summary>
    [JsonPropertyName("example")]
    public string? Example { get; set; }
}

/// <summary>删除模板（<c>template/del_private_template</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Template")]
public class MpDeleteTemplateRequest
{
    /// <summary>获取或设置公众账号下模板消息 ID（官方 <c>template_id</c>，必填；包括类目模板 ID）。</summary>
    [JsonPropertyName("template_id")]
    public string TemplateId { get; set; } = string.Empty;
}

/// <summary>查询拦截的模板消息（<c>wxa/sec/queryblocktmplmsg</c>）请求体。</summary>
/// <remarks>
/// 官方契约（逐页核验 2026-10-07）：<c>tmpl_msg_id</c>/<c>largest_id</c>/<c>limit</c> 三字段均必填；
/// <c>largest_id</c> 为上一页查询结果最大的 id（翻页游标，第一次传 0）；<c>limit</c> 单页最大 <b>100</b>。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Template")]
public class MpQueryBlockTmplMsgRequest
{
    /// <summary>获取或设置被拦截的模板消息 id（官方 <c>tmpl_msg_id</c>，必填）。</summary>
    [JsonPropertyName("tmpl_msg_id")]
    public string TmplMsgId { get; set; } = string.Empty;

    /// <summary>获取或设置翻页游标（官方 <c>largest_id</c>，上一页查询结果最大的 id；第一次传 0）。</summary>
    [JsonPropertyName("largest_id")]
    public long LargestId { get; set; }

    /// <summary>获取或设置单页查询大小（官方 <c>limit</c>，最大 100）。</summary>
    [JsonPropertyName("limit")]
    public int Limit { get; set; }
}

/// <summary>查询拦截的模板消息（<c>wxa/sec/queryblocktmplmsg</c>）响应。</summary>
/// <remarks>
/// <para>
/// <b>官方文档矛盾（照录，逐页核验 2026-10-07）</b>：①字段表称 <c>msginfo.id</c> 类型为 string，
/// 示例 JSON 中 <c>"id": 2</c> 是数字，且请求体 <c>largest_id</c> 为 number——类型标注与示例互相矛盾
/// ⇒ SDK 按请求体 number 形态把 id 建模为 string（容纳两种形态的读取语义由调用方处理，
/// 反序列化按官方示例的宽松形态）。②接口语义为分页查询（largest_id 翻页 + limit 单页大小），
/// 但 <c>msginfo</c> 标注为单个 object 且示例只有单条记录，与「单页查询最大 100 条」语义不一致，
/// 疑似官方遗漏数组形态——SDK 照官方字段表建模为单对象。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Template")]
public class MpQueryBlockTmplMsgResponse : MpResponse
{
    /// <summary>获取或设置消息信息（官方 <c>msginfo</c>；官方文档形态矛盾照录，见类型 remarks）。</summary>
    [JsonPropertyName("msginfo")]
    public MpBlockedTmplMsgInfo? MsgInfo { get; set; }
}

/// <summary>被拦截的模板消息信息（官方 <c>msginfo</c> 内字段）。</summary>
[HttpJsonSerializable(SerializerClassName = "Template")]
public class MpBlockedTmplMsgInfo
{
    /// <summary>获取或设置记录唯一 ID（官方 <c>id</c>，用于翻页 largest_id；官方类型标注 string、示例为数字，矛盾照录）。</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>获取或设置被拦截的模板消息 id（官方 <c>tmpl_msg_id</c>）。</summary>
    [JsonPropertyName("tmpl_msg_id")]
    public string? TmplMsgId { get; set; }

    /// <summary>获取或设置模板消息的标题（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置模板消息的内容（官方 <c>content</c>）。</summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    /// <summary>获取或设置下发的时间戳（官方 <c>send_timestamp</c>）。</summary>
    [JsonPropertyName("send_timestamp")]
    public long? SendTimestamp { get; set; }

    /// <summary>获取或设置下发目标用户的 openid（官方 <c>openid</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }
}
