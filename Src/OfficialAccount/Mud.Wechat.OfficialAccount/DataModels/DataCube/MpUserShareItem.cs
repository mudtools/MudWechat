// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.DataCube;

/// <summary>
/// 图文转发概况数据条目（官方 <c>getusershare</c> 的 list 元素；分时端点 <c>getusersharehour</c> 多 ref_hour
/// ⇒ 超集共用；官方声明两接口已停止维护）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpUserShareItem
{
    /// <summary>获取或设置数据的日期（官方 <c>ref_date</c>）。</summary>
    [JsonPropertyName("ref_date")]
    public string? RefDate { get; set; }

    /// <summary>获取或设置数据小时（官方 <c>ref_hour</c>；<b>仅分时端点返回</b>）。</summary>
    [JsonPropertyName("ref_hour")]
    public int? RefHour { get; set; }

    /// <summary>获取或设置分享的人数（官方 <c>share_user</c>）。</summary>
    [JsonPropertyName("share_user")]
    public int? ShareUser { get; set; }

    /// <summary>获取或设置分享的次数（官方 <c>share_count</c>）。</summary>
    [JsonPropertyName("share_count")]
    public int? ShareCount { get; set; }

    /// <summary>获取或设置分享的场景（官方 <c>share_scene</c>：1 好友转发 / 2 朋友圈 / 255 其他）。</summary>
    [JsonPropertyName("share_scene")]
    public int? ShareScene { get; set; }
}
