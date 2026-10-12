// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Menu;

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
