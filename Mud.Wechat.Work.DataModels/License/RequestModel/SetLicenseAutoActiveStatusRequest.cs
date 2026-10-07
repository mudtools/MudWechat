// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.License;

/// <summary>
/// 设置企业的许可自动激活状态请求体（<c>/cgi-bin/license/set_auto_active_status</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "License")]
public class SetLicenseAutoActiveStatusRequest
{
    /// <summary>
    /// 获取或设置企业 corpid（官方必填）。
    /// <para>官方约束：要求服务商为企业购买过接口许可，购买指支付完成，购买并退款成功包括在内。</para>
    /// </summary>
    [JsonPropertyName("corpid")]
    public string Corpid { get; set; } = string.Empty;

    /// <summary>获取或设置许可自动激活状态（官方必填）：<c>0</c>-关闭，<c>1</c>-打开。</summary>
    [JsonPropertyName("auto_active_status")]
    public int AutoActiveStatus { get; set; }
}
