// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Vip;

/// <summary>用户积分信息（官方 <c>info</c> 对象，<c>score/get</c>）。</summary>
/// <remarks><b>官方类型照录</b>：<c>score</c> 官方参数表类型为 string（示例 <c>"100"</c>）——与流水中的
/// number 型积分变动值不一致，照录建模。</remarks>
[HttpJsonSerializable(SerializerClassName = "Vip")]
public class ChannelsVipUserScoreInfo
{
    /// <summary>获取或设置用户当前积分余额（官方 <c>score</c>，官方类型为 string）。</summary>
    [JsonPropertyName("score")]
    public string? Score { get; set; }
}
