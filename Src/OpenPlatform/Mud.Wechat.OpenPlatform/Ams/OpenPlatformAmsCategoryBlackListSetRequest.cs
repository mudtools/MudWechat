// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.Ams;

/// <summary>
/// 「设置行业类别黑名单」请求（<c>agency_set_mp_amscategory_blacklist</c>）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/ad-black/SetAmsCategoryBlackList.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Ams")]
public class OpenPlatformAmsCategoryBlackListSetRequest
{
    /// <summary>屏蔽的行业类别（官方必填 <c>ams_category</c>，string，枚举值如 <c>CHESS</c> / <c>MEDICAL_HEALTH</c> 等）。</summary>
    [JsonPropertyName("ams_category")]
    public string? AmsCategory { get; set; }
}
