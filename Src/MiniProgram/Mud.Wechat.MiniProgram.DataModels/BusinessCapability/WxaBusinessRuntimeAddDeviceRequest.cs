// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.BusinessCapability;

/// <summary>
/// 注册 WMPF 设备 ID 请求体（<c>POST /wxa/business/runtime/adddevice</c>）。
/// </summary>
/// <remarks>
/// <para><b>令牌归属</b>：必须使用「移动应用（LauncherAPP）」身份获取的 <c>access_token</c>，
/// 不能使用小程序自身令牌（官方原文）。</para>
/// <para><b>错误码</b>：<c>50093</c> 数据异常 / <c>50097</c> 无权限 / <c>50098</c> 设备超限 /
/// <c>9402101</c> 产品不存在。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "BusinessCapability")]
public class WxaBusinessRuntimeAddDeviceRequest
{
    /// <summary>硬件产品 ID（必填）。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }

    /// <summary>设备 ID 列表（必填）。</summary>
    [JsonPropertyName("device_id_list")]
    public List<string>? DeviceIdList { get; set; }

    /// <summary>设备型号名称（必填）。</summary>
    [JsonPropertyName("model_name")]
    public string? ModelName { get; set; }
}
