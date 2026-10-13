// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.Novel;

/// <summary>
/// 添加作品授权请求
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Novel")]
public class WxaNovelAddBookAuthRequest
{
    /// <summary>
    /// 作品id
    /// </summary>
    [JsonPropertyName("book_id")]
    public string? BookId { get; set; }

    /// <summary>
    /// 被授权账号appid
    /// </summary>
    [JsonPropertyName("grantee_appid")]
    public string? GranteeAppId { get; set; }

    /// <summary>
    /// 授权到期时间，unix时间戳，单位秒，按CST时区
    /// </summary>
    [JsonPropertyName("expire_time")]
    public long? ExpireTime { get; set; }
}

/// <summary>
/// 添加作品授权响应
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Novel")]
public class WxaNovelAddBookAuthResponse : WxaResponse
{
    /// <summary>
    /// 授权结果列表
    /// </summary>
    [JsonPropertyName("results")]
    public List<WxaNovelAuthResult>? Results { get; set; }
}

/// <summary>
/// 授权结果项
/// </summary>
public class WxaNovelAuthResult
{
    /// <summary>
    /// 错误码
    /// </summary>
    [JsonPropertyName("errcode")]
    public long? ErrorCode { get; set; }

    /// <summary>
    /// 错误信息
    /// </summary>
    [JsonPropertyName("errmsg")]
    public string? ErrorMessage { get; set; }
}
