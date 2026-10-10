// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.QrcodeJump;

/// <summary>
/// 获取已设置的二维码规则（<c>POST /cgi-bin/wxopen/qrcodejumpget</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// <b>本组参数仅「扫服务号二维码打开小程序」场景需要</b>（官方原文：获取「扫普通二维码打开小程序」规则时
/// <b>无需任何入参</b>）——但官方字段表把这几个参数标为「必填」，两处矛盾已在官方页并存（照录，
/// SDK 以可空超集承载，由调用方按场景携带）。
/// </para>
/// <para>
/// 官方「服务号调用说明」原文：服务商调用本接口须先获得服务号授权<b>权限集 id 为 3</b> 的权限集，
/// 否则返回 <c>61007</c>；服务号须<b>先关联小程序</b>才可调用（关联入口：<c>linkMiniprogram</c> 接口，
/// 或公众号管理后台「广告与服务 — 小程序管理」）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "QrcodeJump")]
public class MpQrcodeJumpGetRequest
{
    /// <summary>获取或设置小程序的 appid（官方 <c>appid</c>；仅获取「扫服务号二维码打开小程序」规则时需传）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>获取或设置查询类型（官方 <c>get_type</c>，默认 <c>0</c>；取值见 <c>MpQrcodeJumpGetTypes</c>）。
    /// 属性名为 <c>QueryType</c>：避免与 <see cref="object.GetType()"/> 同名（CS0108）。</summary>
    [JsonPropertyName("get_type")]
    public int? QueryType { get; set; }

    /// <summary>获取或设置前缀列表（官方 <c>prefix_list</c>，<c>get_type = 1</c> 时必传，<b>最多 200 个</b>）。</summary>
    [JsonPropertyName("prefix_list")]
    public List<string>? PrefixList { get; set; }

    /// <summary>获取或设置页码（官方 <c>page_num</c>，<c>get_type = 2</c> 时必传，从 1 开始）。</summary>
    [JsonPropertyName("page_num")]
    public int? PageNumber { get; set; }

    /// <summary>获取或设置每页数量（官方 <c>page_size</c>，<c>get_type = 2</c> 时必传，<b>最大 200</b>）。</summary>
    [JsonPropertyName("page_size")]
    public int? PageSize { get; set; }
}

/// <summary>
/// 获取已设置的二维码规则（<c>POST /cgi-bin/wxopen/qrcodejumpget</c>）响应。
/// </summary>
/// <remarks>
/// <para>
/// 官方字段表（逐页核验 2026-10-07）：<c>rule_list</c>（二维码规则详情列表）/
/// <c>qrcodejump_open</c>（是否已打开二维码跳转链接设置）/ <c>list_size</c>（二维码规则数量）/
/// <c>qrcodejump_pub_quota</c>（<b>本月还可发布的次数</b>）/ <c>total_count</c>（规则总数据量，用于分页查询）。
/// </para>
/// <para>
/// <b>官方文档缺陷（照录）</b>：返回示例中出现 <c>permit_sub_rule</c> 字段，但返回参数表未收录、未定义
/// ——SDK 不建模该字段（示例字段无字段表形态说明）。
/// </para>
/// <para>官方错误码：<c>-1</c> / <c>0</c> / <c>40001</c> / <c>40097</c> / <c>40166</c> / <c>44990</c> / <c>85075</c> / <c>886001</c>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "QrcodeJump")]
public class MpQrcodeJumpGetResponse : MpResponse
{
    /// <summary>获取或设置二维码规则详情列表（官方 <c>rule_list</c>）。</summary>
    [JsonPropertyName("rule_list")]
    public List<MpQrcodeJumpRule>? RuleList { get; set; }

    /// <summary>获取或设置是否已打开二维码跳转链接设置（官方 <c>qrcodejump_open</c>）。</summary>
    [JsonPropertyName("qrcodejump_open")]
    public int? QrcodeJumpOpen { get; set; }

    /// <summary>获取或设置二维码规则数量（官方 <c>list_size</c>）。</summary>
    [JsonPropertyName("list_size")]
    public int? ListSize { get; set; }

    /// <summary>获取或设置本月还可发布的次数（官方 <c>qrcodejump_pub_quota</c>）。</summary>
    [JsonPropertyName("qrcodejump_pub_quota")]
    public int? QrcodeJumpPublishQuota { get; set; }

    /// <summary>获取或设置二维码规则总数据量（官方 <c>total_count</c>，用于分页查询）。</summary>
    [JsonPropertyName("total_count")]
    public int? TotalCount { get; set; }
}

/// <summary>
/// 二维码规则条目（官方 <c>rule_list[]</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>条件返回（官方原文）</b>：<c>open_version</c> 与 <c>debug_url</c>
/// <b>仅「扫普通二维码打开小程序」场景返回</b>；服务号场景不返回 ⇒ 两者可空。
/// </para>
/// <para>
/// <b>官方文档矛盾（照录）</b>：<c>prefix</c> 字段说明写「服务号二维码规则已过滤不展示」，
/// 但服务号返回示例中实际返回了 <c>prefix</c> ⇒ SDK 保留该字段。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "QrcodeJump")]
public class MpQrcodeJumpRule
{
    /// <summary>获取或设置二维码规则（官方 <c>prefix</c>；普通二维码为规则前缀，服务号为带参二维码 url）。</summary>
    [JsonPropertyName("prefix")]
    public string? Prefix { get; set; }

    /// <summary>获取或设置小程序功能页面（官方 <c>path</c>）。</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

    /// <summary>获取或设置发布标志位（官方 <c>state</c>，<c>1</c> = 未发布 / <c>2</c> = 已发布；见 <c>MpQrcodeJumpStates</c>）。</summary>
    [JsonPropertyName("state")]
    public int? State { get; set; }

    /// <summary>获取或设置测试范围（官方 <c>open_version</c>；<b>仅普通二维码场景返回</b>，取值见 <c>MpQrcodeJumpOpenVersions</c>）。</summary>
    [JsonPropertyName("open_version")]
    public int? OpenVersion { get; set; }

    /// <summary>获取或设置测试链接（官方 <c>debug_url</c>，<b>最多 5 个</b>；<b>仅普通二维码场景返回</b>）。</summary>
    [JsonPropertyName("debug_url")]
    public List<string>? DebugUrls { get; set; }
}

/// <summary>
/// 增加或修改二维码规则（<c>POST /cgi-bin/wxopen/qrcodejumpadd</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// <b>场景分流（官方原文，「扫普通二维码」与「扫服务号二维码」两族参数）</b>：
/// </para>
/// <list type="bullet">
/// <item><b>两场景都需要</b>：<c>prefix</c> / <c>path</c> / <c>is_edit</c>。</item>
/// <item><b>仅「扫服务号二维码」</b>：<c>appid</c>（跳转的小程序 appid）。</item>
/// <item><b>仅「扫普通二维码」</b>：<c>open_version</c> / <c>debug_url</c> / <c>permit_sub_rule</c>。</item>
/// </list>
/// <para>
/// <b>官方文档矛盾（照录）</b>：字段表把 <c>appid</c>/<c>open_version</c>/<c>permit_sub_rule</c> 均标「必填」，
/// 但说明文字限定为上述场景专属，且各场景示例均未携带对方参数 ⇒ SDK 以可空超集承载，
/// 由调用方按场景携带（不本地校验）。
/// </para>
/// <para>
/// 官方原文：<b>「已经发布的规则，不支持修改」</b>（<c>is_edit = 1</c> 仅可改未发布规则）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "QrcodeJump")]
public class MpQrcodeJumpAddRequest
{
    /// <summary>获取或设置二维码规则（官方 <c>prefix</c>，必填；如 <c>http://weixin.qq.com/q/kZgfwMTm72Wxxxx</c>）。</summary>
    [JsonPropertyName("prefix")]
    public string Prefix { get; set; } = string.Empty;

    /// <summary>获取或设置小程序 appid（官方 <c>appid</c>；仅「扫服务号二维码」场景需传）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>获取或设置小程序功能页面（官方 <c>path</c>，必填；如 <c>pages/index/index</c>）。</summary>
    [JsonPropertyName("path")]
    public string Path { get; set; } = string.Empty;

    /// <summary>获取或设置编辑标志位（官方 <c>is_edit</c>，必填；<c>0</c> 新增 / <c>1</c> 修改已有规则，见 <c>MpQrcodeJumpEditFlags</c>）。</summary>
    [JsonPropertyName("is_edit")]
    public int IsEdit { get; set; }

    /// <summary>获取或设置测试范围（官方 <c>open_version</c>；仅「扫普通二维码」场景需传，取值见 <c>MpQrcodeJumpOpenVersions</c>）。</summary>
    [JsonPropertyName("open_version")]
    public int? OpenVersion { get; set; }

    /// <summary>获取或设置测试链接（官方 <c>debug_url</c>，<b>至多 5 个</b>；仅「扫普通二维码」场景需传）。</summary>
    [JsonPropertyName("debug_url")]
    public List<string>? DebugUrls { get; set; }

    /// <summary>获取或设置是否独占子规则（官方 <c>permit_sub_rule</c>；仅「扫普通二维码」场景需传，
    /// <c>1</c> = 不占用 / <c>2</c> = 占用，见 <c>MpQrcodeJumpSubRuleModes</c>）。</summary>
    [JsonPropertyName("permit_sub_rule")]
    public int? PermitSubRule { get; set; }
}

/// <summary>
/// 发布已设置的二维码规则（<c>POST /cgi-bin/wxopen/qrcodejumppublish</c>）请求体。
/// </summary>
/// <remarks>
/// 官方契约：需先调用 <c>qrcodejumpadd</c> 添加规则，再调用本接口发布生效；
/// 发布后用户扫码命中该规则即跳转到<b>正式版</b>小程序指定页面。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "QrcodeJump")]
public class MpQrcodeJumpPublishRequest
{
    /// <summary>获取或设置二维码规则（官方 <c>prefix</c>，必填）；服务号场景为服务号的带参二维码 url。</summary>
    [JsonPropertyName("prefix")]
    public string Prefix { get; set; } = string.Empty;
}

/// <summary>
/// 删除已设置的二维码规则（<c>POST /cgi-bin/wxopen/qrcodejumpdelete</c>）请求体。
/// </summary>
/// <remarks>官方契约：<c>prefix</c> 必填；<c>appid</c> 为删除「扫服务号二维码打开小程序」规则时的必传项。</remarks>
[HttpJsonSerializable(SerializerClassName = "QrcodeJump")]
public class MpQrcodeJumpDeleteRequest
{
    /// <summary>获取或设置二维码规则（官方 <c>prefix</c>，必填）。</summary>
    [JsonPropertyName("prefix")]
    public string Prefix { get; set; } = string.Empty;

    /// <summary>获取或设置小程序 appid（官方 <c>appid</c>；删除「扫服务号二维码」规则时需传）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }
}
