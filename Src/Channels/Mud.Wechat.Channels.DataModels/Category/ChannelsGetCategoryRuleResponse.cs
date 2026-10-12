// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Category;

/// <summary>
/// 获取类目规则（<c>getcategoryrule</c>）响应——官方双语义同路由（<c>rule_id=1</c> 保证金 / <c>rule_id=2</c> 发货方式）
/// 响应形态不同，以可空超集合并承载：按请求 <c>rule_id</c> 取对应字段。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsGetCategoryRuleResponse : ChannelsResponse
{
    /// <summary>获取或设置保证金（官方 <c>deposit</c>，单位：分；仅 <c>rule_id=1</c> 保证金语义返回）。</summary>
    [JsonPropertyName("deposit")]
    public long? Deposit { get; set; }

    /// <summary>获取或设置发货方式规则列表（官方 <c>list</c>；仅 <c>rule_id=2</c> 发货方式语义返回）。</summary>
    [JsonPropertyName("list")]
    public List<ChannelsCategoryDeliverRule>? List { get; set; }
}
