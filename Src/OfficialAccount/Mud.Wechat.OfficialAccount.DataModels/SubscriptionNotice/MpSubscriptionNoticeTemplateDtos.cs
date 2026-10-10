// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.SubscriptionNotice;

/// <summary>
/// 选用模板（<c>wxaapi/newtmpl/addtemplate</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约（逐页核验 2026-10-07）：<c>tid</c>（模板标题 id）/<c>kidList</c>/<c>sceneDesc</c> 均必填。
/// 官方「注意事项」原文：①「模板标题 id 可通过接口获取或后台查看」；②「<b>关键词组合需 2-5 个</b>」；
/// ③「<b>服务场景描述限制 15 字</b>」。
/// </para>
/// <para><c>kidList</c> 为关键词 id 数字列表，顺序可自由搭配（官方示例 [3,5,4]）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "SubscriptionNotice")]
public class MpAddSubscribeTemplateRequest
{
    /// <summary>获取或设置模板标题 id（官方 <c>tid</c>，必填；官方示例为字符串形态 "401"）。</summary>
    [JsonPropertyName("tid")]
    public string Tid { get; set; } = string.Empty;

    /// <summary>获取或设置组合好的模板关键词 id 列表（官方 <c>kidList</c>，数字数组；2~5 个，顺序可自由搭配）。</summary>
    [JsonPropertyName("kidList")]
    public List<int> KidList { get; set; } = new List<int>();

    /// <summary>获取或设置服务场景描述（官方 <c>sceneDesc</c>，15 个字以内）。</summary>
    [JsonPropertyName("sceneDesc")]
    public string SceneDesc { get; set; } = string.Empty;
}

/// <summary>选用模板（<c>wxaapi/newtmpl/addtemplate</c>）响应。</summary>
/// <remarks>官方响应字段为 <c>priTmplId</c>（非 pid/template_id——逐页核验确认）。</remarks>
[HttpJsonSerializable(SerializerClassName = "SubscriptionNotice")]
public class MpAddSubscribeTemplateResponse : MpResponse
{
    /// <summary>获取或设置添加至帐号下的模板 id（官方 <c>priTmplId</c>，发送订阅通知时所需）。</summary>
    [JsonPropertyName("priTmplId")]
    public string? PriTmplId { get; set; }
}

/// <summary>获取已有模板列表（<c>wxaapi/newtmpl/gettemplate</c>）响应。</summary>
/// <remarks>
/// 官方契约（逐页核验 2026-10-07）：<b>GET</b>、无请求体；响应字段为
/// <c>priTmplId</c>/<c>title</c>/<c>content</c>/<c>example</c>/<c>type</c>/<c>keywordEnumValueList</c>
/// （<b>无 tid、无 keywordList</b>——与任务预期不同的三处实质偏差之一，以页面为准）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "SubscriptionNotice")]
public class MpSubscribeTemplateListResponse : MpResponse
{
    /// <summary>获取或设置模板列表（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public List<MpSubscribeTemplate>? Data { get; set; }
}

/// <summary>已有模板条目（官方 <c>data</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "SubscriptionNotice")]
public class MpSubscribeTemplate
{
    /// <summary>获取或设置添加至帐号下的模板 id（官方 <c>priTmplId</c>）。</summary>
    [JsonPropertyName("priTmplId")]
    public string? PriTmplId { get; set; }

    /// <summary>获取或设置模版标题（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置模版内容（官方 <c>content</c>，形如 <c>{{date2.DATA}}</c> 占位形态）。</summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    /// <summary>获取或设置模板内容示例（官方 <c>example</c>）。</summary>
    [JsonPropertyName("example")]
    public string? Example { get; set; }

    /// <summary>获取或设置模版类型（官方 <c>type</c>，2 = 一次性订阅 / 3 = 长期订阅）。</summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>获取或设置枚举参数值范围（官方 <c>keywordEnumValueList</c>，仅枚举型关键词返回）。</summary>
    [JsonPropertyName("keywordEnumValueList")]
    public List<MpSubscribeTemplateKeywordEnum>? KeywordEnumValueList { get; set; }
}

/// <summary>枚举参数值范围条目（官方 <c>keywordEnumValueList</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "SubscriptionNotice")]
public class MpSubscribeTemplateKeywordEnum
{
    /// <summary>获取或设置枚举参数的 key（官方 <c>keywordCode</c>，形如 <c>enum_string2.DATA</c>）。</summary>
    [JsonPropertyName("keywordCode")]
    public string? KeywordCode { get; set; }

    /// <summary>获取或设置枚举参数值范围列表（官方 <c>enumValueList</c>）。</summary>
    [JsonPropertyName("enumValueList")]
    public List<string>? EnumValueList { get; set; }
}

/// <summary>删除模板（<c>wxaapi/newtmpl/deltemplate</c>）请求体。</summary>
/// <remarks>
/// 官方契约（逐页核验 2026-10-07）：请求体字段<b>实际为 <c>priTmplId</c></b>
/// （与选用模板响应同键——无 pid/template_id，以页面为准）；
/// 官方错误码表多行解决方案列为空白（页面原文如此，照录）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "SubscriptionNotice")]
public class MpDeleteSubscribeTemplateRequest
{
    /// <summary>获取或设置要删除的模板 id（官方 <c>priTmplId</c>，必填）。</summary>
    [JsonPropertyName("priTmplId")]
    public string PriTmplId { get; set; } = string.Empty;
}

/// <summary>获取类目（<c>wxaapi/newtmpl/getcategory</c>）响应。</summary>
/// <remarks>
/// 官方契约（逐页核验 2026-10-07）：<b>GET</b>、无请求体；data 元素<b>仅 id/name 两字段</b>
/// （无 type——与任务预期不同，以页面为准）。页面描述「获取小程序、公众号所属类目」系小程序文案复用，照录。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "SubscriptionNotice")]
public class MpSubscribeCategoryListResponse : MpResponse
{
    /// <summary>获取或设置类目列表（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public List<MpSubscribeCategory>? Data { get; set; }
}

/// <summary>类目条目（官方 <c>data</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "SubscriptionNotice")]
public class MpSubscribeCategory
{
    /// <summary>获取或设置类目 id（官方 <c>id</c>，查询公共模板库时需要）。</summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>获取或设置类目的中文名（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}

/// <summary>获取类目下的公共模板（<c>wxaapi/newtmpl/getpubtemplatetitles</c>）响应。</summary>
/// <remarks>
/// <para>
/// 官方契约（逐页核验 2026-10-07）：<b>GET</b>，Query <c>ids</c>（类目 id，多个用逗号隔开）/
/// <c>start</c>（分页，从 0 开始）/<c>limit</c>（单页条数，<b>最大 30</b>）。
/// </para>
/// <para>
/// <b>官方文档矛盾（照录）</b>：<c>categoryId</c> 类型表标 number、返回示例为字符串 "616"
/// ⇒ 按示例字符串形态建模（数字形态反序列化由调用方经原始响应处理——官方示例为可信形态）；
/// 请求示例 URL 中 ids 带字面双引号（官方页面原文如此）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "SubscriptionNotice")]
public class MpSubscribePubTemplateTitlesResponse : MpResponse
{
    /// <summary>获取或设置模版标题列表总数（官方 <c>count</c>）。</summary>
    [JsonPropertyName("count")]
    public int Count { get; set; }

    /// <summary>获取或设置模板标题列表（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public List<MpSubscribePubTemplateTitle>? Data { get; set; }
}

/// <summary>公共模板标题条目（官方 <c>data</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "SubscriptionNotice")]
public class MpSubscribePubTemplateTitle
{
    /// <summary>获取或设置模版标题 id（官方 <c>tid</c>，number 形态——官方示例 99）。</summary>
    [JsonPropertyName("tid")]
    public long Tid { get; set; }

    /// <summary>获取或设置模版标题（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置模版类型（官方 <c>type</c>，2 = 一次性订阅 / 3 = 长期订阅）。</summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>获取或设置模版所属类目 id（官方 <c>categoryId</c>；类型表标 number、示例为字符串 "616"，按示例建模，矛盾照录）。</summary>
    [JsonPropertyName("categoryId")]
    public string? CategoryId { get; set; }
}

/// <summary>获取模板中的关键词（<c>wxaapi/newtmpl/getpubtemplatekeywords</c>）响应。</summary>
/// <remarks>
/// 官方契约（逐页核验 2026-10-07）：<b>GET</b>，Query <c>tid</c>（模板标题 id）；
/// 顶层 <c>count</c> 的官方说明原文为「模版标题列表总数」——系从 titles 接口复制来的文案
/// （本接口返回的是关键词列表），矛盾照录。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "SubscriptionNotice")]
public class MpSubscribePubTemplateKeywordsResponse : MpResponse
{
    /// <summary>获取或设置关键词列表总数（官方 <c>count</c>；官方说明文案与实际语义不符，照录）。</summary>
    [JsonPropertyName("count")]
    public int Count { get; set; }

    /// <summary>获取或设置关键词列表（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public List<MpSubscribePubTemplateKeyword>? Data { get; set; }
}

/// <summary>关键词条目（官方 <c>data</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "SubscriptionNotice")]
public class MpSubscribePubTemplateKeyword
{
    /// <summary>获取或设置关键词 id（官方 <c>kid</c>，选用模板时需要）。</summary>
    [JsonPropertyName("kid")]
    public int Kid { get; set; }

    /// <summary>获取或设置关键词内容（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置关键词内容对应的示例（官方 <c>example</c>）。</summary>
    [JsonPropertyName("example")]
    public string? Example { get; set; }

    /// <summary>获取或设置参数类型（官方 <c>rule</c>，如 thing / time / name）。</summary>
    [JsonPropertyName("rule")]
    public string? Rule { get; set; }
}
