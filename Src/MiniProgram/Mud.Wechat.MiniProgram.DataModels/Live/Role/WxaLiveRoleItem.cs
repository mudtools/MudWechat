// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 成员列表条目（<c>list[]</c>，官方 6 字段，<c>GET /wxaapi/broadcast/role/getrolelist</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/role-management/api_getrolelist.html"/>。
/// </para>
/// <para>
/// <b>一个用户可持多角色</b>：应答用 <c>roleList</c>（官方类型 numarray，官方示例 <c>[2, 3]</c>）而非单值
/// —— 与设置侧的单个 <c>role</c> 字段不对称，照录。
/// </para>
/// <para><b><see cref="Username"/> 官方示例为脱敏值</b>（<c>"o0****0o"</c>）⇒ 不得用于回填设置/删除请求的定位键。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class WxaLiveRoleItem
{
    /// <summary>微信用户头像 URL（<c>headingimg</c>，官方字段名为全小写连写）。</summary>
    [JsonPropertyName("headingimg")]
    public string? HeadingImg { get; set; }

    /// <summary>微信用户昵称（<c>nickname</c>）。</summary>
    [JsonPropertyName("nickname")]
    public string? Nickname { get; set; }

    /// <summary>openid（<c>openid</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>具有的身份列表（<c>roleList</c>，官方类型 numarray，取值见 <see cref="WxaLiveRoleType"/>）。</summary>
    [JsonPropertyName("roleList")]
    public List<long>? RoleList { get; set; }

    /// <summary>更新时间（<c>updateTimestamp</c>，官方类型列为 <b>string</b>，官方示例 <c>"1600340080"</c>）。</summary>
    [JsonPropertyName("updateTimestamp")]
    public string? UpdateTimestamp { get; set; }

    /// <summary>微信号（<c>username</c>，官方示例为<b>脱敏</b>值）。</summary>
    [JsonPropertyName("username")]
    public string? Username { get; set; }
}
