// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.User;

/// <summary>批量查询项（<c>user_list</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "User")]
public class MpBatchUserInfoItem
{
    /// <summary>用户的标识，对当前公众号唯一（必须是<b>已关注</b>用户的 openid）。</summary>
    [JsonPropertyName("openid")]
    public string OpenId { get; set; } = string.Empty;

    /// <summary>国家地区语言版本（<c>zh_CN</c> / <c>zh_TW</c> / <c>en</c>；留空表示官方默认 zh-CN）。</summary>
    [JsonPropertyName("lang")]
    public string? Lang { get; set; }
}
