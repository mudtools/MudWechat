// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 收集表时间题的设置（官方 <c>time_setting</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class WedocFormTimeSetting
{
    /// <summary>
    /// 获取或设置时间格式（官方 <c>time_format_type</c>）。
    /// 官方取值：<c>0</c> 时分、<c>1</c> 时分秒。
    /// </summary>
    [JsonPropertyName("time_format_type")]
    public uint? TimeFormatType { get; set; }
}
