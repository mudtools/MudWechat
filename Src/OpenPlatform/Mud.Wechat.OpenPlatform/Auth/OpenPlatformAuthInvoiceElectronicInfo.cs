// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.Auth;

/// <summary>
/// 电子发票信息（<c>auth_data.invoice_info.electronic</c>，<c>invoice_type = 2</c> 时使用）。
/// 官方注明 2024-05-15 起不再支持纸质增值税专票，统一为数电类型。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/weapp-wxverify/secwxaapi_wxaauth.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Auth")]
public class OpenPlatformAuthInvoiceElectronicInfo
{
    /// <summary>电子发票标识（官方 <c>id</c>，必填）。</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>备注说明（官方 <c>desc</c>，选填）。</summary>
    [JsonPropertyName("desc")]
    public string? Desc { get; set; }
}
