// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.DeviceData;

/// <summary>
/// 获取温度检测数据响应体（<c>/cgi-bin/devicedata/get_temperature_data</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DeviceData")]
public class DeviceDataTemperatureDataResponse
{
    /// <summary>获取或设置温度检测数据列表容器（官方 temperature_data）。</summary>
    [JsonPropertyName("temperature_data")]
    public DeviceDataTemperatureRecordList? TemperatureDataList { get; set; }

    /// <summary>获取或设置翻页标记（官方 next_cursor，非空时以其作为下次请求的 cursor 继续取）。</summary>
    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; set; }
}
