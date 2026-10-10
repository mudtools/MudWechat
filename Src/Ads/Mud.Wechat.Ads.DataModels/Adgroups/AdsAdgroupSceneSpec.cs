// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相关法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Ads.DataModels.Adgroups;

/// <summary>
/// 场景定向（官方 <c>scene_spec</c>，<c>struct</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>本结构整体在 ADX 程序化投放下不可填写提交</b>（官方对 <c>scene_spec</c> 的原文标注），
/// 且其子字段几乎都带「当且仅当投放 X 流量时可以使用」的版位前提（<c>site_set</c> 决定）⇒
/// 可用分支由版位组合决定，SDK 不做本地版位判定，越界由应答 <c>code</c> 表达。
/// </para>
/// <para>
/// <b>层级校正（照官方渲染页的缩进层级，不照直觉）</b>：官方把
/// <c>official_account_media_category</c> / <c>mini_program_and_mini_game</c> / <c>pay_scene</c>
/// 三支列为 <c>wechat_scene</c> 的子字段，而 <c>wechat_position</c> / <c>mobile_union_category</c> /
/// <c>qbsearch_scene</c> / <c>wechat_channels_scene</c> / <c>pc_scene</c> / <c>wechat_search_scene</c>
/// 六支列为 <c>scene_spec</c> 的<b>直接子字段</b>（与 <c>wechat_scene</c> 平级）。
/// 平面阅读该文档表时极易把后六支误挂进 <c>wechat_scene</c> —— 那样发出的报文结构官方解析不到。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Adgroups")]
public class AdsAdgroupSceneSpec
{
    /// <summary>移动联盟场景定向（官方 <c>mobile_union</c>，<c>enum[]</c>，当且仅当投放移动联盟流量时可用）。</summary>
    [JsonPropertyName("mobile_union")]
    public List<string>? MobileUnion { get; set; }

    /// <summary>移动联盟场景<b>屏蔽</b>定向（官方 <c>exclude_mobile_union</c>，<c>enum[]</c>，当且仅当投放移动联盟流量时可用）。</summary>
    [JsonPropertyName("exclude_mobile_union")]
    public List<string>? ExcludeMobileUnion { get; set; }

    /// <summary>定投联盟流量包列表（官方 <c>union_position_package</c>，<c>integer[]</c>）。官方限制：一个营销单元最多 5 个定投流量包，
    /// 且该营销单元所有流量包关联的营销位数量不得超过 10000 个。</summary>
    [JsonPropertyName("union_position_package")]
    public List<long>? UnionPositionPackage { get; set; }

    /// <summary>屏蔽联盟流量包列表（官方 <c>exclude_union_position_package</c>，<c>integer[]</c>）。官方限制同 <see cref="UnionPositionPackage"/>（条数上限 5、关联营销位上限 10000）。</summary>
    [JsonPropertyName("exclude_union_position_package")]
    public List<long>? ExcludeUnionPositionPackage { get; set; }

    /// <summary>腾讯新闻流量场景定向（官方 <c>tencent_news</c>，<c>enum[]</c>，当且仅当投放腾讯新闻流量时可用，功能灰度开放）。</summary>
    [JsonPropertyName("tencent_news")]
    public List<string>? TencentNews { get; set; }

    /// <summary>展示场景（官方 <c>display_scene</c>，<c>enum[]</c>，当且仅当投放移动联盟流量时可用）。</summary>
    [JsonPropertyName("display_scene")]
    public List<string>? DisplayScene { get; set; }

    /// <summary>微信场景定向（官方 <c>wechat_scene</c>，<c>struct</c>），见 <see cref="AdsWechatScene"/>。</summary>
    [JsonPropertyName("wechat_scene")]
    public AdsWechatScene? WechatScene { get; set; }

    /// <summary>微信公众号与小程序定投场景值（官方 <c>wechat_position</c>，<c>integer[]</c>，
    /// 当且仅当 <c>site_set = SITE_SET_WECHAT</c> 时可用；可用值从 <c>scene_spec_tags/get</c> 获取，官方标注<b>不允许修改</b>）。</summary>
    [JsonPropertyName("wechat_position")]
    public List<long>? WechatPosition { get; set; }

    /// <summary>腾讯营销联盟媒体类型场景定向（官方 <c>mobile_union_category</c>，<c>integer[]</c>，
    /// 当且仅当 <c>site_set = SITE_SET_MOBILE_UNION</c> 时可用）。</summary>
    [JsonPropertyName("mobile_union_category")]
    public List<long>? MobileUnionCategory { get; set; }

    /// <summary>QQ 浏览器、应用宝流量场景（官方 <c>qbsearch_scene</c>，<c>enum[]</c>，当且仅当投放 <c>SITE_SET_QBSEARCH</c> 版位时可用）。</summary>
    [JsonPropertyName("qbsearch_scene")]
    public List<string>? QbsearchScene { get; set; }

    /// <summary>微信视频号定投（官方 <c>wechat_channels_scene</c>，<c>integer[]</c>）。</summary>
    [JsonPropertyName("wechat_channels_scene")]
    public List<long>? WechatChannelsScene { get; set; }

    /// <summary>PC 端定投（官方 <c>pc_scene</c>，<c>enum[]</c>，当且仅当投放 <c>SITE_SET_PCQQ</c> 版位时可用）。</summary>
    [JsonPropertyName("pc_scene")]
    public List<string>? PcScene { get; set; }

    /// <summary>搜一搜流量场景（官方 <c>wechat_search_scene</c>，<c>enum[]</c>，当且仅当投放 <c>SITE_SET_WECHAT</c> 版位时可用）。</summary>
    [JsonPropertyName("wechat_search_scene")]
    public List<string>? WechatSearchScene { get; set; }
}

/// <summary>
/// 微信场景定向（官方 <c>scene_spec.wechat_scene</c>，<c>struct</c>）。
/// 三支字段官方均标 <c>integer[]</c> 且未给出可选值枚举（取值依赖账号开通的场景包）⇒ 不做本地校验。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Adgroups")]
public class AdsWechatScene
{
    /// <summary>公众号媒体类型（官方 <c>official_account_media_category</c>，<c>integer[]</c>）。</summary>
    [JsonPropertyName("official_account_media_category")]
    public List<long>? OfficialAccountMediaCategory { get; set; }

    /// <summary>小程序小游戏流量类型（官方 <c>mini_program_and_mini_game</c>，<c>integer[]</c>）。</summary>
    [JsonPropertyName("mini_program_and_mini_game")]
    public List<long>? MiniProgramAndMiniGame { get; set; }

    /// <summary>订单详情页消费场景（官方 <c>pay_scene</c>，<c>integer[]</c>）。</summary>
    [JsonPropertyName("pay_scene")]
    public List<long>? PayScene { get; set; }
}
