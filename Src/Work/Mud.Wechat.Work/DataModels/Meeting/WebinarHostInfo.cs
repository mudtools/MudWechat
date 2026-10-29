// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 网络研讨会主持人对象（创建/修改网络研讨会请求与获取网络研讨会详情响应 <c>hosts</c> 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class WebinarHostInfo
{
    /// <summary>获取或设置主持人 userid（修改时传入会覆盖原有设置；默认为网络研讨会管理员 admin_userid）。</summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }
}
