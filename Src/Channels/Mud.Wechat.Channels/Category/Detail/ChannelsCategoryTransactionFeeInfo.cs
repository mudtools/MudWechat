// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Category;

/// <summary>交易费信息（官方 <c>transactionfee_info</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategoryTransactionFeeInfo
{
    /// <summary>获取或设置类目实收交易佣金比例（官方 <c>basis_point</c>，单位：万分比）。</summary>
    [JsonPropertyName("basis_point")]
    public long? BasisPoint { get; set; }

    /// <summary>获取或设置类目原始佣金比例（官方 <c>original_basis_point</c>，单位：万分比）。</summary>
    [JsonPropertyName("original_basis_point")]
    public long? OriginalBasisPoint { get; set; }

    /// <summary>获取或设置佣金激励类型（官方 <c>incentive_type</c>，见 <see cref="ChannelsCategoryIncentiveTypes"/>）。</summary>
    [JsonPropertyName("incentive_type")]
    public int? IncentiveType { get; set; }
}
