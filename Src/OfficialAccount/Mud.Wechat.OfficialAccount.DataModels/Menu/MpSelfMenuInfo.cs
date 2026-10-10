// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Menu;

/// <summary>
/// 查询自定义菜单信息响应（<c>getCurrentSelfmenuInfo</c>，<c>GET /cgi-bin/get_current_selfmenu_info</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>与 <see cref="MpGetMenuResponse"/> 的本质差异（官方注意事项原文）</b>：本接口<b>不仅能查 API 设置的菜单</b>，
/// 还能查<b>公众平台官网（mp.weixin.qq.com）中设置</b>的菜单；而菜单查询接口只能查到用 API 设置的菜单。
/// 故第三方平台场景用它检测「公众号官网菜单配置」。
/// </para>
/// <para>
/// <b>权限面更宽</b>：官方原文「认证 / 未认证的服务号 / 公众号，以及接口测试号，均拥有该接口权限」
/// （与菜单域其余端点「仅认证」不同）。
/// </para>
/// <para>
/// 官方响应顶层：<c>is_menu_open</c>（<c>0</c> 未开启 / <c>1</c> 开启）与 <c>selfmenu_info</c>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Menu")]
public class MpGetCurrentSelfMenuInfoResponse : MpResponse
{
    /// <summary>菜单是否开启（<c>0</c> 未开启 / <c>1</c> 开启）。</summary>
    [JsonPropertyName("is_menu_open")]
    public int IsMenuOpen { get; set; }

    /// <summary>菜单信息（未开启时可能缺省）。</summary>
    [JsonPropertyName("selfmenu_info")]
    public MpSelfMenuInfo? SelfMenuInfo { get; set; }
}

/// <summary>菜单信息（<c>selfmenu_info</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Menu")]
public class MpSelfMenuInfo
{
    /// <summary>菜单按钮数组。</summary>
    [JsonPropertyName("button")]
    public List<MpSelfMenuButton>? Button { get; set; }
}

/// <summary>
/// 官网 / API 菜单按钮（<c>selfmenu_info.button</c> 元素）。
/// </summary>
/// <remarks>
/// <para>
/// <b>结构差异（勿与创建菜单的 DTO 混用）</b>：本形态的 <c>sub_button</c> 是<b>对象</b>
/// （<c>{ "list": [...] }</c>），而 API 设置/查询菜单的 <c>button[].sub_button</c> 是<b>数组</b>
/// ⇒ 故本域刻意保留两套 DTO（<see cref="MpMenuButton"/> 与 <see cref="MpSelfMenuButton"/>），
/// 合并会静默丢字段。
/// </para>
/// <para>
/// <b>取值随来源而异（官方字段说明原文）</b>：官网设置的菜单用 <c>value</c> 承载
/// （Text 存文字、Img/Voice 存 mediaID、Video 存下载链接、News 存图文到 <c>news_info</c> 并同时把 mediaID 存
/// <c>value</c>、View 存链接到 <c>url</c>）；API 设置的菜单把事件 key 存到 <c>key</c>、链接存到 <c>url</c>。
/// </para>
/// <para>
/// <b>官方字段表缺陷提示</b>：官方该页把 <c>url</c> 与 <c>key</c> 的说明写成与 <c>value</c> 逐字相同
/// （疑似文档复用）；按其语义（API 菜单的链接与事件 key）建模为两个独立字段，勿据字面说明合并。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Menu")]
public class MpSelfMenuButton
{
    /// <summary>菜单类型（官网：<c>text</c>/<c>img</c>/<c>voice</c>/<c>video</c>/<c>news</c>/<c>view</c> 等；API：<c>click</c> 等事件类型）。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>菜单名称。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>菜单值（语义随来源与类型而异，见类型 remarks）。</summary>
    [JsonPropertyName("value")]
    public string? Value { get; set; }

    /// <summary>链接（API 设置的 <c>view</c> 菜单使用）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>事件 key（API 设置的 <c>click</c>/<c>scancode_*</c>/<c>pic_*</c>/<c>location_select</c> 等使用）。</summary>
    [JsonPropertyName("key")]
    public string? Key { get; set; }

    /// <summary>图文消息信息（官网 <c>news</c> 类型菜单使用）。</summary>
    [JsonPropertyName("news_info")]
    public MpSelfMenuNewsInfo? NewsInfo { get; set; }

    /// <summary>二级菜单（本形态为<b>对象</b> <c>{ "list": [...] }</c>，非数组）。</summary>
    [JsonPropertyName("sub_button")]
    public MpSelfMenuButtonList? SubButton { get; set; }
}

/// <summary>二级菜单容器（<c>selfmenu_info.button[].sub_button</c>，形如 <c>{ "list": [...] }</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Menu")]
public class MpSelfMenuButtonList
{
    /// <summary>二级菜单按钮列表。</summary>
    [JsonPropertyName("list")]
    public List<MpSelfMenuButton>? List { get; set; }
}

/// <summary>图文消息容器（<c>news_info</c>，形如 <c>{ "list": [...] }</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Menu")]
public class MpSelfMenuNewsInfo
{
    /// <summary>图文消息列表。</summary>
    [JsonPropertyName("list")]
    public List<MpSelfMenuNewsItem>? List { get; set; }
}

/// <summary>图文消息条目（<c>news_info.list</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Menu")]
public class MpSelfMenuNewsItem
{
    /// <summary>标题。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>摘要。</summary>
    [JsonPropertyName("digest")]
    public string? Digest { get; set; }

    /// <summary>作者。</summary>
    [JsonPropertyName("author")]
    public string? Author { get; set; }

    /// <summary>是否显示封面（<c>0</c> 不显示 / <c>1</c> 显示）。</summary>
    [JsonPropertyName("show_cover")]
    public int ShowCover { get; set; }

    /// <summary>封面图片的 URL。</summary>
    [JsonPropertyName("cover_url")]
    public string? CoverUrl { get; set; }

    /// <summary>正文的 URL。</summary>
    [JsonPropertyName("content_url")]
    public string? ContentUrl { get; set; }

    /// <summary>原文的 URL（若置空则无查看原文入口）。</summary>
    [JsonPropertyName("source_url")]
    public string? SourceUrl { get; set; }
}
