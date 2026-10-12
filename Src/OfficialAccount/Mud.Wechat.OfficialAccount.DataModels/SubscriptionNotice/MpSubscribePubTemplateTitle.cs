// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.SubscriptionNotice;

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
