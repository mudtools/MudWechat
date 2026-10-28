// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Auth;

/// <summary>数据水印（<c>watermark</c>；官方要求宿主校验 <c>appid</c> 以防数据来源伪造）。</summary>
[HttpJsonSerializable(SerializerClassName = "Auth")]
public class WxaWatermark
{
    /// <summary>数据获取时间戳（<c>timestamp</c>，秒级）。</summary>
    [JsonPropertyName("timestamp")]
    public long? Timestamp { get; set; }

    /// <summary>数据来源小程序 AppID（<c>appid</c>）—— 必须与自身一致。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }
}
