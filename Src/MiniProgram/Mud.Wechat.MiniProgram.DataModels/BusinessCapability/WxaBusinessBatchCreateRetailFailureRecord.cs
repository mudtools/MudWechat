// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.BusinessCapability;

/// <summary>
/// 批量预录入门店信息的单条失败记录。
/// </summary>
/// <remarks>
/// <para><b><c>failure_code</c> 官方只给出数值 2~7</b>，未随页发布逐值语义 ⇒ SDK 只建模 <c>int</c>，不臆造常量。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "BusinessCapability")]
public class WxaBusinessBatchCreateRetailFailureRecord
{
    /// <summary>失败记录对应的门店联系人手机号（敏感）。</summary>
    [JsonPropertyName("mobile_phone")]
    public string? MobilePhone { get; set; }

    /// <summary>失败记录对应的统一社会信用代码（敏感）。</summary>
    [JsonPropertyName("registration_number")]
    public string? RegistrationNumber { get; set; }

    /// <summary>失败码（官方值域 2~7，未发布逐值语义）。</summary>
    [JsonPropertyName("failure_code")]
    public int? FailureCode { get; set; }
}
