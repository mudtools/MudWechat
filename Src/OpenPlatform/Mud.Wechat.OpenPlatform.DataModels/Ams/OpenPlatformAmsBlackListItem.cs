// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.Ams;

/// <summary>
/// 广告黑名单明细项（官方 <c>item</c> 数组内层：屏蔽的公众号 / 小程序 / iOS / Android 应用）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/ad-black/GetBlackList.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Ams")]
public class OpenPlatformAmsBlackListItem
{
    /// <summary>屏蔽对象 ID（官方字段名 <c>id</c>）。</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>屏蔽对象名称（官方字段名 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>屏蔽对象头像 URL（官方字段名 <c>url</c>）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}
