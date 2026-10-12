// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.HardwareDevice;

/// <summary>激活设备 license 请求体（<c>POST /wxa/business/license/activedevice</c>）。</summary>
/// <remarks>
/// 官方文档：<c>hardware-device/api_activelicensedevice.html</c>。
/// 批量绑定设备并消耗资源包中的激活码序号（<c>sn_list</c> 长度须与资源包剩余激活数匹配，否则报错）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "HardwareDevice")]
public class WxaLicenseActivateRequest
{
    /// <summary>资源包 ID（<c>pkg_id</c>，必填）。</summary>
    [JsonPropertyName("pkg_id")]
    public string? PkgId { get; set; }

    /// <summary>待激活设备序列号列表（<c>sn_list</c>，必填）。</summary>
    [JsonPropertyName("sn_list")]
    public List<string>? SnList { get; set; }
}
