// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Media;

/// <summary>
/// 永久素材列表条目（官方 <c>item</c> 元素；news 与 image/voice/video 两形态的字段超集 DTO）。
/// </summary>
/// <remarks>
/// <para>
/// <b>两形态（共用裁决，官方同字段表声明）</b>：news 类型条目为
/// <c>{media_id, content.news_item[], update_time}</c>；image/voice/video 类型条目为
/// <c>{media_id, name, update_time, url}</c>。字段集不同 ⇒ 超集建模（未携带者为 null），
/// 由守卫锁定而非拆两个响应 DTO（同响应 <c>item</c> 数组的元素类型必须唯一）。
/// </para>
/// <para>官方「注意事项」：「临时素材无法通过本接口获取」。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Media")]
public class MpMaterialListItem
{
    /// <summary>获取或设置素材 media_id（官方 <c>media_id</c>）。</summary>
    [JsonPropertyName("media_id")]
    public string? MediaId { get; set; }

    /// <summary>获取或设置更新时间（官方 <c>update_time</c>，秒级 Unix 时间戳）。</summary>
    [JsonPropertyName("update_time")]
    public long UpdateTime { get; set; }

    /// <summary>获取或设置素材名称（官方 <c>name</c>；<b>仅 image/voice/video 形态返回</b>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置素材 URL（官方 <c>url</c>；<b>仅 image/voice/video 形态返回</b>）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>获取或设置图文内容容器（官方 <c>content</c>；<b>仅 news 形态返回</b>）。</summary>
    [JsonPropertyName("content")]
    public MpMaterialNewsContent? Content { get; set; }
}
