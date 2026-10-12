// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.HomePage;

/// <summary>
/// 撤销申请请求体（背景图 <c>background/apply/cancel</c> 与精选展示位 <c>banner/apply/cancel</c> 请求体字段集一致 ⇒ 共用）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "HomePage")]
public class ChannelsHomepageApplyIdRequest
{
    /// <summary>获取或设置申请编号（官方 <c>apply_id</c>，必填；仅能撤销流程中的申请）。</summary>
    [JsonPropertyName("apply_id")]
    public long ApplyId { get; set; }
}
