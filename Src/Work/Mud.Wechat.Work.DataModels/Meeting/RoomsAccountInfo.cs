// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// Rooms 会议室账号信息对象（获取 Rooms 会议室详情响应 <c>account_info</c> 嵌套对象）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class RoomsAccountInfo
{
    /// <summary>获取或设置账号类型：0 - 普通；1 - 专款；2 - 试用。</summary>
    [JsonPropertyName("account_type")]
    public int? AccountType { get; set; }

    /// <summary>获取或设置有效期限（普通账号有效期限即企业账号的有效期限，固定返回「-」）。</summary>
    [JsonPropertyName("valid_period")]
    public string? ValidPeriod { get; set; }
}
