// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Category;

/// <summary>发货方式类目规则（官方 <c>list</c> 数组元素，<c>getcategoryrule</c> 发货方式语义）。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategoryDeliverRule
{
    /// <summary>获取或设置发货方式（官方 <c>deliver_type</c>，见 <see cref="ChannelsCategoryDeliverTypes"/>）。</summary>
    [JsonPropertyName("deliver_type")]
    public int? DeliverType { get; set; }

    /// <summary>获取或设置支持的账号类型列表（官方 <c>acct_type</c>，官方类型 numarray，见 <see cref="ChannelsCategoryAcctTypes"/>）。</summary>
    [JsonPropertyName("acct_type")]
    public List<int>? AcctType { get; set; }
}
