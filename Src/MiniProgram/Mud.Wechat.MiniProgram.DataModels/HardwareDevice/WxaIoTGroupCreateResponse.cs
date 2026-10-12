// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.HardwareDevice;

/// <summary>创建设备组应答（<c>POST /wxa/business/group/createid</c>）。</summary>
/// <remarks>官方文档：<c>hardware-device/api_createiotgroupid.html</c>；便于用户一次性订阅多个设备。</remarks>
[HttpJsonSerializable(SerializerClassName = "HardwareDevice")]
public class WxaIoTGroupCreateResponse : WxaResponse
{
    /// <summary>设备组 ID（<c>group_id</c>）。</summary>
    [JsonPropertyName("group_id")]
    public string? GroupId { get; set; }

    /// <summary>设备组名称（<c>group_name</c>）。</summary>
    [JsonPropertyName("group_name")]
    public string? GroupName { get; set; }
}
