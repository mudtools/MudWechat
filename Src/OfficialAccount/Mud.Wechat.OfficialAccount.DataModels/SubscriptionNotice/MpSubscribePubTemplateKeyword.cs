// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.SubscriptionNotice;

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
