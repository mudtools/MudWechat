// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.WeData;

/// <summary>
/// 设置登录配置请求（<c>POST /wedata/wedata_set_login_config</c>）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/we-analysis/weDataSetLoginConfig.html"/></para>
/// <para>官方约束：<c>set_type</c> 为 1 时填 <c>recheck_url</c>（反查地址），为 2 时填 <c>associate_appid</c>（关联小程序 AppId 列表）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "WeData")]
public class OpenPlatformWeDataSetLoginConfigRequest
{
    /// <summary>设置类型（官方必填 <c>set_type</c>，number；1 = 反查地址、2 = 关联小程序）。</summary>
    [JsonPropertyName("set_type")]
    public int SetType { get; set; }

    /// <summary>反查地址（官方 <c>recheck_url</c>；<c>set_type = 1</c> 时填写）。</summary>
    [JsonPropertyName("recheck_url")]
    public string? RecheckUrl { get; set; }

    /// <summary>关联小程序 AppId 列表（官方 <c>associate_appid</c>；<c>set_type = 2</c> 时填写）。</summary>
    [JsonPropertyName("associate_appid")]
    public List<string>? AssociateAppId { get; set; }
}
