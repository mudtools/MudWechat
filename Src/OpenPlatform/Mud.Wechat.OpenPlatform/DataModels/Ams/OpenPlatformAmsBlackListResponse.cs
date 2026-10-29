// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.Ams;

/// <summary>
/// 「获取屏蔽的广告主」应答（<c>agency_get_black_list</c>，四类屏蔽列表各为分组数组）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/ad-black/GetBlackList.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Ams")]
public class OpenPlatformAmsBlackListResponse : OpenPlatformAmsResponse
{
    /// <summary>屏蔽的公众号列表（官方字段名 <c>blacklist_biz</c>）。</summary>
    [JsonPropertyName("blacklist_biz")]
    public List<OpenPlatformAmsBlackListGroup>? BlacklistBiz { get; set; }

    /// <summary>屏蔽的小程序列表（官方字段名 <c>blacklist_weapp</c>）。</summary>
    [JsonPropertyName("blacklist_weapp")]
    public List<OpenPlatformAmsBlackListGroup>? BlacklistWeapp { get; set; }

    /// <summary>屏蔽的 iOS 应用列表（官方字段名 <c>blacklist_ios</c>）。</summary>
    [JsonPropertyName("blacklist_ios")]
    public List<OpenPlatformAmsBlackListGroup>? BlacklistIos { get; set; }

    /// <summary>屏蔽的 Android 应用列表（官方字段名 <c>blacklist_android</c>）。</summary>
    [JsonPropertyName("blacklist_android")]
    public List<OpenPlatformAmsBlackListGroup>? BlacklistAndroid { get; set; }
}
