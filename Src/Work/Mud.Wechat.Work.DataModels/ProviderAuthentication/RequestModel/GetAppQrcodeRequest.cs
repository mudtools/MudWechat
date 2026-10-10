// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ProviderAuthentication;

/// <summary>
/// 获取应用二维码请求体（<c>/cgi-bin/service/get_app_qrcode</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "ProviderAuthentication")]
public class GetAppQrcodeRequest
{
    /// <summary>获取或设置第三方应用 id（官方 suite_id，必填，即 ww 或 wx 开头的 suiteid）。</summary>
    [JsonPropertyName("suite_id")]
    public string SuiteId { get; set; } = string.Empty;

    /// <summary>获取或设置第三方应用 id（官方 appid，单应用不需要该参数，多应用旧套件才需要传该参数；不传默认为 1）。</summary>
    [JsonPropertyName("appid")]
    public long? Appid { get; set; }

    /// <summary>
    /// 获取或设置 state 值（官方 state，用于区分不同的安装渠道，可填写 a-zA-Z0-9，长度不可超过 32 个字节，默认为空）；
    /// 扫应用带参二维码授权安装后，「获取企业永久授权码」接口会返回该 state 值。
    /// </summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>
    /// 获取或设置二维码样式选项（官方 style，默认为不带说明外框小尺寸）：
    /// 0 - 带说明外框的二维码（适合于实体物料）；1 - 带说明外框的二维码（适合于屏幕类）；
    /// 2 - 不带说明外框（小尺寸）；3 - 不带说明外框（中尺寸）；4 - 不带说明外框（大尺寸）。
    /// </summary>
    [JsonPropertyName("style")]
    public long? Style { get; set; }

    /// <summary>
    /// 获取或设置结果返回方式（官方 result_type，官方默认为 1）：1 - 二维码图片 buffer；2 - 二维码图片 url。
    /// <para>
    /// 官方默认值 1 时本端点返回 image/png 二进制流（非 JSON），SDK 无法反序列化——
    /// 走本 SDK JSON 契约面时须传 2 以获取 JSON 形态的二维码 url。
    /// </para>
    /// </summary>
    [JsonPropertyName("result_type")]
    public long? ResultType { get; set; }
}
