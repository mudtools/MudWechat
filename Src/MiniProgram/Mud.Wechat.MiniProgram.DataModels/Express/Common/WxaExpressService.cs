// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>服务类型（<c>service</c>，顺丰必填，其它快递公司选填）。</summary>
/// <remarks>官方字段以商家寄件 <c>order/add</c> 的「Body.service Object Payload」为准。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressService
{
    /// <summary>服务类型 ID（<c>service_type</c>，必填；取值见各快递公司面单服务类型）。</summary>
    [JsonPropertyName("service_type")]
    public int ServiceType { get; set; }

    /// <summary>服务类型名称（<c>service_name</c>，必填；仅展示，例如“标准快递”）。</summary>
    [JsonPropertyName("service_name")]
    public string? ServiceName { get; set; }
}
