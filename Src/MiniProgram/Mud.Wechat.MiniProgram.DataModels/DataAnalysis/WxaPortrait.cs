// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.DataAnalysis;

/// <summary>用户画像（省 / 市 / 性别 / 平台 / 终端 / 年龄六维）。</summary>
[HttpJsonSerializable(SerializerClassName = "DataAnalysis")]
public class WxaPortrait
{
    /// <summary>省份分布（<c>province</c>）。</summary>
    [JsonPropertyName("province")]
    public List<WxaPortraitItem>? Province { get; set; }

    /// <summary>城市分布（<c>city</c>）。</summary>
    [JsonPropertyName("city")]
    public List<WxaPortraitItem>? City { get; set; }

    /// <summary>性别分布（<c>genders</c>）。</summary>
    [JsonPropertyName("genders")]
    public List<WxaPortraitItem>? Genders { get; set; }

    /// <summary>平台分布（<c>platforms</c>，Android / iOS 等）。</summary>
    [JsonPropertyName("platforms")]
    public List<WxaPortraitItem>? Platforms { get; set; }

    /// <summary>终端分布（<c>devices</c>，iPhone / android / 其他）。</summary>
    [JsonPropertyName("devices")]
    public List<WxaPortraitItem>? Devices { get; set; }

    /// <summary>年龄区间分布（<c>ages</c>）。</summary>
    [JsonPropertyName("ages")]
    public List<WxaPortraitItem>? Ages { get; set; }
}
