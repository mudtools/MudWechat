// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.QrCodeLink;

/// <summary>生成 NFC scheme 请求体（<c>POST /wxa/generatenfcscheme</c>）。</summary>
/// <remarks>
/// <para>官方文档：<c>qrcode-link/url-scheme/api_generatenfcscheme.html</c>。</para>
/// <para>
/// <b>NFC 场景（官方原文）</b>：适用于 NFC 拉起小程序的业务场景（如智能硬件碰一碰直达页面）；
/// <see cref="Sn"/>（设备序列号）与 <see cref="ModelId"/>（设备型号 ID）为<b> NFC 专属必填参数</b>，
/// 其余字段与 <see cref="WxaSchemeRequest"/> 同形但<b>不可等价互换</b>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "QrCodeLink")]
public class WxaNfcSchemeRequest
{
    /// <summary>设备序列号（<c>sn</c>，必填）。</summary>
    [JsonPropertyName("sn")]
    public string? Sn { get; set; }

    /// <summary>设备型号 ID（<c>model_id</c>，必填）。</summary>
    [JsonPropertyName("model_id")]
    public string? ModelId { get; set; }

    /// <summary>跳转小程序配置（<c>jump_wxa</c>，必填），见 <see cref="WxaJumpInfo"/>。</summary>
    [JsonPropertyName("jump_wxa")]
    public WxaJumpInfo? JumpWxa { get; set; }

    /// <summary>失效类型（<c>expire_type</c>，选填）：<c>0</c> 失效时间间隔 / <c>1</c> 失效日期（默认 <c>0</c>）。</summary>
    [JsonPropertyName("expire_type")]
    public long? ExpireType { get; set; }

    /// <summary>失效时间间隔（<c>expire_interval</c>，<c>expire_type = 0</c> 时必填；单位天，官方范围 7~30）。</summary>
    [JsonPropertyName("expire_interval")]
    public long? ExpireInterval { get; set; }

    /// <summary>失效日期（<c>expire_time</c>，<c>expire_type = 1</c> 时必填；Unix 时间戳）。</summary>
    [JsonPropertyName("expire_time")]
    public long? ExpireTime { get; set; }

    /// <summary>是否过期（<c>is_expire</c>，选填；官方原文：使用失效字段前先将此值设为 <c>true</c>）。</summary>
    [JsonPropertyName("is_expire")]
    public bool? IsExpire { get; set; }
}
