// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.WeData;

/// <summary>
/// 用户绑定信息（官方 <c>info</c> 数组项，查询用户绑定列表应答）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/we-analysis/weDataQueryBindList.html"/></para>
/// <para>昵称与头像属个人信息，<b>不得</b>进日志、遥测或异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "WeData")]
public class OpenPlatformWeDataBindInfo
{
    /// <summary>用户在服务商系统的唯一标识（官方 <c>uid</c>）。</summary>
    [JsonPropertyName("uid")]
    public string? UserId { get; set; }

    /// <summary>是否已绑定微信号（官方 <c>is_bind</c>，数字 0/1）。</summary>
    [JsonPropertyName("is_bind")]
    public int IsBind { get; set; }

    /// <summary>用户微信昵称（官方 <c>nickname</c>）。</summary>
    [JsonPropertyName("nickname")]
    public string? Nickname { get; set; }

    /// <summary>用户微信头像 URL（官方 <c>head_url</c>）。</summary>
    [JsonPropertyName("head_url")]
    public string? HeadImageUrl { get; set; }

    /// <summary>创建时间戳（官方 <c>create_time</c>，Unix 秒）。</summary>
    [JsonPropertyName("create_time")]
    public long CreateTime { get; set; }

    /// <summary>更新时间戳（官方 <c>update_time</c>，Unix 秒）。</summary>
    [JsonPropertyName("update_time")]
    public long UpdateTime { get; set; }

    /// <summary>用户权限集列表（官方 <c>perm</c>）。</summary>
    [JsonPropertyName("perm")]
    public List<OpenPlatformWeDataPermission>? PermissionList { get; set; }
}
